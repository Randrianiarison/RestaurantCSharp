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
    public partial class Login : Form
    {
        private const string ConnectionString = "Server=localhost;Database=restaurants;Uid=zandry;Pwd=admin;";
        public Login()
        {
            InitializeComponent();
        }
        private void Login_Load(object sender, EventArgs e)
        {

        }
        private void gunaCircleButton1_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Voulez-vous vraiment quitter l'application ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
        private void textBox1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "exemple@gmail.com")
            {
                textBox1.Text = " ";
            }
        }
        private void textBox2_Click(object sender, EventArgs e)
        {
            if (textBox2.Text == "**********")
            {
                textBox2.Text = " ";
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            string username = textBox1.Text.Trim();
            string password = textBox2.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Veuillez saisir un nom d'utilisateur et un mot de passe valides.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT id FROM admin WHERE email = @Username AND password = @Password";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);
                        command.Parameters.AddWithValue("@Password", password);

                        object result = command.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            int adminId = Convert.ToInt32(result);
                            if (adminId > 0)
                            {
                                this.Hide();
                                Form1 acc = new Form1();
                                acc.Show();
                            }
                            else
                            {
                                MessageBox.Show("Identifiants de connexion incorrects.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Identifiants de connexion incorrects.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erreur de connexion : " + ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void gunaButton1_Click(object sender, EventArgs e)
        {
            Oulier mdp = new Oulier();
            mdp.Show();
        }
    }
}
