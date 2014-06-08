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
    /// <summary>
    /// Simple string comparison
    /// </summary>
    /// <param name="sRaw">The base string</param>
    /// <param name="sFind">The string to search for</param>
    /// <param name="cComp">Overrideable comparison type</param>
    /// <returns>Returns true if sRaw contains sFind</returns>
    public static bool Contains(this string sRaw, string sFind,
        StringComparison cComp = StringComparison.OrdinalIgnoreCase)
    {
      return sRaw.IndexOf(sFind, cComp) > -1;
    }

    /// <summary>
    /// Simple string equivalence check
    /// </summary>
    /// <param name="sA"></param>
    /// <param name="sB"></param>
    /// <returns>Whether the strings are identical</returns>
    public static bool Equals(string sA, string sB)
    {
      return string.Equals(sA, sB, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Convert unicode to usable Ascii
    /// </summary>
    /// <remarks>Author: Adam Sills (October 23, 2009)</remarks>
    /// <param name="sRaw">The string to decode</param>
    /// <returns>Ascii version of the input</returns>
    public static string DecodeNonAscii(string sUnicode)
    {
      return Regex.Replace(sUnicode, @"\\u(?<Value>[a-zA-Z0-9]{4})",
          m => {
            return ((char)int.Parse(m.Groups["Value"].Value,
                System.Globalization.NumberStyles.HexNumber)).ToString();
          });
    }

    /// <summary>
    /// Turns Artist and Title fields into their EH format
    /// </summary>
    /// <param name="Artist"></param>
    /// <param name="Title"></param>
    /// <returns></returns>
    public static string GetFormattedTitle(string Artist, string Title)
    {
      return string.Format((!string.IsNullOrEmpty(Artist))
          ? "[{0}] {1}" : "{1}", Artist, Title);
    }

    /// <summary>
    /// Return a filename without its extension
    /// Overcomes Microsoft not handling periods in filenames
    /// </summary>
    /// <param name="sName"></param>
    /// <returns></returns>
    public static string GetNameSansExtension(string sName)
    {
      StringBuilder sb = new StringBuilder(sName);
      int indx = sName.LastIndexOf('\\');
      if (indx > -1)
        sb.Remove(0, indx + 1);

      indx = sb.ToString().LastIndexOf('.');
      if (indx > -1) {
        switch (sb.Length - indx) {
          case 3:
            return sb.Remove(indx, 3).ToString();
          case 4:
            return sb.Remove(indx, 4).ToString();
        }
      }
      return sb.ToString();
    }

    /// <summary>
    /// Converts HTML to Ascii
    /// </summary>
    /// <param name="sRaw"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Tries to find an incorrect filepath relative to the executable
    /// </summary>
    /// <param name="sRaw"></param>
    /// <returns></returns>
    public static string RelativePath(string sRaw)
    {
      bool bDiverged = false;
      string sPath = "";
      string[] sOldNodes = ExtString.Split(sRaw, "\\");
      string[] sCurrNodes = ExtString.Split(Environment.CurrentDirectory, "\\");

      //swap out point of divergence
      for (int i = 0; i < sOldNodes.Length; i++) {
        if (i < sCurrNodes.Length
              && !(sOldNodes[i].Equals(sCurrNodes[i],
              StringComparison.OrdinalIgnoreCase))) {
          sPath += sCurrNodes[i] + "\\";
        }
        else {
          sPath += sOldNodes[i] + "\\";
          bDiverged = true;
        }
      }
      sPath = (sPath.Length > 0) ? sPath.Substring(0, sPath.Length - 1) : null;

      return (bDiverged && (Directory.Exists(sPath) || File.Exists(sPath)))
          ? sPath : null;
    }

    /// <summary>
    /// Finds the value of divergence between two strings
    /// </summary>
    /// <param name="sA"></param>
    /// <param name="sB"></param>
    /// <param name="bIgnoreCase"></param>
    /// <returns></returns>
    public static double SoerensonDiceCoef(string sA, string sB, bool bIgnoreCase = true)
    {
      HashSet<string> hsA = new HashSet<string>(),
          hsB = new HashSet<string>();

      if (bIgnoreCase) {
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

    /// <summary>
    /// Splits string using multiple filter terms
    /// Also removes empty entries from the results
    /// </summary>
    /// <param name="sRaw"></param>
    /// <param name="sFilter"></param>
    /// <returns></returns>
    public static string[] Split(string sRaw, params string[] sFilter)
    {
      return sRaw.Split(sFilter, StringSplitOptions.RemoveEmptyEntries)
        .Select(x => x.Trim()).ToArray<string>();
    }
  }
}