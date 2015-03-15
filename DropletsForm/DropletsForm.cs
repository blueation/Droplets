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
        //Options and DebugHelpers
        public static bool noRestartButton = true;
        public static bool OnlyForcedUpdate = false;
        public static Button OnlyForcedUpdateButton = new Button();

        //GameState
        public static bool inMenu = true;
        public static bool inLevelSelect = false;
        public static int levelnr = -1;
        public static bool playMusic = true;
        public static bool playSound = true;

        //MenuGUIState
        public static DropletButton PlayButton;
        public static DropletButton SoundButton;
        public static Image musicimage = new Bitmap("assets/Music.png");
        public static Image soundimage = new Bitmap("assets/Sound.png");
        public static Image muteimage = new Bitmap("assets/Mute.png");
        public static Image onlymusicimage = new Bitmap("assets/OnlyMusic.png");
        public static DropletButton QuitButton;
        public static Font font = new System.Drawing.Font("Helvetica", 32, FontStyle.Bold, GraphicsUnit.Pixel);

        //SelectState
        public static Dictionary<string, Level> LevelDictionary = new Dictionary<string, Level>();
        public static List<Chapter> chapters = new List<Chapter>();
        public static Label ChapterNrName = new Label();
        public static DropletButton[] LevelArray = new DropletButton[12];  //12 or so
        public static DropletButton PreviousButton;
        public static Image prevposs = new Bitmap("assets/Back.png");
        public static Image previmpo = new Bitmap("assets/BackImpossible.png");
        public static DropletButton NextButton;
        public static Image nextposs = new Bitmap("assets/Next.png");
        public static Image nextimpo = new Bitmap("assets/NextImpossible.png");
        public static int selectedChapter;
        public static int selectionIndex;

        //LevelGUIState
        public static string loadedstring;
        public static string levelname;
        public static int zonesnumber;
        public static int zonesfilled;
        public static bool completed = false;
        public static int completedtiming = 0;
        public static DropletButton BackButton;
        public static DropletButton UndoButton;
        public static Image UndoUndo = new Bitmap("assets/Reset.png");
        public static Image UndoReset = new Bitmap("assets/Reset 2.png");
        public static DropletButton ResetButton;
        public static DropletButton ProgressButton;
        
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

        //OutputState
        public static TTASLock DrawLock = new TTASLock();
        WMPLib.IWMPPlaylist bgplaylist;
        static WMPLib.WindowsMediaPlayer bgplayer = new WMPLib.WindowsMediaPlayer();
        static WMPLib.WindowsMediaPlayer soundplayer = new WMPLib.WindowsMediaPlayer();
        public WMPLib.IWMPMedia positive1;

        private static System.Timers.Timer update;

        public DropletsGame()
        {
            PlayButton = new DropletButton("Play", this);
            SoundButton = new DropletButton("Music", this);
            QuitButton = new DropletButton("Quit", this);
            QuitButton = new DropletButton("Quit", this);
            BackButton = new DropletButton("Level Select Alt", this);
            UndoButton = new DropletButton("Undo", this);
            ResetButton = new DropletButton("Reset", this);
            NextButton = new DropletButton("Next", this);
            PreviousButton = new DropletButton("Back", this);
            ProgressButton = new DropletButton("Progress", this);

            bgplaylist = bgplayer.playlistCollection.newPlaylist("Music/playlist");
            bgplaylist.appendItem(bgplayer.newMedia("music/05 Unbound.mp3"));
            bgplaylist.appendItem(bgplayer.newMedia("music/11 The White River.mp3"));
            bgplaylist.appendItem(bgplayer.newMedia("music/12 Silence Unbroken.mp3"));
            bgplaylist.appendItem(bgplayer.newMedia("music/16 Journey's End.mp3"));
            bgplayer.currentPlaylist = bgplaylist;
            bgplayer.controls.play();
            bgplayer.settings.setMode("loop", true);
            //bgplayer.PlayStateChange += bgplayer_PlayStateChange;

            this.Text = "Droplets";
            this.Icon = new Icon("assets/droplets.ico");
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
                LevelArray[i] = new DropletButton("Generic", this);
                LevelArray[i].Location = new System.Drawing.Point(this.ClientSize.Width / 7 * (i % 4 + 2) - LevelArray[i].Width / 2, this.ClientSize.Height / 4 * (i / 4 + 1) - LevelArray[i].Height / 2);
                this.Controls.Add(LevelArray[i]);
            }
            PreviousButton.Location = new System.Drawing.Point(this.ClientSize.Width / 7 - BackButton.Width / 2, this.ClientSize.Height / 2 - BackButton.Height / 2);
            this.Controls.Add(PreviousButton);
            NextButton.Location = new System.Drawing.Point(this.ClientSize.Width / 7 * 6 - BackButton.Width / 2, this.ClientSize.Height / 2 - BackButton.Height / 2);
            this.Controls.Add(NextButton);
            ChapterNrName.Location = new System.Drawing.Point(70, 0);
            ChapterNrName.Size = new Size(ClientSize.Width - 140, (int)(ClientSize.Height / 5.4));
            ChapterNrName.TextAlign = ContentAlignment.MiddleCenter;
            ChapterNrName.Text = "Testing";
            ChapterNrName.Font = new Font("Helvetica", 32, FontStyle.Bold, GraphicsUnit.Pixel);
            ChapterNrName.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(ChapterNrName);

            BackButton.Location = new System.Drawing.Point(10, 10);
            this.Controls.Add(BackButton);
            if (noRestartButton)
            {
                UndoButton.BackgroundImage = new Bitmap("assets/Reset.png");
                UndoButton.Location = new System.Drawing.Point(this.ClientSize.Width - 70, this.ClientSize.Height - 70);
            }
            else
            {
                UndoButton.Location = new System.Drawing.Point(this.ClientSize.Width - 140, this.ClientSize.Height - 70);
                ResetButton.Location = new System.Drawing.Point(this.ClientSize.Width - 70, this.ClientSize.Height - 70);
                this.Controls.Add(ResetButton);
            }
            this.Controls.Add(UndoButton);
            ProgressButton.Location = new System.Drawing.Point(this.ClientSize.Width / 2 - PlayButton.Width / 2, this.ClientSize.Height / 2 - PlayButton.Height / 2);
            this.Controls.Add(ProgressButton);

            PlayButton.Click += this.PlayHandler; PlayButton.text.Click += this.PlayHandler;
            BackButton.Click += this.BackHandler; BackButton.text.Click += this.BackHandler;
            UndoButton.Click += this.UndoHandler; UndoButton.text.Click += this.UndoHandler;
            ResetButton.Click += this.ResetHandler; ResetButton.text.Click += this.ResetHandler;
            NextButton.Click += this.NextHandler; NextButton.text.Click += this.NextHandler;
            PreviousButton.Click += this.PreviousHandler; PreviousButton.text.Click += this.PreviousHandler;
            QuitButton.Click += this.QuitHandler; QuitButton.text.Click += this.QuitHandler;
            SoundButton.Click += this.SoundButtonHandler; SoundButton.text.Click += this.SoundButtonHandler;
            ProgressButton.Click += this.ProgressButtonHandler; ProgressButton.text.Click += this.ProgressButtonHandler;

            for (int i = 0; i < 12; i++ )
            {
                int j = i; //handles a bug in csharp, ask me for details if you want to know more. -Blueation
                LevelArray[i].Click += (sender, e) => LevelArrayHandler(sender, e, j); //passing a variable to the handler as well.
                LevelArray[i].text.Click += (sender, e) => LevelArrayHandler(sender, e, j);
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

//To be removed in final product
            OnlyForcedUpdateButton.Location = new System.Drawing.Point(0, 0);
            OnlyForcedUpdateButton.Size = new Size(10, 10);
            OnlyForcedUpdateButton.TabStop = false;
            this.Controls.Add(OnlyForcedUpdateButton);
            OnlyForcedUpdateButton.Click += OnlyForcedUpdateButtonHelper;
        }

#region DebugButtons
        public void OnlyForcedUpdateButtonHelper(object o, EventArgs ea)
        {
            this.Focus();

            OnlyForcedUpdate = !OnlyForcedUpdate;
            if (!OnlyForcedUpdate)
            {
                update = new System.Timers.Timer(10);
                update.Elapsed += new ElapsedEventHandler(Update);
                update.Enabled = true;
            }
            else
                update.Dispose();
        }
#endregion
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
            if (completedtiming >= 100)
            {
                ProgressButton.Visible = true;
                UndoButton.BackgroundImage = UndoReset;
                //the timer runs (on) its own thread, which means it may not update UI elements, the progressbutton must therefore be made visible by other means
            }

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
            if (completedtiming >= 100)
            {
                ProgressButton.Visible = true;
                UndoButton.BackgroundImage = UndoReset;
                //the timer runs (on) its own thread, which means it may not update UI elements, the progressbutton must therefore be made visible by other means
            }
            
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

                bgplayer.controls.stop();
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

                bgplayer.controls.play();
            }
            else
            {
                playSound = true;
                SoundButton.BackgroundImage = musicimage;
            }
        }

        public void LevelArrayHandler(object o, EventArgs ea, int i)
        {
            Level temp = chapters[selectedChapter].levels[i + selectionIndex * 12];
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
            if (selectionIndex < (chapters[selectedChapter].levels.Count - 1) / 12)
                selectionIndex++;
            else if (selectedChapter < chapters.Count - 1)
            {
                selectedChapter++;
                selectionIndex = 0;
            }
            RefreshLevelSet();
        }

        public void PreviousHandler(object o, EventArgs ea)
        {
            if (selectionIndex > 0)
                selectionIndex--;
            else if (selectedChapter > 0)
            {
                selectedChapter--;
                selectionIndex = (chapters[selectedChapter].levels.Count - 1) / 12;
            }
            RefreshLevelSet();
        }

        public void BackHandler(object o, EventArgs ea)
        {
            SetupLevelSelection();
        }

        public void UndoHandler(object o, EventArgs ea)
        {
            if (!completed)
            {
                DrawLock.LockIt();
                    PopHistory();
                DrawLock.UnlockIt();
            }
            else
            {
                UndoButton.BackgroundImage = UndoUndo;
                ResetHandler(o, ea);
            }
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

        public void ProgressButtonHandler(object o, EventArgs ea)
        {
            int current = chapters[selectedChapter].levels.IndexOf(LevelDictionary[loadedstring]);

            if (current < chapters[selectedChapter].levels.Count - 1)
            {
                current++;
                SetupLevel(chapters[selectedChapter].levels[current]);
            }
            else if (selectedChapter < chapters.Count - 1)
            {
                selectedChapter++;
                current = 0;
                selectionIndex = 0;
                SetupLevel(chapters[selectedChapter].levels[selectionIndex]);
            }
        }
#endregion
#region KeyEvents
        public void KeyDownHandler(object o, KeyEventArgs kea)
        {
            if (OnlyForcedUpdate && kea.KeyCode == Keys.Enter)
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
            if (completed && completedtiming < 100)
            {
                completedtiming++;
            }

            if (levelnr >= 0 && !completed)
            {
                int AllFilled = 0;
                foreach (SubmitZone zone in SubmitZones)
                    zone.Filled = false;
                DrawLock.LockIt();
                //Console.WriteLine("Lock of Draw: Update");
                foreach (Source s in Sources)
                {
                    if (s.Active)
                    {
                        foreach (SubmitZone zone in SubmitZones)
                        {
                            if (!zone.Filled)
                            {
                                bool testZone = zone.isCollision(s);
                                zone.Filled = testZone;
                                if (testZone)
                                {
                                    AllFilled++;
                                    s.retractThisUpdate = false;
                                }
                            }
                        }

                        DragLock.LockIt();
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
                        DragLock.UnlockIt();
                    }
                }

                foreach (Source s in NewSources)
                    Sources.Add(s);
                NewSources.Clear();

                zonesfilled = AllFilled;

                if (SubmitZones.Count == AllFilled)
                {
                    completed = true;
                    LevelCompleted();
                }

                bool invalidatedForm = false;

                if (!Dragging)
                {
                    foreach (Source s in Sources)
                    {
                        if (s.retractThisUpdate)
                            s.Retract();
                        s.retractThisUpdate = true;
                    }
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

            List<List<Level>> onlysortedbychapter = new List<List<Level>>();
            List<string> chapterpaths = new List<string>();

            List<List<Level>> sorted = new List<List<Level>>();

            foreach (string filepath in levelpaths)
            {
                Level tempstored = LevelLoader.LoadLevel(filepath);
                LevelDictionary.Add(filepath, tempstored);

                string chapterpath;
                int lvlnameindex = filepath.LastIndexOf('/');
                if (lvlnameindex > 0)
                    chapterpath = filepath.Substring(0, lvlnameindex);
                else chapterpath = "undefined";

                if (!chapterpaths.Contains(chapterpath))
                {
                    chapterpaths.Add(chapterpath);
                    onlysortedbychapter.Add(new List<Level>());
                }

                onlysortedbychapter[chapterpaths.IndexOf(chapterpath)].Add(tempstored);
            }

            for (int i = 0; i < chapterpaths.Count; i++)
            {
                Chapter newchapter = new Chapter(chapterpaths[i], i);
                newchapter.levels = onlysortedbychapter[i].OrderBy(lvl => (lvl.nr + 1000).ToString()).ToList<Level>();
                chapters.Add(newchapter);
            }
        }

        public void SetupLevel(Level level)
        {
            inMenu = false;
            inLevelSelect = false;
            completed = false;
            completedtiming = 0;

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
            ChapterNrName.Visible = false;

            BackButton.Visible = true;
            UndoButton.Visible = true;
            UndoButton.BackgroundImage = UndoUndo;
            ResetButton.Visible = true;
            ProgressButton.Visible = false;

            zonesnumber = SubmitZones.Count;
            GameHistory = new History(5, Sources);
        }

        public void SetupLevelSelection()
        {
            inMenu = false;
            inLevelSelect = true;
            levelnr = -1;
            completed = false;
            completedtiming = 0;

            PlayButton.Visible = false;
            SoundButton.Visible = false;
            QuitButton.Visible = true;

            NextButton.Visible = true;
            PreviousButton.Visible = true;
            ChapterNrName.Visible = true;

            BackButton.Visible = false;
            UndoButton.Visible = false;
            ResetButton.Visible = false;
            ProgressButton.Visible = false;

            RefreshLevelSet();
        }

        public void RefreshLevelSet()
        {
            foreach (Control c in LevelArray)
                c.Visible = false;

            int i = 0;
            while (i < 12 && i + selectionIndex * 12 < chapters[selectedChapter].levels.Count)
            {
                LevelArray[i].Visible = true;
                LevelArray[i].text.Text = chapters[selectedChapter].levels[i + selectionIndex * 12].nr.ToString();
                i++;
            }

            if (selectionIndex <= 0 && selectedChapter <= 0)
                PreviousButton.BackgroundImage = previmpo;
            else
                PreviousButton.BackgroundImage = prevposs;

            if (i + selectionIndex * 12 >= chapters[selectedChapter].levels.Count && selectedChapter >= chapters.Count - 1)
                NextButton.BackgroundImage = nextimpo;
            else
                NextButton.BackgroundImage = nextposs;

            ChapterNrName.Text = "Chapter " + (selectedChapter + 1);
        }

        public void SetupMainMenu()
        {
            inMenu = true;
            inLevelSelect = false;
            levelnr = -1;
            completed = false;
            completedtiming = 0;

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
            selectedChapter = 0;
            ChapterNrName.Visible = false;

            BackButton.Visible = false;
            ProgressButton.Visible = false;
        }
#endregion
#region Output Logic
        public void bgplayer_PlayStateChange(int state)
        {
            //Console.WriteLine(state);
        }

        public void PlayPositive()
        {
            if (playSound)
            {
                if (positive1 == null)
                {
                    soundplayer.URL = "assets/Positive.wav";
                    positive1 = soundplayer.controls.currentItem;
                }
                else
                    soundplayer.controls.playItem(positive1);
            }
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
            PlayPositive();
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

                    if (completedtiming >= 100)
                        pea.Graphics.FillRectangle(new SolidBrush(System.Drawing.Color.FromArgb(128, 51, 51, 51)), this.ClientRectangle);

                    string text = zonesfilled + "/" + zonesnumber;
                    SizeF textsize = pea.Graphics.MeasureString(text, font);
                    pea.Graphics.DrawString(text, font, new SolidBrush(System.Drawing.Color.Black)
                                           , this.ClientSize.Width - 35 - (textsize.Width / 2), textsize.Height / 2);

                    DrawLock.UnlockIt();
                    //Console.WriteLine("Unlock of Draw: Draw");
                }
                else
                    Console.WriteLine("draw failed");
            }
        }
    }
}
