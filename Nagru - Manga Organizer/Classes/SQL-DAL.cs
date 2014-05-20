using System;
using System.IO;
using System.Net;
using System.Text;
using System.Linq;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using System.Data.SQLite;
//using Finisar.SQLite;
using SCA = SharpCompress.Archive;

namespace Nagru___Manga_Organizer
{
	/* SQL Conveniences */
	public static class SQL
	{
		#region Database Connection

		/// <summary>
		/// Returns whether or not a DB connection is currently open
		/// </summary>
		public static bool Connected
		{
			private set { Connected = value; }
			get
			{
				return csSQLCon.IsOpen();
			}
		}

		/// <summary>
		/// Opens a connection with the database
		/// </summary>
		/// <param name="_filePath">Can override the automatic DB filepath</param>
		/// <returns>Returns whether the operation suceeded or failed</returns>
		public static bool Connect(string _filePath = null)
		{
			//convert the old DB if it still exists
			if (string.IsNullOrEmpty(_filePath)) {
				//Get default old DB location
				_filePath = Properties.Settings.Default.SavLoc != string.Empty ?
						Properties.Settings.Default.SavLoc : Environment.CurrentDirectory;
				_filePath += "\\MangaDatabase.bin";
			}
			if (File.Exists(_filePath) 
					|| File.Exists(_filePath = ExtString.RelativePath(_filePath))) {
				csSQLCon.Import(_filePath);
			}

			return csSQLCon.IsOpen();
		}

		#endregion

		#region Retrieve from the DB

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static string[] GetArtists()
		{
			DataTable dt = csSQLCon.GetArtists();
			string[] asArtists = new string[dt.Rows.Count];

			for(int i = 0; i < dt.Rows.Count; i++) {
				asArtists[i] = dt.Rows[i][1].ToString();
			}

			return asArtists;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static string[] GetTypes()
		{
			DataTable dt = csSQLCon.GetTypes();
			string[] asTags = new string[dt.Rows.Count];

			for (int i = 0; i < dt.Rows.Count; i++) {
				asTags[i] = dt.Rows[i][1].ToString();
			}

			return asTags;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static string[] GetTags()
		{
			DataTable dt = csSQLCon.GetTags();
			string[] asTags = new string[dt.Rows.Count];

			for (int i = 0; i < dt.Rows.Count; i++) {
				asTags[i] = dt.Rows[i][1].ToString();
			}

			return asTags;
		}

		/// <summary>
		/// Returns the EH formatted title of a Manga
		/// </summary>
		/// <param name="_mangaID">The ID of the record to access</param>
		/// <returns></returns>
		public static string GetMangaTitle(int mangaID)
		{
			DataTable dt = csSQLCon.DB_GetEntryDetails(mangaID);
			string sArtist = "", sTitle = "";
			sArtist = dt.Rows[0]["Artist"].ToString();
			sTitle = dt.Rows[0]["Title"].ToString();
			dt.Clear();
			dt.Dispose();

			return ExtString.GetFormattedTitle(sArtist, sTitle);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="mangaID"></param>
		/// <returns></returns>
		public static decimal GetMangaRating(int mangaID)
		{
			DataTable dt = csSQLCon.DB_GetEntryDetails(mangaID);
			decimal dcRating = 0;
			dcRating = Convert.ToDecimal(dt.Rows[0]["Rating"].ToString());
			dt.Clear();
			dt.Dispose();

			return dcRating;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="mangaID"></param>
		/// <returns></returns>
		public static DataTable GetManga(int mangaID)
		{
			return csSQLCon.DB_GetEntryDetails(mangaID);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="mangaID"></param>
		/// <param name="columnName"></param>
		/// <returns></returns>
		public static string GetMangaDetail(int mangaID, string columnName)
		{
			return (csSQLCon.DB_GetEntryDetails(mangaID)).Rows[0][columnName].ToString();
		}

		public static DataTable GetAllEntries()
		{
			return csSQLCon.Entries();
		}
		#endregion

		#region Update the DB

		/// <summary>
		/// 
		/// </summary>
		/// <param name="Artist"></param>
		/// <param name="Title"></param>
		/// <param name="Tags"></param>
		/// <param name="Location"></param>
		/// <param name="PublishedDate"></param>
		/// <param name="Pages"></param>
		/// <param name="Type"></param>
		/// <param name="Rating"></param>
		/// <param name="Description"></param>
		/// <param name="URL"></param>
		/// <param name="MangaID"></param>
		/// <returns></returns>
		public static int SaveManga(string Artist, string Title, string Tags, string Location,
				DateTime PublishedDate, decimal Pages, string Type, decimal Rating,
				string Description, string URL = null, int MangaID = -1)
		{
			return csSQLCon.DB_SaveEntry(Artist, Title, Tags, Location,
			PublishedDate, Convert.ToInt32(Pages), Type, Rating,
			Description, URL, MangaID);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="Location"></param>
		/// <returns></returns>
		public static int AddMangaFromPath(string Location)
		{
			//Try to format raw title string
			string[] asTitle = Main.SplitTitle(
					ExtString.GetNameSansExtension(Location));
			int iPages = 0;

			//Get filecount
			string[] sFiles = new string[0];
			if (File.Exists(Location))
				sFiles = new string[1] { Location };
			else
				sFiles = ExtDir.GetFiles(Location,
					SearchOption.TopDirectoryOnly, "*.zip|*.cbz|*.cbr|*.rar|*.7z");

			if (sFiles.Length > 0) {
				if (Main.IsArchive(sFiles[0])) {
					SCA.IArchive scArchive = SCA.ArchiveFactory.Open(sFiles[0]);
					iPages = (scArchive.Entries.Count() > ushort.MaxValue) ?
							ushort.MaxValue : (ushort)Math.Abs(scArchive.Entries.Count());
					scArchive.Dispose();
				}
			}
			else {
				iPages = (ushort)ExtDir.GetFiles(
					Location, SearchOption.TopDirectoryOnly).Length;
			}

			//save manga
			return csSQLCon.DB_SaveEntry(asTitle[0], asTitle[1], "", 
				Location, DateTime.Now, iPages, "Manga", 0, "");
		}
		
		/// <summary>
		/// Deletes an entry from the database
		/// </summary>
		/// <param name="mangaID">The ID of the record to be deleted</param>
		/// <param name="DeleteSource">Determines whether to delete the source files</param>
		/// <returns>Returns the success state of the operation</returns>
		public static bool DeleteManga(int mangaID, bool DeleteSource = false)
		{
			int error = csSQLCon.Entry_Delete(mangaID);
			return (error == 0);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="Rating"></param>
		/// <returns></returns>
		public static bool UpdateRating(decimal Rating)
		{
			return false;
		}
		#endregion

		#region Search the DB

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static bool ContainsEntry(string Artist, string Title)
		{
			return csSQLCon.EntryExists(Artist, Title);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="SearchTerms"></param>
		/// <param name="MangaID"></param>
		/// <returns></returns>
		public static DataTable Search(string SearchTerms, int MangaID = -1)
		{
			return csSQLCon.Search(SearchTerms, MangaID);
		}

		#endregion
	}
}
