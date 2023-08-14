using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GestionRestaurant
{
    public partial class Oulier : Form
    {

        private const string ConnectionString = "Server=localhost;Database=restaurants;Uid=zandry;Pwd=admin;";
        public Oulier()
        {
            InitializeComponent();
        }

        private void gunaCircleButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtrech_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string searchText = txtrech.Text.Trim();

            if (string.IsNullOrWhiteSpace(searchText))
            {
                MessageBox.Show("Veuillez entrer un terme de recherche valide.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                try
                {
                    string query = "SELECT id, username, email , password FROM admin WHERE username LIKE @SearchText OR email LIKE @SearchText";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@SearchText", "%" + searchText + "%");

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            StringBuilder searchResults = new StringBuilder();
                            while (reader.Read())
                            {
                                int id = reader.GetInt32("id");
                                string username = reader.GetString("username");
                                string email = reader.GetString("email");
                                string password = reader.GetString("password");
                                searchResults.AppendLine($"Email: {email} \nPassword: {password}");
                            }

                            MessageBox.Show(searchResults.ToString(), "Résultats de la recherche", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Aucun résultat trouvé.", "Résultats de la recherche", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erreur lors de la recherche : " + ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

    }
}
