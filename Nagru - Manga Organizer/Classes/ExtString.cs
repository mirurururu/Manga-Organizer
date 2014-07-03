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
		/// Predict the filepath of a manga
		/// </summary>
		/// <param name="sPath">The base filepath</param>
		/// <param name="sArtist">The name of the Artist</param>
		/// <param name="sTitle">The title of the manga</param>
		/// <returns></returns>
		public static string FindPath(string sPath, string sArtist, string sTitle)
		{
			if (!File.Exists(sPath) && !Directory.Exists(sPath))
			{
				//find base relative
				sPath = ExtString.RelativePath(sPath);

				if (!Directory.Exists(sPath) && !File.Exists(sPath))
				{
					sPath = string.Format("{0}\\{1}"
            , !string.IsNullOrEmpty(SQL.GetSetting(SQL.Setting.RootPath)) ?
              SQL.GetSetting(SQL.Setting.RootPath) : Environment.CurrentDirectory
							, string.Format(!string.IsNullOrEmpty(sArtist) ?
								"[{0}] {1}" : "{1}", sArtist, sTitle)
					);

					if (!Directory.Exists(sPath))
					{
            if (File.Exists(sPath + ".cbz"))
							sPath += ".cbz";
            else if (File.Exists(sPath + ".cbr"))
							sPath += ".cbr";
						else if (File.Exists(sPath + ".zip"))
							sPath += ".zip";
						else if (File.Exists(sPath + ".rar"))
							sPath += ".rar";
						else if (File.Exists(sPath + ".7z"))
							sPath += ".7z";
						else
							sPath = ExtString.RelativePath(sPath);

						if (!Directory.Exists(sPath) && !File.Exists(sPath))
							sPath = null;
					}
				}
			}
			return sPath;
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
    /// Parses the input string into Artist and Title variables
    /// </summary>
    /// <param name="sRaw">The string to parse</param>
    /// <returns>Returns the Artist (0) and Title (1)</returns>
    public static string[] ParseGalleryTitle(string sRaw)
    {
      string[] asName = new string[2] { "", "" };
      string sCircle = "";
      int iPos = -1;

      //strip out circle info & store
      if (sRaw.StartsWith("(")) {
        iPos = sRaw.IndexOf(')');
        if (iPos > -1 && ++iPos < sRaw.Length) {
          sCircle = sRaw.Substring(0, iPos);
          sRaw = sRaw.Remove(0, iPos).TrimStart();
        }
      }

      //split fields using EH format
      int iA = sRaw.IndexOf('['), iB = sRaw.IndexOf(']');
      if ((iPos == -1 || iPos > 0)                            //ensure '(circle) [name]~' or '[name]~' format
          && iA == 0 && iB > -1                               //ensure there's a closing brace
          && iA < iB                                          //ensure the closing brace comes *after*
          && iB + 1 < sRaw.Length)                            //ensure there is text after the brace
      {
        //Re-format for Artist/Title fields
        asName[0] = sRaw.Substring(iA + 1, iB - iA - 1).Trim();
        asName[1] = sRaw.Substring(iB + 1).Trim();
        if (sCircle != "")
          asName[1] += " " + sCircle;
      }
      else {
        asName[1] = sRaw;
      }
      return asName;
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