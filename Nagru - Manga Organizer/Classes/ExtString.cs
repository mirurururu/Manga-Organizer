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
                sTitle = _Title;
            }
        }

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

        private static string EHConvert(string sRaw, string sSite)
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
            sb.Insert(0, string.Format("http://{0}.org/?f_doujinshi=1&f_manga=1&f_artistcg=0&f_gamecg=0&f_western=0&f_non-h=0&f_imageset=0&f_cosplay=0&f_asianporn=0&f_misc=0&f_search=", sSite));
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
            sSearch = EHConvert(sRaw, (bXH) ? "exhentai" : "g.e-hentai");

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
                rq.CookieContainer = new CookieContainer(4);
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
            } if (sPage == string.Empty) {
                return null;
            }

            //strip out usable details
            string sPattern = ".*http://exhentai.org/g/[0-9]{6}/[a-zA-z0-9]{10}/.* onmouseover=.* onmouseout=.*>"
                + "[ \\(\\)a-zA-z0-9.]{0,100}\\[[ \\(\\)a-zA-z0-9.]{1,100}\\][ \\(\\)a-zA-z0-9.]{1,100}.*";
            string[] asplit = sPage.Split('<');
            for (int i = 0; i < asplit.Length; i++) {
                if (asplit[i].Length > 50 && Regex.IsMatch(asplit[i], sPattern)) {
                    lDetails.Add(new stEXH(
                        asplit[i].Substring(8, 39),
                        asplit[i].Split('>')[1].Split('<')[0])
                    );
                }
            }

            return lDetails;
        }
        
        public static string[] EHParse(string sURL)
        {
            const int iPreTag = 11;
            string[] asParse = new string[6];
            string[] asResp = new string[0];
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
            } catch (Exception exc) {
                Console.WriteLine(exc.Message);
                bExc = true;
            }

            //parse returned string
            if (!bExc && asResp.Length >= 11) {
                asParse[0] = HTMLConvertFrom(
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
            int indx = sName.LastIndexOf('\\');
            if(indx > -1) {
                sName = sName.Remove(0, indx + 1);
            }
            indx = sName.LastIndexOf('.');
            if(indx > -1) {
                switch(sName.Length - indx) {
                    case 3: return sName.Remove(indx, 3);
                    case 4: return sName.Remove(indx, 4);
                }
            }
            return sName;
        }

        public static string HTMLConvertFrom(string sRaw)
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

        public static string[] Split(string sRaw, params string[] sFilter)
        {
            return sRaw.Split(sFilter, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}