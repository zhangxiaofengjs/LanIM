using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Com.LanIM.UI.Components
{
    class FormButton
    {
        public CommonForm Owner { get; }
        public Image Image { get; set; }
        public Image ImageHover { get; set; }
        public Image ImagePress { get; set; }
        public Rectangle Bounds { get; set; }

        public event EventHandler Click = null;

        public FormButton(CommonForm owner)
        {
            this.Owner = owner;
        }

        public void RaiseClickEvent()
        {
            OnClick(new EventArgs());
        }

        protected virtual void OnClick(EventArgs args)
        { 
            if(Click != null)
            {
                Click(this, new EventArgs());
            }
        }
    }

    class MinimizeFormButton : FormButton
    {
        public MinimizeFormButton(CommonForm owner) :
            base(owner)
        {
            this.Image = Properties.Resources.min;
            this.ImageHover = Properties.Resources.min_hover;
            this.ImagePress = Properties.Resources.min_press;
        }

        protected override void OnClick(EventArgs args)
        {
            this.Owner.WindowState = FormWindowState.Minimized;
        }
    }

    class MaximizeFormButton : FormButton
    {
        public MaximizeFormButton(CommonForm owner) :
            base(owner)
        {
            this.Image = Properties.Resources.max;
            this.ImageHover = Properties.Resources.max_hover;
            this.ImagePress = Properties.Resources.max_press;
        }

        protected override void OnClick(EventArgs args)
        {
            if(this.Owner.WindowState == FormWindowState.Normal)
            {
                this.Image = Properties.Resources.normal;
                this.ImageHover = Properties.Resources.normal_hover;
                this.ImagePress = Properties.Resources.normal_press;
                this.Owner.WindowState = FormWindowState.Maximized;
            }
            else
            {
                this.Image = Properties.Resources.max;
                this.ImageHover = Properties.Resources.max_hover;
                this.ImagePress = Properties.Resources.max_press;
                this.Owner.WindowState = FormWindowState.Normal;
            }
        }
    }

    class CloseFormButton : FormButton
    {
        public CloseFormButton(CommonForm owner) :
            base(owner)
        {
            this.Image = Properties.Resources.close;
            this.ImageHover = Properties.Resources.close_hover;
            this.ImagePress = Properties.Resources.close_press;
        }

        protected override void OnClick(EventArgs args)
        {
            this.Owner.Close();
        }
    }
}
