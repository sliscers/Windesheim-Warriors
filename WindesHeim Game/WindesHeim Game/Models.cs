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
            byte[] fontArray = global::WindesHeim_Game.Properties.Resources.ufonts_com_transitfront_negativ;
            int dataLength = global::WindesHeim_Game.Properties.Resources.ufonts_com_transitfront_negativ.Length;

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
            byte[] fontArray = global::WindesHeim_Game.Properties.Resources.ufonts_com_sheriff_roman;
            int dataLength = global::WindesHeim_Game.Properties.Resources.ufonts_com_sheriff_roman.Length;

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
            this.play = new System.Windows.Forms.PictureBox();
            this.editor = new System.Windows.Forms.PictureBox();
            this.highscore = new System.Windows.Forms.PictureBox();
            this.tempPlay = new System.Windows.Forms.PictureBox();

            this.backgroundImage = new Panel();
            this.backgroundImage.Size = gameWindow.Size;
            this.backgroundImage.BackgroundImage = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "..\\..\\resources\\menuBackground.png");

            this.play.Image = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "..\\..\\resources\\playButton.png");
            this.play.Location = new System.Drawing.Point(0, 0);
            this.play.Size = new System.Drawing.Size(304, 44);
            this.play.TabIndex = 0;
            this.play.Click += new EventHandler(menuController.play_Click);

            this.editor.Image = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "..\\..\\resources\\editorButton.png");
            this.editor.Location = new System.Drawing.Point(0, 60);
            this.editor.Size = new System.Drawing.Size(304, 44);
            this.editor.TabIndex = 1;
            this.editor.Click += new EventHandler(menuController.editor_Click);

            this.highscore.Image = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "..\\..\\resources\\highscoresButton.png");
            this.highscore.Location = new System.Drawing.Point(0, 120);
            this.highscore.Size = new System.Drawing.Size(304, 44);
            this.highscore.TabIndex = 2;
            this.highscore.Click += new EventHandler(menuController.highscore_Click);

            menuPanel = new Panel();
            menuPanel.AutoSize = true;
            menuPanel.BackColor = Color.Transparent;


            menuPanel.Controls.Add(play);
            menuPanel.Controls.Add(editor);
            menuPanel.Controls.Add(highscore);


            System.Console.WriteLine(gameWindow.Width);
            menuPanel.Location = new Point((gameWindow.Width / 2 - menuPanel.Size.Width / 2), (gameWindow.Height / 2 - menuPanel.Size.Height / 2));
            menuPanel.Anchor = AnchorStyles.None;
            this.backgroundImage.Controls.Add(menuPanel);
            gameWindow.Controls.Add(backgroundImage);
        }
    }

    public class ModelGame : Model
    {
        private ControllerGame gameController;

        //XML Gegevens van level worden hierin meeggegeven
        public static XMLParser level;

        // Houdt alle dynamische gameobjecten vast
        private List<GameObject> gameObjects = new List<GameObject>();

        // Er is maar 1 speler
        public Player player = new Player(new Point(10, 10), 40, 40);



        // Graphicspaneel
        public PictureBox graphicsPanel = new PictureBox();

        
        

        //INITIALISATIES CONTROLS
        //START OBSTACLEPANEL
        internal System.Windows.Forms.Panel obstaclePanel = new Panel();

        // START OBSTACLE 1
        private System.Windows.Forms.Panel pnlObstacle1 = new Panel();
        private System.Windows.Forms.Label lblObstacles = new Label();

        // Icon 1
        private System.Windows.Forms.Panel pnlObstacleIcon1 = new Panel();
        internal System.Windows.Forms.PictureBox pbObstacle1 = new PictureBox();

        //Obstacle Name 1
        internal System.Windows.Forms.Label lblObstacleName1 = new Label();

        //Obstacle Properties 1
        private System.Windows.Forms.Label lblObstacleProps1 = new Label();

        private System.Windows.Forms.Label lblObstaclePosXTitle1 = new Label();
        internal System.Windows.Forms.Label lblObstaclePosX1 = new Label();

        private System.Windows.Forms.Label lblObstaclePosYTitle1 = new Label();
        internal System.Windows.Forms.Label lblObstaclePosY1 = new Label();

        //Obstacle Description 1
        private System.Windows.Forms.Label lblObstacleDescTitle1 = new Label();
        internal System.Windows.Forms.Label lblObstacleDesc1 = new Label();
        // STOP OBSTACLE 1

        // START OBSTACLE 2
        private System.Windows.Forms.Panel pnlObstacle2 = new Panel();

        //Icon 2
        private System.Windows.Forms.Panel pnlObstacleIcon2 = new Panel();
        internal System.Windows.Forms.PictureBox pbObstacle2 = new PictureBox();

        //Obstacle Name 2
        internal System.Windows.Forms.Label lblObstacleName2 = new Label();

        //Obstacle Properties 2
        private System.Windows.Forms.Label lblObstacleProps2 = new Label();

        private System.Windows.Forms.Label lblObstaclePosXTitle2 = new Label();
        internal System.Windows.Forms.Label lblObstaclePosX2 = new Label();
        private System.Windows.Forms.Label lblObstaclePosYTitle2 = new Label();
        internal System.Windows.Forms.Label lblObstaclePosY2 = new Label();

        //Obstacle Description 2
        private System.Windows.Forms.Label lblObstacleDescTitle2 = new Label();
        internal System.Windows.Forms.Label lblObstacleDesc2 = new Label();
        // STOP OBSTACLE 2
        //STOP OBSTACLEPANEL


        //START LOGO PANEL
        private System.Windows.Forms.Panel logoPanel = new Panel();
        //STOP LOGO PANEL


        //START CHARACTER PANEL
        private System.Windows.Forms.Panel characterPanel = new Panel();

        // START CHARACTER
        private System.Windows.Forms.Panel pnlCharacter = new Panel();
        private System.Windows.Forms.Label lblCharacter = new Label();

        //Character Name
        private System.Windows.Forms.Label lblCharacterName = new Label();

        //Character Icon
        private System.Windows.Forms.Panel pnlCharacterIcon = new Panel();
        private System.Windows.Forms.PictureBox pbCharacter = new PictureBox();

        //Character Properties
        private System.Windows.Forms.Label lblCharacterProps = new Label();

        private System.Windows.Forms.Label lblCharacterPosXTitle = new Label();
        internal System.Windows.Forms.Label lblCharacterPosX = new Label();
        private System.Windows.Forms.Label lblCharacterPosYTitle = new Label();
        internal System.Windows.Forms.Label lblCharacterPosY = new Label();

        private System.Windows.Forms.Label lblCharacterSpeedTitle = new Label();
        internal System.Windows.Forms.Label lblCharacterSpeed = new Label();
        // STOP CHARACTER
        //STOP CHARACTER PANEL


        //START CONTROL PANEL
        private System.Windows.Forms.Panel controlPanel = new Panel();

        public System.Windows.Forms.PictureBox btnUp = new PictureBox();
        public System.Windows.Forms.PictureBox btnDown = new PictureBox();
        public System.Windows.Forms.PictureBox btnLeft = new PictureBox();
        public System.Windows.Forms.PictureBox btnRight = new PictureBox();
        //STOP CONTROL PANEL


        //START ACTION PANEL       
        private System.Windows.Forms.Panel actionPanel = new Panel();

        public System.Windows.Forms.PictureBox pbIconSound = new PictureBox();
        private System.Windows.Forms.PictureBox pbIconRestart = new PictureBox();
        private System.Windows.Forms.PictureBox pbIconMenu = new PictureBox();
        //STOP ACTION PANEL

        public ModelGame(ControllerGame controller) : base(controller)
        {
            this.gameController = controller;
        }

        public void InitializeField()
        {
            gameObjects.Clear();

            gameObjects = level.getCleanGameObjects();           
        }

        public override void ControlsInit(Form gameWindow)
        {

            // Registreer key events voor de player
            gameWindow.KeyDown += gameController.OnKeyDownWASD;
            gameWindow.KeyUp += gameController.OnKeyUp;

            // Voeg graphicspaneel toe voor het tekenen van gameobjecten
            graphicsPanel.BackColor = Color.LightGray;
            graphicsPanel.Location = new Point(0, 0);
            graphicsPanel.Size = new Size(845, 475);
            graphicsPanel.Paint += gameController.OnPaintEvent;

            // Overige panels
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GameWindow));

            //START OBSTACLE PANEL
            //Main ObstaclePanel
            this.obstaclePanel.BackColor = System.Drawing.SystemColors.Window;
            this.obstaclePanel.Controls.Add(this.pnlObstacle2);
            this.obstaclePanel.Controls.Add(this.pnlObstacle1);
            this.obstaclePanel.Controls.Add(this.lblObstacles);
            this.obstaclePanel.Location = new System.Drawing.Point(848, 0);
            this.obstaclePanel.Margin = new System.Windows.Forms.Padding(1);
            this.obstaclePanel.Name = "obstaclePanel";
            this.obstaclePanel.Size = new System.Drawing.Size(431, 475);
            this.obstaclePanel.TabIndex = 2;

            //Label Obstacle Title
            this.lblObstacles.AutoSize = true;
            this.lblObstacles.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(67)))), ((int)(((byte)(131)))));
            this.lblObstacles.Location = new System.Drawing.Point(3, 9);
            this.lblObstacles.Name = "lblObstacles";
            this.lblObstacles.Size = new System.Drawing.Size(54, 13);
            this.lblObstacles.TabIndex = 0;
            this.lblObstacles.Text = "Obstakels";

            //Panel Obstacle 1
            this.pnlObstacle1.BackColor = System.Drawing.SystemColors.ControlLight;
            this.pnlObstacle1.Controls.Add(this.lblObstacleDesc1);
            this.pnlObstacle1.Controls.Add(this.lblObstacleDescTitle1);
            this.pnlObstacle1.Controls.Add(this.lblObstaclePosY1);
            this.pnlObstacle1.Controls.Add(this.lblObstaclePosX1);
            this.pnlObstacle1.Controls.Add(this.lblObstaclePosYTitle1);
            this.pnlObstacle1.Controls.Add(this.lblObstaclePosXTitle1);
            this.pnlObstacle1.Controls.Add(this.pnlObstacleIcon1);
            this.pnlObstacle1.Controls.Add(this.lblObstacleProps1);
            this.pnlObstacle1.Controls.Add(this.lblObstacleName1);
            this.pnlObstacle1.Location = new System.Drawing.Point(8, 75);
            this.pnlObstacle1.Name = "pnlObstacle1";
            this.pnlObstacle1.Size = new System.Drawing.Size(412, 190);
            this.pnlObstacle1.TabIndex = 1;

            //Panel ObstacleIcon1
            this.pnlObstacleIcon1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(216)))), ((int)(((byte)(216)))), ((int)(((byte)(216)))));
            this.pnlObstacleIcon1.Controls.Add(this.pbObstacle1);
            this.pnlObstacleIcon1.Location = new System.Drawing.Point(7, 12);
            this.pnlObstacleIcon1.Name = "pnlObstacleIcon1";
            this.pnlObstacleIcon1.Size = new System.Drawing.Size(160, 160);
            this.pnlObstacleIcon1.TabIndex = 5;

            //Picturebox Obstacle 1
            this.pbObstacle1.BackgroundImage = global::WindesHeim_Game.Properties.Resources.bikeEdited;
            this.pbObstacle1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pbObstacle1.Location = new System.Drawing.Point(5, 5);
            this.pbObstacle1.Name = "pbObstacle1";
            this.pbObstacle1.Size = new System.Drawing.Size(150, 150);
            this.pbObstacle1.TabIndex = 0;
            this.pbObstacle1.TabStop = false;

            //Label Obstacle Name 1
            this.lblObstacleName1.AutoSize = true;
            this.lblObstacleName1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(67)))), ((int)(((byte)(131)))));
            this.lblObstacleName1.Location = new System.Drawing.Point(173, 12);
            this.lblObstacleName1.Name = "lblObstacleName1";
            this.lblObstacleName1.Size = new System.Drawing.Size(75, 13);
            this.lblObstacleName1.TabIndex = 3;
            this.lblObstacleName1.Text = "Obstakelnaam";

            //Label Obstacle Properties 1
            this.lblObstacleProps1.AutoSize = true;
            this.lblObstacleProps1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(213)))), ((int)(((byte)(161)))), ((int)(((byte)(15)))));
            this.lblObstacleProps1.Location = new System.Drawing.Point(173, 43);
            this.lblObstacleProps1.Name = "lblObstacleProps1";
            this.lblObstacleProps1.Size = new System.Drawing.Size(81, 13);
            this.lblObstacleProps1.TabIndex = 4;
            this.lblObstacleProps1.Text = "Eigenschappen";

            //Label Obstacle Postition X Title 1
            this.lblObstaclePosXTitle1.AutoSize = true;
            this.lblObstaclePosXTitle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblObstaclePosXTitle1.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.lblObstaclePosXTitle1.Location = new System.Drawing.Point(173, 63);
            this.lblObstaclePosXTitle1.Name = "lblObstaclePosXTitle1";
            this.lblObstaclePosXTitle1.Size = new System.Drawing.Size(71, 18);
            this.lblObstaclePosXTitle1.TabIndex = 6;
            this.lblObstaclePosXTitle1.Text = "Positie X:";

            //Label Obstacle Position X 1
            this.lblObstaclePosX1.AutoSize = true;
            this.lblObstaclePosX1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblObstaclePosX1.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.lblObstaclePosX1.Location = new System.Drawing.Point(250, 63);
            this.lblObstaclePosX1.Name = "lblObstaclePosX1";
            this.lblObstaclePosX1.Size = new System.Drawing.Size(23, 18);
            this.lblObstaclePosX1.TabIndex = 8;
            this.lblObstaclePosX1.Text = "int";

            //Label Obstacle Position Y Title 1
            this.lblObstaclePosYTitle1.AutoSize = true;
            this.lblObstaclePosYTitle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblObstaclePosYTitle1.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.lblObstaclePosYTitle1.Location = new System.Drawing.Point(173, 83);
            this.lblObstaclePosYTitle1.Name = "lblObstaclePosYTitle1";
            this.lblObstaclePosYTitle1.Size = new System.Drawing.Size(70, 18);
            this.lblObstaclePosYTitle1.TabIndex = 7;
            this.lblObstaclePosYTitle1.Text = "Positie Y:";

            //Label Obstacle Position Y 1
            this.lblObstaclePosY1.AutoSize = true;
            this.lblObstaclePosY1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblObstaclePosY1.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.lblObstaclePosY1.Location = new System.Drawing.Point(249, 83);
            this.lblObstaclePosY1.Name = "lblObstaclePosY1";
            this.lblObstaclePosY1.Size = new System.Drawing.Size(23, 18);
            this.lblObstaclePosY1.TabIndex = 9;
            this.lblObstaclePosY1.Text = "int";

            //Label Obstacle Description Title 1
            this.lblObstacleDescTitle1.AutoSize = true;
            this.lblObstacleDescTitle1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(213)))), ((int)(((byte)(161)))), ((int)(((byte)(15)))));
            this.lblObstacleDescTitle1.Location = new System.Drawing.Point(173, 116);
            this.lblObstacleDescTitle1.Margin = new System.Windows.Forms.Padding(0);
            this.lblObstacleDescTitle1.Name = "lblObstacleDescTitle1";
            this.lblObstacleDescTitle1.Size = new System.Drawing.Size(64, 13);
            this.lblObstacleDescTitle1.TabIndex = 10;
            this.lblObstacleDescTitle1.Text = "Beschrijving";

            //Label Obstacle Description 1
            this.lblObstacleDesc1.AutoSize = true;
            this.lblObstacleDesc1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblObstacleDesc1.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.lblObstacleDesc1.Location = new System.Drawing.Point(173, 136);
            this.lblObstacleDesc1.Margin = new System.Windows.Forms.Padding(0);
            this.lblObstacleDesc1.Name = "lblObstacleDesc1";
            this.lblObstacleDesc1.Size = new System.Drawing.Size(85, 18);
            this.lblObstacleDesc1.TabIndex = 15;
            this.lblObstacleDesc1.Text = "beschrijving";

            //Panel Obstacle 2 
            this.pnlObstacle2.BackColor = System.Drawing.SystemColors.ControlLight;
            this.pnlObstacle2.Controls.Add(this.lblObstacleDesc2);
            this.pnlObstacle2.Controls.Add(this.lblObstacleDescTitle2);
            this.pnlObstacle2.Controls.Add(this.lblObstaclePosY2);
            this.pnlObstacle2.Controls.Add(this.pnlObstacleIcon2);
            this.pnlObstacle2.Controls.Add(this.lblObstaclePosX2);
            this.pnlObstacle2.Controls.Add(this.lblObstacleProps2);
            this.pnlObstacle2.Controls.Add(this.lblObstaclePosYTitle2);
            this.pnlObstacle2.Controls.Add(this.lblObstacleName2);
            this.pnlObstacle2.Controls.Add(this.lblObstaclePosXTitle2);
            this.pnlObstacle2.Location = new System.Drawing.Point(8, 285);
            this.pnlObstacle2.Name = "pnlObstacle2";
            this.pnlObstacle2.Size = new System.Drawing.Size(412, 190);
            this.pnlObstacle2.TabIndex = 4;

            //Panel Obstacle Icon 2
            this.pnlObstacleIcon2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(216)))), ((int)(((byte)(216)))), ((int)(((byte)(216)))));
            this.pnlObstacleIcon2.Controls.Add(this.pbObstacle2);
            this.pnlObstacleIcon2.Location = new System.Drawing.Point(7, 12);
            this.pnlObstacleIcon2.Name = "pnlObstacleIcon2";
            this.pnlObstacleIcon2.Size = new System.Drawing.Size(160, 160);
            this.pnlObstacleIcon2.TabIndex = 6;

            //Picturebox Obstacle 2
            this.pbObstacle2.BackgroundImage = global::WindesHeim_Game.Properties.Resources.carEdited;
            this.pbObstacle2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pbObstacle2.Location = new System.Drawing.Point(5, 5);
            this.pbObstacle2.Name = "pbObstacle2";
            this.pbObstacle2.Size = new System.Drawing.Size(150, 150);
            this.pbObstacle2.TabIndex = 1;
            this.pbObstacle2.TabStop = false;

            //Label Obstacle Name 2
            this.lblObstacleName2.AutoSize = true;
            this.lblObstacleName2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(67)))), ((int)(((byte)(131)))));
            this.lblObstacleName2.Location = new System.Drawing.Point(173, 12);
            this.lblObstacleName2.Name = "lblObstacleName2";
            this.lblObstacleName2.Size = new System.Drawing.Size(75, 13);
            this.lblObstacleName2.TabIndex = 3;
            this.lblObstacleName2.Text = "Obstakelnaam";

            //Label Obstacle Properties 2
            this.lblObstacleProps2.AutoSize = true;
            this.lblObstacleProps2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(213)))), ((int)(((byte)(161)))), ((int)(((byte)(15)))));
            this.lblObstacleProps2.Location = new System.Drawing.Point(173, 43);
            this.lblObstacleProps2.Name = "lblObstacleProps2";
            this.lblObstacleProps2.Size = new System.Drawing.Size(81, 13);
            this.lblObstacleProps2.TabIndex = 5;
            this.lblObstacleProps2.Text = "Eigenschappen";

            //Label Obstacle Postition X Title 2
            this.lblObstaclePosXTitle2.AutoSize = true;
            this.lblObstaclePosXTitle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblObstaclePosXTitle2.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.lblObstaclePosXTitle2.Location = new System.Drawing.Point(174, 63);
            this.lblObstaclePosXTitle2.Name = "lblObstaclePosXTitle2";
            this.lblObstaclePosXTitle2.Size = new System.Drawing.Size(71, 18);
            this.lblObstaclePosXTitle2.TabIndex = 10;
            this.lblObstaclePosXTitle2.Text = "Positie X:";

            //Label Obstacle Position X 2
            this.lblObstaclePosX2.AutoSize = true;
            this.lblObstaclePosX2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblObstaclePosX2.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.lblObstaclePosX2.Location = new System.Drawing.Point(249, 63);
            this.lblObstaclePosX2.Name = "lblObstaclePosX2";
            this.lblObstaclePosX2.Size = new System.Drawing.Size(23, 18);
            this.lblObstaclePosX2.TabIndex = 12;
            this.lblObstaclePosX2.Text = "int";

            //Label Obstacle Position Y Title 2
            this.lblObstaclePosYTitle2.AutoSize = true;
            this.lblObstaclePosYTitle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblObstaclePosYTitle2.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.lblObstaclePosYTitle2.Location = new System.Drawing.Point(174, 83);
            this.lblObstaclePosYTitle2.Name = "lblObstaclePosYTitle2";
            this.lblObstaclePosYTitle2.Size = new System.Drawing.Size(70, 18);
            this.lblObstaclePosYTitle2.TabIndex = 11;
            this.lblObstaclePosYTitle2.Text = "Positie Y:";

            //Label Obstacle Position Y 2
            this.lblObstaclePosY2.AutoSize = true;
            this.lblObstaclePosY2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblObstaclePosY2.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.lblObstaclePosY2.Location = new System.Drawing.Point(249, 83);
            this.lblObstaclePosY2.Name = "lblObstaclePosY2";
            this.lblObstaclePosY2.Size = new System.Drawing.Size(23, 18);
            this.lblObstaclePosY2.TabIndex = 13;
            this.lblObstaclePosY2.Text = "int";

            //Label Obstacle Description Title 2
            this.lblObstacleDescTitle2.AutoSize = true;
            this.lblObstacleDescTitle2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(213)))), ((int)(((byte)(161)))), ((int)(((byte)(15)))));
            this.lblObstacleDescTitle2.Location = new System.Drawing.Point(173, 116);
            this.lblObstacleDescTitle2.Margin = new System.Windows.Forms.Padding(0);
            this.lblObstacleDescTitle2.Name = "lblObstacleDescTitle2";
            this.lblObstacleDescTitle2.Size = new System.Drawing.Size(64, 13);
            this.lblObstacleDescTitle2.TabIndex = 11;
            this.lblObstacleDescTitle2.Text = "Beschrijving";

            //Label Obstacle Description 2
            this.lblObstacleDesc2.AutoSize = true;
            this.lblObstacleDesc2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblObstacleDesc2.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.lblObstacleDesc2.Location = new System.Drawing.Point(173, 136);
            this.lblObstacleDesc2.Margin = new System.Windows.Forms.Padding(0);
            this.lblObstacleDesc2.Name = "lblObstacleDesc2";
            this.lblObstacleDesc2.Size = new System.Drawing.Size(85, 18);
            this.lblObstacleDesc2.TabIndex = 14;
            this.lblObstacleDesc2.Text = "beschrijving";
            //STOP OBSTACLE PANEL


            //START LOGO PANEL

            //Main Panel
            this.logoPanel.BackColor = System.Drawing.SystemColors.Window;
            this.logoPanel.BackgroundImage = global::WindesHeim_Game.Properties.Resources.ConceptTransparentBackgroundSmall;
            this.logoPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.logoPanel.Location = new System.Drawing.Point(848, 477);
            this.logoPanel.Margin = new System.Windows.Forms.Padding(1);
            this.logoPanel.Name = "logoPanel";
            this.logoPanel.Size = new System.Drawing.Size(431, 242);
            this.logoPanel.TabIndex = 0;
            //STOP LOGO PANEL


            //START CHARACTER PANEL

            //Main Panel
            this.characterPanel.BackColor = System.Drawing.SystemColors.Window;
            this.characterPanel.Controls.Add(this.pnlCharacter);
            this.characterPanel.Controls.Add(this.lblCharacter);
            this.characterPanel.Location = new System.Drawing.Point(475, 477);
            this.characterPanel.Margin = new System.Windows.Forms.Padding(1);
            this.characterPanel.Name = "characterPanel";
            this.characterPanel.Size = new System.Drawing.Size(372, 242);
            this.characterPanel.TabIndex = 5;

            //Label Character Title 
            this.lblCharacter.AutoSize = true;
            this.lblCharacter.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(67)))), ((int)(((byte)(131)))));
            this.lblCharacter.Location = new System.Drawing.Point(2, 2);
            this.lblCharacter.Name = "lblCharacter";
            this.lblCharacter.Size = new System.Drawing.Size(53, 13);
            this.lblCharacter.TabIndex = 0;
            this.lblCharacter.Text = "Character";

            //Panel Character
            this.pnlCharacter.BackColor = System.Drawing.SystemColors.ControlLight;
            this.pnlCharacter.Controls.Add(this.lblCharacterSpeed);
            this.pnlCharacter.Controls.Add(this.lblCharacterSpeedTitle);
            this.pnlCharacter.Controls.Add(this.lblCharacterPosY);
            this.pnlCharacter.Controls.Add(this.lblCharacterPosX);
            this.pnlCharacter.Controls.Add(this.lblCharacterPosYTitle);
            this.pnlCharacter.Controls.Add(this.lblCharacterPosXTitle);
            this.pnlCharacter.Controls.Add(this.pnlCharacterIcon);
            this.pnlCharacter.Controls.Add(this.lblCharacterProps);
            this.pnlCharacter.Controls.Add(this.lblCharacterName);
            this.pnlCharacter.Location = new System.Drawing.Point(5, 55);
            this.pnlCharacter.Name = "pnlCharacter";
            this.pnlCharacter.Size = new System.Drawing.Size(364, 179);
            this.pnlCharacter.TabIndex = 2;

            //Panel Character Icon
            this.pnlCharacterIcon.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(216)))), ((int)(((byte)(216)))), ((int)(((byte)(216)))));
            this.pnlCharacterIcon.Controls.Add(this.pbCharacter);
            this.pnlCharacterIcon.Location = new System.Drawing.Point(7, 12);
            this.pnlCharacterIcon.Name = "pnlCharacterIcon";
            this.pnlCharacterIcon.Size = new System.Drawing.Size(160, 160);
            this.pnlCharacterIcon.TabIndex = 5;

            //PictureBox Character
            this.pbCharacter.BackgroundImage = global::WindesHeim_Game.Properties.Resources.playerEdited1;
            this.pbCharacter.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pbCharacter.Location = new System.Drawing.Point(5, 5);
            this.pbCharacter.Name = "pbCharacter";
            this.pbCharacter.Size = new System.Drawing.Size(150, 150);
            this.pbCharacter.TabIndex = 0;
            this.pbCharacter.TabStop = false;

            //Label Character Name
            this.lblCharacterName.AutoSize = true;
            this.lblCharacterName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(67)))), ((int)(((byte)(131)))));
            this.lblCharacterName.Location = new System.Drawing.Point(173, 12);
            this.lblCharacterName.Name = "lblCharacterName";
            this.lblCharacterName.Size = new System.Drawing.Size(63, 13);
            this.lblCharacterName.TabIndex = 3;
            this.lblCharacterName.Text = "Spelernaam";

            //Label Character Properties
            this.lblCharacterProps.AutoSize = true;
            this.lblCharacterProps.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(213)))), ((int)(((byte)(161)))), ((int)(((byte)(15)))));
            this.lblCharacterProps.Location = new System.Drawing.Point(173, 43);
            this.lblCharacterProps.Name = "lblCharacterProps";
            this.lblCharacterProps.Size = new System.Drawing.Size(81, 13);
            this.lblCharacterProps.TabIndex = 4;
            this.lblCharacterProps.Text = "Eigenschappen";

            //Label Character Position X Title
            this.lblCharacterPosXTitle.AutoSize = true;
            this.lblCharacterPosXTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCharacterPosXTitle.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.lblCharacterPosXTitle.Location = new System.Drawing.Point(173, 63);
            this.lblCharacterPosXTitle.Name = "lblCharacterPosXTitle";
            this.lblCharacterPosXTitle.Size = new System.Drawing.Size(71, 18);
            this.lblCharacterPosXTitle.TabIndex = 6;
            this.lblCharacterPosXTitle.Text = "Positie X:";

            //Label Character Position X
            this.lblCharacterPosX.AutoSize = true;
            this.lblCharacterPosX.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.lblCharacterPosX.Location = new System.Drawing.Point(250, 63);
            this.lblCharacterPosX.Name = "lblCharacterPosX";
            this.lblCharacterPosX.Size = new System.Drawing.Size(23, 18);
            this.lblCharacterPosX.TabIndex = 8;
            this.lblCharacterPosX.Text = "int";

            //Label Character Position Y Title
            this.lblCharacterPosYTitle.AutoSize = true;
            this.lblCharacterPosYTitle.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.lblCharacterPosYTitle.Location = new System.Drawing.Point(173, 83);
            this.lblCharacterPosYTitle.Name = "lblCharacterPosYTitle";
            this.lblCharacterPosYTitle.Size = new System.Drawing.Size(70, 18);
            this.lblCharacterPosYTitle.TabIndex = 7;
            this.lblCharacterPosYTitle.Text = "Positie Y:";

            //Label Character Position Y
            this.lblCharacterPosY.AutoSize = true;
            this.lblCharacterPosY.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCharacterPosY.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.lblCharacterPosY.Location = new System.Drawing.Point(249, 83);
            this.lblCharacterPosY.Name = "lblCharacterPosY";
            this.lblCharacterPosY.Size = new System.Drawing.Size(23, 18);
            this.lblCharacterPosY.TabIndex = 9;
            this.lblCharacterPosY.Text = "int";

            //Label Character Speed Title
            this.lblCharacterSpeedTitle.AutoSize = true;
            this.lblCharacterSpeedTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCharacterSpeedTitle.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.lblCharacterSpeedTitle.Location = new System.Drawing.Point(173, 114);
            this.lblCharacterSpeedTitle.Name = "lblCharacterSpeedTitle";
            this.lblCharacterSpeedTitle.Size = new System.Drawing.Size(68, 18);
            this.lblCharacterSpeedTitle.TabIndex = 10;
            this.lblCharacterSpeedTitle.Text = "Snelheid:";

            //Label Character Speed
            this.lblCharacterSpeed.AutoSize = true;
            this.lblCharacterSpeed.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCharacterSpeed.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.lblCharacterSpeed.Location = new System.Drawing.Point(249, 114);
            this.lblCharacterSpeed.Name = "lblCharacterSpeed";
            this.lblCharacterSpeed.Size = new System.Drawing.Size(23, 18);
            this.lblCharacterSpeed.TabIndex = 11;
            this.lblCharacterSpeed.Text = "int";
            //STOP CHARACTER PANEL

            //Label Character Speed
            this.lblCharacterSpeed.AutoSize = true;
            this.lblCharacterSpeed.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.lblCharacterSpeed.Location = new System.Drawing.Point(249, 114);
            this.lblCharacterSpeed.Name = "lblCharacterSpeed";
            this.lblCharacterSpeed.Size = new System.Drawing.Size(23, 18);
            this.lblCharacterSpeed.TabIndex = 11;
            this.lblCharacterSpeed.Text = "int";
            //STOP CHARACTER PANEL

            //START CONTROL PANEL

            //Main Panel
            this.controlPanel.BackColor = System.Drawing.SystemColors.Window;
            this.controlPanel.Controls.Add(this.btnLeft);
            this.controlPanel.Controls.Add(this.btnRight);
            this.controlPanel.Controls.Add(this.btnUp);
            this.controlPanel.Controls.Add(this.btnDown);
            this.controlPanel.Location = new System.Drawing.Point(101, 477);
            this.controlPanel.Margin = new System.Windows.Forms.Padding(1);
            this.controlPanel.Name = "controlPanel";
            this.controlPanel.Size = new System.Drawing.Size(372, 242);
            this.controlPanel.TabIndex = 4;

            //Button Up
            this.btnUp.BackgroundImage = global::WindesHeim_Game.Properties.Resources.Up;
            this.btnUp.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnUp.Location = new System.Drawing.Point(136, 13);
            this.btnUp.Margin = new System.Windows.Forms.Padding(0);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(100, 100);
            this.btnUp.TabIndex = 1;
            //this.btnUp.UseVisualStyleBackColor = true;

            //Button Down
            this.btnDown.BackgroundImage = global::WindesHeim_Game.Properties.Resources.Down;
            this.btnDown.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnDown.Location = new System.Drawing.Point(136, 134);
            this.btnDown.Margin = new System.Windows.Forms.Padding(0);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(100, 100);
            this.btnDown.TabIndex = 0;
            //this.btnDown.UseVisualStyleBackColor = true;

            //Button Left
            this.btnLeft.BackgroundImage = global::WindesHeim_Game.Properties.Resources.Left;
            this.btnLeft.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnLeft.Location = new System.Drawing.Point(21, 134);
            this.btnLeft.Margin = new System.Windows.Forms.Padding(0);
            this.btnLeft.Name = "btnLeft";
            this.btnLeft.Size = new System.Drawing.Size(100, 100);
            this.btnLeft.TabIndex = 3;
            //this.btnLeft.UseVisualStyleBackColor = true;

            //Button Right
            this.btnRight.BackgroundImage = global::WindesHeim_Game.Properties.Resources.Right;
            this.btnRight.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnRight.Location = new System.Drawing.Point(250, 134);
            this.btnRight.Margin = new System.Windows.Forms.Padding(0);
            this.btnRight.Name = "btnRight";
            this.btnRight.Size = new System.Drawing.Size(100, 100);
            this.btnRight.TabIndex = 2;
            //this.btnRight.UseVisualStyleBackColor = true;
            //STOP CONTROL PANEL


            //START ACTION PANEL

            //Main Panel
            this.actionPanel.BackColor = System.Drawing.SystemColors.Window;
            this.actionPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.actionPanel.Controls.Add(this.pbIconMenu);
            this.actionPanel.Controls.Add(this.pbIconRestart);
            this.actionPanel.Controls.Add(this.pbIconSound);
            this.actionPanel.Location = new System.Drawing.Point(-11, 467);
            this.actionPanel.Margin = new System.Windows.Forms.Padding(1);
            this.actionPanel.Name = "actionPanel";
            this.actionPanel.Size = new System.Drawing.Size(111, 284);
            this.actionPanel.TabIndex = 3;

            //Picturebox Action Sound
            this.pbIconSound.BackgroundImage = global::WindesHeim_Game.Properties.Resources.soundEdited;
            this.pbIconSound.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pbIconSound.Location = new System.Drawing.Point(34, 22);
            this.pbIconSound.Name = "pbIconSound";
            this.pbIconSound.Size = new System.Drawing.Size(50, 50);
            this.pbIconSound.TabIndex = 6;
            this.pbIconSound.TabStop = false;
            this.pbIconSound.Click += new System.EventHandler(gameController.pbIconSound_Click);
            this.pbIconSound.MouseEnter += new System.EventHandler(gameController.SoundHoverEnter);
            this.pbIconSound.MouseLeave += new System.EventHandler(gameController.SoundHoverLeave);

            //Picturebox Action Restart
            this.pbIconRestart.BackgroundImage = global::WindesHeim_Game.Properties.Resources.restartEdited;
            this.pbIconRestart.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pbIconRestart.Location = new System.Drawing.Point(34, 106);
            this.pbIconRestart.Name = "pbIconRestart";
            this.pbIconRestart.Size = new System.Drawing.Size(50, 50);
            this.pbIconRestart.TabIndex = 7;
            this.pbIconRestart.TabStop = false;
            this.pbIconRestart.MouseEnter += new System.EventHandler(this.RestartHoverEnter);
            this.pbIconRestart.MouseClick += new System.Windows.Forms.MouseEventHandler(gameController.RestartClicked);
            this.pbIconRestart.MouseLeave += new System.EventHandler(this.RestartHoverLeave);

            //Picturebox Action Menu
            this.pbIconMenu.BackgroundImage = global::WindesHeim_Game.Properties.Resources.menuEdited;
            this.pbIconMenu.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pbIconMenu.Location = new System.Drawing.Point(34, 190);
            this.pbIconMenu.Name = "pbIconMenu";
            this.pbIconMenu.Size = new System.Drawing.Size(50, 50);
            this.pbIconMenu.TabIndex = 8;
            this.pbIconMenu.TabStop = false;
            this.pbIconMenu.MouseEnter += new System.EventHandler(this.MenuHoverEnter);
            this.pbIconMenu.MouseClick += new System.Windows.Forms.MouseEventHandler(gameController.MenuClicked);
            this.pbIconMenu.MouseLeave += new System.EventHandler(this.MenuHoverLeave);
            //STOP ACTION PANEL


            // Voeg hieronder de overige panels toe, zoals objectbeschrijvingen etc.
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
        public Button goBack;
        public Button playLevel;
        private Label labelLevels;
        private Label labelLevelPreview;
        public Panel alignPanel;
        public Panel gamePanel;

        private ControllerLevelSelect levelSelectController;

        public ModelLevelSelect(ControllerLevelSelect controller) : base(controller)
        {
            this.levelSelectController = controller;
        }

        public override void ControlsInit(Form gameWindow)
        {
            alignPanel = new Panel();
            alignPanel.AutoSize = true;

            gamePanel = new Panel();
            gamePanel.Location = new System.Drawing.Point(210, 40);
            gamePanel.Size = new System.Drawing.Size(845, 475);
            gamePanel.BackColor = Color.DarkGray;
            gamePanel.Paint += levelSelectController.OnPreviewPaint;

            listBoxLevels = new ListBox();
            listBoxLevels.Size = new System.Drawing.Size(200, 475);
            listBoxLevels.Location = new System.Drawing.Point(0, 40);

            XMLParser.LoadAllLevels();
            foreach (XMLParser xml in XMLParser.Levels)
            {
                listBoxLevels.Items.Add(xml);
            }
            listBoxLevels.SelectedIndexChanged += levelSelectController.level_Select;
            listBoxLevels.SetSelected(0, true);

            labelLevels = new Label();
            labelLevels.Text = "Levels";
            labelLevels.Font = new Font("Arial", 20);
            labelLevels.Location = new System.Drawing.Point(0, 0);
            labelLevels.Size = new System.Drawing.Size(200, 30);
            labelLevels.TextAlign = ContentAlignment.MiddleCenter;

            labelLevelPreview = new Label();
            labelLevelPreview.Text = "Level Preview";
            labelLevelPreview.Font = new Font("Arial", 20);
            labelLevelPreview.Location = new System.Drawing.Point(210, 0);
            labelLevelPreview.Size = new System.Drawing.Size(845, 30);
            labelLevelPreview.TextAlign = ContentAlignment.MiddleCenter;

            goBack = new Button();
            goBack.Size = new System.Drawing.Size(200, 25);
            goBack.Location = new System.Drawing.Point(0, 525);
            goBack.Text = "Go Back";
            goBack.Click += levelSelectController.goBack_Click;

            playLevel = new Button();
            playLevel.Size = new System.Drawing.Size(845, 25);
            playLevel.Location = new System.Drawing.Point(210, 525);
            playLevel.Text = "Play Level";
            playLevel.Click += levelSelectController.playLevel_Click;

            gameWindow.Controls.Add(alignPanel);
            alignPanel.Controls.Add(labelLevels);
            alignPanel.Controls.Add(labelLevelPreview);
            alignPanel.Controls.Add(goBack);
            alignPanel.Controls.Add(playLevel);
            alignPanel.Controls.Add(listBoxLevels);
            alignPanel.Controls.Add(gamePanel);
            alignPanel.Location = new Point(
                (gameWindow.Width / 2 - alignPanel.Size.Width / 2),
                (gameWindow.Height / 2 - alignPanel.Size.Height / 2));
            alignPanel.Anchor = AnchorStyles.None;

        }
    }

    public class ModelHighscores : Model
    {
        private ListBox listBoxLevels;
        private Button goBack;
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

            this.backgroundImage = new Panel();
            this.backgroundImage.Size = gameWindow.Size;
            this.backgroundImage.BackgroundImage = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "..\\..\\resources\\menuBackground.png");

            listBoxLevels = new ListBox();
            listBoxLevels.Size = new System.Drawing.Size(200, 200);
            listBoxLevels.Location = new System.Drawing.Point(0, 0);
            listBoxLevels.SelectedIndexChanged += highscoresController.level_Select;

            listBoxHighscores = new ListBox();
            listBoxHighscores.Size = new System.Drawing.Size(200, 200);
            listBoxHighscores.Location = new System.Drawing.Point(200, 0);

            XMLParser.LoadAllLevels();
            foreach (XMLParser xml in XMLParser.Levels)
            {
                this.levels.Add(xml); //Ingeladen gegevens opslaan in lokale List voor hergebruik
                listBoxLevels.Items.Add(xml);
            }
            listBoxLevels.SetSelected(0, true);

            goBack = new Button();
            goBack.Size = new System.Drawing.Size(200, 25);
            goBack.Location = new System.Drawing.Point(0, 210);
            goBack.Text = "Go Back";
            goBack.Click += new EventHandler(highscoresController.goBack_Click);

            gameWindow.Controls.Add(backgroundImage);
            backgroundImage.Controls.Add(alignPanel);
            alignPanel.Controls.Add(goBack);
            alignPanel.Controls.Add(listBoxLevels);
            alignPanel.Controls.Add(listBoxHighscores);

            alignPanel.Location = new Point(
                (gameWindow.Width / 2 - alignPanel.Size.Width / 2),
                (gameWindow.Height / 2 - alignPanel.Size.Height / 2));
            alignPanel.Anchor = AnchorStyles.None;
        }
    }

    public class ModelEditorSelect : Model
    {
        public ListBox listBoxLevels;
        public Button goBack;
        public Button editLevel;
        public Button newLevel;
        private Label labelLevels;
        private Label labelLevelPreview;
        public Panel alignPanel;
        public Panel gamePanel;

        private ControllerEditorSelect editorSelectController;

        public ModelEditorSelect(ControllerEditorSelect controller) : base(controller)
        {
            this.editorSelectController = controller;
        }

        public override void ControlsInit(Form gameWindow)
        {
            alignPanel = new Panel();
            alignPanel.AutoSize = true;

            gamePanel = new Panel();
            gamePanel.Location = new System.Drawing.Point(210, 40);
            gamePanel.Size = new System.Drawing.Size(845, 475);
            gamePanel.BackColor = Color.DarkGray;
            gamePanel.Paint += editorSelectController.OnPreviewPaint;

            listBoxLevels = new ListBox();
            listBoxLevels.Size = new System.Drawing.Size(200, 475);
            listBoxLevels.Location = new System.Drawing.Point(0, 40);

            XMLParser.LoadAllLevels();
            foreach (XMLParser xml in XMLParser.Levels)
            {
                listBoxLevels.Items.Add(xml);
            }

            listBoxLevels.SelectedIndexChanged += editorSelectController.level_Select;
            listBoxLevels.SetSelected(0, true);

            labelLevels = new Label();
            labelLevels.Text = "Levels";
            labelLevels.Font = new Font("Arial", 20);
            labelLevels.Location = new System.Drawing.Point(0, 0);
            labelLevels.Size = new System.Drawing.Size(200, 30);
            labelLevels.TextAlign = ContentAlignment.MiddleCenter;

            labelLevelPreview = new Label();
            labelLevelPreview.Text = "Level Preview";
            labelLevelPreview.Font = new Font("Arial", 20);
            labelLevelPreview.Location = new System.Drawing.Point(210, 0);
            labelLevelPreview.Size = new System.Drawing.Size(845, 30);
            labelLevelPreview.TextAlign = ContentAlignment.MiddleCenter;

            goBack = new Button();
            goBack.Size = new System.Drawing.Size(200, 25);
            goBack.Location = new System.Drawing.Point(0, 525);
            goBack.Text = "Go Back";
            goBack.Click += editorSelectController.goBack_Click;

            editLevel = new Button();
            editLevel.Size = new System.Drawing.Size(422, 25);
            editLevel.Location = new System.Drawing.Point(210, 525);
            editLevel.Text = "Edit Level";
            editLevel.Click += editorSelectController.editLevel_Click;

            newLevel = new Button();
            newLevel.Size = new System.Drawing.Size(422, 25);
            newLevel.Location = new System.Drawing.Point(632, 525);
            newLevel.Text = "New Level";
            newLevel.Click += editorSelectController.newLevel_Click;

            gameWindow.Controls.Add(alignPanel);
            alignPanel.Controls.Add(labelLevels);
            alignPanel.Controls.Add(labelLevelPreview);
            alignPanel.Controls.Add(goBack);
            alignPanel.Controls.Add(editLevel);
            alignPanel.Controls.Add(newLevel);
            alignPanel.Controls.Add(listBoxLevels);
            alignPanel.Controls.Add(gamePanel);
            alignPanel.Location = new Point(
                (gameWindow.Width / 2 - alignPanel.Size.Width / 2),
                (gameWindow.Height / 2 - alignPanel.Size.Height / 2));
            alignPanel.Anchor = AnchorStyles.None;

        }
    }

    public class ModelEditor : Model
    {
        public ListBox listBoxLevels;
        public Button goBack;
        public Button playLevel;
        public Button undoButton;
        public Button clearButton;
        public Panel alignPanel;
        public Panel gamePanel;
        public PictureBox staticObstacle;
        public PictureBox explodingObstacle;
        public PictureBox movingExplodingObstacle;
        public PictureBox slowingObstacle;
        private Label dragDropLabel;


        //XML Gegevens van level worden hierin meeggegeven
        public static XMLParser level;

        private ControllerEditor editorController;

        public ModelEditor(ControllerEditor controller) : base(controller)
        {
            this.editorController = controller;
        }

        public override void ControlsInit(Form gameWindow)
        {
            dragDropLabel = new Label();
            dragDropLabel.Text = "Drag en drop";
            dragDropLabel.Font = new Font("Arial", 12);
            dragDropLabel.Location = new System.Drawing.Point(920, 50);
            dragDropLabel.Size = new System.Drawing.Size(200, 30);

            gamePanel = new Panel();
            gamePanel.Location = new System.Drawing.Point(0, 0);
            gamePanel.Size = new System.Drawing.Size(845, 475);
            gamePanel.BackColor = Color.DarkGray;
            gamePanel.Paint += new PaintEventHandler(editorController.GamePanel_Paint);

            goBack = new Button();
            goBack.Size = new System.Drawing.Size(200, 25);
            goBack.Location = new System.Drawing.Point(0, 525);
            goBack.Text = "Go Back";
            goBack.Click += editorController.goBack_Click;

            playLevel = new Button();
            playLevel.Size = new System.Drawing.Size(422, 25);
            playLevel.Location = new System.Drawing.Point(210, 525);
            playLevel.Text = "Test Level";
            playLevel.Click += editorController.playLevel_Click;

            undoButton = new Button();
            undoButton.Size = new System.Drawing.Size(100, 25);
            undoButton.Location = new System.Drawing.Point(920, 287);
            undoButton.Text = "Undo";
            undoButton.Click += editorController.undoLastChange_Click;

            clearButton = new Button();
            clearButton.Size = new System.Drawing.Size(100, 25);
            clearButton.Location = new System.Drawing.Point(920, 337);
            clearButton.Text = "Clear";
            clearButton.Click += editorController.clearAll_Click;

            staticObstacle = new PictureBox();
            //staticObstacle.AllowDrop = true;
            staticObstacle.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            staticObstacle.Image = Resources.IconTC;
            staticObstacle.Location = new System.Drawing.Point(920, 87);
            staticObstacle.Name = "staticObstacle";
            staticObstacle.Size = new System.Drawing.Size(40, 40);
            staticObstacle.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            staticObstacle.TabIndex = 999;
            staticObstacle.TabStop = false;
            staticObstacle.MouseDown += editorController.updateMousePosition;
            staticObstacle.MouseMove += editorController.updateDragPosition;
            staticObstacle.MouseUp += editorController.StaticObstacle_MouseUp;

            explodingObstacle = new PictureBox();
            explodingObstacle.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            explodingObstacle.Image = Resources.IconCar;
            explodingObstacle.Location = new System.Drawing.Point(920, 137);
            explodingObstacle.Name = "explodingObstacle";
            explodingObstacle.Size = new System.Drawing.Size(40, 40);
            explodingObstacle.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            explodingObstacle.TabIndex = 999;
            explodingObstacle.TabStop = false;
            explodingObstacle.MouseDown += editorController.updateMousePosition;
            explodingObstacle.MouseMove += editorController.updateDragPosition;
            explodingObstacle.MouseUp += editorController.ExplodingObstacle_MouseUp;

            movingExplodingObstacle = new PictureBox();
            movingExplodingObstacle.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            movingExplodingObstacle.Image = Resources.IconBike;
            movingExplodingObstacle.Location = new System.Drawing.Point(920, 187);
            movingExplodingObstacle.Name = "movingExplodingObstacle";
            movingExplodingObstacle.Size = new System.Drawing.Size(40, 40);
            movingExplodingObstacle.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            movingExplodingObstacle.TabIndex = 999;
            movingExplodingObstacle.TabStop = false;
            movingExplodingObstacle.MouseDown += editorController.updateMousePosition;
            movingExplodingObstacle.MouseMove += editorController.updateDragPosition;
            movingExplodingObstacle.MouseUp += editorController.MovingExplodingObstacle_MouseUp;

            slowingObstacle = new PictureBox();
            slowingObstacle.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            slowingObstacle.Image = Resources.IconES;
            slowingObstacle.Location = new System.Drawing.Point(920, 237);
            slowingObstacle.Name = "slowingObstacle";
            slowingObstacle.Size = new System.Drawing.Size(40, 40);
            slowingObstacle.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            slowingObstacle.TabIndex = 999;
            slowingObstacle.TabStop = false;
            slowingObstacle.MouseDown += editorController.updateMousePosition;
            slowingObstacle.MouseMove += editorController.updateDragPosition;
            slowingObstacle.MouseUp += editorController.SlowingObstacle_MouseUp;

            gameWindow.Controls.Add(staticObstacle);
            gameWindow.Controls.Add(explodingObstacle);
            gameWindow.Controls.Add(movingExplodingObstacle);
            gameWindow.Controls.Add(slowingObstacle);
            gameWindow.Controls.Add(gamePanel);
            gameWindow.Controls.Add(goBack);
            gameWindow.Controls.Add(playLevel);
            gameWindow.Controls.Add(undoButton);
            gameWindow.Controls.Add(clearButton);
            gameWindow.Controls.Add(dragDropLabel);
        }
    }
}
