using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM.UI
{
    class WindowsAPI
    {
        public static uint HIWORD(uint n)
        {
            return (n >> 16) & 0xffff;
        }

        public static uint LOWORD(uint n)
        {
            return n & 0xffff;
        }

        public static int GET_X_LPARAM(uint lp)
        {
            return (int)(short)LOWORD(lp);
        }

        public static int GET_Y_LPARAM(uint lp)
        {
            return (int)(short)HIWORD(lp);
        }

        public static Point MAKEPOINT(uint lp)
        {
            return new Point(GET_X_LPARAM(lp), GET_Y_LPARAM(lp));
        }

        public static Point MAKEPOINT(IntPtr p)
        {
            return MAKEPOINT((uint)p);
        }
    }
}
