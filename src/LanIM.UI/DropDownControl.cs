using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Security.Permissions;
using System.ComponentModel;

namespace Com.LanIM.UI
{
    /* 作者：Starts_2000
     * 日期：2009-09-01
     * 网站：http://www.csharpwin.com CS 程序员之窗。
     * 你可以免费使用或修改以下代码，但请保留版权信息。
     * 具体请查看 CS程序员之窗开源协议（http://www.csharpwin.com/csol.html）。
     */
    class DropDownControl : ToolStripDropDown
    {
        #region Fields

        private ToolStripControlHost _controlHost;
        private Control _popupControl;
		public Control PopupControl {
			get { return _popupControl; }
			set 
			{
				CreateHost(_popupControl);
			}
		}
        private bool _changeRegion;
        private bool _openFocused;
        private bool _acceptAlt;
        private bool _resizableTop;
        private bool _resizableLeft;
        private bool _canResize = false;
        private DropDownControl _ownerPopup;
        private DropDownControl _childPopup;
        private Color _borderColor = Color.LightGray;

        #endregion

        #region Constructors

        public DropDownControl(Control control)
        {
            DoubleBuffered = true;
            ResizeRedraw = true;
            AutoSize = false;
            Padding = Padding.Empty;
            Margin = Padding.Empty;
            CreateHost(control);
        }

        #endregion

        #region Properties

        public bool ChangeRegion
        {
            get { return _changeRegion; }
            set { _changeRegion = value; }
        }

        public bool OpenFocused
        {
            get { return _openFocused; }
            set { _openFocused = value; }
        }

        public bool AcceptAlt
        {
            get { return _acceptAlt; }
            set { _acceptAlt = value; }
        }

        public bool CanResize
        {
            get { return _canResize; }
            set { _canResize = value; }
        }

        public Color BorderColor
        {
            get { return _borderColor; }
            set { _borderColor = value; }
        }

        #endregion

        #region Protected Methods

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (_acceptAlt && ((keyData & Keys.Alt) == Keys.Alt))
            {
                if ((keyData & Keys.F4) != Keys.F4)
                {
                    return false;
                }
                else
                {
                    Close();
                }
            }
            return base.ProcessDialogKey(keyData);
        }
        protected override void OnOpening(CancelEventArgs e)
        {
            if (_popupControl.IsDisposed || _popupControl.Disposing)
            {
                e.Cancel = true;
                base.OnOpening(e);
                return;
            }
            _popupControl.RegionChanged += new EventHandler(PopupControlRegionChanged);
            UpdateRegion();
            base.OnOpening(e);
        }
        protected override void OnOpened(EventArgs e)
        {
            if (_openFocused)
            {
                _popupControl.Focus();
            }
            base.OnOpened(e);
        }
        protected override void OnClosing(ToolStripDropDownClosingEventArgs e)
        {
            _popupControl.RegionChanged -= new EventHandler(PopupControlRegionChanged);
            base.OnClosing(e);
        }
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (_controlHost != null)
            {
                _controlHost.Size = new Size(
                    Width - Padding.Horizontal, Height - Padding.Vertical);
            }
        }

        [SecurityPermission(SecurityAction.LinkDemand, 
            Flags = SecurityPermissionFlag.UnmanagedCode)]
        protected override void WndProc(ref Message m)
        {
//            if (!ProcessGrip(ref m))
//            {
                base.WndProc(ref m);
//            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (!_changeRegion)
            {
                ControlPaint.DrawBorder(
                    e.Graphics,
                    ClientRectangle,
                    _borderColor,
                    ButtonBorderStyle.Solid);
            }
        }

        protected void UpdateRegion()
        {
            if(!_changeRegion)
            {
                return;
            }

            if (base.Region != null)
            {
                base.Region.Dispose();
                base.Region = null;
            }
            if (_popupControl.Region != null)
            {
                base.Region = _popupControl.Region.Clone();
            }
        }

        #endregion

        #region Public Methods

        public void Show(Control control)
        {
            Show(control, control.ClientRectangle);
        }

        public void Show(Control control, bool center)
        {
            Show(control, control.ClientRectangle, center);
        }

        public void Show(Control control, Rectangle rect)
        {
            Show(control, rect, false);
        }

        public void Show(Control control, Rectangle rect, bool center)
        {
            if (control == null)
            {
                throw new ArgumentNullException("control");
            }

            SetOwnerItem(control);

            if (_canResize && !_changeRegion)
            {
                Padding = new Padding(3);
            }
            else if (!_changeRegion)
            {
                Padding = new Padding(1);
            }
            else
            {
                Padding = Padding.Empty;
            }

            int width = Padding.Horizontal;
            int height = Padding.Vertical;

            base.Size = new Size(
                   _popupControl.Width + width,
                   _popupControl.Height + height);

            _resizableTop = false;
            _resizableLeft = false;
            Point location = control.PointToScreen(
                new Point(rect.Left, rect.Bottom));
            Rectangle screen = Screen.FromControl(control).WorkingArea;
            if (center)
            {
                if (location.X + (rect.Width + Size.Width) / 2 > screen.Right)
                {
                    location.X = screen.Right - Size.Width;
                    _resizableLeft = true;
                }
                else
                {
                    location.X = location.X - (Size.Width - rect.Width) / 2;
                }
            }
            else
            {
                if (location.X + Size.Width > (screen.Left + screen.Width))
                {
                    _resizableLeft = true;
                    location.X = (screen.Left + screen.Width) - Size.Width;
                }
            }

            if (location.Y + Size.Height > (screen.Top + screen.Height))
            {
                _resizableTop = true;
                location.Y -= Size.Height + rect.Height;
            }

            location = control.PointToClient(location);
            Show(control, location, ToolStripDropDownDirection.BelowRight);
        }

        #endregion

        #region Private Methods

        private void SetOwnerItem(Control control)
        {
            if (control == null)
            {
                return;
            }
            if (control is DropDownControl)
            {
                DropDownControl popupControl = control as DropDownControl;
                _ownerPopup = popupControl;
                _ownerPopup._childPopup = this;
                OwnerItem = popupControl.Items[0];
                return;
            }
            if (control.Parent != null)
            {
                SetOwnerItem(control.Parent);
            }
        }

        private void CreateHost(Control control)
        {
            if (control == null)
            {
                throw new ArgumentException("control");
            }

            _popupControl = control;
            _controlHost = new ToolStripControlHost(control, "popupControlHost");
            _controlHost.AutoSize = false;
            _controlHost.Padding = Padding.Empty;
            _controlHost.Margin = Padding.Empty;
            base.Size = new Size(
                control.Size.Width + Padding.Horizontal,
                control.Size.Height + Padding.Vertical);
            _popupControl.SizeChanged += new EventHandler(PopupControlSizeChanged);
            base.Items.Clear();
            base.Items.Add(_controlHost);
        }

        private void PopupControlSizeChanged(object sender,EventArgs e)
        {
            base.Size = new Size(
                _popupControl.Size.Width + Padding.Horizontal,
                _popupControl.Size.Height + Padding.Vertical);
        }
        private void PopupControlRegionChanged(object sender, EventArgs e)
        {
            UpdateRegion();
        }

        [SecurityPermission(SecurityAction.LinkDemand,
           Flags = SecurityPermissionFlag.UnmanagedCode)]
        private bool ProcessGrip(ref Message m)
        {
            if (_canResize && !_changeRegion)
            {
                switch (m.Msg)
                {
                    case (int)WindowsConstants.WM_NCHITTEST:
                        return OnNcHitTest(ref m);
                    case (int)WindowsConstants.WM_GETMINMAXINFO:
                        return OnGetMinMaxInfo(ref m);
                }
            }
            return false;
        }

        [SecurityPermission(SecurityAction.LinkDemand,
            Flags = SecurityPermissionFlag.UnmanagedCode)]
        private bool OnGetMinMaxInfo(ref Message m)
        {
            Control hostedControl = _popupControl;
            if (hostedControl != null)
            {
                MINMAXINFO minmax =
                    (MINMAXINFO)Marshal.PtrToStructure(
                    m.LParam, typeof(MINMAXINFO));

                if (hostedControl.MaximumSize.Width != 0)
                {
                    minmax.maxTrackSize.Width = hostedControl.MaximumSize.Width;
                }
                if (hostedControl.MaximumSize.Height != 0)
                {
                    minmax.maxTrackSize.Height = hostedControl.MaximumSize.Height;
                }

                minmax.minTrackSize = new Size(100, 100);
                if (hostedControl.MinimumSize.Width > minmax.minTrackSize.Width)
                {
                    minmax.minTrackSize.Width = 
                        hostedControl.MinimumSize.Width + Padding.Horizontal;
                }
                if (hostedControl.MinimumSize.Height > minmax.minTrackSize.Height)
                {
                    minmax.minTrackSize.Height = 
                        hostedControl.MinimumSize.Height + Padding.Vertical;
                }

                Marshal.StructureToPtr(minmax, m.LParam, false);
            }
            return true;
        }

        private bool OnNcHitTest(ref Message m)
        {
            Point location = WindowsAPI.MAKEPOINT((uint)m.LParam);
            
            Rectangle gripRect = Rectangle.Empty;
            if (_canResize && !_changeRegion)
            {
                if (_resizableLeft)
                {
                    if (_resizableTop)
                    {
                        gripRect = new Rectangle(0, 0, 6, 6);
                    }
                    else
                    {
                        gripRect = new Rectangle(0, Height - 6, 6, 6);
                    }
                }
                else
                {
                    if (_resizableTop)
                    {
                        gripRect = new Rectangle(Width - 6, 0, 6, 6);
                    }
                    else
                    {
                        gripRect = new Rectangle(
                            Width - 6,
                            Height - 6,
                            6,
                            6);
                    }
                }
            }

            if (gripRect.Contains(location))
            {
                if (_resizableLeft)
                {
                    if (_resizableTop)
                    {
                        m.Result = (IntPtr)WindowsConstants.HTTOPLEFT;
                        return true;
                    }
                    else
                    {
                        m.Result = (IntPtr)WindowsConstants.HTBOTTOMLEFT;
                        return true;
                    }
                }
                else
                {
                    if (_resizableTop)
                    {
                        m.Result = (IntPtr)WindowsConstants.HTTOPRIGHT;
                        return true;
                    }
                    else
                    {
                        m.Result = (IntPtr)WindowsConstants.HTBOTTOMRIGHT;
                        return true;
                    }
                }
            }
            else
            {
                Rectangle rectClient = ClientRectangle;
                if (location.X > rectClient.Right - 3 &&
                    location.X <= rectClient.Right &&
                    !_resizableLeft)
                {
                    m.Result = (IntPtr)WindowsConstants.HTRIGHT;
                    return true;
                }
                else if (location.Y > rectClient.Bottom - 3 &&
                    location.Y <= rectClient.Bottom &&
                    !_resizableTop)
                {
                    m.Result = (IntPtr)WindowsConstants.HTBOTTOM;
                    return true;
                }
                else if (location.X > -1 &&
                    location.X < 3 &&
                    _resizableLeft)
                {
                    m.Result = (IntPtr)WindowsConstants.HTLEFT;
                    return true;
                }
                else if (location.Y > -1 &&
                    location.Y < 3 &&
                    _resizableTop)
                {
                    m.Result = (IntPtr)WindowsConstants.HTTOP;
                    return true;
                }
            }
            return false;
        }

        #endregion
    }
}
