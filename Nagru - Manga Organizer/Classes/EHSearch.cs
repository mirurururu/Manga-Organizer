#region Assemblies
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
#endregion

namespace Nagru___Manga_Organizer
{
  /// <summary>
  /// Holds the results of an EH search
  /// </summary>
  public class csEHSearch
  {
    #region Properties
    private List<string> hsURL, hsTitle;
    private byte[] bOpt = new byte[10];
    private string sSearchURL;
    private int iCurrentPage = 0;
    private int iPages = 0;

    #region Interface
    /// <summary>
    /// The URL that was searched
    /// </summary>
    public string SearchURL {
      get {
        return sSearchURL;
      }
    }

    /// <summary>
    /// The current result page
    /// </summary>
    public int CurrentPage {
      get {
        return iCurrentPage + 1;
      }
    }

    /// <summary>
    /// total number of result pages
    /// </summary>
    public int Pages {
      get {
        return iPages + 1;
      }
    }

    /// <summary>
    /// The number of matching manga returned
    /// </summary>
    public int Count {
      get {
        return hsURL.Count;
      }
    }

    /// <summary>
    /// Returns the url at the specified index
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public string URL(int index) {
      return hsURL.ElementAt(index);
    }

    /// <summary>
    /// Returns the title at the specified index
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public string Title(int index) {
      return hsTitle.ElementAt(index);
    }

    /// <summary>
    /// set gallery type search options
    /// </summary>
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

		/// <summary>
		/// Save the gallery type settings
		/// </summary>
		public void SaveOptions()
		{
			SQL.UpdateSetting(SQL.Setting.SearchIgnore,
				string.Join(",", bOpt.Select(x => x.ToString()))
			);
		}
		
		/// <summary>
		/// Adds a possible match from EH
		/// </summary>
		/// <param name="sURL"></param>
		/// <param name="sTitle"></param>
		public void Add(string sURL, string sTitle)
    {
      hsURL.Add(sURL);
			hsTitle.Add(HttpUtility.HtmlDecode(sTitle));
    }

		/// <summary>
		/// Clears the result set
		/// </summary>
    public void Clear()
    {
      hsURL.Clear();
      hsTitle.Clear();
    }

    #endregion

    #endregion

		#region Inner Classes

		#region EH API JSON Class
		/// <summary>
    /// Used to simulate JS Object Literal for JSON 
    /// </summary>
    /// <remarks>Based on Hupotronics' ExLinks</remarks>
    private class csEHAPI
    {
      public string method = "gdata";
      public object[][] gidlist;

      public csEHAPI(string URL) {
        string[] asChunk = Ext.Split(URL, "/");

        if (asChunk.Length == 5) {
          gidlist = new object[1][];
          gidlist[0] = new object[2] { int.Parse(asChunk[3]), asChunk[4] };
        }
      }
    }
    #endregion

    #region EH Metadata
		/// <summary>
		/// Parses the JSON object returned from EH into a C# class
		/// </summary>
    public class gmetadata
    {
      public int gid;
      public string token;
      public string archiver_key;
      public string title;
      public string title_jpn;
      public string category;
      public string thumb;
      public string uploader;
      public DateTime posted;
      public int filecount;
      public int filesize;
      public bool expunged;
      public float rating;
      public int torrentcount;
      public string[] tags;
      private bool error = false;

			/// <summary>
			/// The constructor which parses out the JSON object
			/// </summary>
			/// <param name="JSON">The JSON object literal</param>
      public gmetadata(string JSON)
      {
        posted = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        //Clone the culture and set the decimal separator to "."
        CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
        ci.NumberFormat.CurrencyDecimalSeparator = ".";

        try {
          dynamic JsonObject = JObject.Parse(JSON);
          gid						= Int32.Parse(JsonObject.gmetadata[0].gid.Value.ToString());
          token					= JsonObject.gmetadata[0].token.Value;
          archiver_key	= JsonObject.gmetadata[0].archiver_key.Value;
          title					= HttpUtility.HtmlDecode(JsonObject.gmetadata[0].title.Value);
          title_jpn			= JsonObject.gmetadata[0].title_jpn.Value;
          category			= JsonObject.gmetadata[0].category.Value;
          thumb					= JsonObject.gmetadata[0].thumb.Value;
          uploader			= JsonObject.gmetadata[0].uploader.Value;
          posted				= posted.AddSeconds(long.Parse(JsonObject.gmetadata[0].posted.Value.ToString()));
          filecount			= Int32.Parse(JsonObject.gmetadata[0].filecount.Value.ToString());
          filesize			= Int32.Parse(JsonObject.gmetadata[0].filesize.Value.ToString());
          expunged			= bool.Parse(JsonObject.gmetadata[0].expunged.Value.ToString());
          rating				= float.Parse(JsonObject.gmetadata[0].rating.Value.ToString(), NumberStyles.Any, ci);
          torrentcount	= Int32.Parse(JsonObject.gmetadata[0].torrentcount.Value.ToString());
          tags					= (JsonObject.gmetadata[0].tags as JArray).Select(x => (string)x).ToArray();
        } catch (JsonReaderException exc) {
          Console.WriteLine(exc.Message);
          error = true;
        } catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException exc) {
          Console.WriteLine(exc.Message);
          error = true;
        }
      }

			/// <summary>
			/// Returns all the tags in the object, organized by name
			/// </summary>
			/// <param name="sCurrentTags">Adds in any passed in tags to the returned array</param>
			/// <returns></returns>
      public string GetTags(string sCurrentTags = null)
      {
        if (tags == null) {
          return string.Empty;
        }

        List<string> lRaw = new List<string>(tags.Length * 2);
        lRaw.AddRange(tags);

        if (!string.IsNullOrWhiteSpace(sCurrentTags)) {
          lRaw.AddRange(sCurrentTags.Split(','));
        }
        lRaw.Sort(new TrueCompare());
        
        return String.Join(", ",
					lRaw.Select(
						x => x.Trim()).Distinct().ToArray<string>()
				);
      }

      public bool APIError
      {
        get
        {
          return error;
        }
      }
    }
    #endregion

		#endregion

		#region Constructor

		/// <summary>
		/// Sets the initial values of the object
		/// </summary>
		public csEHSearch()
    {
      hsURL = new List<string>();
      hsTitle = new List<string>();

      #region Gallery Options
      bOpt = SQL.GetSetting(SQL.Setting.GallerySettings)
          .Split(',').Select(x => byte.Parse(x)).ToArray();
      #endregion
    }

		#endregion

		#region Search EH
		
		/// <summary>
		/// Creates the search string
		/// </summary>
		/// <param name="SearchTerms">The search terms from the user</param>
		/// <param name="useEXH">Whether to use regular EH or EXH</param>
		/// <param name="Options">The gallery types to search for</param>
		/// <returns></returns>
		private static string FormatSearch(string SearchTerms, bool useEXH, byte[] Options)
    {
      return string.Format("http://{0}.org/?f_doujinshi={1}&f_manga={2}&f_artistcg={3}"
        + "&f_gamecg={4}&f_western={5}&f_non-h={6}&f_imageset={7}&f_cosplay={8}"
        + "&f_asianporn={9}&f_misc={10}&f_search={11}&f_apply=Apply+Filter",
				useEXH ? "exhentai" : "g.e-hentai", Options[0], Options[1], Options[2], Options[3], Options[4], Options[5],
				Options[6], Options[7], Options[8], Options[9], Uri.EscapeDataString(SearchTerms).Replace("%20", "+"));
    }
		
		/// <summary>
		/// Searches EH and returns a list of gallery titles and addresses
		/// </summary>
		/// <param name="SearchTerms">The terms to search for</param>
    public void Search(string SearchTerms)
    {
      this.Clear();

      //exit if there (probably) isn't an internet connection
      if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
        return;

      string sPage = "";
      bool bException = true;

      //determine if exhentai can be called
      bool bXH = !string.IsNullOrWhiteSpace(
        SQL.GetSetting(SQL.Setting.pass_hash) + SQL.GetSetting(SQL.Setting.member_id));

      //convert raw search terms into web form
			sSearchURL = FormatSearch(SearchTerms, bXH, bOpt);
      
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
          new Cookie("ipb_member_id", SQL.GetSetting(SQL.Setting.member_id)) { Domain = "exhentai.org" },
          new Cookie("ipb_pass_hash", SQL.GetSetting(SQL.Setting.pass_hash)) { Domain = "exhentai.org" }
				});
      }

      try {
        //get webpage
        using (StreamReader sr = new StreamReader(((HttpWebResponse)
            rq.GetResponse()).GetResponseStream())) {
          sPage = sr.ReadToEnd();
        }
        rq.Abort();
        bException = false;
      } catch (WebException exc) {
        Console.WriteLine(exc.Message);
      }

      //find all gallery results
      if (!bException && !string.IsNullOrWhiteSpace(sPage)) {
        string sRegexGallery = ".*http://(ex|g.e-)hentai.org/g/[0-9]{6}/[a-zA-z0-9]{10}/.*"
          + "onmouseover=.* onmouseout=.*";
        const int iMinGallery = 125;

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

		#endregion

		#region Get Metadata
		
		/// <summary>
		/// Loads and parses a response from an EH URL
		/// </summary>
		/// <param name="sAddress"></param>
		public static gmetadata LoadMetadata(string sAddress)
    {
      string sEHResponse = string.Empty;
      gmetadata gmManga = null;
      bool bException = true;

      //exit if there (probably) isn't an internet connection
      if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
        return gmManga;

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
          byte[] byContent = Encoding.ASCII.GetBytes(
            JsonConvert.SerializeObject(new csEHAPI(sAddress)));
          s.Write(byContent, 0, byContent.Length);
        }
        using (StreamReader sr = new StreamReader((
          (HttpWebResponse)rq.GetResponse()).GetResponseStream())) {
          sEHResponse = sr.ReadToEnd();
          rq.Abort();
        }
        bException = false;
      } catch (WebException exc) {
        Console.WriteLine(exc.Message);
      } finally {
        //parse returned JSON
        if (!bException && !string.IsNullOrWhiteSpace(sEHResponse)) {
          gmManga = new gmetadata(sEHResponse);
        }
      }

      return gmManga;
    }

		#endregion
  }
}
