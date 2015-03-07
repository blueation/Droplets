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
        public static Control PlayButton;
        public static Control SoundButton;
        public static Control QuitButton;

        //SelectState
        public static Label ChapterNrName;
        public static List<Control> LevelList;  //12 or so
        public static Control Previous;
        public static Control Next;

        //LevelGUIState
        public static string levelname;
        public static Label ChapterLevel;
        public static int zonesnumber;
        public static Label CompletePercentage;
        public static Control BackButton;
        public static Control UndoButton;
        public static Control RestartButton;
        
        //LevelState
        public static List<Source> Sources;
        public static List<Source> NewSources;
        public static List<SubmitZone> SubmitZones;

        //InputState
        public static Source DragSource;
        public static bool Dragging = false;
        public static TTASLock DragLock = new TTASLock();

        //OutputState
        public static TTASLock DrawLock = new TTASLock();

        private static System.Timers.Timer update;

        public DropletsGame()
        {
            Sources = new List<Source>();
            NewSources = new List<Source>();
            SubmitZones = new List<SubmitZone>();
            //LoadBenchmarkLevel();

            Level test = LevelLoader.LoadLevel("Levels/level0.txt");
            LoadLevel(test);

            this.ClientSize = new Size(800, 480);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.CenterToScreen();
            this.DoubleBuffered = true;
            this.BackColor = System.Drawing.Color.GhostWhite;

            this.MouseDown += this.MouseDownHandler;
            this.MouseUp += this.MouseUpHandler;
            this.MouseMove += this.MouseMoveHandler;
            update = new System.Timers.Timer(10);
            update.Elapsed += new ElapsedEventHandler(Update);
            update.Enabled = true;

            this.Paint += this.Draw;
        }

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

        public void Update(object o, ElapsedEventArgs e)
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
                                #region DEFAULTBEHAVIOUR
                                {
                                    if (s.SourceColour.ToString() != s2.SourceColour.ToString())
                                    {
                                        int newsize = Math.Min(s.SourceSize.toInt, s2.SourceSize.toInt);

                                        s.SourceSize = new BlobSize().fromInt(s.SourceSize.toInt - newsize);
                                        s.ExtensionAnchor = s.SourceAnchor;
                                        if (s.SourceSize.toInt == 0)
                                            s.Deactivate();
                                        s.FullRetract();

                                        s2.SourceSize = new BlobSize().fromInt(s2.SourceSize.toInt - newsize);
                                        s2.ExtensionAnchor = s2.SourceAnchor;
                                        if (s2.SourceSize.toInt == 0)
                                            s2.Deactivate();
                                        s2.FullRetract();

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
                                    else
                                    {
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

                DrawLock.UnlockIt();
                //Console.WriteLine("Unlock of Draw: Update");

                if (!Dragging)
                {
                    foreach (Source s in Sources)
                        s.Retract();
                    this.Invalidate();
                }
            }
        }

        public void LoadLevel(Level level)
        {
            Sources = level.sources;
            SubmitZones = level.submitzones;
            levelnr = level.nr;
            levelname = level.name;
        }

        public void LoadBenchmarkLevel()
        {
            Sources.Add(new Source(new BlueColour(), new SmallSize(), new Vector2(240, 240)));
            Sources.Add(new Source(new GreenColour(), new LargeSize(), new Vector2(400, 240)));
            Sources.Add(new Source(new BlueColour(), new SmallSize(), new Vector2(180, 180)));
            Sources.Add(new Source(new BlueColour(), new SmallSize(), new Vector2(20, 180)));
        }

        public void LevelCompleted()
        {

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
