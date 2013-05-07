using System;
using System.Text;

namespace Nagru___Manga_Organizer
{
    /* Extends String.Contains to support case options & multiple filters  */
    public static class ExtString
    {
        public static bool Contains(this string sSource, string sCheck,
            StringComparison scComp = StringComparison.OrdinalIgnoreCase)
        {
            string[] sFilters = sCheck.Split('\0');
            for (int i = 0; i < sFilters.Length; i++)
                if ((sSource.IndexOf(sFilters[i], scComp) >= 0))
                    return true;

            return false;
        }

        public static string[] Split(string sRaw, string sFilter, 
            StringSplitOptions SplitOption = StringSplitOptions.RemoveEmptyEntries)
        {
            return sRaw.Split(new string[] { sFilter }, SplitOption);
        }

        /* Convert unicode to usable Ascii
           Author: Adam Sills, (October 23, 2009)         */
        public static string DecodeNonAscii(string sRaw)
        {
            return System.Text.RegularExpressions.Regex.Replace(sRaw, @"\\u(?<Value>[a-zA-Z0-9]{4})",
                m =>
                {
                    return ((char)int.Parse(m.Groups["Value"].Value, System.Globalization.NumberStyles.HexNumber)).ToString();
                });
        }
    }
}
