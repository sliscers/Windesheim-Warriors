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

        private Button play;
        private Button editor;
        private Button highscore;
        private Button tempPlay;

        public ModelMenu(ControllerMenu controller) : base(controller)
        {
            this.menuController = controller;
        }

        public override void ControlsInit(Form gameWindow)
        {
            this.play = new System.Windows.Forms.Button();
            this.editor = new System.Windows.Forms.Button();
            this.highscore = new System.Windows.Forms.Button();
            this.tempPlay = new System.Windows.Forms.Button();

            this.play.Location = new System.Drawing.Point(254, 52);
            this.play.Name = "play";
            this.play.Size = new System.Drawing.Size(259, 33);
            this.play.TabIndex = 0;
            this.play.Text = "Play";
            this.play.UseVisualStyleBackColor = true;

            this.editor.Location = new System.Drawing.Point(254, 91);
            this.editor.Name = "editor";
            this.editor.Size = new System.Drawing.Size(259, 33);
            this.editor.TabIndex = 1;
            this.editor.Text = "Editor";
            this.editor.UseVisualStyleBackColor = true;

            this.highscore.Location = new System.Drawing.Point(254, 130);
            this.highscore.Name = "highscore";
            this.highscore.Size = new System.Drawing.Size(259, 33);
            this.highscore.TabIndex = 2;
            this.highscore.Text = "highscore";
            this.highscore.UseVisualStyleBackColor = true;

            this.tempPlay.Location = new System.Drawing.Point(254, 169);
            this.tempPlay.Name = "play test";
            this.tempPlay.Size = new System.Drawing.Size(259, 33);
            this.tempPlay.TabIndex = 2;
            this.tempPlay.Text = "play test";
            this.tempPlay.UseVisualStyleBackColor = true;

            this.play.Click += new EventHandler(menuController.play_Click);
            this.highscore.Click += new EventHandler(menuController.highscore_Click);
            this.tempPlay.Click += new EventHandler(menuController.button_Click);

            this.highscore.Click += new EventHandler(menuController.highscore_Click);

            gameWindow.BackgroundImage = Resources.menuBackground;
            gameWindow.Controls.Add(play);
            gameWindow.Controls.Add(editor);
            gameWindow.Controls.Add(highscore);
            gameWindow.Controls.Add(tempPlay);
        }
    }

    public class ModelGame : Model
    {
        private ControllerGame gameController;

        // Houdt alle dynamische gameobjecten vast
        private List<GameObject> gameObjects = new List<GameObject>();
        
        // Er is maar 1 speler
        public Player player = new Player(new Point(10, 10), 40, 40);

        // Graphicspaneel
        public PictureBox graphicsPanel = new PictureBox();

        private Boolean mute = false;

        private Color windesheimBlauw = Color.FromArgb(0, 67, 131);

        private System.Windows.Forms.Panel logoPanel;
        private System.Windows.Forms.Panel obstaclePanel;
        private System.Windows.Forms.Panel actionPanel;
        private System.Windows.Forms.Panel controlPanel;
        private System.Windows.Forms.Panel characterPanel;
        private System.Windows.Forms.Label lblObstacles;
        private System.Windows.Forms.Label lblCharacter;
        private System.Windows.Forms.Button btnLeft;
        private System.Windows.Forms.Button btnDown;
        private System.Windows.Forms.Panel pnlObstacle1;
        private System.Windows.Forms.Label lblObstacleName1;
        private System.Windows.Forms.Panel pnlObstacle2;
        private System.Windows.Forms.Label lblObstacleName2;
        private System.Windows.Forms.PictureBox pbIconMenu;
        private System.Windows.Forms.PictureBox pbIconRestart;
        private System.Windows.Forms.PictureBox pbIconSound;
        private System.Windows.Forms.Label lblObstacleProps1;
        private System.Windows.Forms.Label lblObstacleProps2;
        private System.Windows.Forms.Panel pnlObstacleIcon2;
        private System.Windows.Forms.PictureBox pbObstacle2;
        private System.Windows.Forms.Panel pnlObstacleIcon1;
        private System.Windows.Forms.PictureBox pbObstacle1;
        private System.Windows.Forms.Label lblObstaclePosXTitle1;
        private System.Windows.Forms.Label lblObstaclePosYTitle1;
        private System.Windows.Forms.Label lblObstaclePosX1;
        private System.Windows.Forms.Label lblObstaclePosY2;
        private System.Windows.Forms.Label lblObstaclePosX2;
        private System.Windows.Forms.Label lblObstaclePosYTitle2;
        private System.Windows.Forms.Label lblObstaclePosXTitle2;
        private System.Windows.Forms.Label lblObstaclePosY1;
        private System.Windows.Forms.Label lblObstacleDescTitle1;
        private System.Windows.Forms.Label lblObstacleDesc2;
        private System.Windows.Forms.Label lblObstacleDescTitle2;
        private System.Windows.Forms.Label lblObstacleDesc1;
        private System.Windows.Forms.Button btnRight;
        private System.Windows.Forms.Button btnUp;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblCharacterPosY;
        private System.Windows.Forms.Label lblCharacterPosX;
        private System.Windows.Forms.Label lblCharacterPosYTitle;
        private System.Windows.Forms.Label lblCharacterPosXTitle;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label lblCharacterProps;
        private System.Windows.Forms.Label lblCharacterName;
        private System.Windows.Forms.Label lblCharacterSpeed;
        private System.Windows.Forms.Label lblCharacterSpeedTitle;





        public ModelGame(ControllerGame controller) : base(controller)
        {
            this.gameController = controller;

            InitializeField();
        }
            
        public void InitializeField()
        {
            gameObjects.Clear();

            // Toevoegen aan list, zodat we het kunnen volgen
            gameObjects.Add(new MovingExplodingObstacle(new Point(520, 20), 40, 40));
            gameObjects.Add(new StaticObstacle(new Point(150, 200), 40, 40));
            gameObjects.Add(new ExplodingObstacle(new Point(380, 400), 40, 40));
            gameObjects.Add(new SlowingObstacle(new Point(420, 100), 40, 40));
            gameObjects.Add(new Checkpoint(new Point(10, 10), AppDomain.CurrentDomain.BaseDirectory + "..\\..\\resources\\IconWIN.png", 40, 40));
            gameObjects.Add(new Checkpoint(new Point(720, 720), AppDomain.CurrentDomain.BaseDirectory + "..\\..\\resources\\IconSP.png", 40, 40));
        }

        public override void ControlsInit(Form gameWindow)
        {
            // Registreer key events voor de player
            gameWindow.KeyDown += gameController.OnKeyDownWASD;
            gameWindow.KeyUp += gameController.OnKeyUp;

            // Voeg graphicspaneel toe voor het tekenen van gameobjecten
            graphicsPanel.BackColor = Color.LightGray;
            graphicsPanel.Location = new Point(0, 0);
            graphicsPanel.Size = new Size(845, 480);
            graphicsPanel.Paint += gameController.OnPaintEvent;

            // Overige panels

            //obstaclePanel1.Location = new Point(845, 0);
            //obstaclePanel1.Size = new Size(445, 240);
            //obstaclePanel1.BackColor = Color.White;




            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GameWindow));
            this.obstaclePanel = new System.Windows.Forms.Panel();
            this.pnlObstacle2 = new System.Windows.Forms.Panel();
            this.lblObstacleDesc2 = new System.Windows.Forms.Label();
            this.lblObstacleDescTitle2 = new System.Windows.Forms.Label();
            this.lblObstaclePosY2 = new System.Windows.Forms.Label();
            this.pnlObstacleIcon2 = new System.Windows.Forms.Panel();
            this.pbObstacle2 = new System.Windows.Forms.PictureBox();
            this.lblObstaclePosX2 = new System.Windows.Forms.Label();
            this.lblObstacleProps2 = new System.Windows.Forms.Label();
            this.lblObstaclePosYTitle2 = new System.Windows.Forms.Label();
            this.lblObstacleName2 = new System.Windows.Forms.Label();
            this.lblObstaclePosXTitle2 = new System.Windows.Forms.Label();
            this.pnlObstacle1 = new System.Windows.Forms.Panel();
            this.lblObstacleDesc1 = new System.Windows.Forms.Label();
            this.lblObstacleDescTitle1 = new System.Windows.Forms.Label();
            this.lblObstaclePosY1 = new System.Windows.Forms.Label();
            this.lblObstaclePosX1 = new System.Windows.Forms.Label();
            this.lblObstaclePosYTitle1 = new System.Windows.Forms.Label();
            this.lblObstaclePosXTitle1 = new System.Windows.Forms.Label();
            this.pnlObstacleIcon1 = new System.Windows.Forms.Panel();
            this.pbObstacle1 = new System.Windows.Forms.PictureBox();
            this.lblObstacleProps1 = new System.Windows.Forms.Label();
            this.lblObstacleName1 = new System.Windows.Forms.Label();
            this.lblObstacles = new System.Windows.Forms.Label();
            this.actionPanel = new System.Windows.Forms.Panel();
            this.pbIconMenu = new System.Windows.Forms.PictureBox();
            this.pbIconRestart = new System.Windows.Forms.PictureBox();
            this.pbIconSound = new System.Windows.Forms.PictureBox();
            this.controlPanel = new System.Windows.Forms.Panel();
            this.btnLeft = new System.Windows.Forms.Button();
            this.btnRight = new System.Windows.Forms.Button();
            this.btnUp = new System.Windows.Forms.Button();
            this.btnDown = new System.Windows.Forms.Button();
            this.characterPanel = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblCharacterSpeed = new System.Windows.Forms.Label();
            this.lblCharacterSpeedTitle = new System.Windows.Forms.Label();
            this.lblCharacterPosY = new System.Windows.Forms.Label();
            this.lblCharacterPosX = new System.Windows.Forms.Label();
            this.lblCharacterPosYTitle = new System.Windows.Forms.Label();
            this.lblCharacterPosXTitle = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lblCharacterProps = new System.Windows.Forms.Label();
            this.lblCharacterName = new System.Windows.Forms.Label();
            this.lblCharacter = new System.Windows.Forms.Label();
            this.logoPanel = new System.Windows.Forms.Panel();
            this.obstaclePanel.SuspendLayout();
            this.pnlObstacle2.SuspendLayout();
            this.pnlObstacleIcon2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbObstacle2)).BeginInit();
            this.pnlObstacle1.SuspendLayout();
            this.pnlObstacleIcon1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbObstacle1)).BeginInit();
            this.actionPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbIconMenu)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbIconRestart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbIconSound)).BeginInit();
            this.controlPanel.SuspendLayout();
            this.characterPanel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();

            
            // obstaclePanel
            // 
            this.obstaclePanel.BackColor = System.Drawing.SystemColors.Window;
            this.obstaclePanel.Controls.Add(this.pnlObstacle2);
            this.obstaclePanel.Controls.Add(this.pnlObstacle1);
            this.obstaclePanel.Controls.Add(this.lblObstacles);
            this.obstaclePanel.Location = new System.Drawing.Point(848, 0);
            this.obstaclePanel.Margin = new System.Windows.Forms.Padding(1);
            this.obstaclePanel.Name = "obstaclePanel";
            this.obstaclePanel.Size = new System.Drawing.Size(431, 475);
            this.obstaclePanel.TabIndex = 2;

            // 
            // pnlObstacle2
            // 
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
            // 
            // lblObstacleDesc2
            // 
            this.lblObstacleDesc2.AutoSize = true;
            this.lblObstacleDesc2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblObstacleDesc2.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.lblObstacleDesc2.Location = new System.Drawing.Point(173, 136);
            this.lblObstacleDesc2.Margin = new System.Windows.Forms.Padding(0);
            this.lblObstacleDesc2.Name = "lblObstacleDesc2";
            this.lblObstacleDesc2.Size = new System.Drawing.Size(85, 18);
            this.lblObstacleDesc2.TabIndex = 14;
            this.lblObstacleDesc2.Text = "beschrijving";
            // 
            // lblObstacleDescTitle2
            // 
            this.lblObstacleDescTitle2.AutoSize = true;
            this.lblObstacleDescTitle2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(213)))), ((int)(((byte)(161)))), ((int)(((byte)(15)))));
            this.lblObstacleDescTitle2.Location = new System.Drawing.Point(173, 116);
            this.lblObstacleDescTitle2.Margin = new System.Windows.Forms.Padding(0);
            this.lblObstacleDescTitle2.Name = "lblObstacleDescTitle2";
            this.lblObstacleDescTitle2.Size = new System.Drawing.Size(64, 13);
            this.lblObstacleDescTitle2.TabIndex = 11;
            this.lblObstacleDescTitle2.Text = "Beschrijving";
            // 
            // lblObstaclePosY2
            // 
            this.lblObstaclePosY2.AutoSize = true;
            this.lblObstaclePosY2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblObstaclePosY2.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.lblObstaclePosY2.Location = new System.Drawing.Point(249, 83);
            this.lblObstaclePosY2.Name = "lblObstaclePosY2";
            this.lblObstaclePosY2.Size = new System.Drawing.Size(23, 18);
            this.lblObstaclePosY2.TabIndex = 13;
            this.lblObstaclePosY2.Text = "int";
            // 
            // pnlObstacleIcon2
            // 
            this.pnlObstacleIcon2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(216)))), ((int)(((byte)(216)))), ((int)(((byte)(216)))));
            this.pnlObstacleIcon2.Controls.Add(this.pbObstacle2);
            this.pnlObstacleIcon2.Location = new System.Drawing.Point(7, 12);
            this.pnlObstacleIcon2.Name = "pnlObstacleIcon2";
            this.pnlObstacleIcon2.Size = new System.Drawing.Size(160, 160);
            this.pnlObstacleIcon2.TabIndex = 6;
            // 
            // pbObstacle2
            // 
            this.pbObstacle2.BackgroundImage = global::WindesHeim_Game.Properties.Resources.carEdited;
            this.pbObstacle2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pbObstacle2.Location = new System.Drawing.Point(5, 5);
            this.pbObstacle2.Name = "pbObstacle2";
            this.pbObstacle2.Size = new System.Drawing.Size(150, 150);
            this.pbObstacle2.TabIndex = 1;
            this.pbObstacle2.TabStop = false;
            // 
            // lblObstaclePosX2
            // 
            this.lblObstaclePosX2.AutoSize = true;
            this.lblObstaclePosX2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblObstaclePosX2.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.lblObstaclePosX2.Location = new System.Drawing.Point(249, 63);
            this.lblObstaclePosX2.Name = "lblObstaclePosX2";
            this.lblObstaclePosX2.Size = new System.Drawing.Size(23, 18);
            this.lblObstaclePosX2.TabIndex = 12;
            this.lblObstaclePosX2.Text = "int";
            // 
            // lblObstacleProps2
            // 
            this.lblObstacleProps2.AutoSize = true;
            this.lblObstacleProps2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(213)))), ((int)(((byte)(161)))), ((int)(((byte)(15)))));
            this.lblObstacleProps2.Location = new System.Drawing.Point(173, 43);
            this.lblObstacleProps2.Name = "lblObstacleProps2";
            this.lblObstacleProps2.Size = new System.Drawing.Size(81, 13);
            this.lblObstacleProps2.TabIndex = 5;
            this.lblObstacleProps2.Text = "Eigenschappen";
            // 
            // lblObstaclePosYTitle2
            // 
            this.lblObstaclePosYTitle2.AutoSize = true;
            this.lblObstaclePosYTitle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblObstaclePosYTitle2.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.lblObstaclePosYTitle2.Location = new System.Drawing.Point(174, 83);
            this.lblObstaclePosYTitle2.Name = "lblObstaclePosYTitle2";
            this.lblObstaclePosYTitle2.Size = new System.Drawing.Size(70, 18);
            this.lblObstaclePosYTitle2.TabIndex = 11;
            this.lblObstaclePosYTitle2.Text = "Positie Y:";
            // 
            // lblObstacleName2
            // 
            this.lblObstacleName2.AutoSize = true;
            this.lblObstacleName2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(67)))), ((int)(((byte)(131)))));
            this.lblObstacleName2.Location = new System.Drawing.Point(173, 12);
            this.lblObstacleName2.Name = "lblObstacleName2";
            this.lblObstacleName2.Size = new System.Drawing.Size(75, 13);
            this.lblObstacleName2.TabIndex = 3;
            this.lblObstacleName2.Text = "Obstakelnaam";
            // 
            // lblObstaclePosXTitle2
            // 
            this.lblObstaclePosXTitle2.AutoSize = true;
            this.lblObstaclePosXTitle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblObstaclePosXTitle2.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.lblObstaclePosXTitle2.Location = new System.Drawing.Point(174, 63);
            this.lblObstaclePosXTitle2.Name = "lblObstaclePosXTitle2";
            this.lblObstaclePosXTitle2.Size = new System.Drawing.Size(71, 18);
            this.lblObstaclePosXTitle2.TabIndex = 10;
            this.lblObstaclePosXTitle2.Text = "Positie X:";
            // 
            // pnlObstacle1
            // 
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
            // 
            // lblObstacleDesc1
            // 
            this.lblObstacleDesc1.AutoSize = true;
            this.lblObstacleDesc1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblObstacleDesc1.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.lblObstacleDesc1.Location = new System.Drawing.Point(173, 136);
            this.lblObstacleDesc1.Margin = new System.Windows.Forms.Padding(0);
            this.lblObstacleDesc1.Name = "lblObstacleDesc1";
            this.lblObstacleDesc1.Size = new System.Drawing.Size(85, 18);
            this.lblObstacleDesc1.TabIndex = 15;
            this.lblObstacleDesc1.Text = "beschrijving";
            // 
            // lblObstacleDescTitle1
            // 
            this.lblObstacleDescTitle1.AutoSize = true;
            this.lblObstacleDescTitle1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(213)))), ((int)(((byte)(161)))), ((int)(((byte)(15)))));
            this.lblObstacleDescTitle1.Location = new System.Drawing.Point(173, 116);
            this.lblObstacleDescTitle1.Margin = new System.Windows.Forms.Padding(0);
            this.lblObstacleDescTitle1.Name = "lblObstacleDescTitle1";
            this.lblObstacleDescTitle1.Size = new System.Drawing.Size(64, 13);
            this.lblObstacleDescTitle1.TabIndex = 10;
            this.lblObstacleDescTitle1.Text = "Beschrijving";
            // 
            // lblObstaclePosY1
            // 
            this.lblObstaclePosY1.AutoSize = true;
            this.lblObstaclePosY1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblObstaclePosY1.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.lblObstaclePosY1.Location = new System.Drawing.Point(249, 83);
            this.lblObstaclePosY1.Name = "lblObstaclePosY1";
            this.lblObstaclePosY1.Size = new System.Drawing.Size(23, 18);
            this.lblObstaclePosY1.TabIndex = 9;
            this.lblObstaclePosY1.Text = "int";
            // 
            // lblObstaclePosX1
            // 
            this.lblObstaclePosX1.AutoSize = true;
            this.lblObstaclePosX1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblObstaclePosX1.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.lblObstaclePosX1.Location = new System.Drawing.Point(250, 63);
            this.lblObstaclePosX1.Name = "lblObstaclePosX1";
            this.lblObstaclePosX1.Size = new System.Drawing.Size(23, 18);
            this.lblObstaclePosX1.TabIndex = 8;
            this.lblObstaclePosX1.Text = "int";
            // 
            // lblObstaclePosYTitle1
            // 
            this.lblObstaclePosYTitle1.AutoSize = true;
            this.lblObstaclePosYTitle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblObstaclePosYTitle1.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.lblObstaclePosYTitle1.Location = new System.Drawing.Point(173, 83);
            this.lblObstaclePosYTitle1.Name = "lblObstaclePosYTitle1";
            this.lblObstaclePosYTitle1.Size = new System.Drawing.Size(70, 18);
            this.lblObstaclePosYTitle1.TabIndex = 7;
            this.lblObstaclePosYTitle1.Text = "Positie Y:";
            // 
            // lblObstaclePosXTitle1
            // 
            this.lblObstaclePosXTitle1.AutoSize = true;
            this.lblObstaclePosXTitle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblObstaclePosXTitle1.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.lblObstaclePosXTitle1.Location = new System.Drawing.Point(173, 63);
            this.lblObstaclePosXTitle1.Name = "lblObstaclePosXTitle1";
            this.lblObstaclePosXTitle1.Size = new System.Drawing.Size(71, 18);
            this.lblObstaclePosXTitle1.TabIndex = 6;
            this.lblObstaclePosXTitle1.Text = "Positie X:";
            // 
            // pnlObstacleIcon1
            // 
            this.pnlObstacleIcon1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(216)))), ((int)(((byte)(216)))), ((int)(((byte)(216)))));
            this.pnlObstacleIcon1.Controls.Add(this.pbObstacle1);
            this.pnlObstacleIcon1.Location = new System.Drawing.Point(7, 12);
            this.pnlObstacleIcon1.Name = "pnlObstacleIcon1";
            this.pnlObstacleIcon1.Size = new System.Drawing.Size(160, 160);
            this.pnlObstacleIcon1.TabIndex = 5;
            // 
            // pbObstacle1
            // 
            this.pbObstacle1.BackgroundImage = global::WindesHeim_Game.Properties.Resources.bikeEdited;
            this.pbObstacle1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pbObstacle1.Location = new System.Drawing.Point(5, 5);
            this.pbObstacle1.Name = "pbObstacle1";
            this.pbObstacle1.Size = new System.Drawing.Size(150, 150);
            this.pbObstacle1.TabIndex = 0;
            this.pbObstacle1.TabStop = false;
            // 
            // lblObstacleProps1
            // 
            this.lblObstacleProps1.AutoSize = true;
            this.lblObstacleProps1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(213)))), ((int)(((byte)(161)))), ((int)(((byte)(15)))));
            this.lblObstacleProps1.Location = new System.Drawing.Point(173, 43);
            this.lblObstacleProps1.Name = "lblObstacleProps1";
            this.lblObstacleProps1.Size = new System.Drawing.Size(81, 13);
            this.lblObstacleProps1.TabIndex = 4;
            this.lblObstacleProps1.Text = "Eigenschappen";
            // 
            // lblObstacleName1
            // 
            this.lblObstacleName1.AutoSize = true;
            this.lblObstacleName1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(67)))), ((int)(((byte)(131)))));
            this.lblObstacleName1.Location = new System.Drawing.Point(173, 12);
            this.lblObstacleName1.Name = "lblObstacleName1";
            this.lblObstacleName1.Size = new System.Drawing.Size(75, 13);
            this.lblObstacleName1.TabIndex = 3;
            this.lblObstacleName1.Text = "Obstakelnaam";
            // 
            // lblObstacles
            // 
            this.lblObstacles.AutoSize = true;
            this.lblObstacles.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(67)))), ((int)(((byte)(131)))));
            this.lblObstacles.Location = new System.Drawing.Point(3, 9);
            this.lblObstacles.Name = "lblObstacles";
            this.lblObstacles.Size = new System.Drawing.Size(54, 13);
            this.lblObstacles.TabIndex = 0;
            this.lblObstacles.Text = "Obstakels";
            // 
            // actionPanel
            // 
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
            // 
            // pbIconMenu
            // 
            this.pbIconMenu.BackgroundImage = global::WindesHeim_Game.Properties.Resources.menuEdited;
            this.pbIconMenu.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pbIconMenu.Location = new System.Drawing.Point(34, 190);
            this.pbIconMenu.Name = "pbIconMenu";
            this.pbIconMenu.Size = new System.Drawing.Size(50, 50);
            this.pbIconMenu.TabIndex = 8;
            this.pbIconMenu.TabStop = false;
            this.pbIconMenu.MouseEnter += new System.EventHandler(this.MenuHoverEnter);
            this.pbIconMenu.MouseLeave += new System.EventHandler(this.MenuHoverLeave);
            // 
            // pbIconRestart
            // 
            this.pbIconRestart.BackgroundImage = global::WindesHeim_Game.Properties.Resources.restartEdited;
            this.pbIconRestart.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pbIconRestart.Location = new System.Drawing.Point(34, 106);
            this.pbIconRestart.Name = "pbIconRestart";
            this.pbIconRestart.Size = new System.Drawing.Size(50, 50);
            this.pbIconRestart.TabIndex = 7;
            this.pbIconRestart.TabStop = false;
            this.pbIconRestart.MouseEnter += new System.EventHandler(this.RestartHoverEnter);
            this.pbIconRestart.MouseLeave += new System.EventHandler(this.RestartHoverLeave);
            // 
            // pbIconSound
            // 
            this.pbIconSound.BackgroundImage = global::WindesHeim_Game.Properties.Resources.soundEdited1;
            this.pbIconSound.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pbIconSound.Location = new System.Drawing.Point(34, 22);
            this.pbIconSound.Name = "pbIconSound";
            this.pbIconSound.Size = new System.Drawing.Size(50, 50);
            this.pbIconSound.TabIndex = 6;
            this.pbIconSound.TabStop = false;
            this.pbIconSound.Click += new System.EventHandler(this.pbIconSound_Click);
            this.pbIconSound.MouseEnter += new System.EventHandler(this.SoundHoverEnter);
            this.pbIconSound.MouseLeave += new System.EventHandler(this.SoundHoverLeave);
            // 
            // controlPanel
            // 
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
            // 
            // btnLeft
            // 
            this.btnLeft.BackgroundImage = global::WindesHeim_Game.Properties.Resources.Left;
            this.btnLeft.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnLeft.Location = new System.Drawing.Point(21, 134);
            this.btnLeft.Margin = new System.Windows.Forms.Padding(0);
            this.btnLeft.Name = "btnLeft";
            this.btnLeft.Size = new System.Drawing.Size(100, 100);
            this.btnLeft.TabIndex = 3;
            this.btnLeft.UseVisualStyleBackColor = true;
            // 
            // btnRight
            // 
            this.btnRight.BackgroundImage = global::WindesHeim_Game.Properties.Resources.Right;
            this.btnRight.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnRight.Location = new System.Drawing.Point(250, 134);
            this.btnRight.Margin = new System.Windows.Forms.Padding(0);
            this.btnRight.Name = "btnRight";
            this.btnRight.Size = new System.Drawing.Size(100, 100);
            this.btnRight.TabIndex = 2;
            this.btnRight.UseVisualStyleBackColor = true;
            // 
            // btnUp
            // 
            this.btnUp.BackgroundImage = global::WindesHeim_Game.Properties.Resources.Up;
            this.btnUp.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnUp.Location = new System.Drawing.Point(136, 13);
            this.btnUp.Margin = new System.Windows.Forms.Padding(0);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(100, 100);
            this.btnUp.TabIndex = 1;
            this.btnUp.UseVisualStyleBackColor = true;
            // 
            // btnDown
            // 
            this.btnDown.BackgroundImage = global::WindesHeim_Game.Properties.Resources.Down;
            this.btnDown.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnDown.Location = new System.Drawing.Point(136, 134);
            this.btnDown.Margin = new System.Windows.Forms.Padding(0);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(100, 100);
            this.btnDown.TabIndex = 0;
            this.btnDown.UseVisualStyleBackColor = true;
            // 
            // characterPanel
            // 
            this.characterPanel.BackColor = System.Drawing.SystemColors.Window;
            this.characterPanel.Controls.Add(this.panel1);
            this.characterPanel.Controls.Add(this.lblCharacter);
            this.characterPanel.Location = new System.Drawing.Point(475, 477);
            this.characterPanel.Margin = new System.Windows.Forms.Padding(1);
            this.characterPanel.Name = "characterPanel";
            this.characterPanel.Size = new System.Drawing.Size(372, 242);
            this.characterPanel.TabIndex = 5;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ControlLight;
            this.panel1.Controls.Add(this.lblCharacterSpeed);
            this.panel1.Controls.Add(this.lblCharacterSpeedTitle);
            this.panel1.Controls.Add(this.lblCharacterPosY);
            this.panel1.Controls.Add(this.lblCharacterPosX);
            this.panel1.Controls.Add(this.lblCharacterPosYTitle);
            this.panel1.Controls.Add(this.lblCharacterPosXTitle);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.lblCharacterProps);
            this.panel1.Controls.Add(this.lblCharacterName);
            this.panel1.Location = new System.Drawing.Point(5, 55);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(364, 179);
            this.panel1.TabIndex = 2;
            // 
            // lblCharacterSpeed
            // 
            this.lblCharacterSpeed.AutoSize = true;
            this.lblCharacterSpeed.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCharacterSpeed.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.lblCharacterSpeed.Location = new System.Drawing.Point(249, 114);
            this.lblCharacterSpeed.Name = "lblCharacterSpeed";
            this.lblCharacterSpeed.Size = new System.Drawing.Size(23, 18);
            this.lblCharacterSpeed.TabIndex = 11;
            this.lblCharacterSpeed.Text = "int";
            // 
            // lblCharacterSpeedTitle
            // 
            this.lblCharacterSpeedTitle.AutoSize = true;
            this.lblCharacterSpeedTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCharacterSpeedTitle.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.lblCharacterSpeedTitle.Location = new System.Drawing.Point(173, 114);
            this.lblCharacterSpeedTitle.Name = "lblCharacterSpeedTitle";
            this.lblCharacterSpeedTitle.Size = new System.Drawing.Size(68, 18);
            this.lblCharacterSpeedTitle.TabIndex = 10;
            this.lblCharacterSpeedTitle.Text = "Snelheid:";
            // 
            // lblCharacterPosY
            // 
            this.lblCharacterPosY.AutoSize = true;
            this.lblCharacterPosY.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCharacterPosY.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.lblCharacterPosY.Location = new System.Drawing.Point(249, 83);
            this.lblCharacterPosY.Name = "lblCharacterPosY";
            this.lblCharacterPosY.Size = new System.Drawing.Size(23, 18);
            this.lblCharacterPosY.TabIndex = 9;
            this.lblCharacterPosY.Text = "int";
            // 
            // lblCharacterPosX
            // 
            this.lblCharacterPosX.AutoSize = true;
            this.lblCharacterPosX.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCharacterPosX.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.lblCharacterPosX.Location = new System.Drawing.Point(250, 63);
            this.lblCharacterPosX.Name = "lblCharacterPosX";
            this.lblCharacterPosX.Size = new System.Drawing.Size(23, 18);
            this.lblCharacterPosX.TabIndex = 8;
            this.lblCharacterPosX.Text = "int";
            // 
            // lblCharacterPosYTitle
            // 

            this.lblCharacterPosYTitle.AutoSize = true;
            this.lblCharacterPosYTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCharacterPosYTitle.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.lblCharacterPosYTitle.Location = new System.Drawing.Point(173, 83);
            this.lblCharacterPosYTitle.Name = "lblCharacterPosYTitle";
            this.lblCharacterPosYTitle.Size = new System.Drawing.Size(70, 18);
            this.lblCharacterPosYTitle.TabIndex = 7;
            this.lblCharacterPosYTitle.Text = "Positie Y:";
            // 
            // lblCharacterPosXTitle
            // 
            this.lblCharacterPosXTitle.AutoSize = true;
            this.lblCharacterPosXTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCharacterPosXTitle.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.lblCharacterPosXTitle.Location = new System.Drawing.Point(173, 63);
            this.lblCharacterPosXTitle.Name = "lblCharacterPosXTitle";
            this.lblCharacterPosXTitle.Size = new System.Drawing.Size(71, 18);
            this.lblCharacterPosXTitle.TabIndex = 6;
            this.lblCharacterPosXTitle.Text = "Positie X:";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(216)))), ((int)(((byte)(216)))), ((int)(((byte)(216)))));
            this.panel2.Controls.Add(this.pictureBox1);
            this.panel2.Location = new System.Drawing.Point(7, 12);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(160, 160);
            this.panel2.TabIndex = 5;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImage = global::WindesHeim_Game.Properties.Resources.playerEdited1;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox1.Location = new System.Drawing.Point(5, 5);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(150, 150);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // lblCharacterProps
            // 
            this.lblCharacterProps.AutoSize = true;
            this.lblCharacterProps.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(213)))), ((int)(((byte)(161)))), ((int)(((byte)(15)))));
            this.lblCharacterProps.Location = new System.Drawing.Point(173, 43);
            this.lblCharacterProps.Name = "lblCharacterProps";
            this.lblCharacterProps.Size = new System.Drawing.Size(81, 13);
            this.lblCharacterProps.TabIndex = 4;
            this.lblCharacterProps.Text = "Eigenschappen";
            // 
            // lblCharacterName
            // 
            this.lblCharacterName.AutoSize = true;
            this.lblCharacterName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(67)))), ((int)(((byte)(131)))));
            this.lblCharacterName.Location = new System.Drawing.Point(173, 12);
            this.lblCharacterName.Name = "lblCharacterName";
            this.lblCharacterName.Size = new System.Drawing.Size(63, 13);
            this.lblCharacterName.TabIndex = 3;
            this.lblCharacterName.Text = "Spelernaam";
            // 
            // lblCharacter
            // 
            this.lblCharacter.AutoSize = true;
            this.lblCharacter.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(67)))), ((int)(((byte)(131)))));
            this.lblCharacter.Location = new System.Drawing.Point(2, 2);
            this.lblCharacter.Name = "lblCharacter";
            this.lblCharacter.Size = new System.Drawing.Size(53, 13);
            this.lblCharacter.TabIndex = 0;
            this.lblCharacter.Text = "Character";
            // 
            // logoPanel
            // 
            this.logoPanel.BackColor = System.Drawing.SystemColors.Window;
            this.logoPanel.BackgroundImage = global::WindesHeim_Game.Properties.Resources.ConceptTransparentBackgroundSmall;
            this.logoPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.logoPanel.Location = new System.Drawing.Point(848, 477);
            this.logoPanel.Margin = new System.Windows.Forms.Padding(1);
            this.logoPanel.Name = "logoPanel";
            this.logoPanel.Size = new System.Drawing.Size(431, 242);
            this.logoPanel.TabIndex = 0;


            // Voeg hieronder de overige panels toe, zoals objectbeschrijvingen etc.

            gameWindow.Controls.Add(graphicsPanel);
            gameWindow.Controls.Add(obstaclePanel);
            gameWindow.Controls.Add(characterPanel);
            gameWindow.Controls.Add(controlPanel);
            gameWindow.Controls.Add(actionPanel);
            gameWindow.Controls.Add(logoPanel);

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
        
        private void pbIconSound_Click(object sender, EventArgs e)
        {
            if (mute)
            {
                pbIconSound.BackgroundImage = Resources.muteEditedOnHover;
                mute = false;
            }
            else
            {
                pbIconSound.BackgroundImage = Resources.soundEditedOnHover;
                mute = true;
            }
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
        private void SoundHoverLeave(object sender, EventArgs e)
        {


            if (mute)
            {
                pbIconSound.BackgroundImage = Resources.soundEdited1;
                
                mute = false;
            }
            else
            {
                pbIconSound.BackgroundImage = Resources.muteEdited;
                mute = true;
            }


        }

        private void SoundHoverEnter(object sender, EventArgs e)
        {
            if (mute)
            {
                pbIconSound.BackgroundImage = Resources.muteEditedOnHover;
                mute = false;
            }
            else
            {
                pbIconSound.BackgroundImage = Resources.soundEditedOnHover;
                mute = true;
            }
        }
        public List<GameObject> GameObjects
        {
            get { return gameObjects; }
        }
    }

    public class ModelLevelSelect : Model
    {
        private ListBox levels;
        private Button goBack;
        private Button playLevel;
        private Label labelLevels;
        private Label labelLevelPreview;
        private Panel alignPanel;
        private Panel gamePanel;

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


            levels = new ListBox();
            levels.Size = new System.Drawing.Size(200, 475);
            levels.Location = new System.Drawing.Point(0, 40);
            string[] fileEntries = Directory.GetFiles("../levels/");
            foreach (string file in fileEntries)
            {
                XMLParser xml = new XMLParser(file);
                xml.ReadXML();
                levels.Items.Add(xml.gameProperties.title);
            }

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
            goBack.Click += new EventHandler(levelSelectController.goBack_Click);

            playLevel = new Button();
            playLevel.Size = new System.Drawing.Size(845, 25);
            playLevel.Location = new System.Drawing.Point(210, 525);
            playLevel.Text = "Play Level";
            playLevel.Click += new EventHandler(levelSelectController.goBack_Click);

            gameWindow.Controls.Add(alignPanel);
            alignPanel.Controls.Add(labelLevels);
            alignPanel.Controls.Add(labelLevelPreview);
            alignPanel.Controls.Add(goBack);
            alignPanel.Controls.Add(playLevel);
            alignPanel.Controls.Add(levels);
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
        private Label labelHighscores;
        private Panel alignPanel;
        private ListBox listBoxHighscores;

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

            listBoxLevels = new ListBox();
            listBoxLevels.Size = new System.Drawing.Size(200, 475);
            listBoxLevels.Location = new System.Drawing.Point(0, 40);

            listBoxHighscores = new ListBox();
            listBoxHighscores.Size = new System.Drawing.Size(200, 475);
            listBoxHighscores.Location = new System.Drawing.Point(200, 40);

            string[] fileEntries = Directory.GetFiles("../levels/");
            foreach (string file in fileEntries)
            {
                XMLParser xml = new XMLParser(file);
                xml.ReadXML();
                this.levels.Add(xml); //Ingeladen gegevens opslaan in lokale List voor hergebruik
                listBoxLevels.Items.Add(xml.gameProperties.title);

                int i = 0;
                foreach (GameHighscores highscore in xml.gameHighscores)
                {
                    i++;
                    char[] a = highscore.name.ToCharArray();
                    a[0] = char.ToUpper(a[0]);

                    listBoxHighscores.Items.Add(i + ". " + new string(a) + " score: " + highscore.score);
                }
            }

            labelHighscores = new Label();
            labelHighscores.Text = "Highscores";
            labelHighscores.Font = new Font("Arial", 20);
            labelHighscores.Location = new System.Drawing.Point(0, 0);
            labelHighscores.Size = new System.Drawing.Size(200, 30);
            labelHighscores.TextAlign = ContentAlignment.MiddleCenter;

            goBack = new Button();
            goBack.Size = new System.Drawing.Size(200, 25);
            goBack.Location = new System.Drawing.Point(0, 525);
            goBack.Text = "Go Back";
            goBack.Click += new EventHandler(highscoresController.goBack_Click);

            gameWindow.Controls.Add(alignPanel);
            alignPanel.Controls.Add(labelHighscores);
            alignPanel.Controls.Add(goBack);
            alignPanel.Controls.Add(listBoxLevels);
            alignPanel.Controls.Add(listBoxHighscores);


            alignPanel.Location = new Point(
                (gameWindow.Width / 2 - alignPanel.Size.Width / 2),
                (gameWindow.Height / 2 - alignPanel.Size.Height / 2));
            alignPanel.Anchor = AnchorStyles.None;
        }
    }
}
