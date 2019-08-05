using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM.UI
{
    public class BrushPool
    {
        private static Dictionary<Color, Brush> _brushCache = new Dictionary<Color, Brush>(8);

        public static Brush GetBrush(Color c)
        {
            if(_brushCache.TryGetValue(c, out Brush b))
            {
                return b;
            }

            Brush bb = new SolidBrush(c);
            _brushCache.Add(c, bb);
            return bb;
        }
    }
}
