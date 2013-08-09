using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Nagru___Manga_Organizer
{
    public static class ExtString
    {
        public static bool Contains(this string sRaw, string sFind,
            StringComparison cComp = StringComparison.OrdinalIgnoreCase)
        {
            if (sRaw.IndexOf(sFind, cComp) >= 0) return true;
            return false;
        }

        public static string[] Split(string sRaw, params string[] sFilter)
        {
            return sRaw.Split(sFilter, StringSplitOptions.RemoveEmptyEntries);
        }

        public static string ReplaceHTML(string sRaw)
        {
            return DecodeNonAscii(sRaw
                .Replace("&amp;", "&")
                .Replace("&quot;", "\"")
                .Replace("&lt;", "<")
                .Replace("&gt;", ">")
                .Replace("&#039;", "'")
                .Replace("&frac14;", "¼")
                .Replace("&frac12;", "½")
                .Replace("&frac34;", "¾"));
        }

        /* Convert unicode to usable Ascii
           Author: Adam Sills (October 23, 2009)         */
        public static string DecodeNonAscii(string sRaw)
        {
            return Regex.Replace(sRaw, @"\\u(?<Value>[a-zA-Z0-9]{4})",
                m => {
                    return ((char)int.Parse(m.Groups["Value"].Value, 
                        System.Globalization.NumberStyles.HexNumber)).ToString();
                });
        }
    }
}