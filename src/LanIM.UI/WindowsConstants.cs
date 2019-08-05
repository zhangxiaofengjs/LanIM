using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM.UI
{
    class WindowsConstants
    {
        public const int WM_NCHITTEST = 0x84;
        public const int WM_NCLBUTTONDBLCLK = 0xa3;
        public const int HTCLIENT = 0x0001;
        public const int HTCAPTION = 0x0002;
        public const int HTLEFT = 10;
        public const int HTRIGHT = 11;
        public const int HTTOP = 12;
        public const int HTTOPLEFT = 13;
        public const int HTTOPRIGHT = 14;
        public const int HTBOTTOM = 15;
        public const int HTBOTTOMLEFT = 16;
        public const int HTBOTTOMRIGHT = 17;
        public const int SC_MAXIMIZE = 0xF030;//最大化信息
        public const int SC_MINIMIZE = 0xF020;//最小化信息
        public const int WM_GETMINMAXINFO = 0x0024;
        public const int WM_NCACTIVATE = 0x0086;
    }

    [StructLayout(layoutKind: LayoutKind.Sequential)]
    public struct MINMAXINFO
    {
        public Point reserved;
        public Size maxSize;
        public Point maxPosition;
        public Size minTrackSize;
        public Size maxTrackSize;
    }
}
