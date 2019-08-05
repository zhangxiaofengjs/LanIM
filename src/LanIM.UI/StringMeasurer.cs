using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Com.LanIM.Components
{
    public class StringPart
    {
        public string String { get; set; }
        public TextFormatFlags TextFormatFlags { get; set; }
        public Font Font { get; set; }
        public Rectangle Bounds { get; set; }
        public bool Wrap { get; set; } //是否因为空间不够而被换行的？
    }

    public class StringMeasurer
    {
        private static Dictionary<char, int> _charWidthCache = new Dictionary<char, int>(512);
        private static StringFormat _format = new StringFormat(StringFormat.GenericDefault);
        private const TextFormatFlags _textFormatFlags = TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix | TextFormatFlags.PreserveGraphicsClipping;

        public static StringFormat Format
        {
            get
            {
                return _format;
            }
        }

        public static TextFormatFlags TextFormatFlags
        {
            get
            {
                return _textFormatFlags;
            }
        }

        static StringMeasurer()
        {
            _format = (StringFormat)StringFormat.GenericTypographic.Clone();
            _format.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
        }

        public static List<StringPart> Measure(Graphics g, Font font, int width, string str)
        {
            List<StringPart> spList = new List<StringPart>(16);

            int pos = 0;
            int x = 0, y = 0, w = 0, h = font.Height;
            string cutStr = "";
            bool cFlag = false;
            bool cWrapFlag = false;
            for (int i = 0; i < str.Length;)
            {
                cWrapFlag = false;
                if (i == str.Length - 1)
                {
                    w += GetCharWidth(str[i], g, font);
                    cutStr = str.Substring(pos, i - pos + 1);
                    cFlag = true;
                    i++;
                }
                else if (str[i] == '\r')
                {
                    cutStr = str.Substring(pos, i - pos);
                    if (i + 1 < str.Length)
                    {
                        //\r\n
                        i += 2;
                    }
                    else
                    {
                        i++;
                    }
                    cFlag = true;
                }
                else if (str[i] == '\n')
                {
                    cutStr = str.Substring(pos, i - pos);
                    i++;
                    cFlag = true;
                }
                else
                {
                    w += GetCharWidth(str[i], g, font);
                    if (w > width)
                    {
                        cutStr = str.Substring(pos, i - pos);
                        cFlag = true;
                        cWrapFlag = true;
                    }
                    else
                    {
                        cFlag = false;
                    }
                }

                if(cFlag)
                {
                    StringPart sp = new StringPart
                    {
                        Bounds = new Rectangle(x, y, w, h),
                        String = cutStr,
                        TextFormatFlags = TextFormatFlags,
                        Font = font,
                        Wrap = cWrapFlag,
                    };

                    spList.Add(sp);

                    pos = i;
                    y += h; w = 0;
                }
                else
                {
                    i++;
                }
            }

            return spList;
        }

        private static int GetCharWidth(char ch, Graphics g, Font font)
        {
            if (_charWidthCache.TryGetValue(ch, out int w))
            {
                return w;
            }
            //float wf = g.MeasureString(ch.ToString(), font, int.MaxValue, Format).Width;
            //好像这个更加精确一些，用这个
            int wf = TextRenderer.MeasureText(g, ch.ToString(), font, new Size(int.MaxValue, int.MaxValue), TextFormatFlags).Width;
            _charWidthCache.Add(ch, wf);
            return wf;
        }

        public static int GetCharIndex(Graphics g, Font font, int testX, string str)
        {
            float w = 0;
            for (int i = 0; i < str.Length;i++)
            {
                w += GetCharWidth(str[i], g, font);
                if (w >= testX)
                {
                    return i;
                }
            }
            return str.Length - 1;
        }

        public static int Width(Graphics g, Font font, string str, TextFormatFlags formatFlags)
        {
            int w = 0;
            for (int i = 0; i < str.Length;i++)
            {
                w += GetCharWidth(str[i], g, font);
            }
            return w;
        }
    }
}
