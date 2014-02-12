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
			/*try {
				DB_Connect();
			} catch (Exception exc) {
				Console.WriteLine(exc.Message);
			}*/
		}
		
		public static void Import(string sPath)
		{
			if (!File.Exists(sPath))
				return;

			List<Main.csEntry> lData = FileSerializer.Deserialize
				<List<Main.csEntry>>(sPath) ?? new List<Main.csEntry>(0);

			//input into new DB
			for (int i = 0; i < lData.Count; i++) {
				//ensure sizes are valid
				#region Force value sizes to be inside valid range
				if (lData[i].sTitle.Length > 250)
					lData[i].sTitle = lData[i].sTitle.Substring(0, 250);
				if (lData[i].sLoc.Length > 500)
					lData[i].sLoc = "";
				if (lData[i].sDesc.Length > 4000)
					lData[i].sDesc = lData[i].sDesc.Substring(0, 4000);
				#endregion

				//add the entry
				Entry_Save(
					ExtString.Split(lData[i].sArtist, ","),
					ExtString.Split(lData[i].sTags, ","), lData[i].sType,
					lData[i].sTitle, lData[i].iPages, lData[i].byRat, 
					lData[i].sDesc, lData[i].sLoc, "", lData[i].dtDate);
			}

			//deprecate old serialized DB
			File.Move(sPath + "\\MangaDatabase.bin", sPath + "\\MangaDatabase_Deprecated.bin");
		}

		private static void DB_Create()
		{
			#region DB Creation Script
			string sQuery = @"
begin transaction

-- Create Schema
create database MangaOrganizer
go

use MangaOrganizer
go


-- Create Table: Artist
-------------------------------------------
create table Artist
(
	ArtistID	int				not null	identity(0,1)
	,Name		nvarchar(100)	not null
)
on [primary]
go

alter table Artist add constraint
pk_artist primary key clustered (ArtistID)
with(statistics_norecompute = off, ignore_dup_key = off, allow_row_locks = on, allow_page_locks = on) on [primary]


-- Create Table: Tag
-------------------------------------------
create table Tag
(
	TagID		int				not null	identity(0,1)
	,Tag		nvarchar(100)	not null
)
on [primary]
go

alter table Tag add constraint
pk_tag primary key clustered (TagID)
with(statistics_norecompute = off, ignore_dup_key = off, allow_row_locks = on, allow_page_locks = on) on [primary]


-- Create Table: Type
create table Type
(
	TypeID		int				not null	identity(0,1)
	,Type		int				not null
)
on [primary]
go

alter table Type add constraint
pk_type primary key clustered (TypeID)
with(statistics_norecompute = off, ignore_dup_key = off, allow_row_locks = on, allow_page_locks = on) on [primary]


-- Create Table: MangaArtist
-------------------------------------------
create table MangaArtist
(
	MangaArtistID	int			not null	identity(0,1)
	,MangaID		int			not null
	,ArtistID		int			not null
)
on [primary]
go

alter table MangaArtist add constraint
pk_mangaArtist primary key clustered (MangaArtistID)
with(statistics_norecompute = off, ignore_dup_key = off, allow_row_locks = on, allow_page_locks = on) on [primary]


-- Create Table: MangaTag
-------------------------------------------
create table MangaTag
(
	MangaTagID		int			not null	identity(0,1)
	,MangaID		int			not null
	,TagID			int			not null
)
on [primary]
go

alter table MangaTag add constraint
pk_mangaTag primary key clustered (MangaTagID)
with(statistics_norecompute = off, ignore_dup_key = off, allow_row_locks = on, allow_page_locks = on) on [primary]


-- Create Table: Manga
-------------------------------------------
create table Manga
(
	MangaID			int				not null	identity(0,1)
	,TypeID			int				null
	,Title			nvarchar(250)	not null
	,Pages			int				not null
	,Rating			decimal(1,1)	not null
	,Description	nvarchar(4000)	null
	,Location		nvarchar(500)	null
	,GalleryURL		nvarchar(40)	null
	,PublishedDate	date			null
	,CreatedDBTime	datetime		not null
	,AuditDBTime	datetime		not null
)
on [primary]
go

alter table Manga add constraint
pk_manga primary key clustered (MangaID)
with(statistics_norecompute = off, ignore_dup_key = off, allow_row_locks = on, allow_page_locks = on) on [primary]


-- Create Foreign Key: mangaArtist.mangaID -> manga.mangaID
alter table [MangaArtist] add constraint [fk_mangaID]
foreign_key ([MangaID])
references [Manga] ([MangaID])
on update no action
on delete no action

-- Create Foreign Key: mangaArtist.artistID -> artist.artistID
alter table [MangaArtist] add constraint [fk_artistID]
foreign_key ([ArtistID])
references [Artist] ([ArtistID])
on update no action
on delete no action

-- Create Foreign Key: mangaTag.mangaID -> manga.mangaID
alter table [MangaTag] add constraint [fk_mangaID]
foreign_key ([MangaID])
references [Manga] ([MangaID])
on update no action
on delete no action

-- Create Foreign Key: mangaTag.tagID -> tag.tagID
alter table [MangaTag] add constraint [fk_tagID]
foreign_key ([TagID])
references [Tag] ([TagID])
on update no action
on delete no action

-- Create Foreign Key: manga.typeID -> type.typeID
alter table [Manga] add constraint [fk_typeID]
foreign_key ([TypeID])
references [Type] ([TypeID])
on update no action
on delete no action

commit transaction";
			#endregion

			//create DB schema
			using (SQLiteCommand sqCmd = sqConn.CreateCommand())
			{
				sqCmd.CommandType = CommandType.Text;
				sqCmd.CommandText = sQuery;
				sqCmd.ExecuteNonQuery();
			}

			//insert default types
			using (SQLiteCommand sqCmd = sqConn.CreateCommand())
			{
				sqCmd.CommandType = CommandType.Text;
				sqCmd.CommandText = @"
				insert into dbo.Type(Type)
				values('Doujinshi'),('Manga'),('ArtistCG'),('Game CG'),('Western'),
				('Non-H'),('Image Set'),('Cosplay'),('Asian Porn'),('Misc')";
				sqCmd.ExecuteNonQuery();
			}
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
			else {
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

			if(!bExist) {
				DB_Create();
			}
		}

		private static void DB_AddTag(int iMangaID, string[] asTags)
		{
			List<Int32> lTagID = new List<int>(asTags.Length);

			for (int i = 0; i < asTags.Length; i++) {
				if (string.IsNullOrEmpty(asTags[i].ToString())) continue;

				//add tag if it doesn't exist already
				using (SQLiteCommand sqCmd = sqConn.CreateCommand())
				{
					sqCmd.Parameters.Add(new SQLiteParameter("@tag", DbType.String) { Value =  asTags[i] });
					sqCmd.CommandType = CommandType.Text;
					sqCmd.CommandText = @"
					if not exists(
						select
							1
						from
							Tag
						where
							Tag = @tag
					)
					begin
						insert into Tag(Tag)
						values(@tag)
					end";
					sqCmd.ExecuteNonQuery();
					
					//add tagID to list
					sqCmd.CommandText = @"
					select
						TagID
					from
						Tag
					where
						Tag = @tag
					";
					sqCmd.ExecuteNonQuery();

					using(SQLiteDataReader dr = sqCmd.ExecuteReader(CommandBehavior.SingleResult)) {
						lTagID.Add(dr.GetInt32(dr.GetOrdinal("TagID")));
						dr.Close();
					}
				}

				//link tag if it isn't already
				using (SQLiteCommand sqCmd = sqConn.CreateCommand())
				{
					sqCmd.Parameters.Add(new SQLiteParameter("@tagID", DbType.Int32) { Value = lTagID[i] });
					sqCmd.Parameters.Add(new SQLiteParameter("@mangaID", DbType.Int32) { Value = iMangaID });
					sqCmd.CommandType = CommandType.Text;
					sqCmd.CommandText = @"
					if not exists(
						select
							1
						from
							MangaTag
						where
							MangaID = @mangaID
						and
							TagID = @tagID
					)
					begin
						insert into MangaTag(MangaID, TagID)
						values(@mangaID, @tagID)
					end";
					sqCmd.ExecuteNonQuery();
				}
			}
			
			//delete any invalid links
			using (SQLiteCommand sqCmd = sqConn.CreateCommand())
			{
				sqCmd.CommandType = CommandType.Text;
				sqCmd.CommandText = @"
				delete from MangaTag
				where
					MangaID = @mangaID
				and
					TagID not in (
				";
				sqCmd.CommandText += string.Join(",", lTagID) + ")";
				sqCmd.ExecuteNonQuery();
			}
		}

		private static void DB_AddArtist(int iMangaID, string[] asArtists)
		{
			List<Int32> lArtistID = new List<int>(asArtists.Length);

			for (int i = 0; i < asArtists.Length; i++)
			{
				if (string.IsNullOrEmpty(asArtists[i].ToString())) continue;

				//add artist if it doesn't exist already
				using (SQLiteCommand sqCmd = sqConn.CreateCommand())
				{
					sqCmd.Parameters.Add(new SQLiteParameter("@artist", DbType.String) { Value = asArtists[i] });
					sqCmd.CommandType = CommandType.Text;
					sqCmd.CommandText = @"
					if not exists(
						select
							1
						from
							Artist
						where
							Name = @artist
					)
					begin
						insert into Artist(Name)
						values(@artist)
					end";
					sqCmd.ExecuteNonQuery();

					//add artistID to list
					sqCmd.CommandText = @"
					select
						ArtistID
					from
						Artist
					where
						Name = @artist
					";
					sqCmd.ExecuteNonQuery();

					using (SQLiteDataReader dr = sqCmd.ExecuteReader(CommandBehavior.SingleResult))
					{
						lArtistID.Add(dr.GetInt32(dr.GetOrdinal("ArtistID")));
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
					if not exists(
						select
							1
						from
							MangaArtist
						where
							MangaID = @mangaID
						and
							ArtistID = @tagID
					)
					begin
						insert into MangaTag(MangaID, ArtistID)
						values(@mangaID, @ArtistID)
					end";
					sqCmd.ExecuteNonQuery();
				}
			}

			//delete any invalid links
			using (SQLiteCommand sqCmd = sqConn.CreateCommand())
			{
				sqCmd.CommandType = CommandType.Text;
				sqCmd.CommandText = @"
				delete from MangaArtist
				where
					MangaID = @mangaID
				and
					ArtistID not in (
				";
				sqCmd.CommandText += string.Join(",", lArtistID) + ")";
				sqCmd.ExecuteNonQuery();
			}
		}

		private static void DB_AddType(int iMangaID, string sType)
		{
			int iTypeID = 0;

			//add type if it doesn't exist already
			if (!string.IsNullOrEmpty(sType))
			{
				using (SQLiteCommand sqCmd = sqConn.CreateCommand())
				{
					sqCmd.Parameters.Add(new SQLiteParameter("@type", DbType.String) { Value = sType });
					sqCmd.CommandType = CommandType.Text;
					sqCmd.CommandText = @"
					if not exists(
						select
							1
						from
							Type
						where
							Type = @type
					)
					begin
						insert into Type(Type)
						values(@type)
					end";
					sqCmd.ExecuteNonQuery();

					//get typeID
					sqCmd.CommandText = @"
					select
						TypeID
					from
						Type
					where
						Type = @type
					";
					sqCmd.ExecuteNonQuery();

					using (SQLiteDataReader dr = sqCmd.ExecuteReader(CommandBehavior.SingleResult))
					{
						iTypeID = dr.GetInt32(dr.GetOrdinal("TypeID"));
						dr.Close();
					}
				}

				//set type if it isn't already
				using (SQLiteCommand sqCmd = sqConn.CreateCommand())
				{
					sqCmd.Parameters.Add(new SQLiteParameter("@typeID", DbType.Int32) { Value = iTypeID });
					sqCmd.Parameters.Add(new SQLiteParameter("@mangaID", DbType.Int32) { Value = iMangaID });
					sqCmd.CommandType = CommandType.Text;
					sqCmd.CommandText = @"
					if not exists(
						select
							1
						from
							Manga
						where
							MangaID = @mangaID
						and
							TypeID = @typeID
					)
					begin
						update Manga
						set TypeID = @typeID
						,auditDBTime = getdate()
					end";
					sqCmd.ExecuteNonQuery();
				}
			}
		}

		private static void DB_Close()
		{
			if (sqConn != null
					&& sqConn.State != ConnectionState.Closed) {
				sqConn.Close();
			}
		}

		public static void Entry_Save(string[] asArtist, string[] asTag, string sType, 
			string sTitle, int iPages, decimal dRating, string sDesc, string sLoc, 
			string sURL, DateTime dtPubDate)
		{
			try {
			//ensure key value exists
			if (string.IsNullOrEmpty(sTitle))
				throw new NoNullAllowedException();

			//insert manga record
			int iMangaID = 0;
			using (SQLiteCommand sqCmd = sqConn.CreateCommand())
			{
				sqCmd.Parameters.Add(new SQLiteParameter("@title", DbType.String) { Value = sTitle });
				sqCmd.Parameters.Add(new SQLiteParameter("@pages", DbType.Int32) { Value = iPages });
				sqCmd.Parameters.Add(new SQLiteParameter("@rating", DbType.Decimal) { Value = dRating });
				sqCmd.Parameters.Add(new SQLiteParameter("@description", DbType.String) { Value = sDesc });
				sqCmd.Parameters.Add(new SQLiteParameter("@location", DbType.String) { Value = sLoc });
				sqCmd.Parameters.Add(new SQLiteParameter("@URL", DbType.String) { Value = sURL });
				sqCmd.Parameters.Add(new SQLiteParameter("@pubDate", DbType.Date) { Value = dtPubDate });
				sqCmd.CommandType = CommandType.Text;
				sqCmd.CommandText = @"
				if not exists(
					select
						1
					from
						Manga
					where
						title = @title
				)
				begin
					insert into Manga(title, pages, rating, description, location, galleryURL, publishedDate)
					values(@title,@pages,@rating,@description,@location,@URL,@pubDate)
				end";
				sqCmd.ExecuteNonQuery();

				//get mangaID
				sqCmd.CommandText = @"
					select
						MangaID
					from
						Manga
					where
						title = @title
					";
				sqCmd.ExecuteNonQuery();

				using (SQLiteDataReader dr = sqCmd.ExecuteReader(CommandBehavior.SingleResult))
				{
					iMangaID = dr.GetInt32(dr.GetOrdinal("TypeID"));
					dr.Close();
				}
			}

			//insert artist
			DB_AddArtist(iMangaID, asArtist);

			//insert tags
			DB_AddTag(iMangaID, asTag);

			//insert type
			DB_AddType(iMangaID, sType);
			} catch (Exception exc) {
				Console.WriteLine(exc.Message);
			}
		}

		public static string[] Entry_Summary(int iMangaID)
		{
			List<string> lDetails = new List<string>(10);

			using (SQLiteCommand sqCmd = sqConn.CreateCommand())
			{
				sqCmd.Parameters.Add(new SQLiteParameter("MangaID", DbType.Int32) { Value = iMangaID });
				sqCmd.CommandType = CommandType.Text;
				sqCmd.CommandText = @"
				select
					at.Name
				from
					dbo.MangaArtist ma
				join
					dbo.Artist at on at.ArtistID = at.ArtistID
				where
					ma.MangaID = @mangaID
				";

				lDetails.Add("");
				using(SQLiteDataReader dr = sqCmd.ExecuteReader())
				{
				}
			}
			return new string[0];
		}

		public static string[] Entry_Details(int iMangaID)
		{
			return new string[0];
		}

		public static void Entry_Delete(int iMangaID)
		{
		}

		~csSQL() {
			DB_Close();
			sqConn.Dispose();
		}
	}
}


//using (SQLiteCommand sqCmd = sqConn.CreateCommand())
//{
//	sqCmd.CommandType = CommandType.Text;
//	sqCmd.CommandText =
//	sqCmd.ExecuteNonQuery();
//}