using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Blackjack
{
    public partial class Menu : Form
    {
        public Menu()
        {
            InitializeComponent();
        }

        private void bKilep_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void bHelp_Click(object sender, EventArgs e)
        {
            Help help = new Help();
            help.ShowDialog();
        }

        private void bJatek_Click(object sender, EventArgs e)
        {
            Jatek jatek = new Jatek();
            jatek.Show();
        }
    }
}
