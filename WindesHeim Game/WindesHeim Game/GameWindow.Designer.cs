﻿using System.Drawing;
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
        highscore,
        highscoreInput
    }

    partial class GameWindow
    {
        private ControllerMenu menu;
        private ControllerGame game;
        private ControllerLevelSelect levelSelect;
        private ControllerHighscores highscores;
        private ControllerEditorSelect editorSelect;
        private ControllerEditor editor;
        private ControllerHighscoreInput highscoreInput;


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
            highscoreInput = new ControllerHighscoreInput(this);
            menu = new ControllerMenu(this);
            game = new ControllerGame(this);
            levelSelect = new ControllerLevelSelect(this);
            highscores = new ControllerHighscores(this);
            editorSelect = new ControllerEditorSelect(this);
            editor = new ControllerEditor(this);



            this.SuspendLayout();

            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1280, 720);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            this.Name = "Form1";
            this.Text = "Windesheim Warriors";
            this.Icon = global::WindesHeim_Game.Properties.Resources.IconWINico;
            this.ResumeLayout(false);

            this.setController(ScreenStates.menu);

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
                    game.TimerStart();
                    break;
                case ScreenStates.editorSelect:
                    this.state = ScreenStates.editorSelect;
                    editorSelect.RunController();
                    break;
                case ScreenStates.editor:
                    this.state = ScreenStates.editor;
                    editor.RunController();
                    break;
                case ScreenStates.highscore:
                    this.state = ScreenStates.highscore;
                    highscores.RunController();
                    break;
                case ScreenStates.highscoreInput:
                    highscoreInput.score = game.score;
                    this.state = ScreenStates.highscoreInput;
                    
                    highscoreInput.RunController();
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
