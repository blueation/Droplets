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
        public static Control SoundButton;
        public static Control QuitButton = new DropletButton("Quit");

        //SelectState
        public static Label ChapterNrName;
        public static List<Control> LevelList;  //12 or so
        public static Dictionary<string, Level> LevelDictionary = new Dictionary<string, Level>();
        public static Control Previous;
        public static Control Next;

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
        public static bool OnlyForcedUpdate = false;

        //OutputState
        public static TTASLock DrawLock = new TTASLock();

        private static System.Timers.Timer update;

        public DropletsGame()
        {
            //LoadBenchmarkLevel();
            RetrieveLevels();
            GameHistory = new History(5, Sources);

            this.Text = "Droplets";
            this.ClientSize = new Size(800, 480);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.CenterToScreen();
            this.KeyPreview = true;
            this.DoubleBuffered = true;
            this.BackColor = System.Drawing.Color.GhostWhite;


            BackButton.Location = new System.Drawing.Point(10, 10);
            this.Controls.Add(BackButton);
            UndoButton.Location = new System.Drawing.Point(this.ClientSize.Width - 140, this.ClientSize.Height - 70);
            this.Controls.Add(UndoButton);
            ResetButton.Location = new System.Drawing.Point(this.ClientSize.Width - 70, this.ClientSize.Height - 70);
            this.Controls.Add(ResetButton);


            BackButton.Click += this.BackHandler;
            UndoButton.Click += this.UndoHandler;
            ResetButton.Click += this.ResetHandler;

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
        public void BackHandler(object o, EventArgs ea)
        {
            
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

        public void RetrieveLevels()
        {
            string[] levelpaths = LevelLoader.AllPathsOfDirectory("Levels/");
            
            foreach(string filepath in levelpaths)
                LevelDictionary.Add(filepath, LevelLoader.LoadLevel(filepath));
            SetupLevel(LevelDictionary["Levels/Level0.txt"]);
        }

        public void SetupLevel(Level level)
        {
            loadedstring = level.refname;
            Sources.Clear();
            foreach (Source s in level.sources)
                Sources.Add(s.Copy());
            SubmitZones = level.submitzones;
            levelnr = level.nr;
            levelname = level.name;
        }

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
