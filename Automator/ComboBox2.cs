using System.Drawing;
using System.Windows.Forms;

namespace Arachnode.Automator
{
    class ComboBox2 : ComboBox
    {
        public ComboBox2()
        {
            base.AutoSize = false;
            Height = 23;
        }

        public override Size MinimumSize
        {
            get
            {
                return base.MinimumSize;
            }
            set
            {
                base.MinimumSize = new Size(value.Width, 0);
            }
        }

        public override Size MaximumSize
        {
            get
            {
                return base.MaximumSize;
            }
            set
            {
                base.MinimumSize = new Size(value.Width, int.MaxValue);
            }
        }
    }
}
