using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing.Text;
using WindesHeim_Game.Properties;

namespace WindesHeim_Game
{
    public class Model
    {
        protected Controller controller;

        // Voor fonts
        [DllImport("gdi32.dll")]
        private static extern IntPtr AddFontMemResourceEx(IntPtr pbFont, uint cbFont, IntPtr pdv, [In] ref uint pcFonts);

        public FontFamily windesheimFontFamily;
        public Font windesheimSheriffFont;
        public Font windesheimSheriffFontSmall;
        public Font windesheimSheriffFontXSmall;
        public Font windesheimTransitFont;
        public FontFamily windesheimFontFamilyTransit;

        public Model(Controller controller)
        {
            this.controller = controller;

            //Windesheimfonts definieren
            SetWindesheimFontSheriff();
            SetWindesheimFontTransit();
        }

        public virtual void ControlsInit(Form GameWindow)
        {
        }

        public virtual void GraphicsInit(Graphics g)
        {
        }

        private void SetWindesheimFontTransit()
        {
            byte[] fontArray = Resources.ufonts_com_transitfront_negativ;
            int dataLength = Resources.ufonts_com_transitfront_negativ.Length;

            IntPtr ptrData = Marshal.AllocCoTaskMem(dataLength);
            Marshal.Copy(fontArray, 0, ptrData, dataLength);

            uint cFonts = 0;
            AddFontMemResourceEx(ptrData, (uint)fontArray.Length, IntPtr.Zero, ref cFonts);

            PrivateFontCollection pfc = new PrivateFontCollection();
            pfc.AddMemoryFont(ptrData, dataLength);

            Marshal.FreeCoTaskMem(ptrData);

            windesheimFontFamilyTransit = pfc.Families[0];
            windesheimTransitFont = new Font(windesheimFontFamilyTransit, 11f, FontStyle.Regular);
        }

        private void SetWindesheimFontSheriff()
        {
            byte[] fontArray = Properties.Resources.ufonts_com_sheriff_roman;
            int dataLength = Resources.ufonts_com_sheriff_roman.Length;

            IntPtr ptrData = Marshal.AllocCoTaskMem(dataLength);
            Marshal.Copy(fontArray, 0, ptrData, dataLength);

            uint cFonts = 0;
            AddFontMemResourceEx(ptrData, (uint)fontArray.Length, IntPtr.Zero, ref cFonts);

            PrivateFontCollection pfc = new PrivateFontCollection();
            pfc.AddMemoryFont(ptrData, dataLength);

            Marshal.FreeCoTaskMem(ptrData);

            windesheimFontFamily = pfc.Families[0];
            windesheimSheriffFont = new Font(windesheimFontFamily, 28f, FontStyle.Regular);
            windesheimSheriffFontSmall = new Font(windesheimFontFamily, 14f, FontStyle.Regular);
            windesheimSheriffFontXSmall = new Font(windesheimFontFamily, 12f, FontStyle.Regular);

        }
    }

    public class ModelMenu : Model
    {
        private ControllerMenu menuController;

        private PictureBox play;
        private PictureBox editor;
        private PictureBox highscore;
        private PictureBox tempPlay;
        private Panel menuPanel;
        private Panel backgroundImage;

        public ModelMenu(ControllerMenu controller) : base(controller)
        {
            this.menuController = controller;
        }

        public override void ControlsInit(Form gameWindow)
        {
            play = new PictureBox();
            editor = new PictureBox();
            highscore = new PictureBox();
            tempPlay = new PictureBox();

            backgroundImage = new Panel();
            backgroundImage.Size = gameWindow.Size;
            backgroundImage.BackgroundImage = Resources.menuBackground;

            play.Image = Resources.playButton;
            play.Location = new System.Drawing.Point(0, 0);
            play.Size = new System.Drawing.Size(304, 44);
            play.TabIndex = 0;
            play.Click += new EventHandler(menuController.play_Click);

            editor.Image = Resources.editorButton;
            editor.Location = new System.Drawing.Point(0, 60);
            editor.Size = new System.Drawing.Size(304, 44);
            editor.TabIndex = 1;
            editor.Click += new EventHandler(menuController.editor_Click);

            highscore.Image = Resources.highscoresButton;
            highscore.Location = new System.Drawing.Point(0, 120);
            highscore.Size = new System.Drawing.Size(304, 44);
            highscore.TabIndex = 2;
            highscore.Click += new EventHandler(menuController.highscore_Click);

            menuPanel = new Panel();
            menuPanel.AutoSize = true;
            menuPanel.BackColor = Color.Transparent;

            backgroundImage.Controls.Add(menuPanel);
            gameWindow.Controls.Add(backgroundImage);
            menuPanel.Controls.Add(play);
            menuPanel.Controls.Add(editor);
            menuPanel.Controls.Add(highscore);


            System.Console.WriteLine(gameWindow.Width);
            menuPanel.Location = new Point((gameWindow.Width / 2 - menuPanel.Size.Width / 2), (gameWindow.Height / 2 - menuPanel.Size.Height / 2));
        }
    }

    public class ModelGame : Model
    {
        private ControllerGame gameController;

        //XML Gegevens van level worden hierin meeggegeven
        public static XMLParser level;

        //Score
        public Label score = new Label();

        // Houdt alle dynamische gameobjecten vast
        private List<GameObject> gameObjects = new List<GameObject>();

        // Er is maar 1 speler
        public Player player = new Player(new Point(10, 10), 40, 40);

        // Graphicspaneel
        public PictureBox graphicsPanel = new PictureBox();     

        //INITIALISATIES CONTROLS
        //START OBSTACLEPANEL
        internal Panel obstaclePanel = new Panel();

        // START OBSTACLE 1
        private Panel pnlObstacle1 = new Panel();
        private Label lblObstacles = new Label();

        // Icon 1
        private Panel pnlObstacleIcon1 = new Panel();
        internal PictureBox pbObstacle1 = new PictureBox();

        //Obstacle Name 1
        internal Label lblObstacleName1 = new Label();

        //Obstacle Properties 1
        private Label lblObstacleProps1 = new Label();

        private Label lblObstaclePosXTitle1 = new Label();
        internal Label lblObstaclePosX1 = new Label();

        private Label lblObstaclePosYTitle1 = new Label();
        internal Label lblObstaclePosY1 = new Label();

        //Obstacle Description 1
        private Label lblObstacleDescTitle1 = new Label();
        internal Label lblObstacleDesc1 = new Label();
        // STOP OBSTACLE 1

        // START OBSTACLE 2
        private Panel pnlObstacle2 = new Panel();

        //Icon 2
        private Panel pnlObstacleIcon2 = new Panel();
        internal PictureBox pbObstacle2 = new PictureBox();

        //Obstacle Name 2
        internal Label lblObstacleName2 = new Label();

        //Obstacle Properties 2
        private Label lblObstacleProps2 = new Label();

        private Label lblObstaclePosXTitle2 = new Label();
        internal Label lblObstaclePosX2 = new Label();
        private Label lblObstaclePosYTitle2 = new Label();
        internal Label lblObstaclePosY2 = new Label();

        //Obstacle Description 2
        private Label lblObstacleDescTitle2 = new Label();
        internal Label lblObstacleDesc2 = new Label();
        // STOP OBSTACLE 2
        //STOP OBSTACLEPANEL


        //START LOGO PANEL
        private Panel logoPanel = new Panel();
        //STOP LOGO PANEL


        //START CHARACTER PANEL
        private Panel characterPanel = new Panel();

        // START CHARACTER
        private Panel pnlCharacter = new Panel();
        private Label lblCharacter = new Label();

        //Character Name
        private Label lblCharacterName = new Label();

        //Character Icon
        private Panel pnlCharacterIcon = new Panel();
        private PictureBox pbCharacter = new PictureBox();

        //Character Properties
        private Label lblCharacterProps = new Label();

        private Label lblCharacterPosXTitle = new Label();
        internal Label lblCharacterPosX = new Label();
        private Label lblCharacterPosYTitle = new Label();
        internal Label lblCharacterPosY = new Label();

        private Label lblCharacterSpeedTitle = new Label();
        internal Label lblCharacterSpeed = new Label();
        // STOP CHARACTER
        //STOP CHARACTER PANEL


        //START CONTROL PANEL
        private Panel controlPanel = new Panel();

        public PictureBox btnUp = new PictureBox();
        public PictureBox btnDown = new PictureBox();
        public PictureBox btnLeft = new PictureBox();
        public PictureBox btnRight = new PictureBox();
        //STOP CONTROL PANEL


        //START ACTION PANEL       
        private Panel actionPanel = new Panel();

        public PictureBox pbIconSound = new PictureBox();
        private PictureBox pbIconRestart = new PictureBox();
        private PictureBox pbIconMenu = new PictureBox();
        //STOP ACTION PANEL

        public ModelGame(ControllerGame controller) : base(controller)
        {
            this.gameController = controller;
        }

        public void InitializeField()
        {
            player.Location = new Point(0, 0);
            gameObjects.Clear();
            gameObjects = level.getCleanGameObjects();           
        }

        public override void ControlsInit(Form gameWindow)
        {
            score.ForeColor = Color.Black;
            score.TextAlign = ContentAlignment.MiddleRight;
            score.Location = new System.Drawing.Point(780, 10);
            score.Size = new System.Drawing.Size(50, 25);
            score.BackColor = Color.Transparent;

            // Registreer key events voor de player
            gameWindow.KeyDown += gameController.OnKeyDownWASD;
            gameWindow.KeyUp += gameController.OnKeyUp;

            // Voeg graphicspaneel toe voor het tekenen van gameobjecten
            graphicsPanel.BackColor = Color.White;
            graphicsPanel.BorderStyle = BorderStyle.FixedSingle;
            graphicsPanel.Location = new Point(0, 0);
            graphicsPanel.Size = new Size(845, 475);
            graphicsPanel.Paint += gameController.OnPaintEvent;

            // Overige panels
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GameWindow));

            //START OBSTACLE PANEL
            //Main ObstaclePanel
            obstaclePanel.BackColor = System.Drawing.SystemColors.Window;
            obstaclePanel.Controls.Add(pnlObstacle2);
            obstaclePanel.Controls.Add(pnlObstacle1);
            obstaclePanel.Controls.Add(lblObstacles);
            obstaclePanel.Location = new System.Drawing.Point(848, 0);
            obstaclePanel.Margin = new Padding(1);
            obstaclePanel.Name = "obstaclePanel";
            obstaclePanel.Size = new System.Drawing.Size(431, 475);
            obstaclePanel.TabIndex = 2;

            //Label Obstacle Title
            lblObstacles.AutoSize = true;
            lblObstacles.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(67)))), ((int)(((byte)(131)))));
            lblObstacles.Location = new System.Drawing.Point(3, 9);
            lblObstacles.Name = "lblObstacles";
            lblObstacles.Size = new System.Drawing.Size(54, 13);
            lblObstacles.TabIndex = 0;
            lblObstacles.Text = "Obstacles";

            //Panel Obstacle 1
            pnlObstacle1.BackColor = System.Drawing.SystemColors.ControlLight;
            pnlObstacle1.Controls.Add(lblObstacleDesc1);
            pnlObstacle1.Controls.Add(lblObstacleDescTitle1);
            pnlObstacle1.Controls.Add(lblObstaclePosY1);
            pnlObstacle1.Controls.Add(lblObstaclePosX1);
            pnlObstacle1.Controls.Add(lblObstaclePosYTitle1);
            pnlObstacle1.Controls.Add(lblObstaclePosXTitle1);
            pnlObstacle1.Controls.Add(pnlObstacleIcon1);
            pnlObstacle1.Controls.Add(lblObstacleProps1);
            pnlObstacle1.Controls.Add(lblObstacleName1);
            pnlObstacle1.Location = new System.Drawing.Point(8, 75);
            pnlObstacle1.Name = "pnlObstacle1";
            pnlObstacle1.Size = new System.Drawing.Size(412, 190);
            pnlObstacle1.TabIndex = 1;

            //Panel ObstacleIcon1
            pnlObstacleIcon1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(216)))), ((int)(((byte)(216)))), ((int)(((byte)(216)))));
            pnlObstacleIcon1.Controls.Add(pbObstacle1);
            pnlObstacleIcon1.Location = new System.Drawing.Point(7, 12);
            pnlObstacleIcon1.Name = "pnlObstacleIcon1";
            pnlObstacleIcon1.Size = new System.Drawing.Size(160, 160);
            pnlObstacleIcon1.TabIndex = 5;

            //Picturebox Obstacle 1
            pbObstacle1.BackgroundImage = global::WindesHeim_Game.Properties.Resources.bikeEdited;
            pbObstacle1.BackgroundImageLayout = ImageLayout.Zoom;
            pbObstacle1.Location = new System.Drawing.Point(5, 5);
            pbObstacle1.Name = "pbObstacle1";
            pbObstacle1.Size = new System.Drawing.Size(150, 150);
            pbObstacle1.TabIndex = 0;
            pbObstacle1.TabStop = false;

            //Label Obstacle Name 1
            lblObstacleName1.AutoSize = true;
            lblObstacleName1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(67)))), ((int)(((byte)(131)))));
            lblObstacleName1.Location = new System.Drawing.Point(173, 12);
            lblObstacleName1.Name = "lblObstacleName1";
            lblObstacleName1.Size = new System.Drawing.Size(75, 13);
            lblObstacleName1.TabIndex = 3;
            lblObstacleName1.Text = "Obstaclename";

            //Label Obstacle Properties 1
            lblObstacleProps1.AutoSize = true;
            lblObstacleProps1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(213)))), ((int)(((byte)(161)))), ((int)(((byte)(15)))));
            lblObstacleProps1.Location = new System.Drawing.Point(173, 43);
            lblObstacleProps1.Name = "lblObstacleProps1";
            lblObstacleProps1.Size = new System.Drawing.Size(81, 13);
            lblObstacleProps1.TabIndex = 4;
            lblObstacleProps1.Text = "Properties";

            //Label Obstacle Postition X Title 1
            lblObstaclePosXTitle1.AutoSize = true;
            lblObstaclePosXTitle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            lblObstaclePosXTitle1.ForeColor = System.Drawing.SystemColors.ControlDark;
            lblObstaclePosXTitle1.Location = new System.Drawing.Point(173, 63);
            lblObstaclePosXTitle1.Name = "lblObstaclePosXTitle1";
            lblObstaclePosXTitle1.Size = new System.Drawing.Size(71, 18);
            lblObstaclePosXTitle1.TabIndex = 6;
            lblObstaclePosXTitle1.Text = "Position X:";

            //Label Obstacle Position X 1
            lblObstaclePosX1.AutoSize = true;
            lblObstaclePosX1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            lblObstaclePosX1.ForeColor = System.Drawing.SystemColors.ControlDark;
            lblObstaclePosX1.Location = new System.Drawing.Point(250, 63);
            lblObstaclePosX1.Name = "lblObstaclePosX1";
            lblObstaclePosX1.Size = new System.Drawing.Size(23, 18);
            lblObstaclePosX1.TabIndex = 8;
            lblObstaclePosX1.Text = "int";

            //Label Obstacle Position Y Title 1
            lblObstaclePosYTitle1.AutoSize = true;
            lblObstaclePosYTitle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            lblObstaclePosYTitle1.ForeColor = System.Drawing.SystemColors.ControlDark;
            lblObstaclePosYTitle1.Location = new System.Drawing.Point(173, 83);
            lblObstaclePosYTitle1.Name = "lblObstaclePosYTitle1";
            lblObstaclePosYTitle1.Size = new System.Drawing.Size(70, 18);
            lblObstaclePosYTitle1.TabIndex = 7;
            lblObstaclePosYTitle1.Text = "Position Y:";

            //Label Obstacle Position Y 1
            lblObstaclePosY1.AutoSize = true;
            lblObstaclePosY1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            lblObstaclePosY1.ForeColor = System.Drawing.SystemColors.ControlDark;
            lblObstaclePosY1.Location = new System.Drawing.Point(249, 83);
            lblObstaclePosY1.Name = "lblObstaclePosY1";
            lblObstaclePosY1.Size = new System.Drawing.Size(23, 18);
            lblObstaclePosY1.TabIndex = 9;
            lblObstaclePosY1.Text = "int";

            //Label Obstacle Description Title 1
            lblObstacleDescTitle1.AutoSize = true;
            lblObstacleDescTitle1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(213)))), ((int)(((byte)(161)))), ((int)(((byte)(15)))));
            lblObstacleDescTitle1.Location = new System.Drawing.Point(173, 116);
            lblObstacleDescTitle1.Margin = new Padding(0);
            lblObstacleDescTitle1.Name = "lblObstacleDescTitle1";
            lblObstacleDescTitle1.Size = new System.Drawing.Size(64, 13);
            lblObstacleDescTitle1.TabIndex = 10;
            lblObstacleDescTitle1.Text = "Description";

            //Label Obstacle Description 1
            lblObstacleDesc1.AutoSize = true;
            lblObstacleDesc1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            lblObstacleDesc1.ForeColor = System.Drawing.SystemColors.ControlDark;
            lblObstacleDesc1.Location = new System.Drawing.Point(173, 136);
            lblObstacleDesc1.Margin = new Padding(0);
            lblObstacleDesc1.Name = "lblObstacleDesc1";
            lblObstacleDesc1.Size = new System.Drawing.Size(85, 18);
            lblObstacleDesc1.TabIndex = 15;
            lblObstacleDesc1.Text = "Description";

            //Panel Obstacle 2 
            pnlObstacle2.BackColor = System.Drawing.SystemColors.ControlLight;
            pnlObstacle2.Controls.Add(lblObstacleDesc2);
            pnlObstacle2.Controls.Add(lblObstacleDescTitle2);
            pnlObstacle2.Controls.Add(lblObstaclePosY2);
            pnlObstacle2.Controls.Add(pnlObstacleIcon2);
            pnlObstacle2.Controls.Add(lblObstaclePosX2);
            pnlObstacle2.Controls.Add(lblObstacleProps2);
            pnlObstacle2.Controls.Add(lblObstaclePosYTitle2);
            pnlObstacle2.Controls.Add(lblObstacleName2);
            pnlObstacle2.Controls.Add(lblObstaclePosXTitle2);
            pnlObstacle2.Location = new System.Drawing.Point(8, 285);
            pnlObstacle2.Name = "pnlObstacle2";
            pnlObstacle2.Size = new System.Drawing.Size(412, 190);
            pnlObstacle2.TabIndex = 4;

            //Panel Obstacle Icon 2
            pnlObstacleIcon2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(216)))), ((int)(((byte)(216)))), ((int)(((byte)(216)))));
            pnlObstacleIcon2.Controls.Add(pbObstacle2);
            pnlObstacleIcon2.Location = new System.Drawing.Point(7, 12);
            pnlObstacleIcon2.Name = "pnlObstacleIcon2";
            pnlObstacleIcon2.Size = new System.Drawing.Size(160, 160);
            pnlObstacleIcon2.TabIndex = 6;

            //Picturebox Obstacle 2
            pbObstacle2.BackgroundImage = global::WindesHeim_Game.Properties.Resources.carEdited;
            pbObstacle2.BackgroundImageLayout = ImageLayout.Zoom;
            pbObstacle2.Location = new System.Drawing.Point(5, 5);
            pbObstacle2.Name = "pbObstacle2";
            pbObstacle2.Size = new System.Drawing.Size(150, 150);
            pbObstacle2.TabIndex = 1;
            pbObstacle2.TabStop = false;

            //Label Obstacle Name 2
            lblObstacleName2.AutoSize = true;
            lblObstacleName2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(67)))), ((int)(((byte)(131)))));
            lblObstacleName2.Location = new System.Drawing.Point(173, 12);
            lblObstacleName2.Name = "lblObstacleName2";
            lblObstacleName2.Size = new System.Drawing.Size(75, 13);
            lblObstacleName2.TabIndex = 3;
            lblObstacleName2.Text = "Obstaclename";

            //Label Obstacle Properties 2
            lblObstacleProps2.AutoSize = true;
            lblObstacleProps2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(213)))), ((int)(((byte)(161)))), ((int)(((byte)(15)))));
            lblObstacleProps2.Location = new System.Drawing.Point(173, 43);
            lblObstacleProps2.Name = "lblObstacleProps2";
            lblObstacleProps2.Size = new System.Drawing.Size(81, 13);
            lblObstacleProps2.TabIndex = 5;
            lblObstacleProps2.Text = "Properties";

            //Label Obstacle Postition X Title 2
            lblObstaclePosXTitle2.AutoSize = true;
            lblObstaclePosXTitle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            lblObstaclePosXTitle2.ForeColor = System.Drawing.SystemColors.ControlDark;
            lblObstaclePosXTitle2.Location = new System.Drawing.Point(174, 63);
            lblObstaclePosXTitle2.Name = "lblObstaclePosXTitle2";
            lblObstaclePosXTitle2.Size = new System.Drawing.Size(71, 18);
            lblObstaclePosXTitle2.TabIndex = 10;
            lblObstaclePosXTitle2.Text = "Position X:";

            //Label Obstacle Position X 2
            lblObstaclePosX2.AutoSize = true;
            lblObstaclePosX2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            lblObstaclePosX2.ForeColor = System.Drawing.SystemColors.ControlDark;
            lblObstaclePosX2.Location = new System.Drawing.Point(249, 63);
            lblObstaclePosX2.Name = "lblObstaclePosX2";
            lblObstaclePosX2.Size = new System.Drawing.Size(23, 18);
            lblObstaclePosX2.TabIndex = 12;
            lblObstaclePosX2.Text = "int";

            //Label Obstacle Position Y Title 2
            lblObstaclePosYTitle2.AutoSize = true;
            lblObstaclePosYTitle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            lblObstaclePosYTitle2.ForeColor = System.Drawing.SystemColors.ControlDark;
            lblObstaclePosYTitle2.Location = new System.Drawing.Point(174, 83);
            lblObstaclePosYTitle2.Name = "lblObstaclePosYTitle2";
            lblObstaclePosYTitle2.Size = new System.Drawing.Size(70, 18);
            lblObstaclePosYTitle2.TabIndex = 11;
            lblObstaclePosYTitle2.Text = "Position Y:";

            //Label Obstacle Position Y 2
            lblObstaclePosY2.AutoSize = true;
            lblObstaclePosY2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            lblObstaclePosY2.ForeColor = System.Drawing.SystemColors.ControlDark;
            lblObstaclePosY2.Location = new System.Drawing.Point(249, 83);
            lblObstaclePosY2.Name = "lblObstaclePosY2";
            lblObstaclePosY2.Size = new System.Drawing.Size(23, 18);
            lblObstaclePosY2.TabIndex = 13;
            lblObstaclePosY2.Text = "int";

            //Label Obstacle Description Title 2
            lblObstacleDescTitle2.AutoSize = true;
            lblObstacleDescTitle2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(213)))), ((int)(((byte)(161)))), ((int)(((byte)(15)))));
            lblObstacleDescTitle2.Location = new System.Drawing.Point(173, 116);
            lblObstacleDescTitle2.Margin = new Padding(0);
            lblObstacleDescTitle2.Name = "lblObstacleDescTitle2";
            lblObstacleDescTitle2.Size = new System.Drawing.Size(64, 13);
            lblObstacleDescTitle2.TabIndex = 11;
            lblObstacleDescTitle2.Text = "Description";

            //Label Obstacle Description 2
            lblObstacleDesc2.AutoSize = true;
            lblObstacleDesc2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            lblObstacleDesc2.ForeColor = System.Drawing.SystemColors.ControlDark;
            lblObstacleDesc2.Location = new System.Drawing.Point(173, 136);
            lblObstacleDesc2.Margin = new Padding(0);
            lblObstacleDesc2.Name = "lblObstacleDesc2";
            lblObstacleDesc2.Size = new System.Drawing.Size(85, 18);
            lblObstacleDesc2.TabIndex = 14;
            lblObstacleDesc2.Text = "Description";
            //STOP OBSTACLE PANEL


            //START LOGO PANEL

            //Main Panel
            logoPanel.BackColor = System.Drawing.SystemColors.Window;
            logoPanel.BackgroundImage = global::WindesHeim_Game.Properties.Resources.ConceptTransparentBackgroundSmall;
            logoPanel.BackgroundImageLayout = ImageLayout.Zoom;
            logoPanel.Location = new System.Drawing.Point(848, 477);
            logoPanel.Margin = new Padding(1);
            logoPanel.Name = "logoPanel";
            logoPanel.Size = new System.Drawing.Size(431, 242);
            logoPanel.TabIndex = 0;
            //STOP LOGO PANEL


            //START CHARACTER PANEL

            //Main Panel
            characterPanel.BackColor = System.Drawing.SystemColors.Window;
            characterPanel.Controls.Add(pnlCharacter);
            characterPanel.Controls.Add(lblCharacter);
            characterPanel.Location = new System.Drawing.Point(475, 477);
            characterPanel.Margin = new Padding(1);
            characterPanel.Name = "characterPanel";
            characterPanel.Size = new System.Drawing.Size(372, 242);
            characterPanel.TabIndex = 5;

            //Label Character Title 
            lblCharacter.AutoSize = true;
            lblCharacter.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(67)))), ((int)(((byte)(131)))));
            lblCharacter.Location = new System.Drawing.Point(2, 2);
            lblCharacter.Name = "lblCharacter";
            lblCharacter.Size = new System.Drawing.Size(53, 13);
            lblCharacter.TabIndex = 0;
            lblCharacter.Text = "Character";

            //Panel Character
            pnlCharacter.BackColor = System.Drawing.SystemColors.ControlLight;
            pnlCharacter.Controls.Add(lblCharacterSpeed);
            pnlCharacter.Controls.Add(lblCharacterSpeedTitle);
            pnlCharacter.Controls.Add(lblCharacterPosY);
            pnlCharacter.Controls.Add(lblCharacterPosX);
            pnlCharacter.Controls.Add(lblCharacterPosYTitle);
            pnlCharacter.Controls.Add(lblCharacterPosXTitle);
            pnlCharacter.Controls.Add(pnlCharacterIcon);
            pnlCharacter.Controls.Add(lblCharacterProps);
            pnlCharacter.Controls.Add(lblCharacterName);
            pnlCharacter.Location = new System.Drawing.Point(5, 55);
            pnlCharacter.Name = "pnlCharacter";
            pnlCharacter.Size = new System.Drawing.Size(364, 179);
            pnlCharacter.TabIndex = 2;

            //Panel Character Icon
            pnlCharacterIcon.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(216)))), ((int)(((byte)(216)))), ((int)(((byte)(216)))));
            pnlCharacterIcon.Controls.Add(pbCharacter);
            pnlCharacterIcon.Location = new System.Drawing.Point(7, 12);
            pnlCharacterIcon.Name = "pnlCharacterIcon";
            pnlCharacterIcon.Size = new System.Drawing.Size(160, 160);
            pnlCharacterIcon.TabIndex = 5;

            //PictureBox Character
            pbCharacter.BackgroundImage = global::WindesHeim_Game.Properties.Resources.playerEdited1;
            pbCharacter.BackgroundImageLayout = ImageLayout.Zoom;
            pbCharacter.Location = new System.Drawing.Point(5, 5);
            pbCharacter.Name = "pbCharacter";
            pbCharacter.Size = new System.Drawing.Size(150, 150);
            pbCharacter.TabIndex = 0;
            pbCharacter.TabStop = false;

            //Label Character Name
            lblCharacterName.AutoSize = true;
            lblCharacterName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(67)))), ((int)(((byte)(131)))));
            lblCharacterName.Location = new System.Drawing.Point(173, 12);
            lblCharacterName.Name = "lblCharacterName";
            lblCharacterName.Size = new System.Drawing.Size(63, 13);
            lblCharacterName.TabIndex = 3;
            lblCharacterName.Text = "Player";

            //Label Character Properties
            lblCharacterProps.AutoSize = true;
            lblCharacterProps.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(213)))), ((int)(((byte)(161)))), ((int)(((byte)(15)))));
            lblCharacterProps.Location = new System.Drawing.Point(173, 43);
            lblCharacterProps.Name = "lblCharacterProps";
            lblCharacterProps.Size = new System.Drawing.Size(81, 13);
            lblCharacterProps.TabIndex = 4;
            lblCharacterProps.Text = "Properties";

            //Label Character Position X Title
            lblCharacterPosXTitle.AutoSize = true;
            lblCharacterPosXTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            lblCharacterPosXTitle.ForeColor = System.Drawing.SystemColors.ControlDark;
            lblCharacterPosXTitle.Location = new System.Drawing.Point(173, 63);
            lblCharacterPosXTitle.Name = "lblCharacterPosXTitle";
            lblCharacterPosXTitle.Size = new System.Drawing.Size(71, 18);
            lblCharacterPosXTitle.TabIndex = 6;
            lblCharacterPosXTitle.Text = "Position X:";

            //Label Character Position X
            lblCharacterPosX.AutoSize = true;
            lblCharacterPosX.ForeColor = System.Drawing.SystemColors.ControlDark;
            lblCharacterPosX.Location = new System.Drawing.Point(250, 63);
            lblCharacterPosX.Name = "lblCharacterPosX";
            lblCharacterPosX.Size = new System.Drawing.Size(23, 18);
            lblCharacterPosX.TabIndex = 8;
            lblCharacterPosX.Text = "int";

            //Label Character Position Y Title
            lblCharacterPosYTitle.AutoSize = true;
            lblCharacterPosYTitle.ForeColor = System.Drawing.SystemColors.ControlDark;
            lblCharacterPosYTitle.Location = new System.Drawing.Point(173, 83);
            lblCharacterPosYTitle.Name = "lblCharacterPosYTitle";
            lblCharacterPosYTitle.Size = new System.Drawing.Size(70, 18);
            lblCharacterPosYTitle.TabIndex = 7;
            lblCharacterPosYTitle.Text = "Position Y:";

            //Label Character Position Y
            lblCharacterPosY.AutoSize = true;
            lblCharacterPosY.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            lblCharacterPosY.ForeColor = System.Drawing.SystemColors.ControlDark;
            lblCharacterPosY.Location = new System.Drawing.Point(249, 83);
            lblCharacterPosY.Name = "lblCharacterPosY";
            lblCharacterPosY.Size = new System.Drawing.Size(23, 18);
            lblCharacterPosY.TabIndex = 9;
            lblCharacterPosY.Text = "int";

            //Label Character Speed Title
            lblCharacterSpeedTitle.AutoSize = true;
            lblCharacterSpeedTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            lblCharacterSpeedTitle.ForeColor = System.Drawing.SystemColors.ControlDark;
            lblCharacterSpeedTitle.Location = new System.Drawing.Point(173, 114);
            lblCharacterSpeedTitle.Name = "lblCharacterSpeedTitle";
            lblCharacterSpeedTitle.Size = new System.Drawing.Size(68, 18);
            lblCharacterSpeedTitle.TabIndex = 10;
            lblCharacterSpeedTitle.Text = "Speed:";

            //Label Character Speed
            lblCharacterSpeed.AutoSize = true;
            lblCharacterSpeed.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            lblCharacterSpeed.ForeColor = System.Drawing.SystemColors.ControlDark;
            lblCharacterSpeed.Location = new System.Drawing.Point(249, 114);
            lblCharacterSpeed.Name = "lblCharacterSpeed";
            lblCharacterSpeed.Size = new System.Drawing.Size(23, 18);
            lblCharacterSpeed.TabIndex = 11;
            lblCharacterSpeed.Text = "int";
            //STOP CHARACTER PANEL

            //Label Character Speed
            lblCharacterSpeed.AutoSize = true;
            lblCharacterSpeed.ForeColor = System.Drawing.SystemColors.ControlDark;
            lblCharacterSpeed.Location = new System.Drawing.Point(249, 114);
            lblCharacterSpeed.Name = "lblCharacterSpeed";
            lblCharacterSpeed.Size = new System.Drawing.Size(23, 18);
            lblCharacterSpeed.TabIndex = 11;
            lblCharacterSpeed.Text = "int";
            //STOP CHARACTER PANEL

            //START CONTROL PANEL

            //Main Panel
            controlPanel.BackColor = System.Drawing.SystemColors.Window;
            controlPanel.Controls.Add(btnLeft);
            controlPanel.Controls.Add(btnRight);
            controlPanel.Controls.Add(btnUp);
            controlPanel.Controls.Add(btnDown);
            controlPanel.Location = new System.Drawing.Point(101, 477);
            controlPanel.Margin = new Padding(1);
            controlPanel.Name = "controlPanel";
            controlPanel.Size = new System.Drawing.Size(372, 242);
            controlPanel.TabIndex = 4;

            //Button Up
            btnUp.BackgroundImage = global::WindesHeim_Game.Properties.Resources.Up;
            btnUp.BackgroundImageLayout = ImageLayout.Zoom;
            btnUp.Location = new System.Drawing.Point(136, 13);
            btnUp.Margin = new Padding(0);
            btnUp.Name = "btnUp";
            btnUp.Size = new System.Drawing.Size(100, 100);
            btnUp.TabIndex = 1;
            //btnUp.UseVisualStyleBackColor = true;

            //Button Down
            btnDown.BackgroundImage = global::WindesHeim_Game.Properties.Resources.Down;
            btnDown.BackgroundImageLayout = ImageLayout.Zoom;
            btnDown.Location = new System.Drawing.Point(136, 134);
            btnDown.Margin = new Padding(0);
            btnDown.Name = "btnDown";
            btnDown.Size = new System.Drawing.Size(100, 100);
            btnDown.TabIndex = 0;
            //btnDown.UseVisualStyleBackColor = true;

            //Button Left
            btnLeft.BackgroundImage = global::WindesHeim_Game.Properties.Resources.Left;
            btnLeft.BackgroundImageLayout = ImageLayout.Zoom;
            btnLeft.Location = new System.Drawing.Point(21, 134);
            btnLeft.Margin = new Padding(0);
            btnLeft.Name = "btnLeft";
            btnLeft.Size = new System.Drawing.Size(100, 100);
            btnLeft.TabIndex = 3;
            //btnLeft.UseVisualStyleBackColor = true;

            //Button Right
            btnRight.BackgroundImage = global::WindesHeim_Game.Properties.Resources.Right;
            btnRight.BackgroundImageLayout = ImageLayout.Zoom;
            btnRight.Location = new System.Drawing.Point(250, 134);
            btnRight.Margin = new Padding(0);
            btnRight.Name = "btnRight";
            btnRight.Size = new System.Drawing.Size(100, 100);
            btnRight.TabIndex = 2;
            //btnRight.UseVisualStyleBackColor = true;
            //STOP CONTROL PANEL


            //START ACTION PANEL

            //Main Panel
            actionPanel.BackColor = System.Drawing.SystemColors.Window;
            actionPanel.BorderStyle = BorderStyle.FixedSingle;
            actionPanel.Controls.Add(pbIconMenu);
            actionPanel.Controls.Add(pbIconRestart);
            actionPanel.Controls.Add(pbIconSound);
            actionPanel.Location = new System.Drawing.Point(-11, 467);
            actionPanel.Margin = new Padding(1);
            actionPanel.Name = "actionPanel";
            actionPanel.Size = new System.Drawing.Size(111, 284);
            actionPanel.TabIndex = 3;

            //Picturebox Action Sound
            pbIconSound.BackgroundImage = global::WindesHeim_Game.Properties.Resources.soundEdited;
            pbIconSound.BackgroundImageLayout = ImageLayout.Zoom;
            pbIconSound.Location = new System.Drawing.Point(34, 22);
            pbIconSound.Name = "pbIconSound";
            pbIconSound.Size = new System.Drawing.Size(50, 50);
            pbIconSound.TabIndex = 6;
            pbIconSound.TabStop = false;
            pbIconSound.Click += new System.EventHandler(gameController.pbIconSound_Click);
            pbIconSound.MouseEnter += new System.EventHandler(gameController.SoundHoverEnter);
            pbIconSound.MouseLeave += new System.EventHandler(gameController.SoundHoverLeave);

            //Picturebox Action Restart
            pbIconRestart.BackgroundImage = global::WindesHeim_Game.Properties.Resources.restartEdited;
            pbIconRestart.BackgroundImageLayout = ImageLayout.Zoom;
            pbIconRestart.Location = new System.Drawing.Point(34, 106);
            pbIconRestart.Name = "pbIconRestart";
            pbIconRestart.Size = new System.Drawing.Size(50, 50);
            pbIconRestart.TabIndex = 7;
            pbIconRestart.TabStop = false;
            pbIconRestart.MouseEnter += new System.EventHandler(RestartHoverEnter);
            pbIconRestart.MouseClick += new MouseEventHandler(gameController.RestartClicked);
            pbIconRestart.MouseLeave += new System.EventHandler(RestartHoverLeave);

            //Picturebox Action Menu
            pbIconMenu.BackgroundImage = global::WindesHeim_Game.Properties.Resources.menuEdited;
            pbIconMenu.BackgroundImageLayout = ImageLayout.Zoom;
            pbIconMenu.Location = new System.Drawing.Point(34, 190);
            pbIconMenu.Name = "pbIconMenu";
            pbIconMenu.Size = new System.Drawing.Size(50, 50);
            pbIconMenu.TabIndex = 8;
            pbIconMenu.TabStop = false;
            pbIconMenu.MouseEnter += new System.EventHandler(MenuHoverEnter);
            pbIconMenu.MouseClick += new MouseEventHandler(gameController.MenuClicked);
            pbIconMenu.MouseLeave += new System.EventHandler(MenuHoverLeave);
            //STOP ACTION PANEL

            graphicsPanel.Controls.Add(score);

            // Voeg hieronder de overige panels toe, zoals objectDescriptionen etc.
            gameWindow.Controls.Add(graphicsPanel);
            gameWindow.Controls.Add(obstaclePanel);
            gameWindow.Controls.Add(characterPanel);
            gameWindow.Controls.Add(controlPanel);
            gameWindow.Controls.Add(actionPanel);
            gameWindow.Controls.Add(logoPanel);


            //Windesheim Fonts toevoegen
            SetWindesheimTheme();
        }

        private void SetWindesheimTheme()
        {
            this.lblObstacles.Font = windesheimSheriffFont;
            this.lblCharacter.Font = windesheimSheriffFont;

            this.lblObstacleName1.Font = windesheimSheriffFontSmall;
            this.lblObstacleName2.Font = windesheimSheriffFontSmall;

            this.lblObstacleProps1.Font = windesheimSheriffFontXSmall;
            this.lblObstacleProps2.Font = windesheimSheriffFontXSmall;

            this.lblObstacleDescTitle1.Font = windesheimSheriffFontXSmall;
            this.lblObstacleDescTitle2.Font = windesheimSheriffFontXSmall;

            this.lblObstaclePosXTitle1.Font = windesheimTransitFont;
            this.lblObstaclePosX1.Font = windesheimTransitFont;
            this.lblObstaclePosYTitle1.Font = windesheimTransitFont;
            this.lblObstaclePosY1.Font = windesheimTransitFont;
            this.lblObstacleDesc1.Font = windesheimTransitFont;

            this.lblObstaclePosXTitle2.Font = windesheimTransitFont;
            this.lblObstaclePosX2.Font = windesheimTransitFont;
            this.lblObstaclePosYTitle2.Font = windesheimTransitFont;
            this.lblObstaclePosY2.Font = windesheimTransitFont;
            this.lblObstacleDesc2.Font = windesheimTransitFont;

            this.lblCharacterName.Font = windesheimSheriffFontSmall;
            this.lblCharacterProps.Font = windesheimSheriffFontXSmall;
            this.lblCharacterPosXTitle.Font = windesheimTransitFont;
            this.lblCharacterPosYTitle.Font = windesheimTransitFont;
            this.lblCharacterPosX.Font = windesheimTransitFont;
            this.lblCharacterPosY.Font = windesheimTransitFont;
            this.lblCharacterSpeed.Font = windesheimTransitFont;
            this.lblCharacterSpeedTitle.Font = windesheimTransitFont;
        }

        

        private void RestartHoverEnter(object sender, EventArgs e)
        {
            pbIconRestart.BackgroundImage = Resources.restartEditedOnHover;
        }

        private void RestartHoverLeave(object sender, EventArgs e)
        {
            pbIconRestart.BackgroundImage = Resources.restartEdited;
        }

        private void MenuHoverLeave(object sender, EventArgs e)
        {
            pbIconMenu.BackgroundImage = Resources.menuEdited;
        }

        private void MenuHoverEnter(object sender, EventArgs e)
        {
            pbIconMenu.BackgroundImage = Resources.menuEditedOnHover;
        }

        


        public List<GameObject> GameObjects
        {
            get { return gameObjects; }
        }
    }

    public class ModelLevelSelect : Model
    {
        public ListBox listBoxLevels;
        public PictureBox goBack;
        public PictureBox playLevel;
        public Panel alignPanel;
        public Panel gamePanel;
        private Panel backgroundImage;

        private ControllerLevelSelect levelSelectController;

        public ModelLevelSelect(ControllerLevelSelect controller) : base(controller)
        {
            this.levelSelectController = controller;
        }

        public override void ControlsInit(Form gameWindow)
        {
            alignPanel = new Panel();
            alignPanel.AutoSize = true;
            alignPanel.BackColor = Color.Transparent;

            backgroundImage = new Panel();
            backgroundImage.Location = new System.Drawing.Point(0, 0);
            backgroundImage.Size = new System.Drawing.Size(gameWindow.Width, gameWindow.Height);
            backgroundImage.BackgroundImage = Resources.otherScreen;

            gamePanel = new Panel();
            gamePanel.Location = new System.Drawing.Point(210, 0);
            gamePanel.Size = new System.Drawing.Size(845, 475);
            gamePanel.BackColor = Color.White;
            gamePanel.BorderStyle = BorderStyle.FixedSingle;
            gamePanel.Paint += levelSelectController.OnPreviewPaint;

            listBoxLevels = new ListBox();
            listBoxLevels.Size = new System.Drawing.Size(200, 475);
            listBoxLevels.Location = new System.Drawing.Point(0, 0);

            XMLParser.LoadAllLevels();
            foreach (XMLParser xml in XMLParser.Levels)
            {
                listBoxLevels.Items.Add(xml);
            }
            listBoxLevels.SelectedIndexChanged += levelSelectController.level_Select;
            listBoxLevels.SetSelected(0, true);
            
            goBack = new PictureBox();
            goBack.Size = new System.Drawing.Size(200, 44);
            goBack.Location = new System.Drawing.Point(0, 482);
            goBack.Text = "Go Back";
            goBack.BackgroundImage = Resources.goBack;
            goBack.Click += levelSelectController.goBack_Click;

            playLevel = new PictureBox();
            playLevel.Size = new System.Drawing.Size(200, 44);
            playLevel.Location = new System.Drawing.Point(210, 480);
            playLevel.Text = "Play Level";
            playLevel.BackgroundImage = Resources.playLevel;
            playLevel.Click += levelSelectController.playLevel_Click;

            
            backgroundImage.Controls.Add(alignPanel);
            alignPanel.Controls.Add(goBack);
            alignPanel.Controls.Add(playLevel);
            alignPanel.Controls.Add(listBoxLevels);
            alignPanel.Controls.Add(gamePanel);
            alignPanel.Location = new Point(
                (gameWindow.Width / 2 - alignPanel.Size.Width / 2),
                125);
            gameWindow.Controls.Add(backgroundImage);
        }
    }

    public class ModelHighscores : Model
    {
        private ListBox listBoxLevels;
        private PictureBox goBack;
        private Panel alignPanel;
        public ListBox listBoxHighscores;
        private Panel backgroundImage;

        private List<XMLParser> levels = new List<XMLParser>();

        private ControllerHighscores highscoresController;

        public ModelHighscores(ControllerHighscores controller) : base(controller)
        {
            this.highscoresController = controller;
        }

        public override void ControlsInit(Form gameWindow)
        {
            alignPanel = new Panel();
            alignPanel.AutoSize = true;
            alignPanel.BackColor = Color.Transparent;

            backgroundImage = new Panel();
            backgroundImage.Size = gameWindow.Size;
            backgroundImage.BackgroundImage = Resources.otherScreen;

            listBoxLevels = new ListBox();
            listBoxLevels.Size = new System.Drawing.Size(200, 200);
            listBoxLevels.Location = new System.Drawing.Point(0, 0);
            listBoxLevels.SelectedIndexChanged += highscoresController.level_Select;

            listBoxHighscores = new ListBox();
            listBoxHighscores.Size = new System.Drawing.Size(400, 200);
            listBoxHighscores.Location = new System.Drawing.Point(210, 0);
            listBoxHighscores.HorizontalScrollbar = true;

            XMLParser.LoadAllLevels();
            foreach (XMLParser xml in XMLParser.Levels)
            {
                this.levels.Add(xml); //Ingeladen gegevens opslaan in lokale List voor hergebruik
                listBoxLevels.Items.Add(xml);
            }
            listBoxLevels.SetSelected(0, true);

            goBack = new PictureBox();
            goBack.Size = new System.Drawing.Size(200, 44);
            goBack.Text = "Go Back";
            goBack.BackgroundImage = Resources.goBack;
            goBack.Click += new EventHandler(highscoresController.goBack_Click);

            gameWindow.Controls.Add(backgroundImage);
            backgroundImage.Controls.Add(alignPanel);
            alignPanel.Controls.Add(goBack);
            alignPanel.Controls.Add(listBoxLevels);
            alignPanel.Controls.Add(listBoxHighscores);

            alignPanel.Location = new Point(
                (gameWindow.Width / 2 - alignPanel.Size.Width / 2),
                (gameWindow.Height / 2 - alignPanel.Size.Height / 2));


            goBack.Location = new System.Drawing.Point((alignPanel.Width / 2 - goBack.Size.Width / 2), listBoxLevels.Size.Height + 10);
        }
    }

    public class ModelEditorSelect : Model
    {
        public ListBox listBoxLevels;
        public PictureBox goBack;
        public PictureBox editLevel;
        public PictureBox newLevel;
        public Panel alignPanel;
        public Panel gamePanel;
        private Panel backgroundImage;

        private ControllerEditorSelect editorSelectController;

        public ModelEditorSelect(ControllerEditorSelect controller) : base(controller)
        {
            this.editorSelectController = controller;
        }

        public override void ControlsInit(Form gameWindow)
        {
            alignPanel = new Panel();
            alignPanel.AutoSize = true;
            alignPanel.BackColor = Color.Transparent;

            backgroundImage = new Panel();
            backgroundImage.Location = new System.Drawing.Point(0, 0);
            backgroundImage.Size = new System.Drawing.Size(gameWindow.Width, gameWindow.Height);
            backgroundImage.BackgroundImage = Resources.otherScreen;

            gamePanel = new Panel();
            gamePanel.Location = new System.Drawing.Point(210, 0);
            gamePanel.Size = new System.Drawing.Size(845, 475);
            gamePanel.BackColor = Color.White;
            gamePanel.BorderStyle = BorderStyle.FixedSingle;
            gamePanel.Paint += editorSelectController.OnPreviewPaint;

            listBoxLevels = new ListBox();
            listBoxLevels.Size = new System.Drawing.Size(200, 475);
            listBoxLevels.Location = new System.Drawing.Point(0, 0);

            // Lees levels uit XML file
            XMLParser.LoadAllLevels();
            foreach (XMLParser xml in XMLParser.Levels)
            {
                if(xml != null)
                    listBoxLevels.Items.Add(xml);
            }

            listBoxLevels.SelectedIndexChanged += editorSelectController.level_Select;
            listBoxLevels.SetSelected(0, true);

            goBack = new PictureBox();
            goBack.Size = new System.Drawing.Size(200, 44);
            goBack.Location = new System.Drawing.Point(0, 480);
            goBack.Text = "Go Back";
            goBack.Image = Resources.goBack;
            goBack.Click += editorSelectController.goBack_Click;

            editLevel = new PictureBox();
            editLevel.Size = new System.Drawing.Size(200, 44);
            editLevel.Location = new System.Drawing.Point(210, 480);
            editLevel.Text = "Edit Level";
            editLevel.Image = Resources.editLevel;
            editLevel.Click += editorSelectController.editLevel_Click;

            newLevel = new PictureBox();
            newLevel.Size = new System.Drawing.Size(200, 44);
            newLevel.Location = new System.Drawing.Point(420, 480);
            newLevel.Text = "New Level";
            newLevel.Image = Resources.newLevel;
            newLevel.Click += editorSelectController.newLevel_Click;

            gameWindow.Controls.Add(backgroundImage);
            backgroundImage.Controls.Add(alignPanel);
            alignPanel.Controls.Add(goBack);
            alignPanel.Controls.Add(editLevel);
            alignPanel.Controls.Add(newLevel);
            alignPanel.Controls.Add(listBoxLevels);
            alignPanel.Controls.Add(gamePanel);
            alignPanel.Location = new Point(
                (gameWindow.Width / 2 - alignPanel.Size.Width / 2),
                (gameWindow.Height / 2 - alignPanel.Size.Height / 2));

        }
    }

    public class ModelEditor : Model
    {
        public PictureBox goBack;
        public PictureBox saveLevel;
        public PictureBox testLevel;
        public Button undoButton;
        public Button clearButton;
        public Panel alignPanel;
        private Panel backgroundImage;
        public PictureBox gamePanel;
        public PictureBox staticObstacle;
        public PictureBox explodingObstacle;
        public PictureBox movingExplodingObstacle;
        public PictureBox slowingObstacle;
        private Label dragDropLabel;

        public int widthDragDropPanel = 210;


        //XML Gegevens van level worden hierin meeggegeven
        public static XMLParser level;

        private ControllerEditor editorController;

        public ModelEditor(ControllerEditor controller) : base(controller)
        {
            this.editorController = controller;
        }

        public override void ControlsInit(Form gameWindow)
        {
            alignPanel = new Panel();
            alignPanel.AutoSize = true;
            alignPanel.BackColor = Color.Transparent;

            backgroundImage = new Panel();
            backgroundImage.Location = new System.Drawing.Point(0, 0);
            backgroundImage.Size = new System.Drawing.Size(gameWindow.Width, gameWindow.Height);
            backgroundImage.BackgroundImage = Resources.otherScreen;

            dragDropLabel = new Label();
            dragDropLabel.Text = "Drag en drop";
            dragDropLabel.Font = new Font("Arial", 12);
            dragDropLabel.Location = new System.Drawing.Point(10, 50);
            dragDropLabel.Size = new System.Drawing.Size(200, 30);

            gamePanel = new PictureBox();
            gamePanel.Location = new System.Drawing.Point(0, 0); // 210
            gamePanel.Size = new System.Drawing.Size(845 + widthDragDropPanel, 475);
            gamePanel.BackColor = Color.White;
            gamePanel.Paint += new PaintEventHandler(editorController.GamePanel_Paint);
            gamePanel.MouseDown += editorController.MouseDown;
            gamePanel.MouseMove += editorController.ObjectMouseDrag;
            gamePanel.MouseUp += editorController.MouseUp;
            gamePanel.BorderStyle = BorderStyle.FixedSingle;


            goBack = new PictureBox();
            goBack.Size = new System.Drawing.Size(200, 44);
            goBack.Location = new System.Drawing.Point(0, 525);
            goBack.Text = "Go Back";
            goBack.Image = Resources.goBack;
            goBack.Click += editorController.goBack_Click;

            testLevel = new PictureBox();
            testLevel.Size = new System.Drawing.Size(200, 44);
            testLevel.Location = new System.Drawing.Point(210, 525);
            testLevel.Text = "Test Level";
            testLevel.Image = Resources.testLevel;
            testLevel.Click += editorController.testLevel_Click;

            saveLevel = new PictureBox();
            saveLevel.Size = new System.Drawing.Size(200, 44);
            saveLevel.Location = new System.Drawing.Point(420, 525);
            saveLevel.Text = "Save Level";
            saveLevel.Image = Resources.saveLevel;
            saveLevel.Click += editorController.saveLevel_Click;

            undoButton = new Button();
            undoButton.Size = new System.Drawing.Size(100, 25);
            undoButton.Location = new System.Drawing.Point(10, 300);
            undoButton.Text = "Undo";
            undoButton.Click += editorController.undoLastChange_Click;
            undoButton.TabIndex = 999;

            clearButton = new Button();
            clearButton.Size = new System.Drawing.Size(100, 25);
            clearButton.Location = new System.Drawing.Point(10, 330);
            clearButton.Text = "Clear";
            clearButton.Click += editorController.clearAll_Click;

            staticObstacle = new PictureBox();
            //staticObstacle.AllowDrop = true;
            staticObstacle.BackgroundImageLayout = ImageLayout.None;
            staticObstacle.Image = Resources.IconTC;
            staticObstacle.Location = new System.Drawing.Point(10, 60);
            staticObstacle.Name = "staticObstacle";
            staticObstacle.Size = new System.Drawing.Size(40, 40);
            staticObstacle.SizeMode = PictureBoxSizeMode.Zoom;
            staticObstacle.TabIndex = 999;
            staticObstacle.TabStop = false;
            staticObstacle.MouseDown += editorController.updateMousePosition;
            staticObstacle.MouseMove += editorController.updateDragPosition;
            staticObstacle.MouseUp += editorController.StaticObstacle_MouseUp;

            explodingObstacle = new PictureBox();
            explodingObstacle.BackgroundImageLayout = ImageLayout.None;
            explodingObstacle.Image = Resources.IconCar;
            explodingObstacle.Location = new System.Drawing.Point(10, 110);
            explodingObstacle.Name = "explodingObstacle";
            explodingObstacle.Size = new System.Drawing.Size(40, 40);
            explodingObstacle.SizeMode = PictureBoxSizeMode.Zoom;
            explodingObstacle.TabIndex = 999;
            explodingObstacle.TabStop = false;
            explodingObstacle.MouseDown += editorController.updateMousePosition;
            explodingObstacle.MouseMove += editorController.updateDragPosition;
            explodingObstacle.MouseUp += editorController.ExplodingObstacle_MouseUp;

            movingExplodingObstacle = new PictureBox();
            movingExplodingObstacle.BackgroundImageLayout = ImageLayout.None;
            movingExplodingObstacle.Image = Resources.IconBike;
            movingExplodingObstacle.Location = new System.Drawing.Point(10, 160);
            movingExplodingObstacle.Name = "movingExplodingObstacle";
            movingExplodingObstacle.Size = new System.Drawing.Size(40, 40);
            movingExplodingObstacle.SizeMode = PictureBoxSizeMode.Zoom;
            movingExplodingObstacle.TabIndex = 999;
            movingExplodingObstacle.TabStop = false;
            movingExplodingObstacle.MouseDown += editorController.updateMousePosition;
            movingExplodingObstacle.MouseMove += editorController.updateDragPosition;
            movingExplodingObstacle.MouseUp += editorController.MovingExplodingObstacle_MouseUp;
            
            slowingObstacle = new PictureBox();
            slowingObstacle.BackgroundImageLayout = ImageLayout.None;
            slowingObstacle.Image = Resources.IconES;
            slowingObstacle.Location = new System.Drawing.Point(10, 210);
            slowingObstacle.Name = "slowingObstacle";
            slowingObstacle.Size = new System.Drawing.Size(40, 40);
            slowingObstacle.SizeMode = PictureBoxSizeMode.Zoom;
            slowingObstacle.TabIndex = 999;
            slowingObstacle.TabStop = false;
            slowingObstacle.MouseDown += editorController.updateMousePosition;
            slowingObstacle.MouseMove += editorController.updateDragPosition;
            slowingObstacle.MouseUp += editorController.SlowingObstacle_MouseUp;

            gameWindow.Controls.Add(backgroundImage);
            backgroundImage.Controls.Add(alignPanel);
            alignPanel.Controls.Add(gamePanel);

            gamePanel.Controls.Add(staticObstacle);
            gamePanel.Controls.Add(explodingObstacle);
            gamePanel.Controls.Add(movingExplodingObstacle);
            gamePanel.Controls.Add(slowingObstacle);

            gamePanel.Controls.Add(undoButton);
            gamePanel.Controls.Add(clearButton);

            alignPanel.Controls.Add(gamePanel);
            alignPanel.Controls.Add(goBack);
            alignPanel.Controls.Add(saveLevel);
            alignPanel.Controls.Add(testLevel);
            alignPanel.Controls.Add(dragDropLabel);

            alignPanel.Location = new Point(
                (gameWindow.Width / 2 - alignPanel.Size.Width / 2),
                (gameWindow.Height / 2 - alignPanel.Size.Height / 2));
        }
    }
    public class ModelHighscoreInput : Model
    {
        public PictureBox continueBtn;
        public PictureBox tryAgain;
        public Panel alignPanel;
        public Panel gamePanel;
        private Panel backgroundImage;
        private Label score = new Label();
        private Label place = new Label();
        public TextBox name = new TextBox();

        private ControllerHighscoreInput highscoreInputController;

        public ModelHighscoreInput(ControllerHighscoreInput controller) : base(controller)
        {
            this.highscoreInputController = controller;
        }

        public override void ControlsInit(Form gameWindow)
        {
            alignPanel = new Panel();
            alignPanel.AutoSize = true;
            alignPanel.BackColor = Color.Transparent;

            backgroundImage = new Panel();
            backgroundImage.Location = new System.Drawing.Point(0, 0);
            backgroundImage.Size = new System.Drawing.Size(gameWindow.Width, gameWindow.Height);
            backgroundImage.BackgroundImage = Resources.menuBackground;

            continueBtn = new PictureBox();
            continueBtn.Size = new System.Drawing.Size(200, 44);
            continueBtn.Text = "Continue";
            continueBtn.BackgroundImage = Resources.continueBtn;
            continueBtn.Click += new EventHandler(highscoreInputController.Continue_Click);

            tryAgain = new PictureBox();
            tryAgain.Size = new System.Drawing.Size(200, 44);
            tryAgain.Text = "Try again";
            tryAgain.BackgroundImage = Resources.tryAgain;
            tryAgain.Click += new EventHandler(highscoreInputController.TryAgain_Click);

            name = new TextBox();
            name.Text = Environment.UserName;
            name.Location = new System.Drawing.Point(0, 0);
            name.TextAlign = HorizontalAlignment.Center;
            name.KeyDown += highscoreInputController.KeyDownText;

            score.AutoSize = true;
            score.Text = "SCORE:" + highscoreInputController.score;
            score.Font = new Font("Arial", 20);

            highscoreInputController.GetPlace();

            place.AutoSize = true;
            place.Text = "PLACE: " + highscoreInputController.place;
            place.Font = new Font("Arial", 20);

            gameWindow.Controls.Add(backgroundImage);
            backgroundImage.Controls.Add(alignPanel);
            alignPanel.Controls.Add(continueBtn);
            alignPanel.Controls.Add(tryAgain);
            alignPanel.Controls.Add(score);
            alignPanel.Controls.Add(place);
            alignPanel.Controls.Add(name);
            alignPanel.Location = new Point(
                (gameWindow.Width / 2 - alignPanel.Size.Width / 2),
                (gameWindow.Height / 2 - alignPanel.Size.Height / 2));
            continueBtn.Location = new System.Drawing.Point((alignPanel.Width / 2 - continueBtn.Size.Width / 2), alignPanel.Size.Height + 10);
            tryAgain.Location = new System.Drawing.Point((alignPanel.Width / 2 - tryAgain.Size.Width / 2), alignPanel.Size.Height + 10);
            score.Location = new System.Drawing.Point((alignPanel.Width / 2 - score.Size.Width / 2), 0);
            place.Location = new System.Drawing.Point((alignPanel.Width / 2 - place.Size.Width / 2), 40);
            name.Location = new System.Drawing.Point((alignPanel.Width / 2 - name.Size.Width / 2), 80);
        }
    }
}

