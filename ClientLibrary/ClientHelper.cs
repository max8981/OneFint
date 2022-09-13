using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static System.Net.Mime.MediaTypeNames;
using System.Xml.Linq;
using System.Drawing;
using System.Timers;
using System.Collections.Concurrent;

namespace ClientLibrary
{
    public class ClientHelper
    {
        public static string GetByteString(double value,int mod)
        {
            char[] units = new char[] { '\0', 'K', 'M', 'G', 'T', 'P' };
            int i = 0;
            while (mod < value)
            {
                value /= mod;
                i++;
            }
            return $"{Math.Round(value)}{units[i]}b";
        }
    }
}
