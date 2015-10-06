using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace WindesHeim_Game
{
    public enum ScreenStates
    {
        menu,
        gameSelect,
        game,
        editorSelect,
        editor,
        highscore
    }

    partial class GameWindow
    {
        private ControllerMenu menu;
        private ControllerGame game;
        private ControllerLevelSelect levelSelect;

        private ScreenStates state = ScreenStates.menu;
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            menu = new ControllerMenu(this);
            game = new ControllerGame(this);
            levelSelect = new ControllerLevelSelect(this);

            this.setController(ScreenStates.menu);

            this.SuspendLayout();

            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1280, 720);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            this.Name = "Form1";
            this.Text = "Windesheim Warriors";
            this.ResumeLayout(false);
        }

        public void setController(ScreenStates state)
        {
            switch (state)
            {
                case ScreenStates.menu:
                    this.state = ScreenStates.menu;
                    menu.RunController();
                    break;
                case ScreenStates.gameSelect:
                    this.state = ScreenStates.gameSelect;
                    levelSelect.RunController();
                    break;
                case ScreenStates.game:
                    this.state = ScreenStates.game;
                    game.RunController();
                    break;
                case ScreenStates.editorSelect:
                    this.state = ScreenStates.editorSelect;
                    break;
                case ScreenStates.editor:
                    this.state = ScreenStates.editor;
                    break;
                case ScreenStates.highscore:
                    this.state = ScreenStates.highscore;
                    break;
                default:
                    this.state = ScreenStates.menu;
                    break;
            }
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
            Graphics g = pe.Graphics;

            switch (state)
            {
                case ScreenStates.menu:
                    this.state = ScreenStates.menu;
                    break;
                case ScreenStates.gameSelect:
                    this.state = ScreenStates.gameSelect;
                    break;
                case ScreenStates.game:
                    this.state = ScreenStates.game;
                    break;
                case ScreenStates.editorSelect:
                    this.state = ScreenStates.editorSelect;
                    break;
                case ScreenStates.editor:
                    this.state = ScreenStates.editor;
                    break;
                case ScreenStates.highscore:
                    this.state = ScreenStates.highscore;
                    break;
                default:
                    this.state = ScreenStates.menu;
                    break;
            }
        }
    }
}
