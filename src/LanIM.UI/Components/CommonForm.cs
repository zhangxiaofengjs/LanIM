using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Com.LanIM.UI.Components
{
    class CommonForm : Form
    {
        private const int WM_NCHITTEST = 0x84;
        private const int WM_NCLBUTTONDBLCLK = 0xa3;
        private const int HTCAPTION = 0x0002;
        private const int HTLEFT = 10;
        private const int HTRIGHT = 11;
        private const int HTTOP = 12;
        private const int HTTOPLEFT = 13;
        private const int HTTOPRIGHT = 14;
        private const int HTBOTTOM = 15;
        private const int HTBOTTOMLEFT = 16;
        private const int HTBOTTOMRIGHT = 17;

        private const int CAPTION_HEIGHT = 25;
        private const int FORM_BUTTON_WIDTH = 34;
        private const int FORM_BUTTON_HEIGHT = CAPTION_HEIGHT;
        
        private static readonly Pen BORDER_PEN = new Pen(Color.FromArgb(214, 214, 214));

        private List<FormButton> _formButtons = new List<FormButton>();
        private Rectangle _formButtonsBounds = Rectangle.Empty;

        public CommonForm()
        {
            base.SetStyle(ControlStyles.DoubleBuffer, true);
            base.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            base.SetStyle(ControlStyles.UserPaint, true);
            base.SetStyle(ControlStyles.ResizeRedraw, true);
            base.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            base.SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            this.BackColor = Color.FromArgb(245, 245, 245);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Padding = new Padding(5, CAPTION_HEIGHT, 5, 5);
            this.MaximizedBounds = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea;

            _formButtons.Add(new CloseFormButton(this));
            _formButtons.Add(new MaximizeFormButton(this));
            _formButtons.Add(new MinimizeFormButton(this));
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            CalcFormButtonBounds();

            this.Invalidate();
        }

        private void CalcFormButtonBounds()
        {
            int x = 0;
            for (int i = 0; i < _formButtons.Count; i++)
            {
                var button = _formButtons[i];

                x = this.Width - FORM_BUTTON_WIDTH * (i + 1) - 1;
                button.Bounds = new Rectangle(x, 1, FORM_BUTTON_WIDTH, FORM_BUTTON_HEIGHT);
            }
            _formButtonsBounds = new Rectangle(x, 1, this.Width - x, FORM_BUTTON_HEIGHT);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            DrawBorder(e);
            DrawFormButtons(e);
        }

        private void DrawBorder(PaintEventArgs e)
        {
            Rectangle rect = this.ClientRectangle;
            rect.Width -= 1;
            rect.Height -= 1;
            e.Graphics.DrawRectangle(BORDER_PEN, rect);
        }

        private void DrawFormButtons(PaintEventArgs e)
        {
            Point p = PointToClient(MousePosition);
            foreach (var button in _formButtons)
            {
                Image img = button.Image;
                if(button.Bounds.Contains(p))
                {
                    if(Control.MouseButtons== MouseButtons.Left)
                    {
                        img = button.ImagePress;
                    }
                    else
                    {
                        img = button.ImageHover;
                    }
                }
                e.Graphics.DrawImage(img, button.Bounds);
            }
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_NCHITTEST:
                    // 拉伸，调整窗口大小或者移动
                    if (ResizeOrMove(ref m)) { return;}
                    break;
                case WM_NCLBUTTONDBLCLK:
                    //双击窗口标题栏，最大化之类
                    this.WindowState = (this.WindowState == FormWindowState.Normal ? 
                        FormWindowState.Maximized : FormWindowState.Normal);
                    return;
            }

            base.WndProc(ref m);
        }

        private bool ResizeOrMove(ref Message m)
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                //最大化时不可拖动等动作
                //return false;
            }

        Point vPoint = new Point((int)m.LParam & 0xFFFF, (int)m.LParam >> 16 & 0xFFFF);
            vPoint = PointToClient(vPoint);
            int posInterval = 5;
            if (vPoint.X <= posInterval)
            {
                #region left border
                if (vPoint.Y <= posInterval)
                {
                    m.Result = (IntPtr)HTTOPLEFT;
                }
                else if (vPoint.Y >= ClientSize.Height - posInterval)
                {
                    m.Result = (IntPtr)HTBOTTOMLEFT;
                }
                else
                {
                    m.Result = (IntPtr)HTLEFT;
                }
                #endregion
            }
            else if (vPoint.X >= ClientSize.Width - posInterval)
            {
                #region right border

                if (vPoint.Y <= posInterval)
                {
                    m.Result = (IntPtr)HTTOPRIGHT;
                }
                else if (vPoint.Y >= ClientSize.Height - posInterval)
                {
                    m.Result = (IntPtr)HTBOTTOMRIGHT;
                }
                else
                {
                    m.Result = (IntPtr)HTRIGHT;
                }

                #endregion
            }
            else if (vPoint.Y <= posInterval)
            {
                m.Result = (IntPtr)HTTOP;
            }
            else if (vPoint.Y >= ClientSize.Height - posInterval)
            {
                m.Result = (IntPtr)HTBOTTOM;
            }
            else
            {
                if (this.WindowState != FormWindowState.Maximized &&
                    !this._formButtonsBounds.Contains(vPoint))
                {
                    //只要不是右上角按钮&&最大化都可以拖动
                    m.Result = (IntPtr)HTCAPTION;

                    //由于拖动时不可出发mousemove事件，所以要重绘按钮
                    this.Invalidate(this._formButtonsBounds);
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        protected override void OnLocationChanged(EventArgs e)
        {
            base.OnLocationChanged(e);
            
                //this.Invalidate(this._formButtonsBounds);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (this._formButtonsBounds.Contains(e.Location))
            {
                this.Invalidate(this._formButtonsBounds);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if(this._formButtonsBounds.Contains(e.Location))
            {
                this.Invalidate(this._formButtonsBounds);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (this._formButtonsBounds.Contains(e.Location))
            {
                this.Invalidate(this._formButtonsBounds);
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            if (this._formButtonsBounds.Contains(e.Location))
            {
                foreach (var item in _formButtons)
                {
                    if (item.Bounds.Contains(e.Location))
                    {
                        item.RaiseClickEvent();
                        break;
                    }
                }
            }
        }
    }
}
