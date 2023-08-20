using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows.Forms;

namespace GestionRestaurant
{
    public partial class Form1 : Form
    {
        private const string ConnectionString = "Server=localhost;Database=restaurants;Uid=zandry;Pwd=admin;";
        private Timer timer;
        private int startX;
        private int speed = 4;

        public Form1()
        {
            InitializeComponent();
            Notification();
            recept3.Visible = false;
            Livraison lvr = new Livraison();
            lvr.LoadCommandeData();
            recept3.Hide();
            livraison1.Visible = false;
            timer = new Timer
            {
                Interval = 50
            };

            timer.Tick += Timer_Tick;
        }

        private void gunaGradientButton3_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Voulez-vous vraiment quitter l'application ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void gunaGradientButton1_Click(object sender, EventArgs e)
        {
            livraison1.Hide();
            recept3.Hide();
            //home1.Show();
            home1.BringToFront();
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            AnimateLabelHorizontal(gunaLabel1, home1.Width);
        }

        private void AnimateLabelHorizontal(Label label, int containerWidth)
        {
            int newX = label.Location.X + speed;
            if (newX + label.Width > containerWidth)
            {
                newX = 0;
            }
            label.Location = new System.Drawing.Point(newX, label.Location.Y);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            timer.Stop();
        }

        private void gunaGradientButton4_Click(object sender, EventArgs e)
        {
            home1.Hide();
            livraison1.Hide();
            recept3.BringToFront();
            recept3.Show();
        }

        private void gunaGradientButton5_Click(object sender, EventArgs e)
        {
            Livraison lvr = new Livraison();
            lvr.LoadCommandeData();
            livraison1.BringToFront();
            livraison1.Show();
        }

        private void gunaPanel1_Paint(object sender, PaintEventArgs e)
        {
            home1.Visible = false;
        }

        private void home2_Load(object sender, EventArgs e)
        {
            startX = gunaLabel1.Location.X;
            timer.Start();
        }

        private void gunaGradientButton6_Click(object sender, EventArgs e)
        {
            crud cr = new crud();
            this.Visible = false;
            cr.Show();
        }
        public void Notification()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {
                    connection.Open();

                    string query = "SELECT COUNT(*) FROM commande";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        gunaGradientButton5.Text += $" {count}";
                        recept rp = new recept();
                        if (count > 0)
                        {
                            gunaGradientButton5.ForeColor = System.Drawing.Color.Crimson;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des plats : {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
