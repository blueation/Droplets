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
    public partial class DropletButton : UserControl
    {
        public DropletButton(string type)
        {
            this.BackgroundImage = new Bitmap("../../Assets/" + type + ".png");
            this.Size = new Size(60, 60);
            this.TabStop = false;
            this.Visible = false;
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
