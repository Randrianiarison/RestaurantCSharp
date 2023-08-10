using System;
using System.Windows.Forms;

namespace GestionRestaurant
{
    public partial class Form1 : Form
    {
        private Timer timer;
        private int startX;
        private int speed = 4;

        public Form1()
        {
            InitializeComponent();
            recept3.Visible = false;
            home1.Hide(); 
            recept3.Hide(); 

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
            home1.Show();
            home1.BringToFront();
        }

        private void home1_Load(object sender, EventArgs e)
        {
            startX = gunaLabel1.Location.X;
            timer.Start();
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
            recept3.BringToFront();
            recept3.Show();
        }

        private void gunaGradientButton5_Click(object sender, EventArgs e)
        {
          
        }

    }
}
