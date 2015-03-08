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
    public partial class DropletButton : Button
    {
        Bitmap image;

        public DropletButton(string type)
        {
            image = new Bitmap("../../Assets/" + type + ".png");
            this.Size = new Size(60, 60);
            
        }

        protected override void OnPaint(PaintEventArgs pea)
        {
            base.OnPaint(pea);

            pea.Graphics.DrawImage(image, new Rectangle(0, 0, 60, 60));
        }
    }
}
