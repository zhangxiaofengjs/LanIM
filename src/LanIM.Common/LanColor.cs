using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM.Common
{
    public class LanColor
    {
        /// <summary>
        /// 变暗或变亮 -1.0~1.0
        /// 小于0 加深
        /// </summary>
        /// <param name="baseColor"></param>
        /// <param name="factor"></param>
        /// <returns></returns>
        public static Color DarkLight(Color baseColor, float factor)
        {
            float red = (float)baseColor.R;
            float green = (float)baseColor.G;
            float blue = (float)baseColor.B;

            if (factor < 0)
            {
                factor = 1 + factor;
                red *= factor;
                green *= factor;
                blue *= factor;
            }
            else
            {
                red = (255 - red) * factor + red;
                green = (255 - green) * factor + green;
                blue = (255 - blue) * factor + blue;
            }

            if (red < 0) red = 0;

            if (red > 255) red = 255;

            if (green < 0) green = 0;

            if (green > 255) green = 255;

            if (blue < 0) blue = 0;

            if (blue > 255) blue = 255;

            return Color.FromArgb(baseColor.A, (int)red, (int)green, (int)blue);
        }
    }
}
