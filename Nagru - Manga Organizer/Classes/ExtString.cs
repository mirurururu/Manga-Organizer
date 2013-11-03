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
            return (sRaw.IndexOf(sFind, cComp) > -1) ? true : false;
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

        public static string RelativePath(string sRaw)
        {
            string sPath = Environment.CurrentDirectory;
            string sCurr = System.IO.Path.GetFileName(sPath);
            string[] sNodes = sRaw.Split('\\');
            int iPos = -1;

            //find point of divergence
            for (int i = 0; i < sNodes.Length; i++) {
                if (sNodes[i].Length == sCurr.Length
                        && sNodes[i].Equals(sCurr,
                        StringComparison.OrdinalIgnoreCase)) {
                    iPos = i + 1;
                    break;
                }
            }

            //if no match, drive letter probably changed
            if (iPos == -1) {
                iPos = 1;
                sPath = sPath.Remove(2);
            }

            //re-construct filepath
            for (int i = iPos; i < sNodes.Length; i++)
                sPath += string.Format("\\{0}", sNodes[i]);

            //validate & re-assign to textbox
            if (System.IO.Directory.Exists(sPath)
                    || System.IO.File.Exists(sPath))
                return sPath;
            else return null;
        }
        
        public static string ReplaceHTML(string sRaw)
        {
            StringBuilder sbSwap = new StringBuilder(sRaw);
            sbSwap.Replace("&amp;",  "&")
                .Replace("&quot;",   "\"")
                .Replace("&lt;",     "<")
                .Replace("&gt;",     ">")
                .Replace("&#039;",   "'")
                .Replace("&frac14;", "¼")
                .Replace("&frac12;", "½")
                .Replace("&frac34;", "¾")
                .Replace("&deg;",    "°")
                .Replace("&plusmn;", "±")
                .Replace("&sup2;",   "²")
                .Replace("&sup3;",   "³")
                .Replace("&iquest;", "¿")
                .Replace("&iexcl;",  "¡");
            return DecodeNonAscii(sbSwap.ToString());
        }

        public static string[] Split(string sRaw, params string[] sFilter)
        {
            return sRaw.Split(sFilter, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}