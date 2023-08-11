using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace GestionRestaurant
{
    public partial class crud : Form
    {
        private const string ConnectionString = "Server=localhost;Database=restaurants;Uid=zandry;Pwd=admin;";

        public crud()
        {
            InitializeComponent();
            RefreshDataGridView();
        }

        private void button1_Click(object sender, EventArgs e)
        {
        }
        private byte[] ConvertImageToByteArray(Image image)
        {
            using (var ms = new System.IO.MemoryStream())
            {
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                return ms.ToArray();
            }
        }

        private void RefreshDataGridView()
        {
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();

                string selectQuery = "SELECT * FROM Plat";

                using (MySqlCommand command = new MySqlCommand(selectQuery, connection))
                {
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                    {
                        using (DataTable dataTable = new DataTable())
                        {
                            adapter.Fill(dataTable);
                            dataGridView1.DataSource = dataTable;
                        }
                    }
                }
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Fichiers image|*.jpg;*.png;*.bmp|Tous les fichiers|*.*";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        Image selectedImage = Image.FromFile(openFileDialog.FileName);
                        pictureBox1.Image = selectedImage;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Erreur lors du chargement de l'image : " + ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void txtdescription_TextChanged(object sender, EventArgs e)
        {

        }

        private void gunaGradientButton1_Click(object sender, EventArgs e)
        {

            string nom = txtnom.Text;
            string description = txtdescription.Text;
            decimal prix = decimal.Parse(txtprix.Text);
            string categorie = comboBox1.SelectedItem.ToString();
            int nombre = int.Parse(txtnombre.Text);
            byte[] imageBytes = ConvertImageToByteArray(pictureBox1.Image);

            try
            {
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {
                    connection.Open();

                    string insertQuery = "INSERT INTO Plat (Nom, Description, Prix, Categorie, Nombre, Image) VALUES (@Nom, @Description, @Prix, @Categorie, @Nombre, @Image)";

                    using (MySqlCommand command = new MySqlCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Nom", nom);
                        command.Parameters.AddWithValue("@Description", description);
                        command.Parameters.AddWithValue("@Prix", prix);
                        command.Parameters.AddWithValue("@Categorie", categorie);
                        command.Parameters.AddWithValue("@Nombre", nombre);
                        command.Parameters.AddWithValue("@Image", imageBytes);

                        command.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Le plat a été ajouté avec succès !");
                txtdescription.Text = "";
                txtnom.Text = " ";
                txtnombre.Value = 0;
                txtprix.Value = 0;
                RefreshDataGridView();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Erreur lors de l'insertion du plat : " + ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                txtnom.Text = row.Cells[1].Value.ToString();
                txtdescription.Text = row.Cells[2].Value.ToString();
                txtprix.Text = row.Cells[3].Value.ToString();
                comboBox1.SelectedItem = row.Cells[4].Value.ToString();
                txtnombre.Text = row.Cells[5].Value.ToString();
                byte[] imageBytes = (byte[])row.Cells[6].Value;
                using (var ms = new System.IO.MemoryStream(imageBytes))
                {
                    pictureBox1.Image = Image.FromStream(ms);
                }
            }
        }

        private void gunaGradientButton3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int selectedRowIndex = dataGridView1.SelectedRows[0].Index;
                int platId = Convert.ToInt32(dataGridView1.Rows[selectedRowIndex].Cells["ID_Plat"].Value); 

                try
                {
                    using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                    {
                        connection.Open();

                        string deleteQuery = "DELETE FROM Plat WHERE ID_Plat = @PlatId"; 

                        using (MySqlCommand command = new MySqlCommand(deleteQuery, connection))
                        {
                            command.Parameters.AddWithValue("@PlatId", platId);
                            command.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show("Le plat a été supprimé avec succès !");
                    RefreshDataGridView(); 
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("Erreur lors de la suppression du plat : " + ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Sélectionnez un plat à supprimer.", "Avertissement", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            RefreshDataGridView();
        }

        private void gunaGradientButton2_Click(object sender, EventArgs e)
        {
            Update();
        }
        private void Update()
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int selectedRowIndex = dataGridView1.SelectedRows[0].Index;
                int platId = Convert.ToInt32(dataGridView1.Rows[selectedRowIndex].Cells["ID_Plat"].Value);

                string newNom = txtnom.Text;
                string newDescription = txtdescription.Text;
                decimal newPrix = decimal.Parse(txtprix.Text);
                string newCategorie = comboBox1.SelectedItem.ToString();
                int newNombre = int.Parse(txtnombre.Text);
                byte[] newImageBytes = ConvertImageToByteArray(pictureBox1.Image);

                try
                {
                    using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                    {
                        connection.Open();

                        string updateQuery = "UPDATE Plat SET Nom = @NewNom, Description = @NewDescription, Prix = @NewPrix, Categorie = @NewCategorie, Nombre = @NewNombre, Image = @NewImage WHERE ID_Plat = @PlatId";

                        using (MySqlCommand command = new MySqlCommand(updateQuery, connection))
                        {
                            command.Parameters.AddWithValue("@NewNom", newNom);
                            command.Parameters.AddWithValue("@NewDescription", newDescription);
                            command.Parameters.AddWithValue("@NewPrix", newPrix);
                            command.Parameters.AddWithValue("@NewCategorie", newCategorie);
                            command.Parameters.AddWithValue("@NewNombre", newNombre);
                            command.Parameters.AddWithValue("@NewImage", newImageBytes);
                            command.Parameters.AddWithValue("@PlatId", platId);

                            command.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show("Le plat a été modifié avec succès !");
                    RefreshDataGridView();
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("Erreur lors de la modification du plat : " + ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Sélectionnez un plat à modifier.", "Avertissement", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                RefreshDataGridView();
            }
        }

        private void dataGridView1_CellClick_1(object sender, DataGridViewCellEventArgs e)
        {
            txtnom.Text = dataGridView1.SelectedRows[0].Cells["Nom"].ToString();
        }
        private void gunaGradientButton4_Click(object sender, EventArgs e)
        {
            Form1 fr = new Form1();
            fr.Enabled = true;
            this.Close();
        }
    }
}
