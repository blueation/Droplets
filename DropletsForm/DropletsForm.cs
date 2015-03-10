using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Design;
using Microsoft.Xna.Framework;
using System.Timers;

namespace Droplets
{
    class DropletsGame : Form
    {
        //GameState
        public static bool inMenu = true;
        public static bool inLevelSelect = false;
        public static int levelnr = -1;
        public static bool playMusic = true;
        public static bool playSound = true;

        //MenuGUIState
        public static Control PlayButton = new DropletButton("Play");
        public static Control SoundButton = new DropletButton("Music");
        public static Image musicimage = new Bitmap("../../assets/Music.png");
        public static Image soundimage = new Bitmap("../../assets/Sound.png");
        public static Image muteimage = new Bitmap("../../assets/Mute.png");
        public static Image onlymusicimage = new Bitmap("../../assets/OnlyMusic.png");
        public static Control QuitButton = new DropletButton("Quit");

        //SelectState
        public static Label ChapterNrName;
        public static DropletButton[] LevelArray = new DropletButton[12];  //12 or so
        public static Dictionary<string, Level> LevelDictionary = new Dictionary<string, Level>();
        public static Control PreviousButton = new DropletButton("Back");
        public static Image prevposs = new Bitmap("../../assets/Back.png");
        public static Image previmpo = new Bitmap("../../assets/BackImpossible.png");
        public static Control NextButton = new DropletButton("Next");
        public static Image nextposs = new Bitmap("../../assets/Next.png");
        public static Image nextimpo = new Bitmap("../../assets/NextImpossible.png");
        public static int selectionIndex;

        //LevelGUIState
        public static string loadedstring;
        public static string levelname;
        public static Label ChapterLevel;
        public static int zonesnumber;
        public static Label CompletePercentage;
        public static Control BackButton = new DropletButton("Level Select Alt");
        public static Control UndoButton = new DropletButton("Undo");
        public static Control ResetButton = new DropletButton("Reset");
        
        //LevelState
        public static List<Source> Sources = new List<Source>();
        public static List<Source> NewSources = new List<Source>();
        public static List<SubmitZone> SubmitZones = new List<SubmitZone>();
        public static History GameHistory;

        //InputState
        public static Source DragSource = null;
        public static Source LastDragged = null;
        public static bool Dragging = false;
        public static TTASLock DragLock = new TTASLock();
        public static bool OnlyForcedUpdate = true;

        //OutputState
        public static TTASLock DrawLock = new TTASLock();

        private static System.Timers.Timer update;

        public DropletsGame()
        {
            this.Text = "Droplets";
            this.ClientSize = new Size(800, 480);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.CenterToScreen();
            this.KeyPreview = true;
            this.DoubleBuffered = true;
            this.BackColor = System.Drawing.Color.GhostWhite;

            PlayButton.Location = new System.Drawing.Point(this.ClientSize.Width / 2 - PlayButton.Width / 2, this.ClientSize.Height / 2 - PlayButton.Height / 2);
            this.Controls.Add(PlayButton);
            SoundButton.Location = new System.Drawing.Point(10, this.ClientSize.Height - 70);
            this.Controls.Add(SoundButton);
            QuitButton.Location = new System.Drawing.Point(this.ClientSize.Width - 70, this.ClientSize.Height - 70);
            this.Controls.Add(QuitButton);

            for (int i = 0; i < 12; i++ )
            {
                LevelArray[i] = new DropletButton("Generic");
                LevelArray[i].Location = new System.Drawing.Point(this.ClientSize.Width / 7 * (i % 4 + 2) - LevelArray[i].Width / 2, this.ClientSize.Height / 4 * (i / 4 + 1) - LevelArray[i].Height / 2);
                this.Controls.Add(LevelArray[i]);
            }
            PreviousButton.Location = new System.Drawing.Point(this.ClientSize.Width / 7 - BackButton.Width / 2, this.ClientSize.Height / 2 - BackButton.Height / 2);
            this.Controls.Add(PreviousButton);
            NextButton.Location = new System.Drawing.Point(this.ClientSize.Width / 7 * 6 - BackButton.Width / 2, this.ClientSize.Height / 2 - BackButton.Height / 2);
            this.Controls.Add(NextButton);

            BackButton.Location = new System.Drawing.Point(10, 10);
            this.Controls.Add(BackButton);
            UndoButton.Location = new System.Drawing.Point(this.ClientSize.Width - 140, this.ClientSize.Height - 70);
            this.Controls.Add(UndoButton);
            ResetButton.Location = new System.Drawing.Point(this.ClientSize.Width - 70, this.ClientSize.Height - 70);
            this.Controls.Add(ResetButton);

            PlayButton.Click += this.PlayHandler;
            BackButton.Click += this.BackHandler;
            UndoButton.Click += this.UndoHandler;
            ResetButton.Click += this.ResetHandler;
            NextButton.Click += this.NextHandler;
            PreviousButton.Click += this.PreviousHandler;
            QuitButton.Click += this.QuitHandler;
            SoundButton.Click += this.SoundButtonHandler;

            for (int i = 0; i < 12; i++ )
            {
                int j = i; //handles a bug in csharp, ask me for details if you want to know more. -Blueation
                LevelArray[i].Click += (sender, e) => LevelArrayHandler(sender, e, j); //passing a variable to the handler as well.
            }

            this.MouseDown += this.MouseDownHandler;
            this.MouseUp += this.MouseUpHandler;
            this.MouseMove += this.MouseMoveHandler;
            if (!OnlyForcedUpdate)
            {
                update = new System.Timers.Timer(10);
                update.Elapsed += new ElapsedEventHandler(Update);
                update.Enabled = true;
            }
            this.KeyDown += this.KeyDownHandler;

            this.Paint += this.Draw;

            RetrieveLevels();
            SetupMainMenu();
            GameHistory = new History(5, Sources);
        }

#region MouseEvents
        public void MouseDownHandler(object o, MouseEventArgs mea)
        {
            //Console.WriteLine("MouseDown" + mea.X + ", " + mea.Y);
            if (levelnr >= 0)
            {
                foreach (Source s in Sources)
                {
                    if (s.Active && s.isIn(mea.X, mea.Y))
                    {
                        DragLock.LockIt();
                        Dragging = true;
                        DragSource = s;
                        LastDragged = s;
                        s.dragged = true;
                        DragLock.UnlockIt();
                    }
                }
            }
            this.Invalidate();
        }

        public void MouseUpHandler(object o, MouseEventArgs mea)
        {
            if (levelnr >= 0)
            {
                DragLock.LockIt();
                    //Console.WriteLine("MouseUp" + mea.X + ", " + mea.Y);
                    if (DragSource != null)
                        DragSource.dragged = false;
                    //     Console.WriteLine("angle:{0}", MathHelper.angleCalculate(DragSource.SourceAnchor, DragSource.ExtensionAnchor));
                    DragSource = null;
                    Dragging = false;
                DragLock.UnlockIt();
            }
            this.Invalidate();
        }

        public void MouseMoveHandler(object o, MouseEventArgs mea)
        {
            if (levelnr >= 0)
            {
                DragLock.LockIt();
                    if (DragSource != null)
                    {
                        if (MathHelper.distance(DragSource.SourceAnchor, mea.X, mea.Y) <= DragSource.SourceSize.getMaxStretch)
                            DragSource.ExtensionAnchor = new Vector2(mea.X, mea.Y);
                        else
                        {
                            Vector2 mouse = new Vector2(mea.X, mea.Y);
                            Vector2 extension = mouse - DragSource.SourceAnchor;
                            extension.Normalize();
                            extension *= DragSource.SourceSize.getMaxStretch;
                            extension += DragSource.SourceAnchor;
                            DragSource.ExtensionAnchor = new Vector2(extension.X, extension.Y);
                        }
                    }
                DragLock.UnlockIt();
            }
            this.Invalidate();
        }
#endregion
#region ButtonEvents
        public void PlayHandler(object o, EventArgs ea)
        {
            SetupLevelSelection();
        }

        public void SoundButtonHandler(object o, EventArgs ea)
        {
            if (playMusic && playSound)
            {
                playMusic = false;
                SoundButton.BackgroundImage = soundimage;
            }
            else if (!playMusic && playSound)
            {
                playSound = false;
                SoundButton.BackgroundImage = muteimage;
            }
            else if (!playMusic && !playSound)
            {
                playMusic = true;
                SoundButton.BackgroundImage = onlymusicimage;
            }
            else
            {
                playSound = true;
                SoundButton.BackgroundImage = musicimage;
            }
        }

        public void LevelArrayHandler(object o, EventArgs ea, int i)
        {
            Level temp = LevelDictionary.ToArray()[i + selectionIndex * 12].Value;
            SetupLevel(temp);
        }

        public void QuitHandler(object o, EventArgs ea)
        {
            if (inMenu)
                this.Close();
            else if (inLevelSelect)
                SetupMainMenu();
            else
                SetupLevelSelection();
        }

        public void NextHandler(object o, EventArgs ea)
        {
            selectionIndex--;
            if (selectionIndex < 0)
                selectionIndex = 0;
            else
                RefreshLevelSet();
        }

        public void PreviousHandler(object o, EventArgs ea)
        {
            selectionIndex++;
            if (selectionIndex * 12 > LevelDictionary.Count)
                selectionIndex--;
            else
                RefreshLevelSet();
        }

        public void BackHandler(object o, EventArgs ea)
        {
            SetupLevelSelection();
        }

        public void UndoHandler(object o, EventArgs ea)
        {
            DrawLock.LockIt();
                PopHistory();
            DrawLock.UnlockIt();
            this.Invalidate();
        }

        public void ResetHandler(object o, EventArgs ea)
        {
            DrawLock.LockIt();
                GameHistory.Clear();
                SetupLevel(LevelDictionary[loadedstring]);
            DrawLock.UnlockIt();
            this.Invalidate();
        }
#endregion
#region KeyEvents
        public void KeyDownHandler(object o, KeyEventArgs kea)
        {
            if (OnlyForcedUpdate && kea.KeyCode == Keys.Enter) //TODO: find out how to fix that the buttons of the game catch the enter...
                Update();
        }
#endregion
#region Game Logic
        public void Update(object o, ElapsedEventArgs e)
        {
            Update();
        }

        public void Update()
        {
            if (levelnr >= 0)
            {
                int AllFilled = 0;
                DrawLock.LockIt();
                //Console.WriteLine("Lock of Draw: Update");
                foreach (Source s in Sources)
                {
                    if (s.Active)
                    {
                        foreach (SubmitZone zone in SubmitZones)
                        {
                            bool testZone = zone.isCollision(s);
                            zone.Filled = testZone;
                            if (testZone)
                            {
                                AllFilled++;
                            }
                        }

                        foreach (Source s2 in Sources)
                        {
                            if (s != s2 && s2.Active)
                            {
                                Tuple<float, float> newloc = s.returnCollision(s2);

                                if (newloc != null && s.DefaultBehaviour && s2.DefaultBehaviour)
#region DEFAULT BEHAVIOUR
                                {
                                    if (s.SourceColour.ToString() != s2.SourceColour.ToString())
#region NOT SAME COLOUR BEHAVIOUR
                                    {
                                        PushHistory();

                                        int newsize = Math.Min(s.SourceSize.toInt, s2.SourceSize.toInt);

                                        s.SourceSize = new BlobSize().fromInt(s.SourceSize.toInt - newsize);
                                        if (s.SourceSize.toInt == 0)
                                            s.Deactivate();
                                        s.FullRetract();

                                        s2.SourceSize = new BlobSize().fromInt(s2.SourceSize.toInt - newsize);
                                        if (s2.SourceSize.toInt == 0)
                                            s2.Deactivate();
                                        s2.FullRetract();

                                        if (DragSource != null)
                                            DragSource.dragged = false;
                                        DragSource = null;
                                        Dragging = false;
                                        this.Invalidate();

                                        //Console.WriteLine("made new source! size:{0}", newsize);
                                        BlobSize bSize = new BlobSize().fromInt(newsize);
                                        BlobColour bColour = ColourMixer.mix(s.SourceColour, s2.SourceColour);
                                        Vector2 bLoc = new Vector2(newloc.Item1, newloc.Item2);
                                        NewSources.Add(new Source(bColour, bSize, bLoc));
                                    }
#endregion
                                    else
#region SAME COLOUR BEHAVIOUR
                                    {
                                        PushHistory();

                                        int newsize = s.SourceSize.toInt + s2.SourceSize.toInt;
                                        if (newsize > 3)
                                            newsize = 3;

                                        s.SourceSize = new BlobSize().fromInt(0);
                                        s.ExtensionAnchor = s.SourceAnchor;
                                        if (s.SourceSize.toInt == 0)
                                            s.Deactivate();
                                        s.FullRetract();

                                        s2.SourceSize = new BlobSize().fromInt(0);
                                        s2.ExtensionAnchor = s2.SourceAnchor;
                                        if (s2.SourceSize.toInt == 0)
                                            s2.Deactivate();
                                        s2.FullRetract();

                                        DragSource = null;
                                        Dragging = false;
                                        this.Invalidate();

                                        //Console.WriteLine("made new source! size:{0}", newsize);
                                        BlobSize bSize = new BlobSize().fromInt(newsize);
                                        BlobColour bColour = s.SourceColour;
                                        Vector2 bLoc = new Vector2(newloc.Item1, newloc.Item2);
                                        NewSources.Add(new Source(bColour, bSize, bLoc));
                                    }
#endregion
                                }
#endregion
                                else if (newloc != null && s.SourceColour.ToString() == "White")
#region WHITEBEHAVIOUR 1
                                {
                                    PushHistory();

                                    s.Deactivate();
                                    s.FullRetract();

                                    s2.FullRetract();

                                    BlobColour bColour = ColourMixer.mix(s.SourceColour, s2.SourceColour);
                                    Vector2 bLoc = new Vector2(newloc.Item1, newloc.Item2);
                                    NewSources.Add(new Source(bColour, s.SourceSize, bLoc));
                                }
#endregion
                                else if (newloc != null && s2.SourceColour.ToString() == "White")
#region WHITEBEHAVIOUR 2
                                {
                                    PushHistory();

                                    s.FullRetract();

                                    s2.Deactivate();
                                    s2.FullRetract();

                                    BlobColour bColour = ColourMixer.mix(s.SourceColour, s2.SourceColour);
                                    Vector2 bLoc = new Vector2(newloc.Item1, newloc.Item2);
                                    NewSources.Add(new Source(bColour, s2.SourceSize, bLoc));
                                }
#endregion
                            }
                        }
                    }
                }

                foreach (Source s in NewSources)
                    Sources.Add(s);
                NewSources.Clear();

                if (SubmitZones.Count == AllFilled)
                    LevelCompleted();

                bool invalidatedForm = false;

                if (!Dragging)
                {
                    foreach (Source s in Sources)
                        s.Retract();
                    invalidatedForm = true;
                }

                DrawLock.UnlockIt();
                //Console.WriteLine("Unlock of Draw: Update");

                if (invalidatedForm)
                    this.Invalidate(); ;
            }
        }

        public void PushHistory()
        {
            GameHistory.Add(Sources);
        }

        public void PopHistory()
        {
            Sources.Clear();
            List<Source> retrieved = GameHistory.Retrieve();
            foreach (Source s in retrieved)
                Sources.Add(s.Copy());
        }
#endregion
#region Menu and Level Logic
        public void RetrieveLevels()
        {
            string[] levelpaths = LevelLoader.AllPathsOfDirectory("Levels/");
            
            foreach(string filepath in levelpaths)
                LevelDictionary.Add(filepath, LevelLoader.LoadLevel(filepath));
        }

        public void SetupLevel(Level level)
        {
            inMenu = false;
            inLevelSelect = false;

            loadedstring = level.refname;
            Sources.Clear();
            foreach (Source s in level.sources)
                Sources.Add(s.Copy());
            SubmitZones = level.submitzones;
            levelnr = level.nr;
            levelname = level.name;

            PlayButton.Visible = false;
            SoundButton.Visible = false;
            QuitButton.Visible = false;

            NextButton.Visible = false;
            PreviousButton.Visible = false;
            foreach (Control c in LevelArray)
                c.Visible = false;
            selectionIndex = 0;

            BackButton.Visible = true;
            UndoButton.Visible = true;
            ResetButton.Visible = true;
        }

        public void SetupLevelSelection()
        {
            inMenu = false;
            inLevelSelect = true;
            levelnr = -1;

            PlayButton.Visible = false;
            SoundButton.Visible = false;
            QuitButton.Visible = true;

            NextButton.Visible = true;
            PreviousButton.Visible = true;
            selectionIndex = 0;

            BackButton.Visible = false;
            UndoButton.Visible = false;
            ResetButton.Visible = false;

            RefreshLevelSet();
        }

        public void RefreshLevelSet()
        {
            foreach (Control c in LevelArray)
                c.Visible = false;

            int i = 0;
            while (i < 12 && i + selectionIndex * 12 < LevelDictionary.Count)
            {
                LevelArray[i].Visible = true;
                i++;
            }

            if (selectionIndex <= 0)
                PreviousButton.BackgroundImage = previmpo;
            else
                PreviousButton.BackgroundImage = prevposs;

            if (i + selectionIndex * 12 >= LevelDictionary.Count)
                NextButton.BackgroundImage = nextimpo;
            else
                NextButton.BackgroundImage = nextposs;
        }

        public void SetupMainMenu()
        {
            inMenu = true;
            inLevelSelect = false;
            levelnr = -1;

            PlayButton.Visible = true;
            QuitButton.Visible = true;
            SoundButton.Visible = true;
            UndoButton.Visible = false;
            ResetButton.Visible = false;

            NextButton.Visible = false;
            PreviousButton.Visible = false;
            foreach (Control c in LevelArray)
                c.Visible = false;
            selectionIndex = 0;

            BackButton.Visible = false;
            UndoButton.Visible = false;
            ResetButton.Visible = false;
        }
#endregion
        public void LoadBenchmarkLevel()
        {
            levelnr = 0;
            Sources.Add(new Source(new BlueColour(), new SmallSize(), new Vector2(240, 240)));
            Sources.Add(new Source(new GreenColour(), new LargeSize(), new Vector2(400, 240)));
            Sources.Add(new Source(new BlueColour(), new SmallSize(), new Vector2(180, 180)));
            Sources.Add(new Source(new BlueColour(), new SmallSize(), new Vector2(20, 180)));
            Sources.Add(new Source(new WhiteColour(), new MediumSize(), new Vector2(500, 240)));
        }

        public void LevelCompleted()
        {
            Console.WriteLine("Hurray! All zones filled!");
        }

        public void Draw(object o, PaintEventArgs pea)
        {
            if (levelnr >= 0)
            {
                if (DrawLock.TryLock())
                {
                    //Console.WriteLine("Lock of Draw: Draw");
                    foreach (SubmitZone s in SubmitZones)
                    {
                        s.Draw(pea.Graphics);
                    }
                    foreach (Source s in Sources)
                    {
                        s.Draw(pea.Graphics);
                    }
                    
                    DrawLock.UnlockIt();
                    //Console.WriteLine("Unlock of Draw: Draw");
                }
                else
                    Console.WriteLine("draw failed");
            }
        }
    }
}
