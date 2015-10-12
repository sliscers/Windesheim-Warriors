using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindesHeim_Game
{
    public partial class GameWindow : Form
    {
        public GameWindow()
        {
            InitializeComponent();
            this.KeyPreview = true;            
            KeyPress += GameWindow_KeyPress;
        }

        private void GameWindow_KeyPress(object sender, KeyPressEventArgs e) {
            e.Handled = true;
            Console.WriteLine("hallo");
        }
    }
}
