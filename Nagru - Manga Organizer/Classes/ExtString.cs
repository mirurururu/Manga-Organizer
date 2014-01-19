using System;
using System.IO;
using System.Net;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Nagru___Manga_Organizer
{
    public static class ExtString
    {
        public static bool Contains(this string sRaw, string sFind,
            StringComparison cComp = StringComparison.OrdinalIgnoreCase)
        {
            return sRaw.IndexOf(sFind, cComp) > -1;
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

        public static string GetNameSansExtension(string sName)
        {
            StringBuilder sb = new StringBuilder(sName);
            int indx = sName.LastIndexOf('\\');
            if (indx > -1) sb.Remove(0, indx + 1);

            indx = sb.ToString().LastIndexOf('.');
            if(indx > -1) {
                switch(sb.Length - indx) {
                    case 3: return sb.Remove(indx, 3).ToString();
                    case 4: return sb.Remove(indx, 4).ToString();
                }
            }
            return sb.ToString();
        }

        public static string HTMLConvertToPlainText(string sRaw)
        {
            StringBuilder sbSwap = new StringBuilder(sRaw);
            sbSwap.Replace("&amp;", "&")
                .Replace("&quot;", "\"")
                .Replace("&lt;", "<")
                .Replace("&gt;", ">")
                .Replace("&#039;", "'")
                .Replace("&frac14;", "¼")
                .Replace("&frac12;", "½")
                .Replace("&frac34;", "¾")
                .Replace("&deg;", "°")
                .Replace("&plusmn;", "±")
                .Replace("&sup2;", "²")
                .Replace("&sup3;", "³")
                .Replace("&iquest;", "¿")
                .Replace("&iexcl;", "¡");
            return DecodeNonAscii(sbSwap.ToString());
        }

        public static string RelativePath(string sRaw)
        {
            string sPath = "";
            string[] sOldNodes = Split(sRaw, "\\"),
                sCurrNodes = Split(Environment.CurrentDirectory, "\\");

            //swap out point of divergence
            for (int i = 0; i < sOldNodes.Length; i++) {
                if (i < sCurrNodes.Length
                        && !(sOldNodes[i].Equals(sCurrNodes[i], 
                        StringComparison.OrdinalIgnoreCase))) {
                    sPath += sCurrNodes[i] + "\\";
                }
                else sPath += sOldNodes[i] + "\\";
            }
            sPath = sPath.Substring(0, sPath.Length - 1);

            return (Directory.Exists(sPath) || File.Exists(sPath))
                ? sPath : null;
        }

        public static double SoerensonDiceCoef(string sA, string sB, bool bIgnoreCase = true)
        {
            HashSet<string> hsA = new HashSet<string>(),
                hsB = new HashSet<string>();

            if(bIgnoreCase) {
                sA = sA.ToLower();
                sB = sB.ToLower();
            }

            //create paired char chunks from strings to compare
            for (int i = 0; i < sA.Length - 1; ) {
                hsA.Add(sA[i] + "" + sA[++i]);
            }
            for (int i = 0; i < sB.Length - 1; ) {
                hsB.Add(sB[i] + "" + sB[++i]);
            }
            int iTotalElements = hsA.Count + hsB.Count;

            hsA.IntersectWith(hsB);
            return (double)(2 * hsA.Count) / iTotalElements;
        }

        public static string[] Split(string sRaw, params string[] sFilter)
        {
            return sRaw.Split(sFilter, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}