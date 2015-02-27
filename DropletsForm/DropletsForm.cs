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

        //GUIState
        public static List<Control> Buttons;
        //LevelState
        public static List<Source> Sources;
        public static List<SubmitZone> SubmitZones;
        //InputState
        public static Source DragSource;
        public static bool Dragging = false;

        private static System.Timers.Timer update;

        public DropletsGame()
        {
            Sources = new List<Source>();
            SubmitZones = new List<SubmitZone>();
            LoadLevel(1);

            this.DoubleBuffered = true;

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
            Console.WriteLine("MouseDown" + mea.X + ", " + mea.Y);

            foreach (Source s in Sources)
            {
                if (s.Active && s.isIn(mea.X, mea.Y))
                {
                    Dragging = true;
                    DragSource = s;
                    this.Invalidate();
                }
            }
        }

        public void MouseUpHandler(object o, MouseEventArgs mea)
        {
            Console.WriteLine("MouseUp" + mea.X + ", " + mea.Y);
            if (DragSource != null)
                 Console.WriteLine("angle:{0}", MathHelper.angleCalculate(DragSource.SourceAnchor, DragSource.ExtensionAnchor));
            DragSource = null;
            Dragging = false;
            this.Invalidate();
        }

        public void MouseMoveHandler(object o, MouseEventArgs mea)
        {
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
            this.Invalidate();
        }

        public void Update(object o, ElapsedEventArgs e)
        {
            int AllFilled = 0;
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
                        if (s != s2 && s2.Active && s.isCollision(s2))
                        {
                            if (s.DefaultBehaviour && s2.DefaultBehaviour)
                            #region DEFAULTBEHAVIOUR
                            {
                                int newsize = Math.Min(s.SourceSize.toInt, s2.SourceSize.toInt);
                                //int newloc = 

                                s.SourceSize = new BlobSize().fromInt(s.SourceSize.toInt - newsize);
                                s.ExtensionAnchor = s.SourceAnchor;
                                if (s.SourceSize.toInt == 0)
                                    s.Deactivate();

                                s2.SourceSize = new BlobSize().fromInt(s2.SourceSize.toInt - newsize);
                                s2.ExtensionAnchor = s2.SourceAnchor;
                                if (s2.SourceSize.toInt == 0)
                                    s2.Deactivate();


                            }
                            #endregion
                        }
                    }
                }
            }

            if (SubmitZones.Count == AllFilled)
                LevelCompleted();

            if (!Dragging)
            {
                foreach (Source s in Sources)
                    s.Retract();
                this.Invalidate();
            }
        }

        public void LoadLevel(int n)
        {
            Sources.Add(new Source(new BlueColour(), new SmallSize(), new Vector2(20, 100)));
            Sources.Add(new Source(new RedColour(), new MediumSize(), new Vector2(100, 100)));
            Sources.Add(new Source(new GreenColour(), new LargeSize(), new Vector2(180, 100)));
        }

        public void LevelCompleted()
        {

        }

        public void Draw(object o, PaintEventArgs pea)
        {
            foreach (Source s in Sources)
            {
                s.Draw(pea.Graphics);
            }
        }
    }
}
