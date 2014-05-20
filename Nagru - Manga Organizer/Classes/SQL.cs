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

namespace Nagru___Manga_Organizer
{
	/* SQL Conveniences */
	public static class csSQLCon
	{
		#region Properties
		public static bool IsOpen()
		{
			return (sqConn.State == ConnectionState.Open);
		}
		#endregion

		#region Variables

		private static SQLiteConnection sqConn;
		private const int SQLITE_MAX_LENGTH = 1000000;
		private static bool bConverting = false;

		#endregion

		#region Class Functions

		static csSQLCon()
		{
			DB_Connect();
		}

		#endregion

		#region Public Methods

		public static bool Import(string _filePath)
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
			}
			EndTransaction();
			lData.Clear();
			bConverting = false;
			
			//deprecate old serialized DB
			File.Move(_filePath, _filePath + "_Deprecated");
			return true;
		}

		public static DataTable Search(string sTerms, int iMangaID = -1)
		{
			if (string.IsNullOrWhiteSpace(sTerms)) {
				return Entries();
			}

			//Set up variables
			StringBuilder sbCmd = new StringBuilder(10000);
			List<string[]> lTerms;
			List<bool[]> lNot;
			string[] asType;

			#region Parse Terms
			string[] asItems = ExtString.Split(sTerms, " ");
			lTerms = new List<string[]>(asItems.Length);
			lNot = new List<bool[]>(asItems.Length);
			asType = new string[asItems.Length];

			for (int i = 0; i < asItems.Length; i++) {
				//check for type limiter
				string[] sSplit = ExtString.Split(asItems[i].Trim(), ":");
				if (sSplit.Length == 2) {
					asType[i] = sSplit[0];
					lTerms.Add(ExtString.Split(sSplit[1], "&", ","));
				}
				else {
					lTerms.Add(ExtString.Split(sSplit[0], "&", ","));
				}

				//check for chained terms
				lNot.Add(new bool[lTerms[i].Length]);
				for (int x = 0; x < lTerms[i].Length; x++) {
					lTerms[i][x] = lTerms[i][x].Replace('_', ' ');
					if (lTerms[i][x].StartsWith("-")) {
						lTerms[i][x] = lTerms[i][x].Substring(1);
						lNot[i][x] = true;
					}
					else {
						lNot[i][x] = false;
					}
				}
			}
			#endregion

			#region Convert to SQL

			#region Data setup
			sbCmd.Append(@"
				select
					mgx.MangaID
					,ifnull(at.Name, '')    Artist
					,mgx.Title
					,mgx.Pages
					,group_concat(tg.Tag)		Tags
					,mgx.PublishedDate
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
					[MangaTag] mgt on mgt.MangaID = mgx.MangaID
				left outer join
					[Tag] tg on tg.TagID = mgt.TagID 
			");
			#endregion

			#region Where-clause setup
			sbCmd.AppendFormat("where ({0} in (mgx.MangaID, -1)) "
				,iMangaID);

			for (int i = 0; i < lTerms.Count; i++) {
				for (int x = 0; x < lTerms[i].Length; x++) {
					switch (asType[i]) {
						case "artist":
						case "a":
							sbCmd.AppendFormat("and at.Name like '%{0}%' ", Cleanse(lTerms[i][x]));
							break;
						case "title":
						case "t":
							sbCmd.AppendFormat("and mgx.Title like '%{0}%' ", Cleanse(lTerms[i][x]));
							break;
						case "tag":
						case "tags":
						case "g":
							sbCmd.AppendFormat("and tg.Tag like '%{0}%' ", Cleanse(lTerms[i][x]));
							break;
						case "description":
						case "desc":
						case "s":
							sbCmd.AppendFormat("and mgx.Description like '%{0}%' ", Cleanse(lTerms[i][x]));
							break;
						case "type":
						case "y":
							sbCmd.AppendFormat("and tp.Type like '%{0}%' ", Cleanse(lTerms[i][x]));
							break;
						case "date":
						case "d":

							DateTime date = new DateTime();
							char c = !string.IsNullOrEmpty(lTerms[i][x]) ? lTerms[i][x][0] : ' ';
							
							if (DateTime.TryParse(lTerms[i][x].Substring(1), out date))
								sbCmd.AppendFormat("and date(mgx.PublishedDate) {0}= date({1}) "
									, (c == '<' || c == '>') ? c : ' '
									, Cleanse(lTerms[i][x]));
							break;
						case "pages":
						case "page":
						case "p":
							c = !string.IsNullOrEmpty(lTerms[i][x]) ? lTerms[i][x][0] : ' ';
							int pg;
							
							if (int.TryParse(lTerms[i][x].Substring(1), out pg))
								sbCmd.AppendFormat("and mgx.Pages {0}= {1} "
									, (c == '<' || c == '>') ? c : ' '
									, Cleanse(lTerms[i][x]));
							break;
						default:
							sbCmd.AppendFormat("and (tg.Tag like '%{0}%' or mgx.Title like '%{0}%' or at.Name like '%{0}%' or mgx.Description like '%{0}%' or tp.Type like '%{0}%' or date(mgx.PublishedDate) like '%{0}%') "
								, Cleanse(lTerms[i][x]));
							break;
					}
				}
			}
			#endregion

			//append final syntax
			sbCmd.Append(@"
				group by mgx.MangaID
				order by at.Name asc");

			#endregion

			return ExecuteQuery(sbCmd.ToString(), CommandBehavior.Default);
		}

		public static DataTable GetArtists()
		{
			string sCommandText = @"
				select
						at.ArtistID
					,ifnull(at.Name, '')				Artist
				from
					[Artist] at
				order by at.Name asc
			";

			return ExecuteQuery(sCommandText);
		}

		public static DataTable GetTags()
		{
			string sCommandText = @"
				select
						tg.TagID
					,tg.Tag
				from
					[Tag] tg
				order by tg.Tag asc
			";

			return ExecuteQuery(sCommandText);
		}

		public static DataTable GetTypes()
		{
			string sCommandText = @"
				select
						tp.TypeID
					,tp.Type
				from
					[Type] tp
				order by tp.Type asc
			";

			return ExecuteQuery(sCommandText);
		}

		public static DataTable Entries()
		{
			string sCommandText = @"
				select
						mgx.MangaID
					,ifnull(at.Name, '')                    Artist
					,mgx.Title
					,mgx.Pages
					,group_concat(tg.Tag)		Tags
					,mgx.PublishedDate
					,typ.Type
					,mgx.Rating
				from
					[Manga] mgx
				left outer join
					[Type] typ on typ.TypeID = mgx.TypeID
				left outer join
					[MangaArtist] mga on mga.MangaID = mgx.MangaID
				left outer join
					[Artist] at on at.ArtistID = mga.ArtistID
				left outer join
					[MangaTag] mgt on mgt.MangaID = mgx.MangaID
				left outer join
					[Tag] tg on tg.TagID = mgt.TagID
				group by mgx.MangaID
				order by at.Name asc
			";

			return ExecuteQuery(sCommandText);
		}

		public static bool EntryExists(string sArtist, string sTitle)
		{
			string sCommandText = @"
				select
						mgx.MangaID
				from
					[Manga] mgx
				left outer join
					[MangaArtist] mga on mga.MangaID = mgx.MangaID
				left outer join
					[Artist] at on at.ArtistID = mga.ArtistID
				where
					at.Artist = @artist
				and
					mgx.Title like '%' + @title + '%'
				group by mgx.MangaID
				order by at.Name asc
			";

			DataTable dt = ExecuteQuery(sCommandText, CommandBehavior.SingleRow
				, new SQLiteParameter("@artist", DbType.Int32) {
					Value = sArtist
				}
				, new SQLiteParameter("@title", DbType.Int32) {
					Value = sTitle
				});
			bool bExists = dt.Rows.Count > 0;
			
			dt.Clear();
			dt.Dispose();

			return bExists;
		}

		public static DataTable DB_GetEntryDetails(int mangaID)
		{
			string sCommandText = @"
				select
						mgx.MangaID
					,ifnull(at.Name, '')		Artist
					,mgx.Title
					,mgx.Location
					,mgx.Pages
					,group_concat(tg.Tag)		Tags
					,mgx.PublishedDate
					,typ.Type
					,mgx.Rating
					,mgx.Description
				from
					[Manga] mgx
				left outer join
					[Type] typ on typ.TypeID = mgx.TypeID
				left outer join
					[MangaArtist] mga on mga.MangaID = mgx.MangaID
				left outer join
					[Artist] at on at.ArtistID = mga.ArtistID
				left outer join
					[MangaTag] mgt on mgt.MangaID = mgx.MangaID
				left outer join
					[Tag] tg on tg.TagID = mgt.TagID
				where mgx.MangaID = @mangaID
				group by mgx.MangaID
				order by at.Name asc
			";

			return ExecuteQuery(sCommandText, CommandBehavior.SingleRow
				, new SQLiteParameter("@mangaID", DbType.Int32) {
					Value = mangaID
				}
			);
		}

		public static int DB_SaveEntry(string sArtist, string sTitle, string sTags, string sLoc,
				DateTime dtPubDate, int iPages, string sType, decimal dRating,
				string sDesc, string sURL = null, int iMangaID = -1)
		{
			if(!bConverting)
				BeginTransaction();

			//setup parameters
			SQLiteParameter[] sqParam = new SQLiteParameter[8];
			sqParam[0] = new SQLiteParameter("@title", DbType.String) { Value = sTitle };
			sqParam[1] = new SQLiteParameter("@mangaID", DbType.String) { Value = iMangaID };
			sqParam[2] = new SQLiteParameter("@pages", DbType.Int32) { Value = iPages };
			sqParam[3] = new SQLiteParameter("@rating", DbType.Decimal) { Value = dRating };
			sqParam[4] = new SQLiteParameter("@description", DbType.String) { Value = sDesc };
			sqParam[5] = new SQLiteParameter("@location", DbType.String) { Value = sLoc };
			sqParam[6] = new SQLiteParameter("@URL", DbType.String) { Value = sURL };
			sqParam[7] = new SQLiteParameter("@pubDate", DbType.Date) { Value = dtPubDate };

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
			if(iMangaID == -1) {
				DataTable dt = ExecuteQuery("select MAX(MangaID) from Manga", CommandBehavior.SingleRow);

				if(dt.Rows.Count > 0) {
					iMangaID = int.Parse(dt.Rows[0][0].ToString());
				}
				if(iMangaID == -1)
					return -1;

				dt.Clear();
				dt.Dispose();
			}

			//insert artist
			DB_UpdateArtist(iMangaID, sArtist);

			//insert tags
			DB_UpdateTag(iMangaID, sTags);

			//update type
			DB_UpdateType(iMangaID, sType);

			if(!bConverting)
				EndTransaction();

			return iMangaID;
		}

		public static int Entry_Delete(int iMangaID)
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
			sCommandText =@"
				delete from Manga
				where MangaID = @mangaID";
			altered += ExecuteNonQuery(sCommandText, CommandBehavior.Default, sqParam);

			EndTransaction();
			return altered;
		}

		#endregion

		#region Private Methods

		#region Handle DB Connection
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
			//else
			//{
			//    string sRelPath = ExtString.RelativePath(sPath);
			//    bExist = (sRelPath != null);
			//    if (bExist) sPath = sRelPath;
			//}

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
		#endregion

		#region Create Database
		private static void DB_Create()
		{
			string sQuery;

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
			csSQLCon.ExecuteNonQuery(sQuery);
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
			csSQLCon.ExecuteNonQuery(sQuery);
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
			csSQLCon.ExecuteNonQuery(sQuery);
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
			csSQLCon.ExecuteNonQuery(sQuery);
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
			csSQLCon.ExecuteNonQuery(sQuery);
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
			csSQLCon.ExecuteNonQuery(sQuery);
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
			csSQLCon.ExecuteNonQuery(sQuery);
			#endregion
			
			#region Tag
			sQuery = @"
				create trigger trTag after update on Tag
				begin
					update Tag set AuditDBTime = CURRENT_TIMESTAMP where tagID = new.rowid;
				end;
			";
			csSQLCon.ExecuteNonQuery(sQuery);
			#endregion
			
			#region Type
			sQuery = @"
				create trigger trType after update on Type
				begin
					update Type set AuditDBTime = CURRENT_TIMESTAMP where typeID = new.rowid;
				end;
			";
			csSQLCon.ExecuteNonQuery(sQuery);
			#endregion
			
			#region MangaArtist
			sQuery = @"
				create trigger trMangaArtist after update on MangaArtist
				begin
					update MangaArtist set AuditDBTime = CURRENT_TIMESTAMP where mangaArtistID = new.rowid;
				end;
			";
			csSQLCon.ExecuteNonQuery(sQuery);
			#endregion
			
			#region MangaTag
			sQuery = @"
				create trigger trMangaTag after update on MangaTag
				begin
					update MangaTag set AuditDBTime = CURRENT_TIMESTAMP where mangaTagID = new.rowid;
				end;
			";
			csSQLCon.ExecuteNonQuery(sQuery);
			#endregion
			
			#region Manga
			sQuery = @"
				create trigger trManga after update on Manga
				begin
					update Manga set AuditDBTime = CURRENT_TIMESTAMP where mangaID = new.rowid;
				end;
			";
			csSQLCon.ExecuteNonQuery(sQuery);
			#endregion
			
			#endregion
			
			#region Populate Default Table Values
			sQuery = @"
				insert into [Type] (Type)
				values('Doujinshi'),('Manga'),('ArtistCG'),('Game CG'),('Western'),('Non-H'),('Image Set'),('Cosplay'),('Asian Porn'),('Misc')";
			csSQLCon.ExecuteNonQuery(sQuery);
			#endregion
		}
		#endregion

		#region Access Database
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
		
		#region Update Database

		private static void DB_UpdateArtist(int iMangaID, string sArtists)
		{
			string sCommandText;
			string[] asArtists = ExtString.Split(sArtists, ",");
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
				DataTable dt = ExecuteQuery(sCommandText, CommandBehavior.SingleRow, pmName);
				if (dt.Rows.Count > 0) {
					lArtistID.Add(int.Parse(dt.Rows[0][0].ToString()));
				}
				else
					throw new SQLiteException("Artist not found");
				dt.Clear();
				dt.Dispose();
				
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
				DataTable dt = ExecuteQuery(sCommandText, CommandBehavior.SingleRow, pmTag);
				if (dt.Rows.Count > 0) {
					lTagID.Add(int.Parse(dt.Rows[0][0].ToString()));
				}
				else
					throw new SQLiteException("Tag not found");
				dt.Clear();
				dt.Dispose();

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
			DataTable dt = ExecuteQuery(sCommandText, CommandBehavior.SingleRow, pmType);
			if(dt.Rows.Count > 0) {
				iTypeID = int.Parse(dt.Rows[0][0].ToString());
			}
			else
				throw new SQLiteException("Type not found");
			dt.Clear();
			dt.Dispose();

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

		#endregion

		#endregion
	}
}
