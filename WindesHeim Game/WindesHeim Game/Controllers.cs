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
        
        

        private bool pressedLeft = false;
        private bool pressedRight = false;
        private bool pressedUp = false;
        private bool pressedDown = false;
        private bool pressedSpeed = false;
        private Obstacle closestObstacle = null;
        private Obstacle nextClosestObstacle = null;


        public void RestartClicked(object sender, MouseEventArgs e)
        {
            ModelGame mg = (ModelGame)model;
            timer.Stop();

            mg.player.Location = new Point(0, 0);
            UpdatePlayerPosition();
            mg.InitializeField();
            timer.Start();
        }

        public void MenuClicked(object sender, MouseEventArgs e)
        {
            ModelGame mg = (ModelGame)model;
            mg.player.Location = new Point(0, 0);
            mg.InitializeField();
            gameWindow.setController(ScreenStates.menu);

        }

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
            MuteSound();
            ModelGame mg = (ModelGame)model;
            mg.graphicsPanel.Invalidate();
            GetClosestObstacle();
            UpdateObstacleLabels(closestObstacle, nextClosestObstacle);
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

            if (mg.player.SpeedCooldown > 0)
            {
                mg.player.SpeedCooldown--;
            }

            if (pressedSpeed && (mg.player.SpeedCooldown == 0))
            {
                mg.player.Speed = mg.player.OriginalSpeed * 2;
                UpdatePlayerSpeed("snel");
                mg.player.SpeedDuration++;

            }
            if (mg.player.SpeedDuration > 50)
            {
                mg.player.SpeedDuration = 0;
                mg.player.Speed = mg.player.OriginalSpeed;
                mg.player.SpeedCooldown = 200;
            }

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
            ModelGame mg = (ModelGame)model;
            mg.lblCharacterPosX.Text = mg.player.Location.X.ToString();
            mg.lblCharacterPosY.Text = mg.player.Location.Y.ToString();
        }

        private void UpdatePlayerSpeed(string speed)
        {
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

            // Loop door alle obstacles objecten en roep methode aan
            foreach (GameObject gameObject in safeListArray)
            {
                if (gameObject is MovingExplodingObstacle)
                {
                    MovingExplodingObstacle gameObstacle = (MovingExplodingObstacle)gameObject;

                    gameObstacle.ChasePlayer(mg.player);

                    // Loop door alle objecten op het veld
                    foreach (GameObject potentialCollision in safeListArray)
                    {
                        // We willen niet onszelf checken, en we willen alleen collision voor StaticObstacles en ExplodingObstacles
                        if (gameObject != potentialCollision && (potentialCollision is StaticObstacle || potentialCollision is ExplodingObstacle))
                        {
                            gameObject.ProcessCollision(potentialCollision);
                        }
                    }

                    if (gameObstacle.CollidesWith(mg.player))
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

                    // Loop door alle objecten op het veld
                    foreach (GameObject potentialCollision in safeListArray)
                    {
                        // We willen niet onszelf checken, maar we willen we collision op alles
                        if (gameObject != potentialCollision)
                        {
                            gameObject.ProcessCollision(potentialCollision);
                        }
                    }

                    if (mg.player.CollidesWith(gameObstacle))
                    {
                        mg.player.Speed = gameObstacle.SlowingSpeed;
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

        public void OnPaintEvent(object sender, PaintEventArgs pe)
        {
            Graphics g = pe.Graphics;
            ModelGame mg = (ModelGame)model;

            // Teken andere gameobjects
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
            

            //Workaround om focus conflict met windows forms en buttons op te lossen
            modelLevelSelect.alignPanel.Controls.Remove(modelLevelSelect.playLevel);
            modelLevelSelect.alignPanel.Controls.Remove(modelLevelSelect.goBack);
            modelLevelSelect.alignPanel.Controls.Remove(modelLevelSelect.listBoxLevels);

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
            foreach (GameHighscores highscore in currentSelectedLevel.gameHighscores)
            {
                i++;
                char[] a = highscore.name.ToCharArray();
                a[0] = char.ToUpper(a[0]);

                modelHighscores.listBoxHighscores.Items.Add(i + ". " + new string(a) + " score: " + highscore.score + " | " + highscore.dateTime.ToString("dd-MM-yy H:mm"));
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
                List<GameObject> previewList = new List<GameObject>(currentSelectedLevel.gameObjects);
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
            gameWindow.setController(ScreenStates.editor);
        }
    }

    public class ControllerEditor : Controller
    {
        private XMLParser level;

        private ModelEditor modelEditor;

        private List<GameObject> gameObjects = new List<GameObject>();

        private Graphics g;

        Graphics gamePanelGraphics;

        private Point MouseDownLocation = new Point(20, 20);
        private bool isDragging = false;
        public int mouseX = 0;
        public int mouseY = 0;

        public ControllerEditor(GameWindow form) : base(form)
        {
            this.model = new ModelEditor(this);
            this.modelEditor = (ModelEditor)model;
        }

        public void goBack_Click(object sender, EventArgs e)
        {
            gameWindow.setController(ScreenStates.editorSelect);
        }

        public void playLevel_Click(object sender, EventArgs e)
        {
            level = new XMLParser("haha test");
            level.gameObjects = gameObjects;
            if (level == null)
            {
                Console.WriteLine("error, level is null");
            }
            else
            {
                ModelGame.level = level;
                gameWindow.setController(ScreenStates.game);
            }

        }

        public override void RunController()
        {
            base.RunController();
            level = ModelEditor.level;
            if (level == null) //New Level aanmaken
            {

            }
            else
            { //Bestaand level bewerken
                gameObjects = level.getCleanGameObjects();
            }

            updatePreview();
        }

        public void StaticObstacle_MouseUp(object sender, MouseEventArgs e)
        {
            modelEditor.staticObstacle.Location = new System.Drawing.Point(920, 77);
            gameObjects.Add(new StaticObstacle(new Point(mouseX, mouseY), 40, 40));
            updatePreview();
        }

        public void ExplodingObstacle_MouseUp(object sender, MouseEventArgs e)
        {
            modelEditor.explodingObstacle.Location = new System.Drawing.Point(920, 137);
            gameObjects.Add(new ExplodingObstacle(new Point(mouseX, mouseY), 40, 40));
            updatePreview();
        }

        public void MovingExplodingObstacle_MouseUp(object sender, MouseEventArgs e)
        {
            modelEditor.movingExplodingObstacle.Location = new System.Drawing.Point(920, 187);
            gameObjects.Add(new MovingExplodingObstacle(new Point(mouseX, mouseY), 40, 40));
            updatePreview();
        }

        public void SlowingObstacle_MouseUp(object sender, MouseEventArgs e)
        {
            modelEditor.slowingObstacle.Location = new System.Drawing.Point(920, 237);
            gameObjects.Add(new SlowingObstacle(new Point(mouseX, mouseY), 40, 40));
            updatePreview();
        }

        public void updateDragPosition(object sender, MouseEventArgs e)
        {
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
                updatePreview();
            }
        }

        public void clearAll_Click(object sender, EventArgs e)
        {
            if (gameObjects.Count != 0)
            {
                gameObjects.Clear();
                updatePreview();
            }
        }

        public void updatePreview()
        {
            modelEditor.gamePanel.Refresh();
            gamePanelGraphics.DrawImage(new Bitmap(Resources.IconWIN), 750, 400, 80, 80);
            gamePanelGraphics.DrawImage(new Bitmap(Resources.IconSP), 5, -5, 80, 80);
            foreach (GameObject gameObject in gameObjects)
            {
                gamePanelGraphics.DrawImage(gameObject.ObjectImage, gameObject.Location.X, gameObject.Location.Y, gameObject.Width, gameObject.Height);
            }
        }

        public void GamePanel_Paint(object sender, PaintEventArgs e)
        {
            var p = sender as Panel;
            g = e.Graphics;
            gamePanelGraphics = modelEditor.gamePanel.CreateGraphics();            
        }

      

    }
}
