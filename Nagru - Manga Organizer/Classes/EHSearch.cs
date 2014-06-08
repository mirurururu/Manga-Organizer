using System;
using System.IO;
using System.Net;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Nagru___Manga_Organizer
{
	/// <summary>
	/// Hold results of e-hentai search
	/// </summary>
	public class csEHSearch
	{
		#region Interface
		/// <summary>
		/// The URL that was searched
		/// </summary>
		public string SearchURL
		{
			get
			{
				return sSearchURL;
			}
		}

		/// <summary>
		/// The current result page
		/// </summary>
		public int CurrentPage
		{
			get
			{
				return iCurrentPage + 1;
			}
		}

		/// <summary>
		/// total number of result pages
		/// </summary>
		public int Pages
		{
			get
			{
				return iPages + 1;
			}
		}

		/// <summary>
		/// The number of matching manga returned
		/// </summary>
		public int Count
		{
			get
			{
				return hsURL.Count;
			}
		}

		/// <summary>
		/// Returns the url at the specified index
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public string URL(int index)
		{
			return hsURL.ElementAt(index);
		}

		/// <summary>
		/// Returns the title at the specified index
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public string Title(int index)
		{
			return hsTitle.ElementAt(index);
		}

		/// <summary>
		/// set gallery type search options
		/// </summary>
		public bool[] Options
		{
			get
			{
				return new bool[10] {
                    (bOpt[0] == 1), (bOpt[1] == 1),
                    (bOpt[2] == 1), (bOpt[3] == 1),
                    (bOpt[4] == 1), (bOpt[5] == 1),
                    (bOpt[6] == 1), (bOpt[7] == 1),
                    (bOpt[8] == 1), (bOpt[9] == 1),
                };
			}
			set
			{
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

		/// <summary>
		/// Returns whether an error has ocurred
		/// </summary>
		public bool Error
		{
			get
			{
				return bConnError;
			}
		}
		#endregion

		#region Properties
		private List<string> hsURL, hsTitle;
		private byte[] bOpt = new byte[10];
		private string sSearchURL;
		private int iCurrentPage = 0;
		private int iPages = 0;
		private bool bConnError = false;
		#endregion

		public csEHSearch()
		{
			hsURL = new List<string>();
			hsTitle = new List<string>();

			#region Gallery Options
			bOpt = Properties.Settings.Default.GalleryTypes
					.Split(',').Select(x => byte.Parse(x)).ToArray();
			#endregion
		}

		public void Add(string sURL, string sTitle)
		{
			hsURL.Add(sURL);
			hsTitle.Add(ExtString.HTMLConvertToPlainText(sTitle));
		}

		public void Clear()
		{
			hsURL.Clear();
			hsTitle.Clear();
		}

		private static string FormatSearch(string sRaw, string sSite, byte[] byOpt)
		{
			return string.Format("http://{0}.org/?f_doujinshi={1}&f_manga={2}&f_artistcg={3}"
				+ "&f_gamecg={4}&f_western={5}&f_non-h={6}&f_imageset={7}&f_cosplay={8}"
				+ "&f_asianporn={9}&f_misc={10}&f_search={11}&f_apply=Apply+Filter",
				sSite, byOpt[0], byOpt[1], byOpt[2], byOpt[3], byOpt[4], byOpt[5],
				byOpt[6], byOpt[7], byOpt[8], byOpt[9], Uri.EscapeDataString(sRaw).Replace("%20", "+"));
		}

		/// <summary>
		/// Used to simulate JS Object Literal for JSON 
		/// </summary>
		/// <remarks>Based on Hupotronics' ExLinks</remarks>
		/// <param name="sURL"></param>
		/// <returns></returns>
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
			string sEHResponse = string.Empty;
			string[] asParse = new string[0];
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
					byte[] byContent = Encoding.ASCII.GetBytes(JSON(sAddress));
					s.Write(byContent, 0, byContent.Length);
				}
				using (StreamReader sr = new StreamReader((
					(HttpWebResponse)rq.GetResponse()).GetResponseStream())) {
					sEHResponse = sr.ReadToEnd();
					rq.Abort();
				}
			} catch (WebException exc) {
				Console.WriteLine(exc.Message);
				bExc = true;
			}

			//parse returned string
			if (!bExc && !string.IsNullOrEmpty(sEHResponse)) {
				try {
					DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
					dynamic dynJsonObj = JObject.Parse(sEHResponse);

					asParse = new string[6];
					asParse[0] = dynJsonObj.gmetadata[0].title.Value;
					asParse[1] = dynJsonObj.gmetadata[0].category.Value;
					asParse[2] = dt.AddSeconds(long.Parse(dynJsonObj.gmetadata[0].posted.Value)).ToShortDateString();
					asParse[3] = dynJsonObj.gmetadata[0].filecount.Value;
					asParse[4] = dynJsonObj.gmetadata[0].rating.Value;
					asParse[5] = string.Join(",", dynJsonObj.gmetadata[0].tags);
				} catch (JsonReaderException exc) {
					Console.WriteLine(exc.Message);
				}
			}

			return asParse;
		}

		public void Search(string sRaw)
		{
			this.Clear();

			//exit if there (probably) isn't an internet connection
			if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
				return;

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
						rq.GetResponse()).GetResponseStream())) {
					sPage = sr.ReadToEnd();
				}
				rq.Abort();
			} catch (WebException exc) {
				Console.WriteLine(exc.Message);
				bConnError = true;
			}

			//find all gallery results
			if (!bConnError && !string.IsNullOrEmpty(sPage)) {
				string sRegexGallery = ".*http://(ex|g.e-)hentai.org/g/[0-9]{6}/[a-zA-z0-9]{10}/.*"
					+ "onmouseover=.* onmouseout=.*";
				//string sRegexPage = "td onclick=\"sp\\([0-9]*\\)\">";
				const int iMinGallery = 125;
				//const int iMinPage = 18;

				string[] asplit = sPage.Split('<');
				for (int i = 0; i < asplit.Length; i++) {
					if (asplit[i].Length >= iMinGallery
							&& Regex.IsMatch(asplit[i], sRegexGallery)) {
						this.Add(
							asplit[i].Split('"')[1],
							asplit[i].Split('>')[1].Split('<')[0]
						);
					}
				}
			}
		}

		/// <summary>
		/// Save the gallery type settings
		/// </summary>
		public void SaveOptions()
		{
			Properties.Settings.Default.GalleryTypes =
					string.Join(",", bOpt.Select(x => x.ToString()));
		}
	}
}
