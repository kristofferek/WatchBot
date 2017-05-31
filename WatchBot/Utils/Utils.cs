using System;
using System.Drawing;

namespace WatchBot.Utils
{
    public class Utils
    {
        public static string HexConverter(Color c)
        {
            return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }

        public static string GetDominantColor(Bitmap bmp)
        {
            //Used for tally
            var r = 0;
            var g = 0;
            var b = 0;

            var total = 0;

            for (var x = 0; x < bmp.Width; x++)
            for (var y = 0; y < bmp.Height; y++)
            {
                Color clr = bmp.GetPixel(x, y);

                if ((clr.R + clr.G + clr.B) <630)
                {
                    Console.WriteLine(clr.R + clr.G + clr.B);
                    r += clr.R;
                    g += clr.G;
                    b += clr.B;

                    total++;
                }
            }

            //Calculate average
            r /= total;
            g /= total;
            b /= total;

            return "#" + r.ToString("X2") + g.ToString("X2") + b.ToString("X2");
        }
    }
}