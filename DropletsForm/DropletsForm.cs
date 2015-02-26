using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Design;

namespace Droplets
{
    class DropletsGame : Form
    {
        //LevelState
        public static List<Source> Sources;
        public static List<SubmitZone> SubmitZones;
        //InputState
        public static Source DragSource;

        public DropletsGame()
        {
            Sources = new List<Source>();
            SubmitZones = new List<SubmitZone>();
            LoadLevel(1);

            this.DoubleBuffered = true;

            this.MouseDown += this.MouseDownHandler;
            this.MouseUp += this.MouseUpHandler;
            this.MouseMove += this.MouseMoveHandler;

            this.Paint += this.Draw;
        }

        public void MouseDownHandler(object o, MouseEventArgs mea)
        {
            Console.WriteLine("MouseDown" + mea.X + ", " + mea.Y);

            foreach (Source s in Sources)
            {
                if (s.isIn(mea.X, mea.Y))
                {
                    DragSource = s;
                    if (s.SourceColour.ToString() == "Blue")
                        s.SourceColour = new RedColour();
                    else
                        s.SourceColour = new BlueColour();
                    this.Invalidate();
                }
            }
        }

        public void MouseUpHandler(object o, MouseEventArgs mea)
        {
            Console.WriteLine("MouseUp" + mea.X + ", " + mea.Y);
            DragSource = null;
            this.Invalidate();
        }

        public void MouseMoveHandler(object o, MouseEventArgs mea)
        {
            if (DragSource != null)
            {
                if (MathHelper.distance(DragSource.SourceAnchor, mea.X, mea.Y) <= DragSource.SourceSize.getMaxStretch)
                    DragSource.ExtensionAnchor = new Tuple<int, int>(mea.X, mea.Y);
            }
            this.Invalidate();
        }

        public void LoadLevel(int n)
        {
            Sources.Add(new Source(new BlueColour(), new SmallSize(), new Tuple<int, int>(50, 50)));
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
