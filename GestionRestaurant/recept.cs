using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace GestionRestaurant
{
    public partial class recept : UserControl
    {
        private const string ConnectionString = "Server=localhost;Database=restaurants;Uid=zandry;Pwd=admin;";
        private int selectedId;
        private Timer animationTimer = new Timer();
        private int currentColorIndex = 0;
        public Color[] lightColors = { Color.Red, Color.Orange, Color.Yellow, Color.Green, Color.Blue, Color.Indigo, Color.Violet };

        public recept()
        {
            InitializeComponent();
            animationTimer.Interval = 500;
            animationTimer.Tick += AnimationTimer_Tick;
            animationTimer.Start();
            LoadTablesData();
            LoadPlatsData();
            dataGridView1.SelectionChanged += DataGridView1_SelectionChanged;
        }
        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            gunaLabel1.ForeColor = lightColors[currentColorIndex];
            gunaLabel3.ForeColor = lightColors[currentColorIndex];
            gunaLabel4.ForeColor = lightColors[currentColorIndex];
            gunaLabel5.ForeColor = lightColors[currentColorIndex];
            gunaLabel6.ForeColor = lightColors[currentColorIndex];

            currentColorIndex = (currentColorIndex + 1) % lightColors.Length;
        }
        private MySqlConnection OpenConnection()
        {
            MySqlConnection connection = new MySqlConnection(ConnectionString);
            connection.Open();
            return connection;
        }

        private void LoadTablesData()
        {
            try
            {
                using (MySqlConnection connection = OpenConnection())
                {
                    string query = "SELECT t.nom FROM tables t WHERE t.id_table NOT IN (SELECT DISTINCT id_table FROM commande)";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string type = reader.GetString("nom");
                            comboBox1.Items.Add(type);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors du chargement des types de parking : " + ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadPlatsData()
        {
            try
            {
                using (MySqlConnection connection = OpenConnection())
                {
                    string query = "SELECT ID_Plat, Nom, Description, Prix, Categorie FROM plat WHERE nombre > 0";
                    MySqlCommand command = new MySqlCommand(query, connection);

                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        dataGridView1.DataSource = dataTable;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors du chargement des plats : " + ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                selectedId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["ID_Plat"].Value);
                string selectedNom = dataGridView1.SelectedRows[0].Cells["Nom"].Value.ToString();
                string selectP = dataGridView1.SelectedRows[0].Cells["Prix"].Value.ToString();
                label6.Text = $"Nom: {selectedNom} \nPrix : {selectP} ";

                LoadThumbnailForSelectedId(selectedId);
            }
        }

        private void LoadThumbnailForSelectedId(int id)
        {
            try
            {
                using (MySqlConnection connection = OpenConnection())
                {
                    string query = "SELECT image FROM plat WHERE ID_Plat = @id";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@id", id);

                    object thumbnailObj = command.ExecuteScalar();
                    if (thumbnailObj != null && thumbnailObj != DBNull.Value)
                    {
                        byte[] thumbnailBytes = (byte[])thumbnailObj;

                        using (MemoryStream ms = new MemoryStream(thumbnailBytes))
                        {
                            Image thumbnail = Image.FromStream(ms);
                            int maxWidth = pictureBox1.Width;
                            int maxHeight = pictureBox1.Height;

                            if (thumbnail.Width > maxWidth || thumbnail.Height > maxHeight)
                            {
                                thumbnail = ScaleImage(thumbnail, maxWidth, maxHeight);
                            }
                            pictureBox1.BorderStyle = BorderStyle.FixedSingle; 
                            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom; 

                            pictureBox1.Image = thumbnail;
                        }
                    }
                    else
                    {
                        pictureBox1.Image = null;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors du chargement de la miniature : " + ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private Image ScaleImage(Image image, int maxWidth, int maxHeight)
        {
            int newWidth = maxWidth;
            int newHeight = maxHeight;

            if (image.Width > image.Height)
            {
                newHeight = (int)((float)image.Height / image.Width * maxWidth);
            }
            else
            {
                newWidth = (int)((float)image.Width / image.Height * maxHeight);
            }

            return new Bitmap(image, newWidth, newHeight);
        }
        private void EnregistrerCommande(int idPlat, int idTable, int quantite, string client)
        {
            try
            {
                using (MySqlConnection connection = OpenConnection())
                {
                    string insertCommandQuery = "INSERT INTO commande (date_commande, id_plat, id_table, quantite, clt) " +
                                               "VALUES (@date, @idPlat, @idTable, @quantite, @clt)";

                    MySqlCommand insertCommand = new MySqlCommand(insertCommandQuery, connection);
                    insertCommand.Parameters.AddWithValue("@date", DateTime.Now);
                    insertCommand.Parameters.AddWithValue("@idPlat", idPlat);
                    insertCommand.Parameters.AddWithValue("@idTable", idTable);
                    insertCommand.Parameters.AddWithValue("@quantite", quantite);
                    insertCommand.Parameters.AddWithValue("@clt", client);

                    int rowsAffected = insertCommand.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("La commande a été enregistrée avec succès.", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadTablesData();
                        LoadPlatsData();
                        Livraison lvr = new Livraison();
                        lvr.LoadCommandeData();
                    }
                    else
                    {
                        MessageBox.Show("La commande n'a pas pu être enregistrée.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors de l'enregistrement de la commande : " + ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void gunaGradientButton1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0 && comboBox1.SelectedItem != null && numeric.Value > 0 && !string.IsNullOrWhiteSpace(textBox1.Text))
            {
                int selectedIdPlat = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["ID_Plat"].Value);
                int selectedIdTable = GetSelectedTableId();

                int quantite = (int)numeric.Value;
                string client = textBox1.Text;

                EnregistrerCommande(selectedIdPlat, selectedIdTable, quantite, client);
                Livraison lvr = new Livraison();
                lvr.LoadCommandeData();
                comboBox1.Text = " ";
                textBox1.Text = " ";
                numeric.Value = 0;
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner un plat, une table, spécifier la quantité et le client.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private int GetSelectedTableId()
        {
            if (comboBox1.SelectedItem != null)
            {
                using (MySqlConnection connection = OpenConnection())
                {
                    string query = "SELECT id_table FROM tables WHERE nom = @nom";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@nom", comboBox1.SelectedItem.ToString());

                    object idObj = command.ExecuteScalar();
                    if (idObj != null)
                    {
                        return Convert.ToInt32(idObj);
                    }
                }
            }

            return -1;
        }


    }
}
