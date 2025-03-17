using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chess_App
{
    public partial class MainMenu : Form
    {
        public MainMenu()
        {
            InitializeComponent();
        }
        private void PlayerVsPlayer_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            form1.Show();
            this.Hide();
        }

        private void PlayerVsComputer_Click(object sender, EventArgs e)
        {
            PlayerVsAIForm playerVsAIForm = new PlayerVsAIForm();
            playerVsAIForm.Show();
            this.Hide();
        }
    }
}
