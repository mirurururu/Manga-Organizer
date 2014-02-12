using System;
using System.IO;
using System.Net;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Nagru___Manga_Organizer
{
	/* Hold results of e-hentai search */
	public class csEHSearch
	{
		#region Interface
		//returns searched URL
		public string SearchURL {
			get {
				return sSearchURL;
			}
		}

		//returns current result page
		public int CurrentPage {
			get {
				return iCurrentPage + 1;
			}
		}

		//returns number of result pages
		public int Pages {
			get {
				return iPages + 1;
			}
		}

		//returns number of sets
		public int Count {
			get {
				return hsURL.Count;
			}
		}

		//returns URL element
		public string URL(int i) {
			return hsURL.ElementAt(i);
		}

		//returns Title element
		public string Title(int i) {
			return hsTitle.ElementAt(i);
		}

		//set gallery type search options
		public bool[] Options {
			get {
				return new bool[10] {
                    (bOpt[0] == 1), (bOpt[1] == 1),
                    (bOpt[2] == 1), (bOpt[3] == 1),
                    (bOpt[4] == 1), (bOpt[5] == 1),
                    (bOpt[6] == 1), (bOpt[7] == 1),
                    (bOpt[8] == 1), (bOpt[9] == 1),
                };
			}
			set {
				if (value.Length != 10) {
					throw new ArgumentOutOfRangeException();
				}

				bOpt = new byte[10] {
                    (byte)((value[0]) ? 1 : 0), (byte)((value[1]) ? 1 : 0),
                    (byte)((value[2]) ? 1 : 0), (byte)((value[3]) ? 1 : 0),
                    (byte)((value[4]) ? 1 : 0), (byte)((value[5]) ? 1 : 0),
                    (byte)((value[6]) ? 1 : 0), (byte)((value[7]) ? 1 : 0),
                    (byte)((value[8]) ? 1 : 0), (byte)((value[9]) ? 1 : 0),
                };
			}
		}

		//returns error state
		public bool Error {
			get { return bConnError; }
		}
		#endregion

		#region Properties
		private HashSet<string> hsURL, hsTitle;
		private byte[] bOpt = new byte[10];
		private string sSearchURL;
		private int iCurrentPage = 0;
		private int iPages = 0;
		private bool bConnError = false;
		#endregion

		public csEHSearch()
		{
			hsURL = new HashSet<string>();
			hsTitle = new HashSet<string>();

			#region Gallery Options
			bOpt = new byte[10] {
                1,		//doujinshi
                1,		//manga
                0,		//artistCG
                0,		//gameCG
                0,		//western
                0,		//non-h
                0,		//imageSet
                0,		//cosplay
                0,		//asian
                0		//misc
            };
			#endregion
		}

		public void Add(string sURL, string sTitle)
		{
			hsURL.Add(sURL);
			hsTitle.Add(ExtString.HTMLConvertToPlainText(sTitle));
		}

		private static string FormatSearch(string sRaw, string sSite, byte[] byOpt)
		{
			return string.Format("http://{0}.org/?f_doujinshi={1}&f_manga={2}&f_artistcg={3}"
				+ "&f_gamecg={4}&f_western={5}&f_non-h={6}&f_imageset={7}&f_cosplay={8}"
				+ "&f_asianporn={9}&f_misc={10}&f_search={11}&f_apply=Apply+Filter",
				sSite, byOpt[0], byOpt[1], byOpt[2], byOpt[3], byOpt[4], byOpt[5],
				byOpt[6], byOpt[7], byOpt[8], byOpt[9], Uri.EscapeDataString(sRaw));
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

		public static string[] LoadMetadata(string sAddress)
		{
			const int iPreTag = 11;
			string[] asParse = new string[6]
				, asResp = new string[0];
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
				using (Stream s = rq.GetRequestStream()) 
				{
					byte[] byContent = Encoding.ASCII.GetBytes(JSON(sAddress));
					s.Write(byContent, 0, byContent.Length);
				}
				using (StreamReader sr = new StreamReader((
					(HttpWebResponse)rq.GetResponse()).GetResponseStream()))
				{
					asResp = ExtString.Split(sr.ReadToEnd(), "\",\"");
					rq.Abort();
				}
			} catch (WebException exc) {
				Console.WriteLine(exc.Message);
				bExc = true;
			}

			//parse returned string
			if (!bExc && asResp.Length >= 11)
			{
				asParse[0] = ExtString.HTMLConvertToPlainText(
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

		public void Search(string sRaw)
		{
			string sPage = "";

			//determine if exhentai can be called
			bool bXH = !(string.IsNullOrEmpty(Properties.Settings.Default.pass_hash)
				|| string.IsNullOrEmpty(Properties.Settings.Default.member_id));

			//convert raw search terms into web form
			sSearchURL = FormatSearch(sRaw, (bXH) ? "exhentai" : "g.e-hentai", bOpt);

			//set up connection
			ServicePointManager.DefaultConnectionLimit = 64;
			HttpWebRequest rq = (HttpWebRequest)
				WebRequest.Create(sSearchURL);
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
				using (StreamReader sr = new StreamReader(((HttpWebResponse)
						rq.GetResponse()).GetResponseStream()))
				{
					sPage = sr.ReadToEnd();
				}
				rq.Abort();
			} catch (WebException exc) {
				Console.WriteLine(exc.Message);
				bConnError = true;
			}

			//find all gallery results
			if (!bConnError && !string.IsNullOrEmpty(sPage))
			{
				string sRegexGallery = ".*http://(ex|g.e-)hentai.org/g/[0-9]{6}/[a-zA-z0-9]{10}/.*"
					+ "onmouseover=.* onmouseout=.*";
				//string sRegexPage = "td onclick=\"sp\\([0-9]*\\)\">";
				const int iMinGallery = 125;
				//const int iMinPage = 18;

				string[] asplit = sPage.Split('<');
				for (int i = 0; i < asplit.Length; i++) {
					if (asplit[i].Length >= iMinGallery
							&& Regex.IsMatch(asplit[i], sRegexGallery))
					{
						this.Add(
							asplit[i].Split('"')[1],
							asplit[i].Split('>')[1].Split('<')[0]
						);
					}
				}
			}
		}
	}
}
