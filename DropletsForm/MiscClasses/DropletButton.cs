using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Droplets
{
    /// <summary>
    /// A specific button that has several defaults set correctly to what is needed.
    /// </summary>
    partial class DropletButton : UserControl
    {
        public Label text = new Label();
        DropletsGame parent;

        public DropletButton(string type, DropletsGame parent)
        {
            this.parent = parent;
            this.BackgroundImage = new Bitmap("Assets/" + type + ".png");
            this.Size = new Size(60, 58);
            this.TabStop = false;
            this.Visible = false;
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.BackColor = Color.Transparent;

            text.TextAlign = ContentAlignment.MiddleCenter;
            text.Font = new Font(text.Font, FontStyle.Bold);
            text.Location = new Point(1, 0);
            text.Size = new Size(60, 60);
            text.TabStop = false;
            text.BackColor = Color.Transparent;
            
            this.Controls.Add(text);
        }

        public override string Text
        {
            get { return text.Text; }
            set { text.Text = value; }
        }

        protected override void OnPaint(PaintEventArgs pea)
        {
            base.OnPaint(pea);
        }

        protected override bool IsInputKey(Keys keyData)
        {
            return true;
            //return base.IsInputKey(keyData);
        }
    }
}
