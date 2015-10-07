﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Windows.Forms;

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

        public void button_Click(object sender, EventArgs e)
        {
            gameWindow.setController(ScreenStates.game);
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
            timer.Start();
            
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

                    if (gameObstacle.CollidesWith(mg.player))
                    {
                        mg.player.Location = new Point(0, 0);
                        UpdatePlayerPosition();
                        mg.InitializeField();
                        mg.GameObjects.Add(new Explosion(gameObstacle.Location, 10, 10));
                        mg.player.ImageURL = AppDomain.CurrentDomain.BaseDirectory + "..\\..\\resources\\Player.png"; ;
                    }
                }

                if (gameObject is SlowingObstacle)
                {
                    SlowingObstacle gameObstacle = (SlowingObstacle)gameObject;
                    gameObstacle.ChasePlayer(mg.player);

                    if (gameObstacle.CollidesWith(mg.player))
                    {
                        mg.player.Speed = mg.player.OriginalSpeed / 2;
                        UpdatePlayerSpeed("langzaam");
                    }
                    else
                    {
                        mg.player.Speed = mg.player.OriginalSpeed;
                        if (pressedSpeed && (mg.player.SpeedCooldown == 0))
                        {
                            UpdatePlayerSpeed("snel");
                        }
                        else
                        {
                            UpdatePlayerSpeed("normaal");
                        }
                    }
                }

                if (gameObject is ExplodingObstacle)
                {
                    ExplodingObstacle gameObstacle = (ExplodingObstacle)gameObject;

                    if (gameObstacle.CollidesWith(mg.player))
                    {
                        mg.player.Location = new Point(0, 0);
                        UpdatePlayerPosition();
                        mg.InitializeField();
                        mg.GameObjects.Add(new Explosion(gameObstacle.Location, 10, 10));
                        mg.player.ImageURL = AppDomain.CurrentDomain.BaseDirectory + "..\\..\\resources\\Player.png"; ;
                    }

                    
                }

                if (gameObject is StaticObstacle)
                {
                    StaticObstacle gameObstacle = (StaticObstacle)gameObject;

                    if (gameObstacle.CollidesWith(mg.player))
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

                // Check of we de explosie kunnen verwijderen
                if (gameObject is Explosion)
                {

                    Explosion explosion = (Explosion)gameObject;

                    DateTime nowDateTime = DateTime.Now;
                    DateTime explosionDateTime = explosion.TimeStamp;
                    TimeSpan difference = nowDateTime - explosionDateTime;

                    double animationTimerTen = (difference.TotalMilliseconds / 100);
                    int animationTimer = Convert.ToInt32(animationTimerTen);
                    Console.WriteLine(animationTimer);


                    switch (animationTimer)
                    {
                        case 1:
                            mg.graphicsPanel.BackColor = ColorTranslator.FromHtml("#FF0000");
                            gameObject.FadeSmall();
                            System.Media.SoundPlayer player = new System.Media.SoundPlayer(AppDomain.CurrentDomain.BaseDirectory + "..\\..\\resources\\EXPLODE.WAV");
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
        }

        public void OnPaintEvent(object sender, PaintEventArgs pe) {
            Graphics g = pe.Graphics;
            ModelGame mg = (ModelGame)model;

            // Teken player
            g.DrawImage(Image.FromFile(mg.player.ImageURL), mg.player.Location.X, mg.player.Location.Y, mg.player.Width, mg.player.Height);

            // Teken andere gameobjects
            foreach (GameObject gameObject in mg.GameObjects) {
                if(gameObject is Obstacle) {
                    g.DrawImage(Image.FromFile(gameObject.ImageURL), gameObject.Location.X, gameObject.Location.Y, gameObject.Width, gameObject.Height);
                }

                if(gameObject is Explosion) {
                    g.DrawImage(Image.FromFile(gameObject.ImageURL), gameObject.Location.X, gameObject.Location.Y, gameObject.Width, gameObject.Height);
                   
                           
                }
            }
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
                mg.player.ImageURL = AppDomain.CurrentDomain.BaseDirectory + "..\\..\\resources\\PlayerLeft.png";
            }
            if (e.KeyCode == Keys.D) {
                pressedRight = true;
                mg.player.ImageURL = AppDomain.CurrentDomain.BaseDirectory + "..\\..\\resources\\Player.png";
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
    }

    public class ControllerLevelSelect : Controller
    {
        public ControllerLevelSelect(GameWindow form) : base(form)
        {
            this.model = new ModelLevelSelect(this);
        }
          
        public void goBack_Click(object sender, EventArgs e)
        {
            gameWindow.setController(ScreenStates.menu);
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
