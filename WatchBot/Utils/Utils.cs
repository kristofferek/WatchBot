using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WatchBot.Utils
{
    public class Utils
    {
        public static String HexConverter(System.Drawing.Color c)
        {
            return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }
    }
}