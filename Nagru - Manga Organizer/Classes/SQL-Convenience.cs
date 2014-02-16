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
	public class csSQL
	{
		#region Properties

		private static SQLiteConnection sqConn;

		#endregion

		static csSQL()
		{
			DB_Connect();
		}

		private static void DB_Connect()
		{
			DB_Close();
			string sPath = Properties.Settings.Default.SavLoc != string.Empty ?
				Properties.Settings.Default.SavLoc : Environment.CurrentDirectory;
			sPath += "\\MangaDB.sqlite";

			//check existence
			bool bExist = false;
			if (File.Exists(sPath)) bExist = true;
			else
			{
				string sRelPath = ExtString.RelativePath(sPath);
				bExist = (sRelPath != null);
				if (bExist) sPath = sRelPath;
			}

			//create connection
			sqConn = new SQLiteConnection();
			if (!bExist) SQLiteConnection.CreateFile(sPath);
			sqConn.ConnectionString = new DbConnectionStringBuilder()
			{
				{"Data Source", sPath},
				{"Version", "3"},
				{"Compress", true},
				{"New", !bExist}
			}.ConnectionString;
			sqConn.Open();

			if (!bExist)
			{
				DB_Create();
			}
		}

		#region Create Database
		private static void DB_Create()
		{
			string sQuery;

			#region Create Tables

			#region Artist
			sQuery = @"
				create table [Artist]
				(
					ArtistID	integer			primary key		autoincrement
					,Name		nvarchar(100)	not null		unique
				)";
			csSQL.ExecuteNonQuery(sQuery);
			#endregion

			#region Tag
			sQuery = @"
				create table [Tag]
				(
					TagID		integer			primary key		autoincrement
					,Tag		nvarchar(100)	not null		unique
				)";
			csSQL.ExecuteNonQuery(sQuery);
			#endregion

			#region Type
			sQuery = @"
				create table [Type]
				(
					TypeID		integer			primary key		autoincrement
					,Type		nvarchar(100)	not null		unique
				)";
			csSQL.ExecuteNonQuery(sQuery);
			#endregion

			#region MangaArtist
			sQuery = @"
				create table [MangaArtist]
				(
					MangaArtistID	integer		primary key		autoincrement
					,MangaID		int			not null
					,ArtistID		int			not null
					,constraint [fk_mangaID] foreign key ([MangaID]) references [Manga] ([MangaID])
					,constraint [fk_artistID] foreign key ([ArtistID]) references [Artist] ([ArtistID])
				)";
			csSQL.ExecuteNonQuery(sQuery);
			#endregion

			#region MangaTag
			sQuery = @"
				create table [MangaTag]
				(
					MangaTagID		integer		primary key		autoincrement
					,MangaID		int			not null
					,TagID			int			not null
					,constraint [fk_mangaID] foreign key ([MangaID]) references [Manga] ([MangaID])
					,constraint [fk_tagID] foreign key ([TagID]) references [Tag] ([TagID])
				)";
			csSQL.ExecuteNonQuery(sQuery);
			#endregion

			#region Manga
			sQuery = @"
				create table [Manga]
				(
					MangaID			integer			primary key		autoincrement
					,TypeID			int				null
					,Title			nvarchar(250)	not null		unique
					,Pages			int				not null
					,Rating			decimal(1,1)	not null
					,Description	nvarchar(4000)	null
					,Location		nvarchar(260)	null			-- MAX_PATH Windows API value
					,GalleryURL		nvarchar(40)	null
					,PublishedDate	date			null
					,CreatedDBTime	datetime		not null		default CURRENT_DATE
					,AuditDBTime	datetime		not null		default CURRENT_DATE
					,constraint [fk_typeID] foreign key ([TypeID]) references [Type] ([TypeID])
				)";
			csSQL.ExecuteNonQuery(sQuery);
			#endregion

			#endregion

			#region Populate Default Table Values
			sQuery = @"
				insert into [Type] (Type)
				values('Doujinshi'),('Manga'),('ArtistCG'),('Game CG'),('Western'), ('Non-H'),('Image Set'),('Cosplay'),('Asian Porn'),('Misc')";
			csSQL.ExecuteNonQuery(sQuery);
			#endregion
		}

		public static void Import(string sPath)
		{
			if (!File.Exists(sPath))
				return;

			List<Main.csEntry> lData = FileSerializer.Deserialize
				<List<Main.csEntry>>(sPath) ?? new List<Main.csEntry>(0);
			System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
			sw.Start(); double dAvg = 0;
			//input into new DB
			for (int i = 0; i < lData.Count; i++) {
				//ensure sizes are valid
				#region Force value sizes to be inside valid range
				if(lData[i].sArtist.Length > 100)
					lData[i].sArtist = lData[i].sArtist.Substring(0, 250);
				if (lData[i].sTitle.Length > 250)
					lData[i].sTitle = lData[i].sTitle.Substring(0, 250);
				if (lData[i].sLoc.Length > 260)
					lData[i].sLoc = "Truncated -- Length over 260 character limit";
				if (lData[i].sDesc.Length > 4000)
					lData[i].sDesc = lData[i].sDesc.Substring(0, 4000);
				#endregion

				//add the entry
				try{
				Entry_Save(
					ExtString.Split(lData[i].sArtist, ","),
					ExtString.Split(lData[i].sTags, ","), lData[i].sType,
					lData[i].sTitle, lData[i].iPages, lData[i].byRat, 
					lData[i].sDesc, lData[i].sLoc, "", lData[i].dtDate);
				}
				catch (SQLiteException exc)
				{
					Console.WriteLine(exc.Message);
				}
				sw.Stop();
				dAvg += (sw.ElapsedMilliseconds / 1000.0);
				Console.WriteLine((sw.ElapsedMilliseconds / 1000.0) + "s, -- "
					+ (double)(dAvg / i) + " average\n\n");
				sw.Restart();
			}

			//deprecate old serialized DB
			File.Move(sPath + "\\MangaDatabase.bin", sPath + "\\MangaDatabase_Deprecated.bin");
		}
		#endregion

		#region Access Database
		private static int ExecuteNonQuery(string sCommand)
		{
			int iRetVal = 0;
			try {
				using (SQLiteCommand sqCmd = sqConn.CreateCommand()) {
					sqCmd.CommandType = CommandType.Text;
					sqCmd.CommandText = sCommand;
					sqCmd.ExecuteNonQuery();
				}
			} catch(SQLiteException exc){
				Console.WriteLine(exc.Message);
				iRetVal = -1;
			}
			return iRetVal;
		}

		public static int GetMangaID(string sTitle)
		{
			int iRetVal = -1;
			using (SQLiteCommand sqCmd = sqConn.CreateCommand()) {
				//get mangaID
				sqCmd.CommandType = CommandType.Text;
				sqCmd.Parameters.Add(new SQLiteParameter("@title", DbType.String) { Value = sTitle });
				sqCmd.CommandText = @"
					select MangaID
					from [Manga]
					where title = @title";
				sqCmd.ExecuteNonQuery();

				using (SQLiteDataReader dr = sqCmd.ExecuteReader(CommandBehavior.SingleResult))
				{
					dr.Read();
					if (dr.HasRows) iRetVal = dr.GetInt32(0);
					dr.Close();
				}
			}
			return iRetVal;
		}

		public static string[] Entry_Summary(int iMangaID)
		{
			List<string> lDetails = new List<string>(10);

			using (SQLiteCommand sqCmd = sqConn.CreateCommand())
			{
				sqCmd.Parameters.Add(new SQLiteParameter("MangaID", DbType.Int32) { Value = iMangaID });
				sqCmd.CommandType = CommandType.Text;
				sqCmd.CommandText = @"";

				lDetails.Add("");
				using (SQLiteDataReader dr = sqCmd.ExecuteReader())
				{
				}
			}
			return new string[0];
		}
		#endregion

		#region Update Database
		public static void Entry_Save(string[] asArtist, string[] asTag, string sType,
			string sTitle, int iPages, decimal dRating, string sDesc, string sLoc,
			string sURL, DateTime dtPubDate)
		{
			System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
			sw.Start();
			
			//ensure key value exists
			if (string.IsNullOrEmpty(sTitle))
				throw new NoNullAllowedException();

			//insert manga record
			int iMangaID = -1;
			using (SQLiteCommand sqCmd = sqConn.CreateCommand())
			{
				//get mangaID
				iMangaID = GetMangaID(sTitle);

				//insert or update record
				sqCmd.Parameters.Add(new SQLiteParameter("@title", DbType.String) { Value = sTitle });
				sqCmd.Parameters.Add(new SQLiteParameter("@mangaID", DbType.String) { Value = iMangaID });
				sqCmd.Parameters.Add(new SQLiteParameter("@pages", DbType.Int32) { Value = iPages });
				sqCmd.Parameters.Add(new SQLiteParameter("@rating", DbType.Decimal) { Value = dRating });
				sqCmd.Parameters.Add(new SQLiteParameter("@description", DbType.String) { Value = sDesc });
				sqCmd.Parameters.Add(new SQLiteParameter("@location", DbType.String) { Value = sLoc });
				sqCmd.Parameters.Add(new SQLiteParameter("@URL", DbType.String) { Value = sURL });
				sqCmd.Parameters.Add(new SQLiteParameter("@pubDate", DbType.Date) { Value = dtPubDate });

				if(iMangaID == -1) {
					sqCmd.CommandText = @"
					insert into [Manga](title, pages, rating, description, location, galleryURL, publishedDate)
					values(@title,@pages,@rating,@description,@location,@URL,@pubDate)";
				}
				else {
					sqCmd.CommandText = @"
					update [Manga]
					set title = @title
					,pages = @pages
					,rating = @rating
					,description = @description
					,location = @location
					,galleryURL = @URL
					,publishedDate = @pubDate
					,AuditDBTime = CURRENT_DATE
					where MangaID = @mangaID";
				}
				sqCmd.ExecuteNonQuery();

				//if the mangaID was not found previously, find it now
				if (iMangaID == -1) iMangaID = GetMangaID(sTitle);
				if (iMangaID == -1) throw new SQLiteException("Manga was not succesfully inserted or updated.");
			}
			sw.Stop();
			Console.WriteLine("Manga: " + sw.ElapsedMilliseconds + "ms");

			//insert artist
			DB_UpdateArtist(iMangaID, asArtist);

			//insert tags
			DB_UpdateTag(iMangaID, asTag);

			//insert type
			DB_UpdateType(iMangaID, sType);
		}

		private static void DB_UpdateArtist(int iMangaID, string[] asArtists)
		{
			System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
			sw.Start();
			
			List<Int32> lArtistID = new List<int>(asArtists.Length);

			for (int i = 0; i < asArtists.Length; i++)
			{
				if (string.IsNullOrEmpty(asArtists[i].ToString())) continue;

				//add artist if it doesn't exist already
				using (SQLiteCommand sqCmd = sqConn.CreateCommand())
				{
					sqCmd.Parameters.Add(new SQLiteParameter("@name", DbType.String) { Value = asArtists[i] });
					sqCmd.CommandType = CommandType.Text;
					sqCmd.CommandText = @"
					insert into [Artist](Name)
					select @name
					where not exists(select 1 from [Artist] where Name = @name)";
					sqCmd.ExecuteNonQuery();

					//add artistID to list
					sqCmd.CommandText = @"
					select ArtistID
					from [Artist] where Name = @name";
					sqCmd.ExecuteNonQuery();

					using (SQLiteDataReader dr = sqCmd.ExecuteReader(CommandBehavior.SingleResult))
					{
						dr.Read();
						if (dr.HasRows) lArtistID.Add(dr.GetInt32(0));
						else throw new SQLiteException("Artist was not succesfully inserted or updated.");
						dr.Close();
					}
				}

				//link artist if it isn't already
				using (SQLiteCommand sqCmd = sqConn.CreateCommand())
				{
					sqCmd.Parameters.Add(new SQLiteParameter("@artistID", DbType.Int32) { Value = lArtistID[i] });
					sqCmd.Parameters.Add(new SQLiteParameter("@mangaID", DbType.Int32) { Value = iMangaID });
					sqCmd.CommandType = CommandType.Text;
					sqCmd.CommandText = @"
					insert into [MangaArtist](MangaID, ArtistID)
					select @mangaID, @artistID
					where not exists(select 1 from [MangaArtist] where MangaID = @mangaID and ArtistID = @artistID)";
					sqCmd.ExecuteNonQuery();
				}
			}

			//delete any invalid links
			using (SQLiteCommand sqCmd = sqConn.CreateCommand())
			{
				sqCmd.Parameters.Add(new SQLiteParameter("@mangaID", DbType.Int32) { Value = iMangaID });
				sqCmd.CommandType = CommandType.Text;
				sqCmd.CommandText = @"
				delete from [MangaArtist] where MangaID = @mangaID and ArtistID not in (";
				sqCmd.CommandText += string.Join(",", lArtistID) + ")";
				sqCmd.ExecuteNonQuery();
			}
			sw.Stop();
			Console.WriteLine("Artists (" + asArtists.Length + "): " + sw.ElapsedMilliseconds + "ms");
		}

		private static void DB_UpdateTag(int iMangaID, string[] asTags)
		{
			System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
			sw.Start();
			List<Int32> lTagID = new List<int>(asTags.Length);

			for (int i = 0; i < asTags.Length; i++)
			{
				if (string.IsNullOrEmpty(asTags[i].ToString())) continue;

				//add tag if it doesn't exist already
				using (SQLiteCommand sqCmd = sqConn.CreateCommand())
				{
					sqCmd.Parameters.Add(new SQLiteParameter("@tag", DbType.String) { Value =  asTags[i].Trim() });
					sqCmd.CommandType = CommandType.Text;
					sqCmd.CommandText = @"
					insert into [Tag](Tag)
					select @tag
					where not exists(select 1 from [Tag] where Tag = @tag)";
					sqCmd.ExecuteNonQuery();
					
					//add tagID to list
					sqCmd.CommandText = @"
					select TagID
					from [Tag]
					where Tag = @tag";
					sqCmd.ExecuteNonQuery();

					using(SQLiteDataReader dr = sqCmd.ExecuteReader(CommandBehavior.SingleResult))
					{
						dr.Read();
						if (dr.HasRows) lTagID.Add(dr.GetInt32(0));
						else throw new SQLiteException("Tag was not succesfully inserted or updated.");
						dr.Close();
					}
					
					//link tag if it isn't already
					sqCmd.Parameters.Add(new SQLiteParameter("@mangaID", DbType.Int32) { Value = iMangaID });
					sqCmd.Parameters.Add(new SQLiteParameter("@tagID", DbType.Int32) { Value = lTagID[i] });
					sqCmd.CommandType = CommandType.Text;
					sqCmd.CommandText = @"
					insert into [MangaTag](MangaID, TagID)
					select @mangaID, @tagID
					where not exists(select 1 from [MangaTag] where MangaID = @mangaID and TagID = @tagID)";
					sqCmd.ExecuteNonQuery();
				}
			}
			
			//delete any invalid links
			using (SQLiteCommand sqCmd = sqConn.CreateCommand())
			{
				sqCmd.Parameters.Add(new SQLiteParameter("@mangaID", DbType.Int32) { Value = iMangaID });
				sqCmd.CommandType = CommandType.Text;
				sqCmd.CommandText = @"
				delete from MangaTag where MangaID = @mangaID and TagID not in (";
				sqCmd.CommandText += string.Join(",", lTagID) + ")";
				sqCmd.ExecuteNonQuery();
			}
			sw.Stop();
			Console.WriteLine("Tags (" + asTags.Length + "): " + sw.ElapsedMilliseconds + "ms");
		}

		private static void DB_UpdateType(int iMangaID, string sType)
		{
			System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
			sw.Start();
			
			int iTypeID = 0;

			//add type if it doesn't exist already
			if (!string.IsNullOrEmpty(sType))
			{
				using (SQLiteCommand sqCmd = sqConn.CreateCommand())
				{
					sqCmd.Parameters.Add(new SQLiteParameter("@type", DbType.String) { Value = sType });
					sqCmd.CommandType = CommandType.Text;
					sqCmd.CommandText = @"
					insert into [Type](Type)
					select @type
					where not exists(select 1 from [Type] where Type = @type)";
					sqCmd.ExecuteNonQuery();

					//get typeID
					sqCmd.CommandText = @"
					select TypeID
					from [Type]
					where Type = @type";
					sqCmd.ExecuteNonQuery();

					using (SQLiteDataReader dr = sqCmd.ExecuteReader(CommandBehavior.SingleResult))
					{
						dr.Read();
						if (dr.HasRows) iTypeID = dr.GetInt32(0);
						else throw new SQLiteException("Type was not succesfully inserted or updated.");
						dr.Close();
					}
				}

				//set type
				using (SQLiteCommand sqCmd = sqConn.CreateCommand())
				{
					sqCmd.Parameters.Add(new SQLiteParameter("@typeID", DbType.Int32) { Value = iTypeID });
					sqCmd.Parameters.Add(new SQLiteParameter("@mangaID", DbType.Int32) { Value = iMangaID });
					sqCmd.CommandType = CommandType.Text;
					sqCmd.CommandText = @"
					update [Manga]
					set TypeID = @typeID, auditDBTime = CURRENT_DATE
					where MangaID = @mangaID and TypeID != @typeID";
					sqCmd.ExecuteNonQuery();
				}
			}
			sw.Stop();
			Console.WriteLine("Type: " + sw.ElapsedMilliseconds + "ms");
		}
		
		public static void Entry_Delete(int iMangaID)
		{
		}
		#endregion

		private static void DB_Close()
		{
			if (sqConn != null
					&& sqConn.State != ConnectionState.Closed) {
				sqConn.Close();
			}
		}

		~csSQL() {
			DB_Close();
			sqConn.Dispose();
		}
	}
}
