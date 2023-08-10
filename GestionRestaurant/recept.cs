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

        public recept()
        {
            InitializeComponent();
            LoadTablesData();
            dataGridView1.SelectionChanged += DataGridView1_SelectionChanged;
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
                    string query = "SELECT nom FROM tables";
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

            LoadPlatsData();
        }

        private void LoadPlatsData()
        {
            try
            {
                using (MySqlConnection connection = OpenConnection())
                {
                    string query = "SELECT ID_Plat, Nom, Description, Prix, Categorie FROM plat";
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
                label6.Text = $" Nom: {selectedNom} \nPrix : {selectP} ";

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
                            // Charger l'image et redimensionner si nécessaire
                            Image thumbnail = Image.FromStream(ms);
                            int maxWidth = pictureBox1.Width;
                            int maxHeight = pictureBox1.Height;

                            if (thumbnail.Width > maxWidth || thumbnail.Height > maxHeight)
                            {
                                thumbnail = ScaleImage(thumbnail, maxWidth, maxHeight);
                            }

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
    }
}
