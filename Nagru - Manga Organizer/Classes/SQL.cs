using System;
using System.IO;
using System.Net;
using System.Text;
using System.Linq;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using System.Data.SQLite;

namespace Nagru___Manga_Organizer
{
	/* Controls access to the database */
	public static class SQL
	{
		#region Properties

		/// <summary>
		/// Returns whether or not the DB connection is currently open
		/// </summary>
		public static bool Connected
		{
			private set
			{
				Connected = value;
			}
			get
			{
				return (sqConn.State == ConnectionState.Open);
			}
		}

		#endregion

		#region Variables

		public delegate void DelVoidInt(int i);
		public static DelVoidInt delProgress = null;

		private static SQLiteConnection sqConn;
		private static bool bConverting = false;
		private const int SQLITE_MAX_LENGTH = 1000000;

		//prevents having to write alter statements 
		//if I need to make changes to them later
		#region Views
		private const string vsManga = @"
				select
					mgx.MangaID
					,ifnull(at.Name, '')    Artist
					,mgx.Title
					,mgx.Pages
					,tg.Tags
					,mgx.Description
					,mgx.PublishedDate
					,mgx.Location
					,mgx.GalleryURL
					,tp.Type
					,mgx.Rating
				from
					[Manga] mgx
				left outer join
					[Type] tp on tp.TypeID = mgx.TypeID
				left outer join
					[MangaArtist] mga on mga.MangaID = mgx.MangaID
				left outer join
					[Artist] at on at.ArtistID = mga.ArtistID
				left outer join
				(
					select mgt.MangaID, group_concat(tx.Tag, ', ') Tags
					from [Tag] tx
					join [MangaTag] mgt on mgt.TagID = tx.TagID
					group by mgt.MangaID
					order by tx.Tag
				) tg on tg.MangaID = mgx.MangaID ";
		private const string vsMangaEnd = " group by mgx.MangaID";
		#endregion

		#endregion

		#region Class Functions

		static SQL()
		{
			DB_Connect();
		}

		#endregion

		#region Public Access

		#region Handle Connection

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
				Import(_filePath);
			}

			return Connected;
		}

		#endregion

		#region Query Database

		/// <summary>
		/// Returns all the Artists in the database
		/// </summary>
		/// <returns></returns>
		public static string[] GetArtists()
		{
			string sCommandText = @"
				select
						at.ArtistID
					,ifnull(at.Name, '')				Artist
				from
					[Artist] at
				order by at.Name asc
			";

			string[] asArtists;
			using (DataTable dt = ExecuteQuery(sCommandText)) {
				asArtists = new string[dt.Rows.Count];

				for (int i = 0; i < dt.Rows.Count; i++) {
					asArtists[i] = dt.Rows[i][1].ToString();
				}
			}

			return asArtists;
		}

		/// <summary>
		/// Returns all the Types in the database
		/// </summary>
		/// <returns></returns>
		public static string[] GetTypes()
		{
			string sCommandText = @"
				select
						tp.TypeID
					,tp.Type
				from
					[Type] tp
				order by tp.Type asc
			";

			string[] asTypes;
			using (DataTable dt = ExecuteQuery(sCommandText)) {
				asTypes = new string[dt.Rows.Count];

				for (int i = 0; i < dt.Rows.Count; i++) {
					asTypes[i] = dt.Rows[i][1].ToString();
				}
			}

			return asTypes;
		}

		/// <summary>
		/// Returns all the Tags in the database
		/// </summary>
		/// <returns></returns>
		public static string[] GetTags()
		{
			string sCommandText = @"
				select
						tg.TagID
					,tg.Tag
				from
					[Tag] tg
				order by tg.Tag asc
			";

			string[] asTags;
			using (DataTable dt = ExecuteQuery(sCommandText)) {
				asTags = new string[dt.Rows.Count];

				for (int i = 0; i < dt.Rows.Count; i++) {
					asTags[i] = dt.Rows[i][1].ToString();
				}
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
			string sArtist = "", sTitle = "";
			using (DataTable dt = DB_GetEntryDetails(mangaID)) {
				sArtist = dt.Rows[0]["Artist"].ToString();
				sTitle = dt.Rows[0]["Title"].ToString();
			}

			return ExtString.GetFormattedTitle(sArtist, sTitle);
		}

		/// <summary>
		/// Returns the full details of a specified manga
		/// </summary>
		/// <param name="mangaID">The ID of the record</param>
		/// <returns>Returns a single row containing manga details</returns>
		public static DataTable GetManga(int mangaID)
		{
			return DB_GetEntryDetails(mangaID);
		}

		/// <summary>
		/// Returns a single detail of a specified manga
		/// </summary>
		/// <param name="mangaID">The ID of the record</param>
		/// <param name="columnName">The name of the column to extract</param>
		/// <returns></returns>
		public static string GetMangaDetail(int mangaID, string columnName)
		{
			string sVal = "";
			using (DataTable dt = DB_GetEntryDetails(mangaID)) {
				sVal = dt.Rows[0][columnName].ToString();
			}
			return sVal;
		}

		public static string GetSetting(string columnName)
		{
			string sVal = "";
			using (DataTable dt = DB_GetSettings(columnName)) {
				sVal = dt.Rows[0][columnName].ToString();
			}
			return sVal;
		}

		/// <summary>
		/// Returns the details of every manga in the database
		/// </summary>
		/// <returns></returns>
		public static DataTable GetAllEntries()
		{
			return GetEntries();
		}

		#region Search Database

		/// <summary>
		/// Returns whether the entry exists in the DB
		/// </summary>
		/// <param name="Artist">The artist's name</param>
		/// <param name="Title">The title of the maga</param>
		/// <returns></returns>
		public static bool ContainsEntry(string Artist, string Title)
		{
			return EntryExists(Artist, Title);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="SearchTerms"></param>
		/// <param name="MangaID"></param>
		/// <returns></returns>
		public static DataTable Search(string SearchTerms, int MangaID = -1)
		{
			return DB_Search(SearchTerms, MangaID);
		}

		#endregion

		#endregion

		#region Update Database

		/// <summary>
		/// Insert or updates a manga
		/// </summary>
		/// <param name="Artist">The name of the artist</param>
		/// <param name="Title">The title of the manga</param>
		/// <param name="Tags">The comma-delimited tags</param>
		/// <param name="Location">The local filepath</param>
		/// <param name="PublishedDate">The date the manga was published to EH</param>
		/// <param name="Pages">The number of pages</param>
		/// <param name="Type">The type of the manga</param>
		/// <param name="Rating">The decimal rating</param>
		/// <param name="Description">User comments</param>
		/// <param name="URL">The source URL of the gallery</param>
		/// <param name="MangaID">If passed through, attempts to update the indicated record</param>
		/// <returns></returns>
		public static int SaveManga(string Artist, string Title, string Tags, string Location,
				DateTime PublishedDate, decimal Pages, string Type, decimal Rating,
				string Description, string URL = null, int MangaID = -1)
		{
			return DB_SaveEntry(Artist, Title, Tags, Location,
				PublishedDate, Convert.ToInt32(Pages), Type, Rating,
				Description, URL, MangaID);
		}

		/// <summary>
		/// Deletes an entry from the database
		/// </summary>
		/// <param name="mangaID">The ID of the record to be deleted</param>
		/// <returns>Returns the success state of the operation</returns>
		public static bool DeleteManga(int mangaID)
		{
			int error = Entry_Delete(mangaID);
			return (error == 0);
		}

		/// <summary>
		/// Updates only the rating of the indicated record
		/// </summary>
		/// <param name="MangaID">The ID of the record to update</param>
		/// <param name="Rating">The new rating value</param>
		/// <returns></returns>
		public static void UpdateRating(int MangaID, decimal Rating)
		{
			string sCommandText = "update Manga set Rating = @rating where MangaID = @mangaID";
			SQLiteParameter sqManga = new SQLiteParameter("@mangaID", DbType.Int32) {
				Value = MangaID
			};
			SQLiteParameter sqRating = new SQLiteParameter("@rating", DbType.Decimal) {
				Value = Rating
			};

			ExecuteNonQuery(sCommandText, CommandBehavior.Default, sqManga, sqRating);
		}

		#endregion

		#endregion

		#region Internal Methods

		#region Handle connection

		private static void DB_Connect()
		{
			DB_Close();
			string sPath = Properties.Settings.Default.SavLoc != string.Empty ?
				Properties.Settings.Default.SavLoc : Environment.CurrentDirectory;
			sPath += "\\MangaDB.sqlite";

			//check existence
			bool bExist = false;
			if (File.Exists(sPath))
				bExist = true;

			//create connection
			sqConn = new SQLiteConnection();
			if (!bExist)
				SQLiteConnection.CreateFile(sPath);
			sqConn.ConnectionString = new DbConnectionStringBuilder()
			{
				{"Data Source", sPath},
				{"Version", "3"},
				{"Compress", true},
				{"New", !bExist}
			}.ConnectionString;
			sqConn.Open();

			if (!bExist) {
				DB_Create();
			}
		}

		private static void DB_Close()
		{
			if (sqConn != null
					&& sqConn.State != ConnectionState.Closed) {
				sqConn.Close();
			}
		}

		#region Convenience
		private static string Cleanse(string sRaw)
		{
			return sRaw.Replace("'", "''").Replace(";", "");
		}

		private static int BeginTransaction()
		{
			int iRetVal = 0;
			using (SQLiteCommand sqCmd = sqConn.CreateCommand()) {
				sqCmd.CommandText = "begin transaction";
				iRetVal = sqCmd.ExecuteNonQuery();
			}
			return iRetVal;
		}

		private static int EndTransaction(int error = 0)
		{
			int iRetVal = 0;
			using (SQLiteCommand sqCmd = sqConn.CreateCommand()) {
				sqCmd.CommandText = (error > -1 ? "commit" : "rollback") + " transaction";
				iRetVal = sqCmd.ExecuteNonQuery();
			}
			return iRetVal;
		}

		private static int ExecuteNonQuery(string CommandText,
			CommandBehavior cmd = CommandBehavior.Default, params SQLiteParameter[] sqParam)
		{
			int altered = 0;

			using (SQLiteCommand sqCmd = sqConn.CreateCommand()) {
				sqCmd.Parameters.AddRange(sqParam);
				sqCmd.CommandText = CommandText;
				altered = sqCmd.ExecuteNonQuery(cmd);
			}

			return altered;
		}

		private static DataTable ExecuteQuery(string CommandText,
			CommandBehavior cmd = CommandBehavior.Default, params SQLiteParameter[] sqParam)
		{
			DataTable dt = new DataTable();

			using (SQLiteCommand sqCmd = sqConn.CreateCommand()) {
				sqCmd.Parameters.AddRange(sqParam);
				sqCmd.CommandText = CommandText;

				using (SQLiteDataReader dr = sqCmd.ExecuteReader(cmd)) {
					dt.Load(dr);
				}
			}

			return dt;
		}
		#endregion

		#endregion

		#region Create Database

		private static bool Import(string _filePath)
		{
			bConverting = true;

			//load the old DB
			List<Main.csEntry> lData = FileSerializer.Deserialize
				<List<Main.csEntry>>(_filePath) ?? new List<Main.csEntry>(0);

			//input into new DB
			BeginTransaction();
			for (int i = 0; i < lData.Count; i++) {
				//ensure sizes are valid
				#region Force value sizes to be inside valid range
				if (lData[i].sArtist.Length > SQLITE_MAX_LENGTH)
					lData[i].sArtist = lData[i].sArtist.Substring(0, SQLITE_MAX_LENGTH);
				if (lData[i].sTitle.Length > SQLITE_MAX_LENGTH)
					lData[i].sTitle = lData[i].sTitle.Substring(0, SQLITE_MAX_LENGTH);
				if (lData[i].sLoc.Length > SQLITE_MAX_LENGTH)
					lData[i].sLoc = string.Format(
						"Truncated -- Length over {0} character limit", SQLITE_MAX_LENGTH);
				if (lData[i].sDesc.Length > SQLITE_MAX_LENGTH)
					lData[i].sDesc = lData[i].sDesc.Substring(0, SQLITE_MAX_LENGTH);
				#endregion

				//add the entry
				DB_SaveEntry(
						lData[i].sArtist, lData[i].sTitle, lData[i].sTags,
						lData[i].sLoc, lData[i].dtDate, lData[i].iPages,
						lData[i].sType, lData[i].byRat, lData[i].sDesc,
						""
				);

				if (delProgress != null) {
					delProgress.Invoke(i + 1);
				}
			}
			EndTransaction();
			lData.Clear();
			bConverting = false;

			//deprecate old serialized DB
			try {
				File.Move(_filePath, _filePath + "_Deprecated");
			} catch (IOException) {
				Console.WriteLine("Could not alter old database");
			}

			return true;
		}

		private static void DB_Create()
		{
			string sQuery;

			//CreateSettings();

			#region Create Tables

			#region Artist
			sQuery = @"
				create table [Artist]
				(
					ArtistID				integer			primary key		autoincrement
					,Name						text				not null			unique
					,Psuedonym			text				null
					,CreatedDBTime	text				not null			default CURRENT_TIMESTAMP
					,AuditDBTime		text				not null			default CURRENT_TIMESTAMP
				)
			";
			ExecuteNonQuery(sQuery);
			#endregion

			#region Tag
			sQuery = @"
				create table [Tag]
				(
					TagID						integer			primary key		autoincrement
					,Tag						text				not null			unique
					,CreatedDBTime	text				not null			default CURRENT_TIMESTAMP
					,AuditDBTime		text				not null			default CURRENT_TIMESTAMP
				)
			";
			ExecuteNonQuery(sQuery);
			#endregion

			#region Type
			sQuery = @"
				create table [Type]
				(
					TypeID					integer		primary key		autoincrement
					,Type						text			not null			unique
					,CreatedDBTime	text			not null			default CURRENT_TIMESTAMP
					,AuditDBTime		text			not null			default CURRENT_TIMESTAMP
				)
			";
			ExecuteNonQuery(sQuery);
			#endregion

			#region MangaArtist
			sQuery = @"
				create table [MangaArtist]
				(
					MangaArtistID		integer		primary key		autoincrement
					,MangaID				integer		not null
					,ArtistID				integer		not null
					,CreatedDBTime	text			not null		default CURRENT_TIMESTAMP
					,AuditDBTime		text			not null		default CURRENT_TIMESTAMP
					,constraint [fk_mangaID] foreign key ([MangaID]) references [Manga] ([MangaID])
					,constraint [fk_artistID] foreign key ([ArtistID]) references [Artist] ([ArtistID])
				)
			";
			ExecuteNonQuery(sQuery);
			#endregion

			#region MangaTag
			sQuery = @"
				create table [MangaTag]
				(
					MangaTagID			integer		primary key		autoincrement
					,MangaID				integer		not null
					,TagID					integer		not null
					,CreatedDBTime	text			not null		default CURRENT_TIMESTAMP
					,AuditDBTime		text			not null		default CURRENT_TIMESTAMP
					,constraint [fk_mangaID] foreign key ([MangaID]) references [Manga] ([MangaID])
					,constraint [fk_tagID] foreign key ([TagID]) references [Tag] ([TagID])
				)
			";
			ExecuteNonQuery(sQuery);
			#endregion

			#region Manga
			sQuery = @"
				create table [Manga]
				(
					MangaID					integer			primary key		autoincrement
					,TypeID					integer			null
					,Title					text
					,Pages					integer			not null			default		0
					,Rating					numeric			not null			default		0
					,Description		text				null
					,Location				text				null
					,GalleryURL			text				null
					,PublishedDate	text				null
					,CreatedDBTime	text				not null			default CURRENT_TIMESTAMP
					,AuditDBTime		text				not null			default CURRENT_TIMESTAMP
					,constraint [fk_typeID] foreign key ([TypeID]) references [Type] ([TypeID])
				)
			";
			ExecuteNonQuery(sQuery);
			#endregion

			#endregion

			#region Create Triggers

			#region Artist
			sQuery = @"
				create trigger trArtist after update on Artist
				begin
					update Artist set AuditDBTime = CURRENT_TIMESTAMP where artistID = new.rowid;
				end;
			";
			ExecuteNonQuery(sQuery);
			#endregion

			#region Tag
			sQuery = @"
				create trigger trTag after update on Tag
				begin
					update Tag set AuditDBTime = CURRENT_TIMESTAMP where tagID = new.rowid;
				end;
			";
			ExecuteNonQuery(sQuery);
			#endregion

			#region Type
			sQuery = @"
				create trigger trType after update on Type
				begin
					update Type set AuditDBTime = CURRENT_TIMESTAMP where typeID = new.rowid;
				end;
			";
			ExecuteNonQuery(sQuery);
			#endregion

			#region MangaArtist
			sQuery = @"
				create trigger trMangaArtist after update on MangaArtist
				begin
					update MangaArtist set AuditDBTime = CURRENT_TIMESTAMP where mangaArtistID = new.rowid;
				end;
			";
			ExecuteNonQuery(sQuery);
			#endregion

			#region MangaTag
			sQuery = @"
				create trigger trMangaTag after update on MangaTag
				begin
					update MangaTag set AuditDBTime = CURRENT_TIMESTAMP where mangaTagID = new.rowid;
				end;
			";
			ExecuteNonQuery(sQuery);
			#endregion

			#region Manga
			sQuery = @"
				create trigger trManga after update on Manga
				begin
					update Manga set AuditDBTime = CURRENT_TIMESTAMP where mangaID = new.rowid;
				end;
			";
			ExecuteNonQuery(sQuery);
			#endregion

			#endregion

			#region Populate Default Table Values
			sQuery = @"
				insert into [Type] (Type)
				values('Doujinshi'),('Manga'),('Artist CG'),('Game CG'),('Western'),('Non-H'),('Image Set'),('Cosplay'),('Asian Porn'),('Misc')";
			ExecuteNonQuery(sQuery);
			#endregion
		}

		private static void CreateSettings()
		{
			bool bExists = false;

			//check if the table already exists
			using (DataTable dt = ExecuteQuery("select * from sqlite_master where tbl_name = 'Settings'")) {
				if (dt.Rows.Count > 0)
					bExists = true;
			}

			//if it doesn't, create it
			if (!bExists) {
				string sCommandText = @"
					create table [Settings]
					(
						SettingsID						integer			primary key		autoincrement
						,RootPath							text				null
						,SavePath							text				null
						,SearchIgnore					text				null
						,FormPosition					text				null
						,ImageBrowser					text				null
						,member_id						text				null
						,pass_hash						text				null
						,NewUser							integer			not null			default		1
						,SendReports					integer			not null			default		1
						,ShowGrid							integer			not null			default		1
						,ShowDate							integer			not null			default		1
						,ReadInterval					integer			not null			default		20000
						,RowColourHighlight		integer			not null			default		-15
						,RowColourAlt					integer			not null			default		-657931
						,BackgroundColor			text				not null			default		'39,40,34'
						,GallerySettings			text				not null			default		'1,1,0,0,0,0,0,0,0,0'
						,CreatedDBTime				text				not null			default		CURRENT_TIMESTAMP
						,AuditDBTime					text				not null			default		CURRENT_TIMESTAMP
					)";
				ExecuteNonQuery(sCommandText);

				//set up trigger for changes
				sCommandText = @"
					create trigger trSettings after update on Settings
					begin
						update Settings set AuditDBTime = CURRENT_TIMESTAMP where settingsID = new.rowid;
					end;";
				ExecuteNonQuery(sCommandText);
			}
		}

		#endregion

		#region Search Database

		private static DataTable DB_Search(string sTerms, int iMangaID = -1)
		{
			if (string.IsNullOrWhiteSpace(sTerms)) {
				return GetEntries();
			}
			
			//Set up variables
			StringBuilder sbCmd = new StringBuilder(5000);
			string[] asItems = ExtString.Split(sTerms, " ");
			string[] asType = new string[asItems.Length];
			bool[][] abNot = new bool[asItems.Length][];
			string[][] asTerms = new string[asItems.Length][];

			#region Parse Terms
			for (int i = 0; i < asItems.Length; i++) {
				//check for type limiter
				string[] sSplit = asItems[i].Trim().Split(':');
				if (sSplit.Length > 1) {
					asType[i] = sSplit[0];
				}

				string[] asSubSplit = ExtString.Split(sSplit[sSplit.Length > 1 ? 1 : 0], "&", ",");
				asTerms[i] = new string[asSubSplit.Length];
				for (int x = 0; x < asSubSplit.Length; x++) {
					asTerms[i][x] = asSubSplit[x];
				}

				//check for chained terms
				abNot[i] = new bool[asTerms[i].Length];
				for (int x = 0; x < asTerms[i].Length; x++) {
					asTerms[i][x] = asTerms[i][x].Replace('_', ' ');
					abNot[i][x] = asTerms[i][x].StartsWith("-");
					if(abNot[i][x]) asTerms[i][x] = asTerms[i][x].Substring(1);
				}
			}
			#endregion

			#region Convert to SQL

			#region Data setup
			sbCmd.Append(vsManga);
			#endregion

			#region Where-clause setup
			sbCmd.AppendFormat(" where ({0} in (mgx.MangaID, -1)) "
				, iMangaID);

			for (int i = 0; i < asTerms.Length; i++) {
				for (int x = 0; x < asTerms[i].Length; x++) {
					switch (asType[i]) {
						case "artist":
						case "a":
							sbCmd.AppendFormat("and at.Name {0} like '%{1}%' "
								, abNot[i][x] ? "not" : ""
								, Cleanse(asTerms[i][x]));
							break;
						case "title":
						case "t":
							sbCmd.AppendFormat("and mgx.Title {0} like '%{1}%' "
								, abNot[i][x] ? "not" : ""
								, Cleanse(asTerms[i][x]));
							break;
						case "tag":
						case "tags":
						case "g":
							sbCmd.AppendFormat("and tg.Tags {0} like '%{1}%' "
								, abNot[i][x] ? "not" : ""
								, Cleanse(asTerms[i][x]));
							break;
						case "description":
						case "desc":
						case "s":
							sbCmd.AppendFormat("and mgx.Description {0} like '%{1}%' "
								, abNot[i][x] ? "not" : ""
								, Cleanse(asTerms[i][x]));
							break;
						case "type":
						case "y":
							sbCmd.AppendFormat("and tp.Type {0} like '%{1}%' "
								, abNot[i][x] ? "not" : ""
								, Cleanse(asTerms[i][x]));
							break;
						case "date":
						case "d":

							DateTime date = new DateTime();
							char c = !string.IsNullOrEmpty(asTerms[i][x]) ? asTerms[i][x][0] : ' ';

							if (DateTime.TryParse(asTerms[i][x].Substring(c != '<' && c != '>' ? 0 : 1), out date))
								sbCmd.AppendFormat("and date(mgx.PublishedDate) {0} date('{1}') "
									, abNot[i][x] ? '!' : (c == '<' || c == '>') ? c : '='
									, date.ToString("yyyy-MM-dd"));
							break;
						case "rating":
						case "r":
							c = !string.IsNullOrEmpty(asTerms[i][x]) ? asTerms[i][x][0] : ' ';
							int rat;

							if (int.TryParse(asTerms[i][x].Substring(c != '<' && c != '>' ? 0 : 1), out rat))
								sbCmd.AppendFormat("and mgx.Rating {0} {1} "
									, abNot[i][x] ? '!' : (c == '<' || c == '>') ? c : '='
									, rat);
							break;
						case "pages":
						case "page":
						case "p":
							c = !string.IsNullOrEmpty(asTerms[i][x]) ? asTerms[i][x][0] : ' ';
							int pg;

							if (int.TryParse(asTerms[i][x].Substring(c != '<' && c != '>' ? 0 : 1), out pg))
								sbCmd.AppendFormat("and mgx.Pages {0} {1} "
									, abNot[i][x] ? '!' : (c == '<' || c == '>') ? c : '='
									, pg);
							break;
						default:
							if (abNot[i][x]) {
								sbCmd.AppendFormat("and (tg.Tags not like '%{0}%' and mgx.Title not like '%{0}%' and at.Name not like '%{0}%' and mgx.Description not like '%{0}%' and tp.Type not like '%{0}%' and date(mgx.PublishedDate) not like '%{0}%') "
								, Cleanse(asTerms[i][x]));
							}
							else {
								sbCmd.AppendFormat("and (tg.Tags like '%{0}%' or mgx.Title like '%{0}%' or at.Name like '%{0}%' or mgx.Description like '%{0}%' or tp.Type like '%{0}%' or date(mgx.PublishedDate) like '%{0}%') "
								, Cleanse(asTerms[i][x]));
							}
							
							break;
					}
				}
			}

			//append final syntax
			sbCmd.Append(vsMangaEnd);

			#endregion

			#endregion

			return ExecuteQuery(sbCmd.ToString(), CommandBehavior.Default);
		}
		#endregion

		#region Query Database

		private static DataTable GetEntries()
		{
			string sCommandText = vsManga + vsMangaEnd;

			return ExecuteQuery(sCommandText);
		}

		private static DataTable DB_GetEntryDetails(int mangaID)
		{
			string sCommandText = vsManga 
				+ " where mgx.MangaID = @mangaID"
				+ vsMangaEnd;

			return ExecuteQuery(sCommandText, CommandBehavior.SingleRow
				, new SQLiteParameter("@mangaID", DbType.Int32) {
					Value = mangaID
				}
			);
		}

		private static bool EntryExists(string sArtist, string sTitle)
		{
			bool bExists = false;
			string sCommandText = vsManga + @"
				where
					at.Name = @artist
				and
					mgx.Title = @title"
				+ vsMangaEnd;

			using (DataTable dt = ExecuteQuery(sCommandText, CommandBehavior.SingleRow
					, new SQLiteParameter("@artist", DbType.String) {
						Value = sArtist
					}
					, new SQLiteParameter("@title", DbType.String) {
						Value = sTitle
					})) {
				bExists = dt.Rows.Count > 0;
			}

			return bExists;
		}

		private static DataTable DB_GetSettings(string sColumn)
		{
			string sCommandText = @"
				select 
						sx.SettingsID
						,sx.RootPath
						,sx.SavePath
						,sx.SearchIgnore
						,sx.FormPosition
						,sx.ImageBrowser
						,sx.member_id
						,sx.pass_hash
						,sx.NewUser
						,sx.SendReports
						,sx.ShowGrid
						,sx.ShowDate
						,sx.ReadInterval
						,sx.RowColourHighlight
						,sx.RowColourAlt
						,sx.BackgroundColor
						,sx.GallerySettings
						,sx.CreatedDBTime
						,sx.AuditDBTime
				from
					Settings sx
			";

			return ExecuteQuery(sCommandText);
		}

		#endregion

		#region Update Database

		private static int DB_SaveEntry(string sArtist, string sTitle, string sTags, string sLoc,
				DateTime dtPubDate, int iPages, string sType, decimal dRating,
				string sDesc, string sURL = null, int iMangaID = -1)
		{
			if (!bConverting)
				BeginTransaction();

			//setup parameters
			SQLiteParameter[] sqParam = new SQLiteParameter[8];
			sqParam[0] = new SQLiteParameter("@title", DbType.String) {
				Value = sTitle
			};
			sqParam[1] = new SQLiteParameter("@mangaID", DbType.String) {
				Value = iMangaID
			};
			sqParam[2] = new SQLiteParameter("@pages", DbType.Int32) {
				Value = iPages
			};
			sqParam[3] = new SQLiteParameter("@rating", DbType.Decimal) {
				Value = dRating
			};
			sqParam[4] = new SQLiteParameter("@description", DbType.String) {
				Value = sDesc
			};
			sqParam[5] = new SQLiteParameter("@location", DbType.String) {
				Value = sLoc
			};
			sqParam[6] = new SQLiteParameter("@URL", DbType.String) {
				Value = sURL
			};
			sqParam[7] = new SQLiteParameter("@pubDate", DbType.String) {
				Value = dtPubDate.ToString("yyyy-MM-dd")
			};

			//determine whether to insert or update
			string sCommandText;
			if (iMangaID == -1) {
				sCommandText = @"
					insert into [Manga](title, pages, rating, description, location, galleryURL, publishedDate)
					values(@title,@pages,@rating,@description,@location,@URL,@pubDate)";
			}
			else {
				sCommandText = @"
					update [Manga]
					set title = @title
					,pages = @pages
					,rating = @rating
					,description = @description
					,location = @location
					,galleryURL = @URL
					,publishedDate = @pubDate
					where MangaID = @mangaID";
			}

			//run the command
			ExecuteNonQuery(sCommandText, CommandBehavior.Default, sqParam);

			//get the new mangaID if applicable
			if (iMangaID == -1) {
				using (DataTable dt = ExecuteQuery("select MAX(MangaID) from Manga", CommandBehavior.SingleRow)) {
					iMangaID = int.Parse(dt.Rows[0][0].ToString());
				}
			}

			//insert artist
			DB_UpdateArtist(iMangaID, sArtist);

			//insert tags
			DB_UpdateTag(iMangaID, sTags);

			//update type
			DB_UpdateType(iMangaID, sType);

			if (!bConverting)
				EndTransaction();

			return iMangaID;
		}

		private static void DB_UpdateArtist(int iMangaID, string sArtists)
		{
			string sCommandText;
			//string[] asArtists = ExtString.Split(sArtists, ",");
			string[] asArtists = new string[1] { sArtists };
			List<Int32> lArtistID = new List<int>(asArtists.Length);

			SQLiteParameter pmMangaID = new SQLiteParameter("@mangaID", DbType.Int32) {
				Value = iMangaID
			};

			for (int i = 0; i < asArtists.Length; i++) {
				//sql parameters
				SQLiteParameter pmName = new SQLiteParameter("@name", DbType.String) {
					Value = asArtists[i]
				};

				//add artist if it doesn't exist already
				sCommandText = @"
					insert into [Artist](Name)
					select @name
					where not exists(select 1 from [Artist] where Name = @name)";
				ExecuteNonQuery(sCommandText, CommandBehavior.Default, pmName);

				//add artistID to list
				sCommandText = @"
					select ArtistID
					from [Artist] where Name = @name";
				using (DataTable dt = ExecuteQuery(sCommandText, CommandBehavior.SingleRow, pmName)) {
					if (dt.Rows.Count > 0) {
						lArtistID.Add(int.Parse(dt.Rows[0][0].ToString()));
					}
					else {
						throw new SQLiteException("Artist was not successfully inserted");
					}
				}

				SQLiteParameter pmArtistID = new SQLiteParameter("@artistID", DbType.Int32) {
					Value = lArtistID[i]
				};

				//link artist if it isn't already
				sCommandText = @"
					insert into [MangaArtist](MangaID, ArtistID)
					select @mangaID, @artistID
					where not exists(select 1 from [MangaArtist] where MangaID = @mangaID and ArtistID = @artistID)";
				ExecuteNonQuery(sCommandText, CommandBehavior.Default, pmMangaID, pmArtistID);
			}

			//delete any invalid links
			sCommandText = @"
				delete from [MangaArtist] 
				where MangaID = @mangaID and ArtistID not in ("
				+ string.Join(",", lArtistID) + ")";
			ExecuteNonQuery(sCommandText, CommandBehavior.Default, pmMangaID);
		}

		private static void DB_UpdateTag(int iMangaID, string sTags)
		{
			string sCommandText;
			string[] asTags = ExtString.Split(sTags, ",");
			List<Int32> lTagID = new List<int>(asTags.Length);

			SQLiteParameter pmMangaID = new SQLiteParameter("@mangaID", DbType.Int32) {
				Value = iMangaID
			};

			for (int i = 0; i < asTags.Length; i++) {
				//sql parameters
				SQLiteParameter pmTag = new SQLiteParameter("@tag", DbType.String) {
					Value = asTags[i]
				};

				//add tag if it doesn't exist already
				sCommandText = @"
					insert into [Tag](Tag)
					select @tag
					where not exists(select 1 from [Tag] where Tag = @tag)";
				ExecuteNonQuery(sCommandText, CommandBehavior.Default, pmTag);

				//add tagID to list
				sCommandText = @"
					select TagID
					from [Tag]
					where Tag = @tag";
				using (DataTable dt = ExecuteQuery(sCommandText, CommandBehavior.SingleRow, pmTag)) {
					if (dt.Rows.Count > 0) {
						lTagID.Add(int.Parse(dt.Rows[0][0].ToString()));
					}
					else {
						throw new SQLiteException("Tag was not successfully inserted");
					}
				}

				SQLiteParameter pmTagID = new SQLiteParameter("@tagID", DbType.Int32) {
					Value = lTagID[i]
				};

				sCommandText = @"
					insert into [MangaTag](MangaID, TagID)
					select @mangaID, @tagID
					where not exists(select 1 from [MangaTag] where MangaID = @mangaID and TagID = @tagID)";
				ExecuteNonQuery(sCommandText, CommandBehavior.Default, pmMangaID, pmTagID);
			}

			//delete any invalid links
			sCommandText = @"
				delete from MangaTag where MangaID = @mangaID and TagID not in ("
				+ string.Join(",", lTagID) + ")";
			ExecuteNonQuery(sCommandText, CommandBehavior.Default, pmMangaID);
		}

		private static void DB_UpdateType(int iMangaID, string sType)
		{
			SQLiteParameter pmMangaID = new SQLiteParameter("@mangaID", DbType.Int32) {
				Value = iMangaID
			};

			//remove type if empty
			if (string.IsNullOrEmpty(sType)) {
				ExecuteNonQuery("update Manga set TypeID = null where MangaID = @mangaID"
					, CommandBehavior.Default, pmMangaID);
				return;
			}

			//declare variables
			SQLiteParameter pmType = new SQLiteParameter("@type", DbType.String) {
				Value = sType
			};
			string sCommandText;
			int iTypeID = 0;

			//add type if it doesn't exist already
			sCommandText = @"
				insert into [Type](Type)
				select @type
				where not exists(select 1 from [Type] where Type = @type)";
			ExecuteNonQuery(sCommandText, CommandBehavior.Default, pmType);

			//get typeID
			sCommandText = @"
				select TypeID
				from [Type]
				where Type = @type";
			using (DataTable dt = ExecuteQuery(sCommandText, CommandBehavior.SingleRow, pmType)) {
				if (dt.Rows.Count > 0) {
					iTypeID = int.Parse(dt.Rows[0][0].ToString());
				}
				else {
					throw new SQLiteException("Type not found");
				}
			}

			SQLiteParameter pmTypeID = new SQLiteParameter("@typeID", DbType.Int32) {
				Value = iTypeID
			};

			//set type
			sCommandText = @"
				update [Manga]
				set TypeID = @typeID
				where MangaID = @mangaID and (TypeID is null or TypeID != @typeID)";
			ExecuteQuery(sCommandText, CommandBehavior.Default, pmMangaID, pmTypeID);
		}

		private static int Entry_Delete(int iMangaID)
		{
			BeginTransaction();
			string sCommandText;
			int altered = 0;

			SQLiteParameter sqParam = new SQLiteParameter("@mangaID", DbType.Int32) {
				Value = iMangaID
			};

			//delete mangaArtist
			sCommandText = @"
				delete from MangaArtist
				where MangaID = @mangaID";
			altered += ExecuteNonQuery(sCommandText, CommandBehavior.Default, sqParam);

			//delete mangaTag
			sCommandText = @"
				delete from MangaTag
				where MangaID = @mangaID";
			altered += ExecuteNonQuery(sCommandText, CommandBehavior.Default, sqParam);

			//deletemanga
			sCommandText = @"
				delete from Manga
				where MangaID = @mangaID";
			altered += ExecuteNonQuery(sCommandText, CommandBehavior.Default, sqParam);

			EndTransaction();
			return altered;
		}

		#endregion

		#endregion
	}
}
