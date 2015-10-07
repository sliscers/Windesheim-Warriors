using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using WindesHeim_Game.Properties;

namespace WindesHeim_Game
{
    public class Controller
    {
        protected Model model;
        protected GameWindow gameWindow;

        public Controller(GameWindow form)
        { 
            this.gameWindow = form;
        }
        public virtual void RunController()
        {
            ScreenInit();
        }

        public virtual void ScreenInit()
        {
            gameWindow.Controls.Clear();
            model.ControlsInit(gameWindow);
        }

        public virtual void GraphicsInit(Graphics g)
        {
            model.GraphicsInit(g);
        }
    }

    public class ControllerMenu : Controller
    {
        public ControllerMenu(GameWindow form) : base(form)
        {
            this.model = new ModelMenu(this);
        }

        public void exit_Click(object sender, EventArgs e)
        {
            //Todo
        }
        public void play_Click(object sender, EventArgs e)
        {
            gameWindow.setController(ScreenStates.gameSelect);
        }
        public void highscore_Click(object sender, EventArgs e)
        {
            gameWindow.setController(ScreenStates.highscore);
        }
    }

        

    public class ControllerGame : Controller
    {
        // Timer voor de gameloop
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();

        private bool pressedLeft = false;
        private bool pressedRight = false;
        private bool pressedUp = false;
        private bool pressedDown = false;
        private bool pressedSpeed = false;
        private int counter;
        private Obstacle closestObstacle = null;
        private Obstacle nextClosestObstacle = null;
       


        public ControllerGame(GameWindow form) : base(form)
        {
            this.model = new ModelGame(this);
            timer.Tick += new EventHandler(GameLoop);
            timer.Interval = 16;
            
        }

        private void GameLoop(object sender, EventArgs e)
        {
            ProcessUserInput();
            ProcessObstacles();
            counter++;

            ModelGame mg = (ModelGame)model;
            mg.graphicsPanel.Invalidate();
            GetClosestObstacle();
            UpdateObstacleLabels(closestObstacle, nextClosestObstacle);
        }

        private void ProcessUserInput() 
        {
            ModelGame mg = (ModelGame) model;

            if(mg.player.SpeedCooldown > 0)
            {
                mg.player.SpeedCooldown--;
            }

            if (pressedSpeed && (mg.player.SpeedCooldown == 0))
            {
                mg.player.Speed = mg.player.OriginalSpeed * 2;
                UpdatePlayerSpeed("snel");
                mg.player.SpeedDuration ++;
              
            }
            if(mg.player.SpeedDuration > 50)
            {
                mg.player.SpeedDuration = 0;
                mg.player.Speed = mg.player.OriginalSpeed;
                mg.player.SpeedCooldown = 200;
            }

            if (pressedDown && mg.player.Location.Y <= (mg.graphicsPanel.Size.Height + mg.graphicsPanel.Location.Y) - mg.player.Height) {
                mg.player.Location = new Point(mg.player.Location.X, mg.player.Location.Y + mg.player.Speed);
                UpdatePlayerPosition();
            }
            if (pressedUp && mg.player.Location.Y >= mg.graphicsPanel.Location.Y) {
                mg.player.Location = new Point(mg.player.Location.X, mg.player.Location.Y - mg.player.Speed);
                UpdatePlayerPosition();
            }
            if (pressedLeft && mg.player.Location.X >= mg.graphicsPanel.Location.X ) {
                mg.player.Location = new Point(mg.player.Location.X - mg.player.Speed, mg.player.Location.Y);
                UpdatePlayerPosition();
            }
            if (pressedRight && mg.player.Location.X <= (mg.graphicsPanel.Size.Width + mg.graphicsPanel.Location.X) - mg.player.Width) {
                mg.player.Location = new Point(mg.player.Location.X + mg.player.Speed, mg.player.Location.Y);
                UpdatePlayerPosition();
            }
            
        }

        private void UpdatePlayerPosition()
        {
            ModelGame mg = (ModelGame)model;
            mg.lblCharacterPosX.Text = mg.player.Location.X.ToString();
            mg.lblCharacterPosY.Text = mg.player.Location.Y.ToString();
        }

        private void UpdatePlayerSpeed(string speed) {
            ModelGame mg = (ModelGame)model;
            if (mg.lblCharacterSpeed != null)
            {
                mg.lblCharacterSpeed.Text = speed;
            }
        }

        private void GetClosestObstacle()
        {
            ModelGame mg = (ModelGame)model;

            int playerX = mg.player.Location.X;
            int playerY = mg.player.Location.Y;
            double difference = 2000;

            // We maken een array aan zodat we door alle objecten kunnen loopen en ze met elkaar kunnen vergelijken
            List<GameObject> comparisonArray = new List<GameObject>(mg.GameObjects);

            foreach (GameObject gameObject in comparisonArray)
            {
                if(gameObject is Obstacle)
                {
                    int obstacleX = gameObject.Location.X;
                    int obstacleY = gameObject.Location.Y;
                    int deltaX = playerX - obstacleX;
                    int deltaY = playerY - obstacleY;
                    if(deltaX < 0)
                    {
                        deltaX *= -1;
                    }
                    if(deltaY < 0)
                    {
                        deltaY *= -1;
                    }
                    int sum = (deltaX * deltaX) + (deltaY * deltaY);
                    double result = Math.Sqrt(sum);
                    if(result < difference)
                    {
                        nextClosestObstacle = closestObstacle;
                        closestObstacle = (Obstacle)gameObject;
                        difference = result;
                    }
                }
            }
        }

        private void UpdateObstacleLabels(Obstacle obstacle1, Obstacle obstacle2)
        {
            ModelGame mg = (ModelGame)model;
            
            if (closestObstacle != null && nextClosestObstacle != null && mg.obstaclePanel != null)
            {
                mg.lblObstaclePosX1.Text = obstacle1.Location.X.ToString();
                mg.lblObstaclePosX2.Text = obstacle2.Location.X.ToString();
                mg.lblObstaclePosY1.Text = obstacle1.Location.Y.ToString();
                mg.lblObstaclePosY2.Text = obstacle2.Location.Y.ToString();
                mg.lblObstacleDesc1.Text = obstacle1.Description;
                mg.lblObstacleDesc2.Text = obstacle2.Description;
                mg.lblObstacleName1.Text = obstacle1.Name;
                mg.lblObstacleName2.Text = obstacle2.Name;
                mg.pbObstacle1.BackgroundImage = obstacle1.PanelIcon;
                mg.pbObstacle2.BackgroundImage = obstacle2.PanelIcon;
        }
        }

        private void ProcessObstacles() 
        {
            ModelGame mg = (ModelGame)model;

            // We moeten een 2e array maken om door heen te loopen
            // Er is kans dat we de array door lopen en ook tegelijkertijd een explosie toevoegen
            // We voegen dan als het ware iets toe en lezen tegelijk, dit mag niet
            List<GameObject> safeListArray = new List<GameObject>(mg.GameObjects);

            // Loop door alle obstacles objecten en roep methode aan
            foreach (GameObject gameObject in safeListArray)
            {
                if (gameObject is MovingExplodingObstacle)
                {
                    MovingExplodingObstacle gameObstacle = (MovingExplodingObstacle)gameObject;
                    gameObstacle.ChasePlayer(mg.player);

                    if (mg.player.CollidesWith(gameObstacle))
                    {
                        mg.player.Location = new Point(0, 0);
                        UpdatePlayerPosition();
                        mg.InitializeField();
                        mg.GameObjects.Add(new Explosion(gameObstacle.Location, 10, 10));
                        mg.player.ObjectImage = Resources.Player;
                    }
                }

                if (gameObject is SlowingObstacle)
                {
                    SlowingObstacle gameObstacle = (SlowingObstacle)gameObject;
                    gameObstacle.ChasePlayer(mg.player);

                    if (mg.player.CollidesWith(gameObstacle))
                    {
                        mg.player.Speed = mg.player.OriginalSpeed / 2;
                        UpdatePlayerSpeed("Langzaam");
                    }
                    else
                    {
                        mg.player.Speed = mg.player.OriginalSpeed;
                        if (pressedSpeed && (mg.player.SpeedCooldown == 0))
                        {
                            UpdatePlayerSpeed("Snel");
                        }
                        else
                        {
                            UpdatePlayerSpeed("Normaal");
                        }
                    }
                }

                if (gameObject is ExplodingObstacle)
                {
                    ExplodingObstacle gameObstacle = (ExplodingObstacle)gameObject;

                    if (mg.player.CollidesWith(gameObstacle))
                    {
                        mg.player.Location = new Point(0, 0);
                        UpdatePlayerPosition();
                        mg.InitializeField();
                        mg.GameObjects.Add(new Explosion(gameObstacle.Location, 10, 10));
                        mg.player.ObjectImage = Resources.Player;
                    }

                    
                }

                if (gameObject is StaticObstacle)
                {
                    StaticObstacle gameObstacle = (StaticObstacle)gameObject;

                    if (mg.player.CollidesWith(gameObstacle)) 
                    {
                        if (pressedUp)
                        {
                            mg.player.Location = new Point(mg.player.Location.X, mg.player.Location.Y + mg.player.Speed);
                        }
                        if (pressedDown)
                        {
                            mg.player.Location = new Point(mg.player.Location.X, mg.player.Location.Y - mg.player.Speed);
                        }
                        if (pressedLeft)
                        {
                            mg.player.Location = new Point(mg.player.Location.X + mg.player.Speed, mg.player.Location.Y);
                        }
                        if (pressedRight)
                        {
                            mg.player.Location = new Point(mg.player.Location.X - mg.player.Speed, mg.player.Location.Y);
                        }
                    }
                }
                if (gameObject is Checkpoint)
                {
                    Checkpoint gameObstacle = (Checkpoint)gameObject;
                    if (mg.player.CollidesWith(gameObstacle) && !gameObstacle.Start)
                    {
                        mg.player.Location = new Point(0, 0);
                        mg.InitializeField();
                        gameWindow.setController(ScreenStates.menu);
                    }
                }

                // Check of we de explosie kunnen verwijderen
                if (gameObject is Explosion)
                {

                    Explosion explosion = (Explosion)gameObject;

                    DateTime nowDateTime = DateTime.Now;
                    DateTime explosionDateTime = explosion.TimeStamp;
                    TimeSpan difference = nowDateTime - explosionDateTime;

                    double animationTimerTen = (difference.TotalMilliseconds / 100);
                    int animationTimer = Convert.ToInt32(animationTimerTen);
                    //Console.WriteLine(animationTimer);


                    switch (animationTimer)
                    {
                        case 1:
                            mg.graphicsPanel.BackColor = ColorTranslator.FromHtml("#FF0000");
                            gameObject.FadeSmall();
                            System.Media.SoundPlayer player = new System.Media.SoundPlayer(Resources.EXPLODE);
                            player.Play();
                            break;
                        case 2:
                            mg.graphicsPanel.BackColor = ColorTranslator.FromHtml("#FC1212");
                            gameObject.FadeSmall();
                            break;
                        case 3:
                            mg.graphicsPanel.BackColor = ColorTranslator.FromHtml("#F92525");
                            gameObject.FadeSmall();
                            break;
                        case 4:
                            mg.graphicsPanel.BackColor = ColorTranslator.FromHtml("#F63737");
                            gameObject.FadeSmall();
                            break;
                        case 5:
                            mg.graphicsPanel.BackColor = ColorTranslator.FromHtml("#F44A4A");
                            gameObject.FadeSmall();
                            break;
                        case 6:
                            mg.graphicsPanel.BackColor = ColorTranslator.FromHtml("#F15C5C");
                            gameObject.FadeSmall();
                            break;
                        case 7:
                            mg.graphicsPanel.BackColor = ColorTranslator.FromHtml("#EE6F6F");
                            gameObject.FadeSmall();
                            break;
                        case 8:
                            mg.graphicsPanel.BackColor = ColorTranslator.FromHtml("#EB8181");
                            gameObject.FadeSmall();
                            break;
                        case 9:
                            mg.graphicsPanel.BackColor = ColorTranslator.FromHtml("#E99494");
                            gameObject.FadeSmall();
                            break;
                        case 10:
                            mg.graphicsPanel.BackColor = ColorTranslator.FromHtml("#E6A6A6");
                            gameObject.FadeSmall();
                            break;
                        case 11:
                            mg.graphicsPanel.BackColor = ColorTranslator.FromHtml("#E3B9B9");
                            gameObject.FadeSmall();
                            break;
                        case 12:
                            mg.graphicsPanel.BackColor = ColorTranslator.FromHtml("#E0CBCB");
                            gameObject.FadeSmall();
                            break;
                    }



                    // Verschil is 3 seconden, dus het bestaat al voor 3 seconden, verwijderen maar!
                    if (difference.TotalSeconds > 1.2)
                    {
                        mg.GameObjects.Remove(gameObject);
                        mg.graphicsPanel.BackColor = ColorTranslator.FromHtml("#DEDEDE");
                    }
                }
            }      
        }


        public override void RunController()
        {
            base.RunController();
            ModelGame mg = (ModelGame)model;
            mg.InitializeField();
        }

        public void OnPaintEvent(object sender, PaintEventArgs pe) {
            Graphics g = pe.Graphics;
            ModelGame mg = (ModelGame)model;

            

            // Teken andere gameobjects
            foreach (GameObject gameObject in mg.GameObjects) {
                if (gameObject is Checkpoint)
                {
                    g.DrawImage(gameObject.ObjectImage, gameObject.Location.X, gameObject.Location.Y, gameObject.Width, gameObject.Height);

                }
                if (gameObject is Obstacle) {
                    g.DrawImage(gameObject.ObjectImage, gameObject.Location.X, gameObject.Location.Y, gameObject.Width, gameObject.Height);
                }

                if(gameObject is Explosion) {
                    g.DrawImage(gameObject.ObjectImage, gameObject.Location.X, gameObject.Location.Y, gameObject.Width, gameObject.Height);
                   
                }
                           
               
               
                }
            // Teken player
            g.DrawImage(mg.player.ObjectImage, mg.player.Location.X, mg.player.Location.Y, mg.player.Width, mg.player.Height);

       
            }

        public void OnKeyDownWASD(object sender, KeyEventArgs e) {
            ModelGame mg = (ModelGame)model;

            if (e.KeyCode == Keys.W) {
                pressedUp = true;
            }
            if (e.KeyCode == Keys.S) {
                pressedDown = true;
            }
            if (e.KeyCode == Keys.A) {
                pressedLeft = true;
                mg.player.ObjectImage = Resources.PlayerLeft;
            }
            if (e.KeyCode == Keys.D) {
                pressedRight = true;
                mg.player.ObjectImage = Resources.Player;
            }
            if (e.KeyCode == Keys.Space)
            {
                pressedSpeed = true;
            }
        }

        public void OnKeyUp(object sender, KeyEventArgs e) {
            ModelGame mg = (ModelGame)model;
            if (e.KeyCode == Keys.W)
            {
                pressedUp = false;
            }
            if (e.KeyCode == Keys.S)
            {
                pressedDown = false;
            }
            if (e.KeyCode == Keys.A)
            {
                pressedLeft = false;
                
            }
            if (e.KeyCode == Keys.D)
            {
                pressedRight = false;
               
            }
            if (e.KeyCode == Keys.Space)
            {
                pressedSpeed = false;
                mg.player.Speed = 5;

            }

          
            }
        public void TimerStart()
        {
            timer.Start();
        }
        public void TimerStop()
        {
            timer.Stop();
        }
    }

    public class ControllerLevelSelect : Controller
    {
        public List<XMLParser> Levels { get; set; } = new List<XMLParser>();

        private XMLParser currentSelectedLevel;

        public ControllerLevelSelect(GameWindow form) : base(form)
        {
            this.model = new ModelLevelSelect(this);
            fillLevels();
        }
          
        public void fillLevels()
        {
            string[] fileEntries = Directory.GetFiles("../levels/");
            foreach (string file in fileEntries)
            {
                XMLParser xml = new XMLParser(file);
                xml.ReadXML();
                this.Levels.Add(xml); //Ingeladen gegevens opslaan in lokale List voor hergebruik
            }
        }
          
        public void goBack_Click(object sender, EventArgs e)
        {
            gameWindow.setController(ScreenStates.menu);
        }

        public void playLevel_Click(object sender, EventArgs e)
        {
            ModelGame.level = currentSelectedLevel;
            gameWindow.setController(ScreenStates.game);

            ModelLevelSelect ml = (ModelLevelSelect)model;
            ml.alignPanel.Controls.Remove(ml.playLevel);
            ml.alignPanel.Controls.Remove(ml.goBack);
            ml.alignPanel.Controls.Remove(ml.listBoxLevels);

        }

        public void level_Select(object sender, EventArgs e)
        {
            ListBox listBoxLevels = (ListBox)sender;
            currentSelectedLevel = (XMLParser)listBoxLevels.SelectedItem;

            ModelLevelSelect ml = (ModelLevelSelect)model;
            ml.gamePanel.Invalidate(); // refresh
        }

        internal void OnPreviewPaint(object sender, PaintEventArgs e) {
            Graphics g = e.Graphics;

            // Teken preview
            if(currentSelectedLevel != null) {
                List<GameObject> previewList = new List<GameObject>(currentSelectedLevel.gameObjects);
                previewList.Add(new Player(new Point(10, 10), 40, 40));
                previewList.Add(new Checkpoint(new Point(750, 400), Resources.IconWIN, 80, 80, false));
                previewList.Add(new Checkpoint(new Point(5, -5), Resources.IconSP, 80, 80, true));

                foreach (GameObject gameObject in previewList) {
                    g.DrawImage(gameObject.ObjectImage, gameObject.Location.X, gameObject.Location.Y, gameObject.Width, gameObject.Height);
                }
            }       
        }
    }
    public class ControllerHighscores : Controller
    {
        public ControllerHighscores(GameWindow form) : base(form)
        {
            this.model = new ModelHighscores(this);
        }
        public void goBack_Click(object sender, EventArgs e)
        {
            gameWindow.setController(ScreenStates.menu);
        }
    }
}
