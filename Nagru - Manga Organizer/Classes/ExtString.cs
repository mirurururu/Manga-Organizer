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
        //hold results of e-hentai search (may expand to include img, etc)
        public struct stEXH
        {
            public string sURL, sTitle;

            public stEXH(string _URL, string _Title)
            {
                sURL = _URL;
                sTitle = HTMLConvertToPlainText(_Title);
            }
        }

        public static bool Contains(this string sRaw, string sFind,
            StringComparison cComp = StringComparison.OrdinalIgnoreCase)
        {
            return (sRaw.IndexOf(sFind, cComp) > -1) ? true : false;
        }

        public static bool CaselessEquals(this string sA, string sB,
            StringComparison cComp = StringComparison.OrdinalIgnoreCase)
        {
            return (sA.Equals(sB, cComp));
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

        private static string EHFormatSearch(string sRaw, string sSite)
        {
            StringBuilder sb = new StringBuilder("");
            string[] asSplit = Main.SplitTitle(sRaw);
            
            //check for artist/title fields and set formatting
            if(!string.IsNullOrEmpty(asSplit[0])) {
                sb.AppendFormat("artist%3A{0}+", asSplit[0]);
                sb.Replace(' ', '_');
            }
            sb.Append(asSplit[1]);
            sb.Replace(' ', '+')
                .Replace(":", "%3A")
                .Replace("&", "%26");

            //insert rest of search string
            sb.Insert(0, string.Format("http://{0}.org/?f_doujinshi=1&f_manga=1&f_artistcg=0&f_gamecg=0&"
            + "f_western=0&f_non-h=0&f_imageset=0&f_cosplay=0&f_asianporn=0&f_misc=0&f_search=", sSite));
            sb.Append("&f_apply=Apply+Filter");

            return sb.ToString();
        }
        
        public static List<stEXH> EHSearch(string sRaw)
        {
            List<stEXH> lDetails = new List<stEXH>(5);
            string sSearch = "", sPage = "";
            bool bXH = true;

            //determine if exhentai can be called
            if(string.IsNullOrEmpty(Properties.Settings.Default.pass_hash)
                || string.IsNullOrEmpty(Properties.Settings.Default.member_id)) {
                bXH = false;
            }

            //convert raw search terms into web form
            sSearch = EHFormatSearch(sRaw, (bXH) ? "exhentai" : "g.e-hentai");

            //set up connection
            ServicePointManager.DefaultConnectionLimit = 64;
            HttpWebRequest rq = (HttpWebRequest)
                WebRequest.Create(sSearch);
            rq.ContentType = "text/html; charset=UTF-8";
            rq.Method = "GET";
            rq.Timeout = 5000;
            rq.KeepAlive = false;
            rq.Proxy = null;

            if (bXH) {
                rq.CookieContainer = new CookieContainer(2);
                rq.CookieContainer.Add(new CookieCollection() {
                    new Cookie("ipb_member_id", Properties.Settings.Default.member_id) { Domain = "exhentai.org" },
                    new Cookie("ipb_pass_hash", Properties.Settings.Default.pass_hash) { Domain = "exhentai.org" }
                });
            }
            
            try {
                //get webpage
                using (StreamReader sr = new StreamReader((
                    (HttpWebResponse)rq.GetResponse()).GetResponseStream())) {
                    sPage = sr.ReadToEnd();
                    rq.Abort();
                }
            } catch (Exception exc) {
                Console.WriteLine(exc.Message);
                return null;
            } if (string.IsNullOrEmpty(sPage)) {
                return null;
            }

            //strip out usable details
            string sRegex = ".*http://(ex|g.e-)hentai.org/g/[0-9]{6}/[a-zA-z0-9]{10}/.* onmouseover=.* onmouseout=.*";
            string[] asplit = sPage.Split('<');
            for (int i = 0; i < asplit.Length; i++) {
                if (asplit[i].Length > 50 && Regex.IsMatch(asplit[i], sRegex)) {
                    lDetails.Add(new stEXH(
                        asplit[i].Split('"')[1],
                        asplit[i].Split('>')[1].Split('<')[0])
                    );
                }
            }

            return lDetails;
        }
        
        public static string[] EHParse(string sURL)
        {
            const int iPreTag = 11;
            string[] asParse = new string[6]
                ,asResp = new string[0];
            bool bExc = false;
            
            //set up connection
            ServicePointManager.DefaultConnectionLimit = 64;
            HttpWebRequest rq = (HttpWebRequest)
                WebRequest.Create("http://g.e-hentai.org/api.php");
            rq.ContentType = "application/json; charset=UTF-8";
            rq.Method = "POST";
            rq.Timeout = 5000;
            rq.KeepAlive = false;
            rq.Proxy = null;

            try {
                //send formatted request to EH API
                using (Stream s = rq.GetRequestStream()) {
                    byte[] byContent = System.Text.Encoding.ASCII.GetBytes(JSON(sURL));
                    s.Write(byContent, 0, byContent.Length);
                }
                using (StreamReader sr = new StreamReader((
                    (HttpWebResponse)rq.GetResponse()).GetResponseStream())) {
                    asResp = ExtString.Split(sr.ReadToEnd(), "\",\"");
                    rq.Abort();
                }
            } catch (WebException exc) {
                Console.WriteLine(exc.Message);
                bExc = true;
            }

            //parse returned string
            if (!bExc && asResp.Length >= 11) {
                asParse[0] = HTMLConvertToPlainText(
                    asResp[2].Split(':')[1].Substring(1));         //set artist/title
                asParse[1] = asResp[4].Split(':')[1].Substring(1); //set entry type
                asParse[2] = asResp[7].Split(':')[1].Substring(1); //set date
                asParse[3] = asResp[8].Split(':')[1].Substring(1); //set page count
                asParse[4] = asResp[9].Split(':')[3].Substring(1); //set star rating

                //set and format tags
                int iLast = asResp.Length - 1;
                asResp[11] = asResp[11].Split(':')[1].Substring(2);
                asResp[iLast] = asResp[iLast].Substring(0, asResp[iLast].Length - 5);
                asParse[5] = string.Join(", ", asResp, iPreTag, asResp.Length - iPreTag);
                return asParse;
            }
            else return new string[0];
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
        
        /* Used to simulate JS Object Literal for JSON 
           Based on Hupotronics' ExLinks   */
        private static string JSON(string sURL)
        {
            string[] asChunk = sURL.Split('/');

            if (asChunk.Length >= 5) {
                return string.Format(
                    "{{\"method\":\"gdata\",\"gidlist\":[[{0},\"{1}\"]]}}",
                    asChunk[4], asChunk[5]);
            }
            return string.Empty;
        }

        public static string RelativePath(string sRaw)
        {
            string sPath = "";
            string[] sOldNodes = Split(sRaw, "\\"),
                sCurrNodes = Split(Environment.CurrentDirectory, "\\");

            //swap out point of divergence
            for (int i = 0; i < sOldNodes.Length; i++) {
                if (i < sCurrNodes.Length
                        && !(CaselessEquals(sOldNodes[i], sCurrNodes[i]))) {
                    sPath += sCurrNodes[i] + "\\";
                }
                else sPath += sOldNodes[i] + "\\";
            }
            sPath = sPath.Substring(0, sPath.Length - 1);

            //validate & re-assign to textbox
            return (Directory.Exists(sPath) || File.Exists(sPath))
                ? sPath : null;
        }

        public static string[] Split(string sRaw, params string[] sFilter)
        {
            return sRaw.Split(sFilter, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}