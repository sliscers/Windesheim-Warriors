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
        }
    }
}
