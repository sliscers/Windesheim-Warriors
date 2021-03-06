﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
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
            gameWindow.Controls.Clear();
            
            ScreenInit();
            gameWindow.Focus();
        }

        public virtual void ScreenInit()
        {
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

        public void play_Click(object sender, EventArgs e)
        {
            bool error = false;

            if (!System.Diagnostics.Debugger.IsAttached)
            {
                if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "/levels/"))
                {
                    Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "/levels/");
                }

                int fileCount = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "/levels/", "*.xml", SearchOption.AllDirectories).Length;

                if (fileCount == 0)
                {
                    //Default levels
                    string xmlString = Resources.level1;
                    File.WriteAllBytes(AppDomain.CurrentDomain.BaseDirectory + "/levels/level1.xml", Encoding.UTF8.GetBytes(Resources.level1));
                    xmlString = Resources.level2;
                    File.WriteAllBytes(AppDomain.CurrentDomain.BaseDirectory + "/levels/level2.xml", Encoding.UTF8.GetBytes(Resources.level2));
                    xmlString = Resources.level3;
                    File.WriteAllBytes(AppDomain.CurrentDomain.BaseDirectory + "/levels/level3.xml", Encoding.UTF8.GetBytes(Resources.level3));
                    xmlString = Resources.level4;
                    File.WriteAllBytes(AppDomain.CurrentDomain.BaseDirectory + "/levels/level4.xml", Encoding.UTF8.GetBytes(Resources.level4));
                }
            }

            if (!error)
            gameWindow.setController(ScreenStates.gameSelect);
        }
        public void editor_Click(object sender, EventArgs e)
        {
            gameWindow.setController(ScreenStates.editorSelect);
        }
        public void highscore_Click(object sender, EventArgs e)
        {
            gameWindow.setController(ScreenStates.highscore);
        }
    }



    public class ControllerGame : Controller
    {
        public Boolean mute = false;
        System.Media.SoundPlayer player = new System.Media.SoundPlayer(Resources.EXPLODE);
        
        // Timer voor de gameloop
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();

        // Initialiseren van de buttons
        private bool pressedLeft = false;
        private bool pressedRight = false;
        private bool pressedUp = false;
        private bool pressedDown = false;
        private bool pressedSpeed = false;

        public int score = 0;

        // Voor de views
        private Obstacle closestObstacle = null;
        private Obstacle nextClosestObstacle = null;

        //Test level in editor
        public static bool editor = false;

        public void RestartClicked(object sender, MouseEventArgs e)
        {
            ModelGame mg = (ModelGame)model;
            // Stop de game loop
            timer.Stop();

            // Reset het veld
            score = 0;
            mg.player.Location = new Point(0, 0);
            UpdatePlayerPosition();
            mg.InitializeField();
            timer.Start();
        }

        public void MenuClicked(object sender, MouseEventArgs e)
        {
            // Reset veld en ga terug naar menu
            ModelGame mg = (ModelGame)model;
            score = 0;
            mg.InitializeField();
            gameWindow.setController(ScreenStates.menu);
            editor = false;

            timer.Stop();
        }

        public ControllerGame(GameWindow form) : base(form)
        {
            this.model = new ModelGame(this);
            timer.Tick += new EventHandler(GameLoop);
            timer.Interval = 16;
        }

        private void GameLoop(object sender, EventArgs e)
        {
            // Alle functionaliteiten binnen deze loop vallen binnen de game
            ProcessUserInput();
            ProcessObstacles();
            MuteSound();
            ModelGame mg = (ModelGame)model;
            countFix(mg);
            mg.graphicsPanel.Invalidate();
            GetClosestObstacle();
            UpdateObstacleLabels(closestObstacle, nextClosestObstacle);
        }

        private void countFix(ModelGame mg)
        {
            score++;
            mg.score.Text = score.ToString();
        }


        public void MuteSound()
        {
            if (mute)
            { 
                player.Stop();
            }
        }




        public void pbIconSound_Click(object sender, EventArgs e)
        {
            ModelGame mg = (ModelGame)model;

            if (mute)
            {
                mg.pbIconSound.BackgroundImage = Resources.soundEditedOnHover;
                mute = false;

            }
            else
            {
                mg.pbIconSound.BackgroundImage = Resources.muteEditedOnHover;
                mute = true;
            }
        }


        public void SoundHoverLeave(object sender, EventArgs e)
        {
            ModelGame mg = (ModelGame)model;
            if (mute)
            {
                mg.pbIconSound.BackgroundImage = Resources.muteEdited;


            }
            else
            {
                mg.pbIconSound.BackgroundImage = Resources.soundEdited;

            }
        }

        public void SoundHoverEnter(object sender, EventArgs e)
        {
            ModelGame mg = (ModelGame)model;
            if (mute)
            {
                mg.pbIconSound.BackgroundImage = Resources.muteEditedOnHover;

            }
            else
            {
                mg.pbIconSound.BackgroundImage = Resources.soundEditedOnHover;

            }
        }

        private void ProcessUserInput()
        {
            ModelGame mg = (ModelGame)model;

            // Sprint controller
            if (mg.player.SpeedCooldown > 0)
            {
                mg.player.SpeedCooldown--;
            }

            // Speler laten sprinten
            if (pressedSpeed && (mg.player.SpeedCooldown == 0))
            {
                mg.player.Speed = mg.player.OriginalSpeed * 2;
                UpdatePlayerSpeed("snel");
                mg.player.SpeedDuration++;

            }

            // Speler heeft lang genoeg gesprint
            if (mg.player.SpeedDuration > 50)
            {
                mg.player.SpeedDuration = 0;
                mg.player.Speed = mg.player.OriginalSpeed;
                mg.player.SpeedCooldown = 200;
            }

            // Afhandelen van control input
            if (pressedDown && mg.player.Location.Y <= (mg.graphicsPanel.Size.Height + mg.graphicsPanel.Location.Y) - mg.player.Height)
            {

                mg.player.Location = new Point(mg.player.Location.X, mg.player.Location.Y + mg.player.Speed);
                UpdatePlayerPosition();
            }
            if (pressedUp && mg.player.Location.Y >= mg.graphicsPanel.Location.Y)
            {

                mg.player.Location = new Point(mg.player.Location.X, mg.player.Location.Y - mg.player.Speed);
                UpdatePlayerPosition();
            }
            if (pressedLeft && mg.player.Location.X >= mg.graphicsPanel.Location.X)
            {

                mg.player.Location = new Point(mg.player.Location.X - mg.player.Speed, mg.player.Location.Y);
                UpdatePlayerPosition();
            }
            if (pressedRight && mg.player.Location.X <= (mg.graphicsPanel.Size.Width + mg.graphicsPanel.Location.X) - mg.player.Width)
            {

                mg.player.Location = new Point(mg.player.Location.X + mg.player.Speed, mg.player.Location.Y);
                UpdatePlayerPosition();
            }

            if (pressedRight)
            {
                mg.btnRight.BackgroundImage = Resources.RightOnClick;
            }
            if (pressedLeft)
            {
                mg.btnLeft.BackgroundImage = Resources.LeftOnClick;
            }
            if (pressedUp)
            {
                mg.btnUp.BackgroundImage = Resources.UpOnClick;
            }
            if (pressedDown)
            {
                mg.btnDown.BackgroundImage = Resources.DownOnClick;
            }
            if (pressedRight == false)
            {
                mg.btnRight.BackgroundImage = Resources.Right;
            }
            if (pressedLeft == false)
            {
                mg.btnLeft.BackgroundImage = Resources.Left;
            }
            if (pressedUp == false)
            {
                mg.btnUp.BackgroundImage = Resources.Up;
            }
            if (pressedDown == false)
            {
                mg.btnDown.BackgroundImage = Resources.Down;
            }

        }

        private void UpdatePlayerPosition()
        {
            // Update labels
            ModelGame mg = (ModelGame)model;
            mg.lblCharacterPosX.Text = mg.player.Location.X.ToString();
            mg.lblCharacterPosY.Text = mg.player.Location.Y.ToString();
        }

        private void UpdatePlayerSpeed(string speed)
        {
            // Update labels
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
                if (gameObject is Obstacle)
                {
                    int obstacleX = gameObject.Location.X;
                    int obstacleY = gameObject.Location.Y;
                    int deltaX = playerX - obstacleX;
                    int deltaY = playerY - obstacleY;
                    if (deltaX < 0)
                    {
                        deltaX *= -1;
                    }
                    if (deltaY < 0)
                    {
                        deltaY *= -1;
                    }
                    int sum = (deltaX * deltaX) + (deltaY * deltaY);
                    double result = Math.Sqrt(sum);
                    if (result < difference)
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

            SlowingObstacle slowedDownObstacle = null;

            // Loop door alle obstacles objecten en roep methode aan
            foreach (GameObject gameObject in safeListArray)
            {
                if (gameObject is MovingExplodingObstacle)
                {
                    MovingExplodingObstacle gameObstacle = (MovingExplodingObstacle)gameObject;

                    //Opslaan van huidige locatie in variable om vervolgens te vergelijken
                    Point currentLocation = new Point(gameObstacle.Location.X, gameObstacle.Location.Y);

                    gameObstacle.ChasePlayer(mg.player, "x");

                    gameObstacle.ProcessCollision(safeListArray, "x");

                    gameObstacle.ChasePlayer(mg.player, "y");
                    gameObstacle.ProcessCollision(safeListArray, "y");

                    if (gameObstacle.IsSmart && currentLocation.Equals(gameObstacle.Location) && !gameObstacle.SmartMovingEnabled)
                    {                        
                        gameObstacle.SmartmovingTime = DateTime.Now.AddMilliseconds(2500);
                        gameObstacle.SmartMovingEnabled = true;
                    }


                    if (gameObstacle.IsSmart && gameObstacle.SmartMovingEnabled)
                    {
                        //Controleert als het object nog steeds slim moet zijn
                        if (gameObstacle.SmartmovingTime >= DateTime.Now)
                        {
                            //Probeert weg te komen van stilstaant object
                            gameObstacle.TryToEscape();
                        }
                        else
                        {
                            gameObstacle.SmartMovingEnabled = false;
                            gameObstacle.SmartmovingDirection = ""; // Reset direction voor smart movement
                            gameObstacle.DirectionTime = default(DateTime);
                        }
                    }

                    if (gameObstacle.CollidesWith(mg.player))
                    {
                        score = 0;
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

                    //Opslaan van huidige locatie in variable om vervolgens te vergelijken
                    Point currentLocation = gameObstacle.Location;

                    gameObstacle.ChasePlayer(mg.player, "x");
                    gameObstacle.ProcessCollision(safeListArray, "x");

                    gameObstacle.ChasePlayer(mg.player, "y");
                    gameObstacle.ProcessCollision(safeListArray, "y");

                    if (gameObstacle.IsSmart && currentLocation.Equals(gameObstacle.Location) && !gameObstacle.SmartMovingEnabled)
                    {
                        gameObstacle.SmartmovingTime = DateTime.Now.AddMilliseconds(2500);
                        gameObstacle.SmartMovingEnabled = true;
                    }

                    if (gameObstacle.IsSmart && gameObstacle.SmartMovingEnabled)
                    {
                        //Controleert als het object nog steeds slim moet zijn
                        if (gameObstacle.SmartmovingTime >= DateTime.Now)
                        {
                            //Probeert weg te komen van stilstaant object
                            gameObstacle.TryToEscape();
                        }
                        else
                        {
                            gameObstacle.SmartMovingEnabled = false;
                            gameObstacle.SmartmovingDirection = ""; // Reset direction voor smart movement
                            gameObstacle.DirectionTime = default(DateTime);
                        }
                    }


                    if (mg.player.CollidesWith(gameObstacle))
                    {
                        slowedDownObstacle = gameObstacle;
                        if(gameObstacle.SlowingSpeed < slowedDownObstacle.SlowingSpeed) {
                            slowedDownObstacle.SlowingSpeed = gameObstacle.SlowingSpeed;
                    }
                        }

                    if (mg.player.Speed == mg.player.OriginalSpeed / 2)
                        UpdatePlayerSpeed("Slow");
                    else if (mg.player.Speed == mg.player.OriginalSpeed)
                            UpdatePlayerSpeed("Normal");
                    else if (mg.player.Speed == mg.player.OriginalSpeed * 2)
                        UpdatePlayerSpeed("Fast");
                }

                if (gameObject is ExplodingObstacle)
                {
                    ExplodingObstacle gameObstacle = (ExplodingObstacle)gameObject;

                    if (mg.player.CollidesWith(gameObstacle))
                    {
                        score = 0;
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
                    //End Game
                    Checkpoint gameObstacle = (Checkpoint)gameObject;
                    if (mg.player.CollidesWith(gameObstacle) && !gameObstacle.Start)
                    {
                        mg.player.Location = new Point(0, 0);
                        mg.InitializeField();
                        timer.Stop();
                        if (editor)
                        {                                                        
                            gameWindow.setController(ScreenStates.editor);                            
                        }
                        else
                        {
                            gameWindow.setController(ScreenStates.highscoreInput);
                        }
                        TimerStop();
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

                    // Explosie animatie
                    switch (animationTimer)
                    {
                        case 1:
                            mg.graphicsPanel.BackColor = ColorTranslator.FromHtml("#FF0000");
                            gameObject.FadeSmall();
                            if (!mute)
                            {
                                
                                player.Play();
                            }
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
                        mg.graphicsPanel.BackColor = System.Drawing.SystemColors.ControlLight;
                    }
                }
            }

            if(slowedDownObstacle != null) {
                mg.player.Speed = slowedDownObstacle.SlowingSpeed;
            }
        }

        public override void RunController()
        {
            base.RunController();
            score = 0;
            ModelGame mg = (ModelGame)model;
            mg.InitializeField();
        }

        public void OnPaintEvent(object sender, PaintEventArgs pe)
        {
            Graphics g = pe.Graphics;
            ModelGame mg = (ModelGame)model;

            // Teken alle gameobjects
            foreach (GameObject gameObject in mg.GameObjects)
            {
                if (gameObject is Checkpoint)
                {
                    g.DrawImage(gameObject.ObjectImage, gameObject.Location.X, gameObject.Location.Y, gameObject.Width, gameObject.Height);
                }

                if (gameObject is Obstacle)
                {
                    g.DrawImage(gameObject.ObjectImage, gameObject.Location.X, gameObject.Location.Y, gameObject.Width, gameObject.Height);
                }

                if (gameObject is Explosion)
                {
                    g.DrawImage(gameObject.ObjectImage, gameObject.Location.X, gameObject.Location.Y, gameObject.Width, gameObject.Height);
                }
                //g.DrawRectangle(new Pen(Color.Red), new Rectangle(gameObject.Location.X, gameObject.Location.Y, gameObject.Width, gameObject.Height));

            }
            // Teken player
            g.DrawImage(mg.player.ObjectImage, mg.player.Location.X, mg.player.Location.Y, mg.player.Width, mg.player.Height);
        }

        public void OnKeyDownWASD(object sender, KeyEventArgs e)
        {
            ModelGame mg = (ModelGame)model;

            if (e.KeyCode == Keys.W)
            {
                pressedUp = true;
            }
            if (e.KeyCode == Keys.S)
            {
                pressedDown = true;
            }
            if (e.KeyCode == Keys.A)
            {
                pressedLeft = true;
                    mg.player.ObjectImage = Resources.PlayerLeft;
            }
            if (e.KeyCode == Keys.D)
            {
                pressedRight = true;
                    mg.player.ObjectImage = Resources.Player;
            }
            if (e.KeyCode == Keys.Space)
            {
                pressedSpeed = true;
            }
        }

        public void OnKeyUp(object sender, KeyEventArgs e)
        {
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
        private XMLParser currentSelectedLevel;

        private ModelLevelSelect modelLevelSelect;

        public ControllerLevelSelect(GameWindow form) : base(form)
        {
            this.model = new ModelLevelSelect(this);
            this.modelLevelSelect = (ModelLevelSelect)model;
        }

        public void goBack_Click(object sender, EventArgs e)
        {
            gameWindow.setController(ScreenStates.menu);
        }

        public void playLevel_Click(object sender, EventArgs e)
        {
            ModelGame.level = currentSelectedLevel;
            ControllerGame.editor = false;
            gameWindow.setController(ScreenStates.game);
        }

        public void level_Select(object sender, EventArgs e)
        {
            ListBox listBoxLevels = (ListBox)sender;
            currentSelectedLevel = (XMLParser)listBoxLevels.SelectedItem;


            modelLevelSelect.gamePanel.Invalidate(); // refresh
        }

        public void OnPreviewPaint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            // Teken preview
            if (currentSelectedLevel != null)
            {
                g.DrawString(currentSelectedLevel.gameProperties.title + " (" + currentSelectedLevel.gameProperties.difficulty + ")", new Font("Arial", 16), new SolidBrush(Color.Black), new Point(((modelLevelSelect.gamePanel.Width - 150) / 2), 0));
                List<GameObject> previewList = new List<GameObject>(currentSelectedLevel.gameObjects);
                previewList.Add(new Checkpoint(new Point(750, 400), Resources.IconWIN, 80, 80, false));
                previewList.Add(new Checkpoint(new Point(5, -5), Resources.IconSP, 80, 80, true));

                foreach (GameObject gameObject in previewList)
                {
                    g.DrawImage(gameObject.ObjectImage, gameObject.Location.X, gameObject.Location.Y, gameObject.Width, gameObject.Height);
                }
            }
        }
    }

    public class ControllerHighscores : Controller
    {
        private XMLParser currentSelectedLevel;

        private ModelHighscores modelHighscores;

        public ControllerHighscores(GameWindow form) : base(form)
        {
            this.model = new ModelHighscores(this);
            this.modelHighscores = (ModelHighscores)model;
        }
        public void goBack_Click(object sender, EventArgs e)
        {
            gameWindow.setController(ScreenStates.menu);
        }

        public void level_Select(object sender, EventArgs e)
        {
            ListBox listBoxLevels = (ListBox)sender;
            currentSelectedLevel = (XMLParser)listBoxLevels.SelectedItem;

            modelHighscores.listBoxHighscores.Items.Clear();

            int i = 0;
            // Laat alle highscores zien
            foreach (GameHighscore highscore in currentSelectedLevel.gameHighscores)
            {
                i++;
                char[] a = highscore.name.ToCharArray();
                a[0] = char.ToUpper(a[0]);

                string highscoreText = i + ". " + new string(a);
                if (a.Length < 5) // Extra spaties bij kleinere namen
                    highscoreText += "    ";
                if (a.Length < 6)
                    highscoreText += "    ";
                highscoreText += "\tscore: " + highscore.score;
                if (highscore.score.ToString().Length < 3) // Extra spaties als score uit 2 of minder cijfers bestaat
                    highscoreText += "  ";
                highscoreText += "    " + highscore.dateTime.ToString("dd-MM-yy H:mm");
                modelHighscores.listBoxHighscores.Items.Add(highscoreText);
                if (i == 0)
                {
                    listBoxLevels.SetSelected(0, true);
                }
            }
        }
    }

    public class ControllerEditorSelect : Controller
    {
        public static XMLParser level;

        private XMLParser currentSelectedLevel;

        private ModelEditorSelect modelEditorSelect;

        public ControllerEditorSelect(GameWindow form) : base(form)
        {
            this.model = new ModelEditorSelect(this);
            modelEditorSelect = (ModelEditorSelect)model;
        }
        public void goBack_Click(object sender, EventArgs e)
        {
            gameWindow.setController(ScreenStates.menu);            
        }
        public void level_Select(object sender, EventArgs e)
        {
            ListBox listBoxLevels = (ListBox)sender;
            currentSelectedLevel = (XMLParser)listBoxLevels.SelectedItem;

            modelEditorSelect.gamePanel.Invalidate(); // refresh
        }

        public void OnPreviewPaint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            
            // Teken preview
            if (currentSelectedLevel != null)
            {
                g.DrawString(currentSelectedLevel.gameProperties.title+ " (" + currentSelectedLevel.gameProperties.difficulty + ")", new Font("Arial", 16), new SolidBrush(Color.Black), new Point(((modelEditorSelect.gamePanel.Width - 150) / 2), 0));
                List<GameObject> previewList = new List<GameObject>(currentSelectedLevel.gameObjects);
                //Start en eindpunt
                previewList.Add(new Checkpoint(new Point(750, 400), Resources.IconWIN, 80, 80, false));
                previewList.Add(new Checkpoint(new Point(5, -5), Resources.IconSP, 80, 80, true));

                foreach (GameObject gameObject in previewList)
                {
                    g.DrawImage(gameObject.ObjectImage, gameObject.Location.X, gameObject.Location.Y, gameObject.Width, gameObject.Height);
                }
            }
        }

        public void editLevel_Click(object sender, EventArgs e)
        {
            ModelEditor.level = currentSelectedLevel;
            gameWindow.setController(ScreenStates.editor);
        }

        public void newLevel_Click(object sender, EventArgs e)
        {
            ModelEditor.level = null;
            ModelGame.level = null;
            gameWindow.setController(ScreenStates.editor);
        }
    }

    public class ControllerEditor : Controller
    {
        private XMLParser level;

        private ModelEditor modelEditor;

        private List<GameObject> gameObjects = new List<GameObject>();

        private Point MouseDownLocation = new Point(20, 20);
        private bool isDragging = false;
        public int mouseX = 0;
        public int mouseY = 0;
        private int defaultSize = 40;

        public ControllerEditor(GameWindow form) : base(form)
        {
            this.model = new ModelEditor(this);
            this.modelEditor = (ModelEditor)model;
        }
        public override void RunController()
        {
            base.RunController();

            if (ModelGame.level != null && level != null)
            { // Als bestaand level nog niet opgeslagen is maar terugkomt van de test
                gameObjects = ModelGame.level.getCleanGameObjects();
            }
            else
            {
                level = ModelEditor.level;
                if (level != null)
                { // Bestaand level bewerken
                    gameObjects = level.getCleanGameObjects();
                }
                else
                { // Niew level make
                    gameObjects = new List<GameObject>();
                }
            }

            ModelGame.level = null;
            modelEditor.gamePanel.Invalidate();
        }

        public void goBack_Click(object sender, EventArgs e)
        {
            gameWindow.setController(ScreenStates.editorSelect);
            // Opschonen
            level = null;
            gameObjects.Clear();
        }

        public void testLevel_Click(object sender, EventArgs e)
        {
            // Maakt nieuw level aan
            XMLParser tempLevel = new XMLParser();
            tempLevel.gameObjects = gameObjects;
            ModelGame.level = tempLevel;
            ControllerGame.editor = true; // Laat Game controller weten dat we aan het testen zijn
            gameWindow.setController(ScreenStates.game);
        }

        public void saveLevel_Click(object sender, EventArgs e)
        {   
            if (level == null)
            {
                //Als nieuw level
                string dialog = showPropertyDialog("Set properties for Level");
                if (dialog != "")
                {
                    string[] returnValues = dialog.Split(new string[] { "|" }, StringSplitOptions.None);
                    string title = returnValues[0].ToString();
                    string difficulty = returnValues[1].ToString();
                    string file;
                    if (System.Diagnostics.Debugger.IsAttached)
                        file = "../levels/" + title + ".xml";
                    else
                        file = AppDomain.CurrentDomain.BaseDirectory + "/levels/" + title + ".xml";
                    if (!File.Exists(file)) { 
                        level = new XMLParser(file);
                        GameProperties gameProperties = new GameProperties(title, difficulty);
                        level.gameProperties = gameProperties;
                        level.gameObjects = gameObjects;
                        level.WriteXML();
                        modelEditor.gamePanel.Invalidate();
                    }
                    else
                        MessageBox.Show("Sorry, '" + title + "' already exist. Try again with another level name.");
                }
            }
            else 
            {
                // Als Edit level
                level.gameObjects = gameObjects;
                level.WriteXML();
                MessageBox.Show("Saved changes to " + level.gameProperties.title);
            }        
        }

        private String showPropertyDialog(string dialogTitle)
        {
            //Mesagebox om eigenschappen level te wijzigen
            prompt = new Form();
            prompt.Width = 250;
            prompt.Height = 210;
            prompt.FormBorderStyle = FormBorderStyle.FixedDialog;
            prompt.Text = "Save Level";
            prompt.StartPosition = FormStartPosition.CenterScreen;

            Label textLabelName = new Label() { Left = 10, Top = 20, Text = "Level name:" };
            TextBox textBoxName = new TextBox() { Left = 110, Top = 18, Width = 100 };
            Label textLabelDifficulty = new Label() { Left = 10, Top = 50, Text = "Level difficulty" };
            ComboBox textBoxDifficulty = new ComboBox() { Left = 110, Top = 48, Width = 100 };
            textBoxDifficulty.Items.AddRange(new string[] { "Easy", "Medium", "Hard", "Expert", "Unmöglich" });
            textBoxDifficulty.SelectedIndex = 0;

            Button cancel = new Button() { Text = "Cancel", Left = 10, Width = 100, Top = 140, DialogResult = DialogResult.Cancel };
            Button confirmation = new Button() { Text = "Save", Left = 110, Width = 100, Top = 140, DialogResult = DialogResult.OK };
            prompt.Controls.Add(textLabelName);
            prompt.Controls.Add(textBoxName);
            prompt.Controls.Add(textLabelDifficulty);
            prompt.Controls.Add(textBoxDifficulty);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(cancel);
            prompt.AcceptButton = confirmation;
            prompt.CancelButton = cancel;

            return prompt.ShowDialog() == DialogResult.OK ? textBoxName.Text + "|" + textBoxDifficulty.Text : "";
        }

        private Form prompt;
        private Label textLabelSlowingSpeed;
        private ComboBox textBoxSlowingSpeed;
        private Label textLabelSmart;
        private CheckBox checkBoxSmart;
        private Label textLabelMovingSpeed;

        private ComboBox textBoxMovingSpeed;

        public String ShowDialog(string type, string dialogTitle)
        {
            // Eigenschappen van gameobject aanpassen, wordt getoond bij het droppen
            prompt = new Form();
            prompt.Width = 250;
            prompt.Height = 210;
            prompt.FormBorderStyle = FormBorderStyle.FixedDialog;
            prompt.Text = dialogTitle;
            prompt.StartPosition = FormStartPosition.CenterScreen;

            textBoxSlowingSpeed = new ComboBox() { Left = 110, Top = 88, Width = 100 };
            textBoxSlowingSpeed.Items.AddRange(new string[] { "Freeze the player", "Very slow", "Slow", "Normal" });
            textBoxSlowingSpeed.SelectedIndex = 2;

            if (type == "MovingExplodingObstacle" || type == "SlowingObstacle")
            {
                textLabelMovingSpeed = new Label() { Left = 10, Top = 30, Text = "Obstacle speed" };
                prompt.Controls.Add(textLabelMovingSpeed);

                textBoxMovingSpeed = new ComboBox() { Left = 110, Top = 28, Width = 100 };
                textBoxMovingSpeed.Items.AddRange(new string[] { "Slow", "Moderate", "Fast", "Unmöglich" });
                textBoxMovingSpeed.SelectedIndex = 0;
                prompt.Controls.Add(textBoxMovingSpeed);

                textLabelSmart = new Label() { Left = 10, Top = 60, Text = "Smart following (default = off):" };
                prompt.Controls.Add(textLabelSmart);

                checkBoxSmart = new CheckBox() { Left = 110, Top = 58, Width = 100 };
                prompt.Controls.Add(checkBoxSmart);               
            }

            if (type == "SlowingObstacle")
            {
                textLabelSlowingSpeed = new Label() { Left = 10, Top = 90, Text = "Player speed \nupon collision" };
                textLabelSlowingSpeed.Size = new Size(textLabelSlowingSpeed.Width, textLabelSlowingSpeed.Height + 20);
                prompt.Controls.Add(textLabelSlowingSpeed);
            
                textBoxSlowingSpeed.SelectedIndex = 2;                    
                prompt.Controls.Add(textBoxSlowingSpeed);
            }

            Button cancel = new Button() { Text = "Cancel", Left = 10, Width = 100, Top = 140, DialogResult = DialogResult.Cancel };
            Button confirmation = new Button() { Text = "Save", Left = 110, Width = 100, Top = 140, DialogResult = DialogResult.OK };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(cancel);
            prompt.AcceptButton = confirmation;
            prompt.CancelButton = cancel;
            return prompt.ShowDialog() == DialogResult.OK ? textBoxMovingSpeed.SelectedItem.ToString() + "|" + textBoxSlowingSpeed.SelectedItem.ToString() + "|" + checkBoxSmart.CheckState : "";
        }

        public void StaticObstacle_MouseUp(object sender, MouseEventArgs e)
        {
            // Loslaten van muisknop
            modelEditor.staticObstacle.Location = new System.Drawing.Point(10, 60);

            if ((mouseX) >= modelEditor.widthDragDropPanel && mouseX <= modelEditor.gamePanel.Width
                && mouseY >= modelEditor.gamePanel.Location.Y && mouseY <= modelEditor.gamePanel.Height)
            {
                gameObjects.Add(new StaticObstacle(new Point(mouseX - modelEditor.widthDragDropPanel, mouseY), defaultSize, defaultSize));
                modelEditor.gamePanel.Invalidate();
            }
        }

        public void ExplodingObstacle_MouseUp(object sender, MouseEventArgs e)
        {
            modelEditor.explodingObstacle.Location = new System.Drawing.Point(10, 110);

            if ((mouseX) >= modelEditor.widthDragDropPanel && mouseX <= modelEditor.gamePanel.Width
                && mouseY >= modelEditor.gamePanel.Location.Y && mouseY <= modelEditor.gamePanel.Height)
            {
                gameObjects.Add(new ExplodingObstacle(new Point(mouseX - modelEditor.widthDragDropPanel, mouseY), defaultSize, defaultSize));
                modelEditor.gamePanel.Invalidate();
            }

        }

        public void MovingExplodingObstacle_MouseUp(object sender, MouseEventArgs e)
        {
            modelEditor.movingExplodingObstacle.Location = new System.Drawing.Point(10, 160);

            if ((mouseX) >= modelEditor.widthDragDropPanel && mouseX <= modelEditor.gamePanel.Width
                && mouseY >= modelEditor.gamePanel.Location.Y && mouseY <= modelEditor.gamePanel.Height)
            {
                String dialog = ShowDialog("MovingExplodingObstacle", "Set properties for Moving Obstacle");
                if (dialog != "")
                {
                    string[] returnValues = dialog.Split(new string[] { "|" }, StringSplitOptions.None);
                    MovingExplodingObstacle moe = new MovingExplodingObstacle(new Point(mouseX - modelEditor.widthDragDropPanel, mouseY), defaultSize, defaultSize);

                        if (returnValues[0] == "Slow")
                        moe.MovingSpeed = 0;
                    else if (returnValues[0] == "Moderate")
                        moe.MovingSpeed = 1;
                    else if (returnValues[0] == "Fast")
                        moe.MovingSpeed = 2;
                    else if (returnValues[0] == "Unmöglich")
                        moe.MovingSpeed = 3;

                        if (returnValues[2] == "Unchecked")
                        moe.IsSmart = false;
                    else
                        moe.IsSmart = true;

                    gameObjects.Add(moe);
                    modelEditor.gamePanel.Invalidate();
                }
            }
        }

        public void SlowingObstacle_MouseUp(object sender, MouseEventArgs e)
        {
            modelEditor.slowingObstacle.Location = new System.Drawing.Point(10, 210);

            if ((mouseX) >= modelEditor.widthDragDropPanel && mouseX <= modelEditor.gamePanel.Width
                && mouseY >= modelEditor.gamePanel.Location.Y && mouseY <= modelEditor.gamePanel.Height)
            {
                String dialog = ShowDialog("SlowingObstacle", "Set properties for Slowing Obstacle");

                if (dialog != "")
                {
                    string[] returnValues = dialog.Split(new string[] { "|" }, StringSplitOptions.None);
                    SlowingObstacle sb = new SlowingObstacle(new Point(mouseX - modelEditor.widthDragDropPanel, mouseY), defaultSize, defaultSize);

                    if (returnValues[0] == "Slow")
                        sb.MovingSpeed = 0;
                    else if (returnValues[0] == "Moderate")
                        sb.MovingSpeed = 1;
                    else if (returnValues[0] == "Fast")
                        sb.MovingSpeed = 2;
                    else if (returnValues[0] == "Unmöglich")
                        sb.MovingSpeed = 3;

                    if (returnValues[1] == "Freeze the player")
                        sb.SlowingSpeed = 0;
                    else if (returnValues[1] == "Very slow")
                        sb.SlowingSpeed = 1;
                    else if (returnValues[1] == "Slow")
                        sb.SlowingSpeed = 2;
                    else if (returnValues[1] == "Normal")
                        sb.SlowingSpeed = 4;

                    if (returnValues[2] == "Unchecked")
                        sb.IsSmart = false;
                    else
                        sb.IsSmart = true;

                    gameObjects.Add(sb);
                    modelEditor.gamePanel.Invalidate();
                }
            }
        }

        public void updateDragPosition(object sender, MouseEventArgs e)
        {
            //Tijdens het slepen van de muis
            if (e.Button == System.Windows.Forms.MouseButtons.Left && isDragging)
            {
                if (sender.Equals(modelEditor.staticObstacle))
                {
                    mouseX = e.X + modelEditor.staticObstacle.Left - MouseDownLocation.X;
                    mouseY = e.Y + modelEditor.staticObstacle.Top - MouseDownLocation.Y;
                    modelEditor.staticObstacle.Left = mouseX;
                    modelEditor.staticObstacle.Top = mouseY;                    
                }
                else if (sender.Equals(modelEditor.explodingObstacle))
                {
                    mouseX = e.X + modelEditor.explodingObstacle.Left - MouseDownLocation.X;
                    mouseY = e.Y + modelEditor.explodingObstacle.Top - MouseDownLocation.Y;
                    modelEditor.explodingObstacle.Left = mouseX;
                    modelEditor.explodingObstacle.Top = mouseY;                    
                }
                else if (sender.Equals(modelEditor.movingExplodingObstacle))
                {
                    mouseX = e.X + modelEditor.movingExplodingObstacle.Left - MouseDownLocation.X;
                    mouseY = e.Y + modelEditor.movingExplodingObstacle.Top - MouseDownLocation.Y;
                    modelEditor.movingExplodingObstacle.Left = mouseX;
                    modelEditor.movingExplodingObstacle.Top = mouseY;                    
                }
                else if (sender.Equals(modelEditor.slowingObstacle))
                {
                    mouseX = e.X + modelEditor.slowingObstacle.Left - MouseDownLocation.X;
                    mouseY = e.Y + modelEditor.slowingObstacle.Top - MouseDownLocation.Y;
                    modelEditor.slowingObstacle.Left = mouseX;
                    modelEditor.slowingObstacle.Top = mouseY;                    
                }
            }
        }

        private GameObject objectDragging = null;

        public void MouseDown(object sender, MouseEventArgs e)
        {
            // Begin van drag and drop
            if (e.Button == MouseButtons.Left)
            {
                foreach (GameObject gameObject in gameObjects)
                {
                    if (!(gameObject is Checkpoint))
                    {
                        GameObject tempGameObject = new GameObject(new Point(e.Location.X - modelEditor.widthDragDropPanel, e.Location.Y), 40, 40);
                        if (gameObject.CollidesWith(tempGameObject))
                        {
                            objectDragging = gameObject;
                        }
                    }                   
                }
            }       
        }

        public void ObjectMouseDrag(object sender, MouseEventArgs e)
        {
            if (objectDragging != null)
            {
                objectDragging.Location = new Point(e.Location.X - modelEditor.widthDragDropPanel - (objectDragging.Width /2), e.Location.Y - (objectDragging.Height / 2));
                modelEditor.gamePanel.Invalidate();
            }
        }

        public void MouseUp(object sender, MouseEventArgs e)
        {
            if (objectDragging != null)
            {
                objectDragging.Location = new Point(e.Location.X - modelEditor.widthDragDropPanel - (objectDragging.Width / 2), e.Location.Y - (objectDragging.Height / 2));

                if (e.Location.X < (modelEditor.gamePanel.Location.X + modelEditor.widthDragDropPanel) || e.Location.X > (modelEditor.gamePanel.Location.X + modelEditor.gamePanel.Width)) {
                    gameObjects.Remove(objectDragging);
                }
                if(e.Location.Y < modelEditor.gamePanel.Location.Y || e.Location.Y > modelEditor.gamePanel.Height) {
                    gameObjects.Remove(objectDragging);
                }

                objectDragging = null;
                modelEditor.gamePanel.Invalidate();
            }
        }

        public void updateMousePosition(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                isDragging = true;
                MouseDownLocation = e.Location;
            }
        }

        public void undoLastChange_Click(object sender, EventArgs e)
        {
            if (gameObjects.Count != 0)
            {                
                gameObjects.RemoveAt(gameObjects.Count - 1);
                modelEditor.gamePanel.Invalidate();
            }
        }

        public void clearAll_Click(object sender, EventArgs e)
        {
            if (gameObjects.Count != 0)
            {
                gameObjects.Clear();
                modelEditor.gamePanel.Invalidate();
            }
        }

        public void GamePanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            //Eindobject
            g.DrawImage(new Bitmap(Resources.IconWIN), 750 + modelEditor.widthDragDropPanel, 400, 80, 80);
            g.DrawRectangle(new Pen(Color.FromArgb(255, 0, 70, 133)), new Rectangle(new Point(750 + modelEditor.widthDragDropPanel, 400), new Size(80, 80)));
            //Startobject
            g.DrawImage(new Bitmap(Resources.IconSP), 5 + modelEditor.widthDragDropPanel, -5, 80, 80);
            g.DrawRectangle(new Pen(Color.FromArgb(255, 0, 70, 133)), new Rectangle(new Point(5 + modelEditor.widthDragDropPanel, -5), new Size(80, 80)));

            foreach (GameObject gameObject in gameObjects)
            {
                g.DrawImage(gameObject.ObjectImage, gameObject.Location.X + modelEditor.widthDragDropPanel, gameObject.Location.Y, gameObject.Width, gameObject.Height);
                g.DrawRectangle(new Pen(Color.FromArgb(255, 0, 70, 133)), new Rectangle(new Point(gameObject.Location.X + modelEditor.widthDragDropPanel - gameObject.CollisionX, gameObject.Location.Y - gameObject.CollisionY), new Size(gameObject.Width + gameObject.CollisionX * 2, gameObject.Height + gameObject.CollisionY * 2)));
            }

            g.FillRectangle(new SolidBrush(Color.LightGray), new Rectangle(new Point(0, 0), new Size(modelEditor.widthDragDropPanel, 475)));

            Font drawFont = new Font("Arial", 16);
            SolidBrush drawBrush = new SolidBrush(Color.Black);

            g.DrawString("Drag and drop", drawFont, drawBrush, new Point(10, 10));
            //level naam
            if (level != null)
                g.DrawString(level.gameProperties.title, drawFont, drawBrush, new Point(((modelEditor.gamePanel.Width + modelEditor.widthDragDropPanel / 2) / 2), 0));

            drawFont = new Font("Arial", 8);
            g.DrawString("Static obstacle", drawFont, drawBrush, new Point(60, 70));
            g.DrawString("Exploding obstacle", drawFont, drawBrush, new Point(60, 120));
            g.DrawString("Moving exploding obstacle", drawFont, drawBrush, new Point(60, 170));
            g.DrawString("Slowing obstacle", drawFont, drawBrush, new Point(60, 220));

            g.DrawString("Undo last", drawFont, drawBrush, new Point(60, 315));
            g.DrawString("Clear level", drawFont, drawBrush, new Point(60, 365));

        }
    }
    public class ControllerHighscoreInput : Controller
    {
        public int score;
        public int place;
       
        public static string playerName = Environment.UserName.ToUpper();

        private ModelHighscoreInput modelHighscoreInput;

        public ControllerHighscoreInput(GameWindow form) : base(form)
        {
            this.model = new ModelHighscoreInput(this);
            modelHighscoreInput = (ModelHighscoreInput)model;
        }

        public void Continue_Click(object sender, EventArgs e)
        {         
            if (modelHighscoreInput.name.Text.Length == 0)
            {
                DialogResult dr = MessageBox.Show("Enter your name to continue", "Error", MessageBoxButtons.OK);
            }
            else
            {
                ModelGame.level.AddHighscore(new GameHighscore(modelHighscoreInput.name.Text, DateTime.Now.ToString(), score));
                gameWindow.setController(ScreenStates.menu);
                //Onthouden van de ingevoerde naam
                playerName = modelHighscoreInput.name.Text;
            }     
        }

        public void GetPlace()
        {            
            int i = 0;
            List<GameHighscore> tempHighscores = new List<GameHighscore>();

            // Vullen van highscorelijst
            foreach (GameHighscore highscore in ModelGame.level.gameHighscores)
            {
                tempHighscores.Add(highscore);
            }
            GameHighscore tempHighscore = new GameHighscore("tempHighscore", DateTime.Now.ToString(), score);
            tempHighscores.Add(tempHighscore);
            tempHighscores = tempHighscores.OrderBy(highscore => highscore.score).ToList();

            i = 0;
            foreach (GameHighscore highscore in tempHighscores)
            {
                i++;
                if (highscore.Equals(tempHighscore))
                {
                    place = i;
                }
            }
        }

        public void KeyDownText(object sender, KeyEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            char character = (char)e.KeyCode;

            if ((character >= 'A' && character <= 'Z') || (character >= 'a' && character <= 'z'))
            {
                textBox.Text += e.KeyCode.ToString().ToUpper();
                textBox.SelectionStart = textBox.Text.Length;
                textBox.SelectionLength = 0;
            }
            else if (e.KeyCode == Keys.Back)
            {
                if (textBox.Text.Length > 0)
                {
                    textBox.Text = textBox.Text.Substring(0, textBox.Text.Length - 1);
                    textBox.SelectionStart = textBox.Text.Length;
                    textBox.SelectionLength = 0;
                }
            }
            else if (e.KeyCode == Keys.Space)
            {
                textBox.Text += " ";
                textBox.SelectionStart = textBox.Text.Length;
                textBox.SelectionLength = 0;
            }
        }

        public void TryAgain_Click(object sender, EventArgs e)
        {
            //Voegt highscore toe 
            ModelGame.level.AddHighscore(new GameHighscore(modelHighscoreInput.name.Text, DateTime.Now.ToString(), score));
            gameWindow.setController(ScreenStates.game);
            //Onthouden van de ingevoerde naam
            playerName = modelHighscoreInput.name.Text;
        }
    }
}

