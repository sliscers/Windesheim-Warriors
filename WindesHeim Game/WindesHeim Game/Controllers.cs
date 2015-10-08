using Lidgren.Network;
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

        public Player GetLocalPlayer() {
            ModelGame mg = (ModelGame)model;

            foreach (GameObject gameObject in mg.GameObjects) {
                if(gameObject is Player) {
                    Player player = (Player)gameObject;
                    if(player.localPlayer) {
                        return player;
                    }
                }
            }
            return null;
        }

        private bool ArePlayersActive() {
            ModelGame mg = (ModelGame)model;

            foreach (GameObject gameObject in mg.GameObjects) {
                if (gameObject is Player) {
                    return true;
                }
            }

            return false;
        }

        private void GameLoop(object sender, EventArgs e)
        {
            if(ArePlayersActive()) {
                ProcessUserInput();
                ProcessObstacles();
                counter++;

                ModelGame mg = (ModelGame)model;
                mg.graphicsPanel.Invalidate();
                GetClosestObstacle();
                UpdateObstacleLabels(closestObstacle, nextClosestObstacle);

                mg.networkPlayer.SendPositionToServer(GetLocalPlayer());
            }       
        }

        private void ProcessUserInput() 
        {
            ModelGame mg = (ModelGame) model;

            if(GetLocalPlayer().SpeedCooldown > 0)
            {
                GetLocalPlayer().SpeedCooldown--;
            }

            if (pressedSpeed && (GetLocalPlayer().SpeedCooldown == 0))
            {
                GetLocalPlayer().Speed = GetLocalPlayer().OriginalSpeed * 2;
                UpdatePlayerSpeed("snel");
                GetLocalPlayer().SpeedDuration ++;
              
            }
            if(GetLocalPlayer().SpeedDuration > 50)
            {
                GetLocalPlayer().SpeedDuration = 0;
                GetLocalPlayer().Speed = GetLocalPlayer().OriginalSpeed;
                GetLocalPlayer().SpeedCooldown = 200;
            }

            if (pressedDown && GetLocalPlayer().Location.Y <= (mg.graphicsPanel.Size.Height + mg.graphicsPanel.Location.Y) - GetLocalPlayer().Height) {
                GetLocalPlayer().Location = new Point(GetLocalPlayer().Location.X, GetLocalPlayer().Location.Y + GetLocalPlayer().Speed);
                UpdatePlayerPosition();
            }
            if (pressedUp && GetLocalPlayer().Location.Y >= mg.graphicsPanel.Location.Y) {
                GetLocalPlayer().Location = new Point(GetLocalPlayer().Location.X, GetLocalPlayer().Location.Y - GetLocalPlayer().Speed);
                UpdatePlayerPosition();
            }
            if (pressedLeft && GetLocalPlayer().Location.X >= mg.graphicsPanel.Location.X ) {
                GetLocalPlayer().Location = new Point(GetLocalPlayer().Location.X - GetLocalPlayer().Speed, GetLocalPlayer().Location.Y);
                UpdatePlayerPosition();
            }
            if (pressedRight && GetLocalPlayer().Location.X <= (mg.graphicsPanel.Size.Width + mg.graphicsPanel.Location.X) - GetLocalPlayer().Width) {
                GetLocalPlayer().Location = new Point(GetLocalPlayer().Location.X + GetLocalPlayer().Speed, GetLocalPlayer().Location.Y);
                UpdatePlayerPosition();
            }
        }

        private void UpdatePlayerPosition()
        {
            ModelGame mg = (ModelGame)model;
            mg.lblCharacterPosX.Text = GetLocalPlayer().Location.X.ToString();
            mg.lblCharacterPosY.Text = GetLocalPlayer().Location.Y.ToString();
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

            int playerX = GetLocalPlayer().Location.X;
            int playerY = GetLocalPlayer().Location.Y;
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

                    gameObstacle.ChasePlayer(GetLocalPlayer());

                    // Loop door alle objecten op het veld
                    foreach (GameObject potentialCollision in safeListArray) {
                        // We willen niet onszelf checken, en we willen alleen collision voor StaticObstacles en ExplodingObstacles
                        if (gameObject != potentialCollision && (potentialCollision is StaticObstacle || potentialCollision is ExplodingObstacle)) {
                            gameObject.ProcessCollision(potentialCollision);
                        }
                    }

                    if (gameObstacle.CollidesWith(GetLocalPlayer())) {
                        GetLocalPlayer().Location = new Point(0, 0);
                        UpdatePlayerPosition();
                        mg.InitializeField();
                        mg.GameObjects.Add(new Explosion(gameObstacle.Location, 10, 10));
                        GetLocalPlayer().ObjectImage = Resources.Player;
                    }
                }

                if (gameObject is SlowingObstacle)
                {
                    SlowingObstacle gameObstacle = (SlowingObstacle)gameObject;

                    gameObstacle.ChasePlayer(GetLocalPlayer());

                    // Loop door alle objecten op het veld
                    foreach (GameObject potentialCollision in safeListArray) {
                        // We willen niet onszelf checken, maar we willen we collision op alles
                        if (gameObject != potentialCollision) {
                            gameObject.ProcessCollision(potentialCollision);
                        }
                    }

                    if (GetLocalPlayer().CollidesWith(gameObstacle))
                    {
                        GetLocalPlayer().Speed = GetLocalPlayer().OriginalSpeed / 2;
                        UpdatePlayerSpeed("Langzaam");
                    }
                    else
                    {
                        GetLocalPlayer().Speed = GetLocalPlayer().OriginalSpeed;
                        if (pressedSpeed && (GetLocalPlayer().SpeedCooldown == 0))
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

                    if (GetLocalPlayer().CollidesWith(gameObstacle))
                    {
                        GetLocalPlayer().Location = new Point(0, 0);
                        UpdatePlayerPosition();
                        mg.InitializeField();
                        mg.GameObjects.Add(new Explosion(gameObstacle.Location, 10, 10));
                        GetLocalPlayer().ObjectImage = Resources.Player;
                    }

                    
                }

                if (gameObject is StaticObstacle)
                {
                    StaticObstacle gameObstacle = (StaticObstacle)gameObject;

                    if (GetLocalPlayer().CollidesWith(gameObstacle)) 
                    {
                        if (pressedUp)
                        {
                            GetLocalPlayer().Location = new Point(GetLocalPlayer().Location.X, GetLocalPlayer().Location.Y + GetLocalPlayer().Speed);
                        }
                        if (pressedDown)
                        {
                            GetLocalPlayer().Location = new Point(GetLocalPlayer().Location.X, GetLocalPlayer().Location.Y - GetLocalPlayer().Speed);
                        }
                        if (pressedLeft)
                        {
                            GetLocalPlayer().Location = new Point(GetLocalPlayer().Location.X + GetLocalPlayer().Speed, GetLocalPlayer().Location.Y);
                        }
                        if (pressedRight)
                        {
                            GetLocalPlayer().Location = new Point(GetLocalPlayer().Location.X - GetLocalPlayer().Speed, GetLocalPlayer().Location.Y);
                        }
                    }
                }
                if (gameObject is Checkpoint)
                {
                    Checkpoint gameObstacle = (Checkpoint)gameObject;
                    if (GetLocalPlayer().CollidesWith(gameObstacle) && !gameObstacle.Start)
                    {
                        GetLocalPlayer().Location = new Point(0, 0);
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

                if(gameObject is Explosion) 
                {
                    g.DrawImage(gameObject.ObjectImage, gameObject.Location.X, gameObject.Location.Y, gameObject.Width, gameObject.Height);           
                }

                //g.DrawRectangle(new Pen(Color.Red), new Rectangle(gameObject.Location.X, gameObject.Location.Y, gameObject.Width, gameObject.Height));
               if(gameObject is Player) {
                    g.DrawImage(gameObject.ObjectImage, gameObject.Location.X, gameObject.Location.Y, gameObject.Width, gameObject.Height);
                }
            }
            // Teken player
            //g.DrawImage(mg.player.ObjectImage, mg.player.Location.X, mg.player.Location.Y, mg.player.Width, mg.player.Height);
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
                GetLocalPlayer().ObjectImage = Resources.PlayerLeft;
            }
            if (e.KeyCode == Keys.D) {
                pressedRight = true;
                GetLocalPlayer().ObjectImage = Resources.Player;
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
                GetLocalPlayer().Speed = 5;

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
            gameWindow.setController(ScreenStates.game);

            //Workaround om focus conflict met windows forms en buttons op te lossen
            modelLevelSelect.alignPanel.Controls.Remove(modelLevelSelect.playLevel);
            modelLevelSelect.alignPanel.Controls.Remove(modelLevelSelect.goBack);
            modelLevelSelect.alignPanel.Controls.Remove(modelLevelSelect.listBoxLevels);
        }

        public void level_Select(object sender, EventArgs e)
        {
            ListBox listBoxLevels = (ListBox)sender;
            currentSelectedLevel = (XMLParser)listBoxLevels.SelectedItem;

            modelLevelSelect.gamePanel.Invalidate(); // refresh
        }

        public void OnPreviewPaint(object sender, PaintEventArgs e) {
            Graphics g = e.Graphics;

            // Teken preview
            if(currentSelectedLevel != null) {
                List<GameObject> previewList = new List<GameObject>(currentSelectedLevel.gameObjects);
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
                if(i == 0)
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
            //gameWindow.setController(ScreenStates.editorNewLevel);
        }
    }

    public class ControllerEditor : Controller
    {
        public static XMLParser level;

        private ModelEditor modelEditor;

        public ControllerEditor(GameWindow form) : base(form)
        {
            this.model = new ModelEditor(this);
            this.modelEditor = (ModelEditor)model;
        }

        public void goBack_Click(object sender, EventArgs e)
        {
            gameWindow.setController(ScreenStates.editorSelect);
        }

        public override void RunController()
        {
            base.RunController();
            if(level == null) //New Level aanmaken
            {

            }else{ //Bestaand level bewerken

            }
        }

        public void playLevel_Click(object sender, EventArgs e)
        {
            if(level == null)
            {
                Console.WriteLine("error, level is null");
            }            else            {
                ModelGame.level = level;
                gameWindow.setController(ScreenStates.game);

                //Workaround om focus conflict met windows forms en buttons op te lossen
                modelEditor.alignPanel.Controls.Remove(modelEditor.playLevel);
                modelEditor.alignPanel.Controls.Remove(modelEditor.goBack);
                modelEditor.alignPanel.Controls.Remove(modelEditor.listBoxLevels);
            }

        }

    }
}
