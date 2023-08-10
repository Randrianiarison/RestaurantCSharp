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
    public partial class Home : UserControl
    {
        private Timer animationTimer = new Timer();
        private int currentColorIndex = 0;
        private Color[] lightColors = { Color.Red, Color.Orange, Color.Yellow, Color.Green, Color.Blue, Color.Indigo, Color.Violet };

        public Home()
        {
            InitializeComponent();
            animationTimer.Interval = 500;
            animationTimer.Tick += AnimationTimer_Tick;
            animationTimer.Start();
        }
        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            label1.ForeColor = lightColors[currentColorIndex];
            currentColorIndex = (currentColorIndex + 1) % lightColors.Length;
        }
    }
}
