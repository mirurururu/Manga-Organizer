#region Assemblies
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
#endregion

namespace Nagru___Manga_Organizer
{
  /// <summary>
  /// Controls access to the database
  /// </summary>
  public sealed class SQL
  {
    #region Database Object Enums

    internal enum DB
    {
      Artist,
      Tag,
      TagNamespace,
      Type,
      Manga,
      MangaArtist,
      MangaTag,
      Settings,
      SystemEventType,
      SystemEvent,
      vsManga,
      vsTagNamespace,
      vsTagSummary
    }

    public enum EventType
    {
      UnhandledException = 1,
      HandledException = 2,
      CustomException = 3,
      DatabaseEvent = 4,
      CSharpEvent = 5,
      NetworkingEvent = 6
    }

    public enum Setting
    {
      DBversion,
      RootPath,
      SearchIgnore,
      FormPosition,
      TagIgnore,
      ImageBrowser,
      Notes,
      member_id,
      pass_hash,
      NewUser,
      SendReports,
      ShowGrid,
      ShowDate,
      ReadInterval,
      RowColourHighlight,
      RowColourAlt,
      BackgroundColour,
      GallerySettings,
      AdminMode,
      CreatedDBTime,
      AuditDBTime
    };

    public enum Manga
    {
      MangaID,
      Artists,
      Title,
      PageCount,
      PageReadCount,
      Tags,
      Description,
      PublishedDate,
      Location,
      GalleryURL,
      Thumbnail,
      Type,
      Rating
    };

    #endregion

    #region Properties

    /// <summary>
    /// Returns whether or not SQL has an open DB connection
    /// </summary>
    /// <returns>Returns true if a connection is open</returns>
    public static bool IsConnected
    {
      get
      {
        return sqlObj != null && sqlConn.Connected();
      }
    }

    private static volatile SQLBase sqlObj = null;
    private static object locker = new object();

    /// <summary>
    /// Thread-safe access of the SQLBase instance
    /// </summary>
    private static SQLBase sqlConn
    {
      get
      {
        if (sqlObj == null) {
          lock (locker) {
            if (sqlObj == null) {
              sqlObj = new SQLBase();
            }
          }
        }

        return sqlObj;
      }
    }

    #endregion

    #region Constructor

    /// <summary>
    /// Instantiates the DB connection
    /// </summary>
    static SQL()
    {
    }

    #endregion

    #region Internal

    //http://stackoverflow.com/questions/10853301/save-and-load-image-sqlite-c-sharp
    internal static byte[] ImageToByte(Image image, ImageFormat format)
    {
      using (MemoryStream ms = new MemoryStream()) {
        image.Save(ms, format);
        byte[] imageBytes = ms.ToArray();
        return imageBytes;
      }
    }

    //public Image Base64ToImage(string base64String)
    internal static Image ByteToImage(byte[] imageBytes)
    {
      Image image = null;
      try {
        using (MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length)) {
          ms.Write(imageBytes, 0, imageBytes.Length);
          image = new Bitmap(ms);
        }
      } catch {
      }
      return image;
    }

    #endregion

    #region Interface

    #region Connection

    /// <summary>
    /// Opens a connection to the database
    /// </summary>
    public static void Connect(string FilePath = null)
    {
      //close the current connection
      SQL.Disconnect();

      //set the path to load from
      if (string.IsNullOrWhiteSpace(FilePath)) {
        FilePath = (!string.IsNullOrWhiteSpace(Properties.Settings.Default.SavLoc)) ?
          Properties.Settings.Default.SavLoc : Environment.CurrentDirectory;
        FilePath += "\\MangaDB.sqlite";
      }

      //connect to the DB
      sqlConn.Connect(FilePath);
    }

    /// <summary>
    /// Vacuum's the DB and closes the connection
    /// </summary>
    public static void Disconnect()
    {
      if (SQL.IsConnected) {
        sqlConn.Close();
      }
    }

    #endregion

    #region Transactions

    /// <summary>
    /// Checks whether a transaction is currently pending
    /// </summary>
    /// <returns>Returns the transaction status</returns>
    public static bool InTransaction()
    {
      return sqlConn.INGLOBALTRAN;
    }

    /// <summary>
    /// Starts a new transaction
    /// USE WITH CAUTION: SQLite DOES NOT SUPPORT MULTIPLE TRANSACTIONS
    /// </summary>
    /// <returns>Returns whether the command was successful</returns>
    public static bool BeginTransaction()
    {
      int iRetVal = sqlConn.BeginTransaction(SetGlobal: true);
      return (iRetVal != -1);
    }

    /// <summary>
    /// Commits the current transaction
    /// </summary>
    /// <returns>Returns whether the command was successful</returns>
    public static bool CommitTransaction()
    {
      int iRetVal = sqlConn.EndTransaction(EndGlobal: true);
      return (iRetVal != -1);
    }

    /// <summary>
    /// Rollbacks the current transaction
    /// </summary>
    /// <returns>Returns whether the command was successful</returns>
    public static bool RollbackTransaction()
    {
      int iRetVal = sqlConn.EndTransaction(Commit: false, EndGlobal: true);
      return (iRetVal != -1);
    }

    #endregion

    #region Query Database

    /// <summary>
    /// Returns all of the available artists
    /// </summary>
    /// <returns>Returns all the artists in the database</returns>
    public static string[] GetArtists()
    {
      string[] asArtists;
      using (DataTable dtArtists = SQLAccess.GetValues(SQL.DB.Artist)) {
        asArtists = new string[dtArtists.Rows.Count];
        for (int i = 0; i < dtArtists.Rows.Count; i++) {
          asArtists[i] = dtArtists.Rows[i]["Name"].ToString();
        }
      }
      return asArtists;
    }

    public static Image GetThumbnail(int MangaID)
    {
      Image bmpThumb = null;
      using (DataTable dt = SQLAccess.GetMangaEntry(MangaID)) {
        if (dt.Rows.Count > 0 && !string.IsNullOrWhiteSpace(dt.Rows[0][11].ToString())) {
          byte[] byThumb = (dt.Rows[0][11] as byte[]);
          System.Buffer.BlockCopy(byThumb, 0, byThumb, 0, byThumb.Length);
          bmpThumb = ByteToImage(byThumb);
        }
      }
      return bmpThumb;
    }

    /// <summary>
    /// Returns all of the available Manga types
    /// </summary>
    /// <returns>Returns all the manga types in the database</returns>
    public static string[] GetTypes()
    {
      string[] asTypes;
      using (DataTable dtTypes = SQLAccess.GetValues(SQL.DB.Type)) {
        asTypes = new string[dtTypes.Rows.Count];
        for (int i = 0; i < dtTypes.Rows.Count; i++) {
          asTypes[i] = dtTypes.Rows[i]["Type"].ToString();
        }
      }
      return asTypes;
    }

    /// <summary>
    /// Returns all of the available tags
    /// </summary>
    /// <returns>Returns all the tags in the database</returns>
    public static string[] GetTags()
    {
      string[] asTags;
      using (DataTable dtTags = SQLAccess.GetValues(SQL.DB.vsTagSummary)) {
        asTags = new string[dtTags.Rows.Count];
        for (int i = 0; i < dtTags.Rows.Count; i++) {
          asTags[i] = dtTags.Rows[i]["Tag"].ToString();
        }
      }
      return asTags;
    }

    /// <summary>
    /// Returns the EH formatted title of a Manga
    /// </summary>
    /// <param name="mangaID">The ID of the record to access</param>
    public static string GetMangaTitle(int mangaID)
    {
      string sTitle = string.Empty;
      using (DataTable dt = SQLAccess.GetMangaEntry(mangaID)) {
        sTitle = Ext.GetFormattedTitle(
          dt.Rows[0]["Artists"].ToString(),
          dt.Rows[0]["Title"].ToString()
        );
      }

      return sTitle;
    }

    /// <summary>
    /// Returns formatted details about all the existing Manga records
    /// </summary>
    /// <param name="OnlyFavourites">Only return entries with a rating of 5.0?</param>
    /// <param name="SearchText">The user search parameters to compare against</param>
    /// <returns>Returns all the manga in the database</returns>
    public static DataTable GetAllManga(string SearchText = null, int iMangaID = -1)
    {
      if (!string.IsNullOrWhiteSpace(SearchText)) {
        return SQLAccess.Search(SearchText, iMangaID);
      }
      else {
        return SQLAccess.GetManga(iMangaID);
      }
    }

    /// <summary>
    /// Returns the full details of a specified manga
    /// </summary>
    /// <param name="mangaID">The ID of the record</param>
    /// <returns>Returns all the info for an individual manga</returns>
    public static DataTable GetMangaDetails(int mangaID)
    {
      return SQLAccess.GetMangaEntry(mangaID);
    }

    /// <summary>
    /// Returns a single detail of a specified manga
    /// </summary>
    /// <param name="mangaID">The ID of the record</param>
    /// <param name="columnName">The name of the column to extract</param>
    public static string GetMangaDetail(int mangaID, Manga Column)
    {
      string sVal = "";
      using (DataTable dt = SQLAccess.GetMangaEntry(mangaID)) {
        sVal = dt.Rows[0][Column.ToString()].ToString();
      }
      return sVal;
    }

    /// <summary>
    /// Returns a setting from the DB
    /// </summary>
    /// <param name="DBSetting"></param>
    /// <returns>Returns the specified setting value</returns>
    public static object GetSetting(Setting DBSetting)
    {
      string sValue = null;
      using (DataTable dt = SQLAccess.GetValues(SQL.DB.Settings)) {
        if (dt != null && dt.Rows.Count > 0) {
          sValue = dt.Rows[0][DBSetting.ToString()].ToString();
        }
      }

      if (!string.IsNullOrWhiteSpace(sValue)) {
        switch (DBSetting) {
          case Setting.BackgroundColour:
          case Setting.RowColourAlt:
          case Setting.RowColourHighlight:
            return Color.FromArgb(int.Parse(sValue));
          case Setting.DBversion:
          case Setting.member_id:
          case Setting.ReadInterval:
            return int.Parse(sValue);
          case Setting.AdminMode:
          case Setting.ShowDate:
          case Setting.ShowGrid:
          case Setting.NewUser:
          case Setting.SendReports:
            return int.Parse(sValue) == 1;
          case Setting.FormPosition:
            int[] ai = sValue.Split(',').Select(x => Int32.Parse(x)).ToArray();
            return new Rectangle(ai[0], ai[1], ai[2], ai[3]);
          case Setting.GallerySettings:
            return sValue.Split(',').Select(x => byte.Parse(x)).ToArray();
          case Setting.SearchIgnore:
          case Setting.TagIgnore:
            return sValue.Split('|').ToArray();
          case Setting.ImageBrowser:
          case Setting.RootPath:
          case Setting.Notes:
          case Setting.pass_hash:
            return sValue;
        }
      }
      return null;
    }

    /// <summary>
    /// Searches for manga that match the search parameters
    /// </summary>
    /// <param name="OnlyFavourites">Only return entries with a rating of 5.0?</param>
    /// <param name="SearchText">The user search parameters to compare against</param>
    /// <param name="mangaID">Can search for only a specific manga</param>
    /// <returns>Returns all the matching manga</returns>
    public static DataTable SearchManga(string SearchText, int MangaID = -1)
    {
      return SQLAccess.Search(SearchText, MangaID);
    }

    /// <summary>
    /// Returns whether the entry exists in the DB
    /// </summary>
    /// <param name="Artist">The artist's name</param>
    /// <param name="Title">The title of the maga</param>
    /// <returns>Returns true if the record already exists in the database</returns>
    public static bool ContainsEntry(string Artist, string Title)
    {
      return SQLAccess.EntryExists(Artist, Title);
    }

    /// <summary>
    /// Finds all manga with a 90% similar title to the passed in string
    /// </summary>
    /// <param name="Title">The manga title to compare to</param>
    /// <returns>Returns a set of manga</returns>
    public static DataTable FindSimilar(string Title)
    {
      return SQLAccess.EntryExists(Title);
    }

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
    public static int SaveManga(string Artist, string Title, DateTime PublishedDate,
        string Tags = null, string Location = null, decimal Pages = 0, int PageReadCount = 0, string Type = null,
        decimal Rating = 0, string Description = null, string URL = null, int MangaID = -1)
    {
      return SQLAccess.DB_SaveEntry(Artist, Title, PublishedDate, Tags,
        Location, Pages, PageReadCount, Type, Rating, Description, URL, MangaID);
    }

    public static bool SaveThumbnail(int MangaID, Image image, ImageFormat imageFormat)
    {
      int iAltered = SQLAccess.SaveMangaThumbnail(MangaID, ImageToByte(image, imageFormat));
      return (iAltered != -1);
    }

    public static bool SaveReadProgress(int MangaID, int CurrentPage)
    {
      int iAltered = SQLAccess.SavePageReadCount(MangaID, CurrentPage);
      return (iAltered != -1);
    }

    /// <summary>
    /// Deletes an entry from the database
    /// </summary>
    /// <param name="mangaID">The ID of the record to be deleted</param>
    /// <returns>Returns the number of affected records</returns>
    public static bool DeleteManga(int mangaID)
    {
      int iAltered = SQLAccess.DeleteEntry(mangaID);
      return (iAltered != -1);
    }

    /// <summary>
    /// Re-creates the MangaTag table to remove PK value gaps
    /// </summary>
    /// <returns>Returns the number of affected records</returns>
    public static int CleanUpReferences()
    {
      return SQLAccess.RecycleMangaTag();
    }

    /// <summary>
    /// Deletes all unused tags from the DB
    /// </summary>
    /// <returns>Returns the number of deleted tags.</returns>
    public static int CleanUpTags()
    {
      return SQLAccess.DeleteUnusedTags();
    }

    /// <summary>
    /// Deletes all unused artists from the DB
    /// </summary>
    /// <returns>Returns the number of deleted tags.</returns>
    public static int CleanUpArtists()
    {
      return SQLAccess.DeleteUnusedArtists();
    }

    /// <summary>
    /// Updates only the rating of the indicated record
    /// </summary>
    /// <param name="MangaID">The ID of the record to update</param>
    /// <param name="Rating">The new rating value</param>
    /// <returns>Returns the number of affected records</returns>
    public static bool UpdateRating(int MangaID, decimal Rating)
    {
      int iAltered = sqlConn.ExecuteNonQuery(
        "update Manga set Rating = @rating where MangaID = @mangaID"
        , SQLBase.NewParameter("@mangaID", DbType.Int32, MangaID)
        , SQLBase.NewParameter("@rating", DbType.Decimal, Rating)
      );
      if (iAltered == -1) {
        //xDialog.DisplayError(msg.FAILED_UPDATE, "Failed to update rating.", EventType.CustomException, MangaID);
      }
      return (iAltered != -1);
    }

    /// <summary>
    /// Updates a program setting
    /// </summary>
    /// <param name="DBSetting">The setting to update</param>
    /// <param name="value">The new value for the setting</param>
    /// <returns>Returns whether the update was successful</returns>
    public static bool UpdateSetting(Setting DBSetting, object value)
    {
      string sObjValue = SQLBase.Sanitize(value.ToString());
      bool bUpdated = false;

      switch (DBSetting) {
        case Setting.BackgroundColour:
        case Setting.RowColourAlt:
        case Setting.RowColourHighlight:
          if (value is Color) {
            Color cNewColor = ((Color)value);
            bUpdated = SQLAccess.UpdateSetting(DBSetting, cNewColor.ToArgb()) > 0;
          }
          break;
        case Setting.member_id:
        case Setting.ShowDate:
        case Setting.ShowGrid:
          int i;
          if (int.TryParse(sObjValue, out i)) {
            bUpdated = SQLAccess.UpdateSetting(DBSetting, i) > 0;
          }
          break;
        case Setting.FormPosition:
          if (value is Rectangle) {
            Rectangle DesktopBounds = ((Rectangle)value);
            string sFmt = string.Format("{0},{1},{2},{3}"
              , DesktopBounds.X > -1 ? DesktopBounds.X : 0
              , DesktopBounds.Y > -1 ? DesktopBounds.Y : 0
              , DesktopBounds.Width
              , DesktopBounds.Height);
            bUpdated = SQLAccess.UpdateSetting(DBSetting, sFmt) > 0;
          }
          break;
        case Setting.GallerySettings:
          if (value is byte[]) {
            bUpdated = SQLAccess.UpdateSetting(DBSetting,
              string.Join(",", (value as byte[]).Select(x => x.ToString()))) > 0;
          }
          break;
        case Setting.ImageBrowser:
          if (Ext.Accessible(sObjValue) == Ext.PathType.ValidFile) {
            bUpdated = SQLAccess.UpdateSetting(DBSetting, sObjValue) > 0;
          }
          break;
        case Setting.RootPath:
          if (Ext.Accessible(sObjValue) == Ext.PathType.ValidDirectory) {
            bUpdated = SQLAccess.UpdateSetting(Setting.RootPath, sObjValue) > 0;
          }
          break;
        case Setting.SearchIgnore:
        case Setting.TagIgnore:
          if (value is string[]) {
            bUpdated = SQLAccess.UpdateSetting(DBSetting,
              string.Join("|", (value as string[]))) > 0;
          }
          break;
        case Setting.Notes:
        case Setting.pass_hash:
          bUpdated = SQLAccess.UpdateSetting(DBSetting, sObjValue) > 0;
          break;
        case Setting.DBversion:
        default:
          bUpdated = false;
          break;
      }

      if (!bUpdated) {
        //xDialog.DisplayError(msg.FAILED_UPDATE, DBSetting.ToString() + " failed to update.",
        //  SQL.EventType.CustomException, value);
      }
      return bUpdated;
    }

    #endregion

    #endregion

    /// <summary>
    /// Holds the connection object and provides the 'base' functionality of the SQL implementation
    /// </summary>
    private class SQLBase : IDisposable
    {
      #region Properties

      internal SQLiteConnection CONN = null;
      internal bool INGLOBALTRAN = false;
      internal string CURRENTLOC = null;
      private const int MAX_LENGTH = 8000;
      private const int DB_VERSION = 2;
      private bool INLOCALTRAN = false;
      private bool DISPOSED = false;
      private bool ERROR_STATE = false;

      #endregion

      #region Constructor

      /// <summary>
      /// Establish a DB connection when instantiated
      /// </summary>
      internal SQLBase()
      {
      }

      /// <summary>
      /// Public implementation of Dispose
      /// </summary>
      public void Dispose()
      {
        this.Dispose(true);
        GC.SuppressFinalize(this);
      }

      /// <summary>
      /// Protected implementation of Dispose
      /// </summary>
      /// <param name="Disposing">Whether we are calling the method from the Dispose override</param>
      protected virtual void Dispose(bool Disposing)
      {
        if (DISPOSED)
          return;

        if (Disposing && CONN != null) {
          CONN.Dispose();
        }

        DISPOSED = true;
      }

      /// <summary>
      /// Destructor
      /// </summary>
      ~SQLBase()
      {
        Dispose(true);
      }

      #endregion

      #region Handle connection

      /// <summary>
      /// Establishes a connection with the database or, if one is not found, create a new instance
      /// </summary>
      internal void Connect(string FilePath = null)
      {
        //set the path to load from
        if (string.IsNullOrWhiteSpace(FilePath)) {
          FilePath = (!string.IsNullOrWhiteSpace(Properties.Settings.Default.SavLoc)) ?
            Properties.Settings.Default.SavLoc : Environment.CurrentDirectory;
          FilePath += "\\MangaDB.sqlite";
        }

        //create connection
        CURRENTLOC = FilePath;
        bool bExists = File.Exists(FilePath);
        CONN = new SQLiteConnection();
        if (!bExists) {
          SQLiteConnection.CreateFile(FilePath);
        }
        CONN.ConnectionString = new DbConnectionStringBuilder()
        {
          {"Data Source", FilePath},
          {"Version", "3"},
          {"Compress", true},
          {"New", !bExists}
        }.ConnectionString;
        CONN.Open();

        //create the DB or check if it needs updating
        if (bExists) {
          this.UpdateVersion();
        }
        else {
          this.CreateDatabase();
        }
      }

      /// <summary>
      /// Returns whether or not the DB connection is currently open
      /// </summary>
      /// <returns>Returns true if a DB connection is open</returns>
      internal bool Connected()
      {
        return (CONN != null &&
          CONN.State == ConnectionState.Open);
      }

      /// <summary>
      /// Displays an error if the connection is lost. Used to wrap the base 
      /// access functions to prevent possible hard errors.
      /// </summary>
      /// <returns>Returns true if there is still an active connection</returns>
      private bool HandledDisconnect()
      {
        if (!this.Connected()) {
          if (!ERROR_STATE) {
            //xDialog.DisplayError(msg.DISCONNECTED, CONN == null ?
            //  "sqlConn has not been instantiated." : "sqlConn instantiated but has invalid ConnectionState",
            //  EventType.CustomException, LogSystemEvent: false);
            ERROR_STATE = true;
          }
        }
        else if (ERROR_STATE) {
          ERROR_STATE = false;
        }
        return !ERROR_STATE;
      }

      /// <summary>
      /// Check if there are updates to the DB, and if so deploy them
      /// </summary>
      internal void UpdateVersion()
      {
        int iCurrentVersion = 0;

        //check if there's a new version of the database
        object objVersion = GetSetting(Setting.DBversion);
        if (objVersion != null) {
          iCurrentVersion = ((int)objVersion);
        }
        if (DB_VERSION != iCurrentVersion) {
          //make copy of current db
          try {
            File.Copy(CURRENTLOC, CURRENTLOC + "_backup");
          } catch (Exception) {
          }

          this.BeginTransaction(SetGlobal: true);

          switch (iCurrentVersion) {
            default:
            case 0:
              #region Update to version 1.0
              BeginTransaction();
              sqlConn.ExecuteNonQuery(Create_Settings);

              #region Grab the current settings and populate the table
              List<SQLiteParameter> sqParam = new List<SQLiteParameter>(20);

              string sQuery = @"
                update [Settings]
                set DBversion         = 1
                ,RootPath							= @rootPath
                ,SavePath							= @savePath
                ,SearchIgnore					= @ignore
                ,FormPosition					= @position
                ,ImageBrowser					= @browser
                ,Notes                = @notes
                ,member_id						= @memberID
                ,pass_hash						= @passHash
                ,NewUser							= 0
                ,ShowGrid							= @showGrid
                ,ShowDate							= @showDate
                ,ReadInterval					= @interval
                ,RowColourHighlight		= @rowHighlight
                ,RowColourAlt					= @rowAlt
                ,BackgroundColour			= @background
                ,GallerySettings			= @galleries";

              sqParam.AddRange(new SQLiteParameter[13]{
                NewParameter("@rootPath", DbType.String, Properties.Settings.Default.DefLoc),
                NewParameter("@savePath", DbType.String, Properties.Settings.Default.SavLoc),
                NewParameter("@ignore", DbType.String, Properties.Settings.Default.Ignore),
                NewParameter("@browser", DbType.String, Properties.Settings.Default.DefProg),
                NewParameter("@notes", DbType.String, Properties.Settings.Default.Notes),
                NewParameter("@passHash", DbType.String, Properties.Settings.Default.pass_hash),
                NewParameter("@showGrid", DbType.Int32, Properties.Settings.Default.DefGrid ? 1 : 0),
                NewParameter("@showDate", DbType.Int32, Properties.Settings.Default.HideDate ? 0 : 1),
                NewParameter("@interval", DbType.Int32, Properties.Settings.Default.Interval),
                NewParameter("@rowHighlight", DbType.Int32, Properties.Settings.Default.RowColorHighlight),
                NewParameter("@rowAlt", DbType.Int32, Properties.Settings.Default.RowColorAlt),
                NewParameter("@background", DbType.Int32, Properties.Settings.Default.DefColour.ToArgb()),
                NewParameter("@galleries", DbType.String, Properties.Settings.Default.GalleryTypes),
              });
              sqParam.Add(NewParameter("@position", DbType.String,
                string.Format("{0},{1},{2},{3}"
                  , Properties.Settings.Default.Position.X
                  , Properties.Settings.Default.Position.Y
                  , Properties.Settings.Default.Position.Width
                  , Properties.Settings.Default.Position.Height)
              ));
              sqParam.Add(NewParameter("@memberID", DbType.Int32,
                !string.IsNullOrWhiteSpace(Properties.Settings.Default.member_id) ?
                  Int32.Parse(Properties.Settings.Default.member_id) : -1)
              );

              ExecuteNonQuery(sQuery, sqParam.ToArray());
              #endregion

              #region Add the Thumbnails column to dbo.Manga
              sQuery = @"
                alter table [Manga]
                add column Thumbnail blob null";
              ExecuteNonQuery(sQuery);
              #endregion

              #region Remove the Audit details from the link tables
              sQuery = @"
                drop trigger trMangaArtist;
                create temporary table [tmpMangaArtist](MangaID, ArtistID);
                insert into [tmpMangaArtist] select MangaID, ArtistID from [MangaArtist];
                drop table [MangaArtist];
        
                create table [MangaArtist]
                (
                  MangaArtistID		integer		primary key		autoincrement
                  ,MangaID				integer		not null
                  ,ArtistID				integer		not null
                  ,constraint [fk_mangaID] foreign key ([MangaID]) references [Manga] ([MangaID])
                  ,constraint [fk_artistID] foreign key ([ArtistID]) references [Artist] ([ArtistID])
                );

                insert into [MangaArtist](MangaID, ArtistID) select MangaID, ArtistID from [tmpMangaArtist];
                drop table [tmpMangaArtist];";
              ExecuteNonQuery(sQuery);

              sQuery = @"
                drop trigger trMangaTag;
                create temporary table [tmpMangaTag](MangaID, TagID);
                insert into [tmpMangaTag] select MangaID, TagID from [MangaTag];
                drop table [MangaTag];
        
                create table [MangaTag]
                (
                  MangaTagID			integer		primary key		autoincrement
                  ,MangaID				integer		not null
                  ,TagID					integer		not null
                  ,constraint [fk_mangaID] foreign key ([MangaID]) references [Manga] ([MangaID])
                  ,constraint [fk_tagID] foreign key ([TagID]) references [Tag] ([TagID])
                );

                insert into [MangaTag](MangaID, TagID) select MangaID, TagID from [tmpMangaTag];
                drop table [tmpMangaTag];";
              ExecuteNonQuery(sQuery);
              #endregion

              EndTransaction();

              //update 1 -> 2
              this.UpdateVersion();
              #endregion
              break;
            case 1:
              #region Convert from 1.0 to 2.0

              //remove unused fields
              StringBuilder sbCmd = new StringBuilder(10000);
              sbCmd.Append("ALTER TABLE Artist RENAME TO TempArtist;");
              sbCmd.Append("DROP TRIGGER trArtist;");
              sbCmd.Append(Create_Artist);
              sbCmd.Append(@"
								INSERT INTO Artist(ArtistID, Name, CreatedDBTime, AuditDBTime)
								SELECT ArtistID, Name, CreatedDBTime, AuditDBTime
								FROM TempArtist;
							");
              sbCmd.Append("DROP TABLE TempArtist;");
              this.ExecuteNonQuery(sbCmd.ToString());
              sbCmd.Clear();

              //add new TagNamespace element to MangaTag
              sbCmd.Append("ALTER TABLE MangaTag RENAME TO TempMangaTag;");
              sbCmd.Append(Create_MangaTag);
              sbCmd.Append(@"
								INSERT INTO MangaTag(MangaID, TagID)
								SELECT MangaID, TagID
								FROM TempMangaTag;
							");
              sbCmd.Append("DROP TABLE TempMangaTag;");
              this.ExecuteNonQuery(sbCmd.ToString());
              sbCmd.Clear();

              //add new elements to Manga
              sbCmd.Append("ALTER TABLE Manga RENAME TO TempManga;");
              sbCmd.Append("DROP TRIGGER trManga;");
              sbCmd.Append(Create_Manga);
              sbCmd.Append(@"
								INSERT INTO Manga(MangaID, TypeID, Title, PageCount, Rating, Description, Location, GalleryURL, PublishedDate, CreatedDBTime, AuditDBTime)
								SELECT MangaID, TypeID, Title, Pages, Rating, Description, Location, GalleryURL, PublishedDate, CreatedDBTime, AuditDBTime
								FROM TempManga;
							");
              sbCmd.Append("DROP TABLE TempManga;");
              this.ExecuteNonQuery(sbCmd.ToString());
              sbCmd.Clear();

              //add new elements to Settings
              sbCmd.Append("ALTER TABLE Settings RENAME TO TempSettings;");
              sbCmd.Append("DROP TRIGGER trSettings;");
              sbCmd.Append(Create_Settings);
              sbCmd.Append("DELETE FROM Settings;");
              sbCmd.Append(@"
								INSERT INTO Settings(SettingsID, RootPath, SearchIgnore, FormPosition, ImageBrowser, Notes, member_id, pass_hash, ShowGrid, ShowDate, RowColourHighlight, RowColourAlt, BackgroundColour, GallerySettings, ReadInterval, NewUser, SendReports, CreatedDBTime, AuditDBTime)
								SELECT SettingsID, RootPath, SearchIgnore, FormPosition, ImageBrowser, Notes, member_id, pass_hash, ShowGrid, ShowDate, RowColourHighlight, RowColourAlt, BackgroundColour, GallerySettings, ReadInterval, NewUser, SendReports, CreatedDBTime, AuditDBTime
								FROM TempSettings;
							");
              sbCmd.Append("DROP TABLE TempSettings;");
              this.ExecuteNonQuery(sbCmd.ToString());
              sbCmd.Clear();

              //create new elements
              sbCmd.AppendFormat("{0}{1}{2}"
                , Create_SystemEventType
                , Create_SystemEvent
                , Create_TagNamespace
              );
              this.ExecuteNonQuery(sbCmd.ToString());
              this.ExecuteNonQuery(Create_vsManga);
              this.ExecuteNonQuery(Create_vsTagSummary);
              sbCmd.Clear();

              //update version number
              sbCmd.AppendFormat("UPDATE Setting set DBversion = @version");
              this.ExecuteNonQuery(sbCmd.ToString(), SQLBase.NewParameter("@version", DbType.Int32, iCurrentVersion));
              sbCmd.Clear();

              #endregion
              break;
            case 2:
              //current DB version
              break;
          }

          this.EndTransaction(EndGlobal: true);
        }
      }

      /// <summary>
      /// Closes the connection to the database
      /// </summary>
      internal void Close()
      {
        if (CONN != null
            && CONN.State != ConnectionState.Closed) {
          if (INGLOBALTRAN || INLOCALTRAN) {
            this.EndTransaction(Commit: false, EndGlobal: true);
          }
          this.ExecuteNonQuery("VACUUM;");
          CONN.Close();
        }
      }

      #endregion

      #region Create Database

      /// <summary>
      /// Creates the tables and triggers in the new DB
      /// </summary>
      private void CreateDatabase()
      {
        this.BeginTransaction(SetGlobal: true);
        this.ExecuteNonQuery(Create_Artist);
        this.ExecuteNonQuery(Create_Tag);
        this.ExecuteNonQuery(Create_TagNamespace);
        this.ExecuteNonQuery(Create_Type);
        this.ExecuteNonQuery(Create_Manga);
        this.ExecuteNonQuery(Create_MangaArtist);
        this.ExecuteNonQuery(Create_MangaTag);
        this.ExecuteNonQuery(Create_Settings);
        this.ExecuteNonQuery(Create_SystemEventType);
        this.ExecuteNonQuery(Create_SystemEvent);
        this.ExecuteNonQuery(Create_vsManga);
        this.ExecuteNonQuery(Create_vsTagSummary);
        this.EndTransaction(EndGlobal: true);
      }

      /// <summary>
      /// Create main.Artist
      /// </summary>
      internal const string Create_Artist = @"
        CREATE TABLE [Artist] (
            [ArtistID] integer PRIMARY KEY NOT NULL,
            [Name] ntext NOT NULL UNIQUE,
            [CreatedDBTime] datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
            [AuditDBTime] datetime NOT NULL DEFAULT CURRENT_TIMESTAMP
        );
        CREATE TRIGGER [trArtist] AFTER UPDATE ON [Artist] FOR EACH ROW
        BEGIN
          update Artist set AuditDBTime = CURRENT_TIMESTAMP where artistID = new.rowid;
        END;
      ";

      /// <summary>
      /// Create main.Tag
      /// </summary>
      internal const string Create_Tag = @"
        CREATE TABLE [Tag] (
            [TagID] integer PRIMARY KEY NOT NULL,
            [Tag] ntext NOT NULL UNIQUE,
            [CreatedDBTime] datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
            [AuditDBTime] datetime NOT NULL DEFAULT CURRENT_TIMESTAMP
        );
        CREATE TRIGGER trTag after update on Tag
        begin
          update Tag set AuditDBTime = CURRENT_TIMESTAMP where tagID = new.rowid;
        end;
      ";

      /// <summary>
      /// Create main.Create_TagNamespace
      /// </summary>
      internal const string Create_TagNamespace = @"
        CREATE TABLE [TagNamespace] (
            [TagNamespaceID] integer PRIMARY KEY NOT NULL,
            [Namespace] ntext NOT NULL UNIQUE,
            [CreatedDBTime] datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
            [AuditDBTime] datetime NOT NULL DEFAULT CURRENT_TIMESTAMP
        );
        CREATE TRIGGER trTagNamespace after update on TagNamespace
        begin
          update TagNamespace set AuditDBTime = CURRENT_TIMESTAMP where tagNamespaceID = new.rowid;
        end;
        INSERT INTO [TagNamespace] (Namespace)
        VALUES('Female'),('Male'),('Misc');
      ";

      /// <summary>
      /// Create main.Type and insert the default values
      /// </summary>
      internal const string Create_Type = @"
        CREATE TABLE [Type] (
            [TypeID] integer PRIMARY KEY NOT NULL,
            [Type] ntext NOT NULL UNIQUE,
            [CreatedDBTime] datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
            [AuditDBTime] datetime NOT NULL DEFAULT CURRENT_TIMESTAMP
        );
        CREATE TRIGGER trType after update on Type
        begin
          update Type set AuditDBTime = CURRENT_TIMESTAMP where typeID = new.rowid;
        end;

        INSERT INTO [Type] (Type)
        VALUES('Doujinshi'),('Manga'),('Artist CG'),('Game CG'),('Western')
          ,('Non-H'),('Image Set'),('Cosplay'),('Asian Porn'),('Misc');
      ";

      /// <summary>
      /// Create the linking table between Manga and Artists
      /// </summary>
      internal const string Create_MangaArtist = @"
        CREATE TABLE [MangaArtist] (
            [MangaArtistID] integer PRIMARY KEY NOT NULL,
            [MangaID] integer NOT NULL UNIQUE,
            [ArtistID] integer NOT NULL UNIQUE,
            CONSTRAINT [FK_MangaArtist_MangaID] FOREIGN KEY ([MangaID]) REFERENCES [Manga] ([MangaID]),
            CONSTRAINT [FK_MangaArtist_ArtistID] FOREIGN KEY ([ArtistID]) REFERENCES [Artist] ([ArtistID])
        );
        CREATE UNIQUE INDEX [idxMangaID_ArtistID] ON [MangaArtist] ([MangaID], [ArtistID]);
      ";

      /// <summary>
      /// Create the linking table between Manga and Tags
      /// </summary>
      internal const string Create_MangaTag = @"
        CREATE TABLE [MangaTag] (
            [MangaTagID] integer PRIMARY KEY NOT NULL,
            [MangaID] integer NOT NULL,
            [TagID] integer NOT NULL,
            [TagNamespaceID] integer,
            CONSTRAINT [FK_MangaTag_MangaID] FOREIGN KEY ([MangaID]) REFERENCES [Manga] ([MangaID]),
            CONSTRAINT [FK_MangaTag_TagID] FOREIGN KEY ([TagID]) REFERENCES [Tag] ([TagID]),
            CONSTRAINT [FK_MangaTag_TagNamespaceID] FOREIGN KEY ([TagNamespaceID]) REFERENCES [TagNamespace] ([TagNamespaceID])
        );
      ";

      /// <summary>
      /// Create main.Manga
      /// </summary>
      internal const string Create_Manga = @"
        CREATE TABLE [Manga] (
            [MangaID] integer PRIMARY KEY NOT NULL,
            [TypeID] integer,
            [Title] ntext NOT NULL,
            [PageCount] integer NOT NULL DEFAULT 0,
            [PageReadCount] integer NOT NULL DEFAULT 0,
            [Rating] decimal NOT NULL DEFAULT 0,
            [Description] ntext,
            [Location] ntext,
            [GalleryURL] ntext,
            [PublishedDate] datetime,
            [Thumbnail] blob NULL,
            [CreatedDBTime] datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
            [AuditDBTime] datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
            CONSTRAINT [FK_Manga_TypeID] FOREIGN KEY ([TypeID]) REFERENCES [Type] ([TypeID])
        );
        CREATE TRIGGER [trManga] AFTER UPDATE ON [main].[Manga] FOR EACH ROW
        BEGIN
          update Manga set AuditDBTime = CURRENT_TIMESTAMP where mangaID = new.rowid;
        END;
      ";

      /// <summary>
      /// Create main.Settings and create a row
      /// </summary>
      internal const string Create_Settings = @"
        CREATE TABLE [Settings] (
            [SettingsID] integer PRIMARY KEY AUTOINCREMENT NOT NULL,
            [DBversion] integer NOT NULL DEFAULT 2,
            [RootPath] ntext,
            [SearchIgnore] ntext,
            [TagIgnore] ntext,
            [FormPosition] ntext,
            [ImageBrowser] ntext,
            [Notes] ntext,
            [member_id] integer,
            [pass_hash] ntext,
            [AdminMode] integer NOT NULL DEFAULT 0,
            [NewUser] integer NOT NULL DEFAULT 1,
            [SendReports] integer NOT NULL DEFAULT 1,
            [ShowGrid] integer NOT NULL DEFAULT 1,
            [ShowDate] integer NOT NULL DEFAULT 1,
            [ReadInterval] integer NOT NULL DEFAULT 20000,
            [RowColourHighlight] integer NOT NULL DEFAULT -15,
            [RowColourAlt] integer NOT NULL DEFAULT -657931,
            [BackgroundColour] integer NOT NULL DEFAULT -14211038,
            [GallerySettings] ntext NOT NULL DEFAULT '1,1,0,0,0,0,0,0,0,0',
            [CreatedDBTime] datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
            [AuditDBTime] datetime NOT NULL DEFAULT CURRENT_TIMESTAMP
        );
        CREATE TRIGGER [trSettings] AFTER UPDATE ON [main].[Settings] FOR EACH ROW
        BEGIN
          update Settings set AuditDBTime = CURRENT_TIMESTAMP where settingsID = new.rowid;
        END;
        INSERT INTO [Settings] (SettingsID)
        VALUES(1);
      ";

      /// <summary>
      /// Create main.SystemEventType
      /// </summary>
      internal const string Create_SystemEventType = @"
        CREATE TABLE [SystemEventType] (
            [SystemEventTypeID] integer PRIMARY KEY NOT NULL,
            [EventType] ntext NOT NULL,
						[EventTypeCD] ntext	NOT NULL UNIQUE,
						[Severity] integer,
            [CreatedDBTime] datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
            [AuditDBTime] datetime NOT NULL DEFAULT CURRENT_TIMESTAMP
        );
        CREATE TRIGGER [trSystemEventType] AFTER UPDATE ON [SystemEventType] FOR EACH ROW
        BEGIN
          update SystemEventType set AuditDBTime = CURRENT_TIMESTAMP where SystemEventTypeID = new.rowid;
        END;
        INSERT INTO [SystemEventType] (EventType, EventTypeCD, Severity)
        VALUES('Unhandled Exception', 'uexc', 1),('Handled Exception', 'hexc', 2),('Custom Exception', 'cexc', 3),('Database Event', 'dbev', 4),('CSharp Event', 'csev', 4),('Networking Event', 'ntev', 4);
      ";

      /// <summary>
      /// Create main.SystemEvent
      /// </summary>
      internal const string Create_SystemEvent = @"
        CREATE TABLE [SystemEvent] (
            [SystemEventID] integer PRIMARY KEY NOT NULL,
						[EventTypeCD] ntext NOT NULL,
						[EventText] ntext NOT NULL,
            [Class] ntext,
            [Method] ntext,
            [LineNumber] integer,
						[InnerException] ntext,
            [StackTrace] ntext,
						[Data] ntext,
            [CreatedDBTime] datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
            [AuditDBTime] datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
            CONSTRAINT [FK_SystemEvent_EventTypeCD] FOREIGN KEY ([EventTypeCD]) REFERENCES [SystemEventType] ([EventTypeCD])
        );
        CREATE TRIGGER [trSystemEvent] AFTER UPDATE ON [SystemEvent] FOR EACH ROW
        BEGIN
          update SystemEvent set AuditDBTime = CURRENT_TIMESTAMP where SystemEventID = new.rowid;
        END;
      ";

      /// <summary>
      /// Create a view that returns all manga details
      /// </summary>
      internal const string Create_vsManga = @"
        create view vsManga
        /*
        Procedure:	vsManga
        Author:		Nagru / October 24, 2014

        Selects out all the relevant details of a manga. This is the base view upon
        which the program depends. Be careful.

        Examples:
        select * from vsManga limit 10
        */
        as

        select
          mgx.MangaID
					,ma.ArtistIDs
          ,ma.Artists
          ,mgx.Title
          ,mgx.PageCount
          ,mgx.PageReadCount
					,mt.TagIDs
          ,mt.Tags
          ,mgx.Description
          ,mgx.PublishedDate
          ,mgx.Location
          ,mgx.Thumbnail
          ,mgx.GalleryURL
          ,ifnull(tp.Type, '')    Type
          ,mgx.Rating
        from
          Manga mgx
        left outer join
          Type tp on tp.TypeID = mgx.TypeID
        left outer join
        (
          select MangaID, group_concat(ArtistID, ', ') ArtistIDs, group_concat(Name, ', ') Artists
          from 
          (
						select mga.MangaID, mga.ArtistID, art.Name
						from Artist art
						join MangaArtist mga on mga.ArtistID = art.ArtistID
						order by art.Name
          )
          group by MangaID
        ) ma on ma.MangaID = mgx.MangaID
        left outer join
        (
          select MangaID, group_concat(TagID, ', ') TagIDs, group_concat(Tag, ', ') Tags
          from 
          (
						select mgt.MangaID, mgt.TagID, tx.Tag
						from Tag tx
						join MangaTag mgt on mgt.TagID = tx.TagID
						order by tx.Tag
          )
          group by MangaID
        ) mt on mt.MangaID = mgx.MangaID
        group by mgx.MangaID
      ";

      /// <summary>
      /// Create a view that returns tag statistics
      /// </summary>
      internal const string Create_vsTagSummary = @"
        create view vsTagSummary
				/*
				Procedure:	vsTagSummary
				Author:		Nagru / October 31, 2014

				Returns statistics about all the tags in the database

				Examples:
				select * from vsTagSummary limit 50
				*/
				as

				select
					tg.Tag
					,mtg.RawTagCount
					,(select count(MangaID) from Manga) RawMangaCount
					,ftg.FavouriteTagCount
					,(select count(MangaID) from Manga where Rating = 5) FavouriteMangaCount
				from
					Tag tg
				left outer join
				(
					select
						TagID, count(TagID) RawTagCount
					from
						MangaTag
					group by
						TagID
				) mtg on tg.TagID = mtg.TagID
				left outer join
				(
					select
						TagID, count(TagID) FavouriteTagCount
					from
						MangaTag mt
					join
						Manga  mg on mt.MangaID = mg.MangaID
					group by
						TagID
				) ftg on tg.TagID = ftg.TagID
      ";

      #endregion

      #region Convenience

      /// <summary>
      /// Removes SQl characters from raw strings.
      /// Should be removed at some point in favour of parameters.
      /// </summary>
      /// <param name="sRaw">The string to clean up</param>
      /// <returns>Returns the passed in string cleansed of special characters</returns>
      internal static string Sanitize(string sRaw)
      {
        StringBuilder sb = new StringBuilder(sRaw);
        sb
          .Replace("'", "''")
          .Replace(";", "");
        return sb.ToString();
      }

      /// <summary>
      /// Wrapper around SQLiteParameter for convenience
      /// </summary>
      /// <param name="ParameterName">The name of the parameter</param>
      /// <param name="dbType">The type of object it should be</param>
      /// <param name="value">The value of the parameter</param>
      /// <returns>Returns a sqlite parameter instance</returns>
      internal static SQLiteParameter NewParameter(string ParameterName, DbType dbType, object value)
      {
        if (dbType == DbType.String
            && value != null
            && value.ToString().Length > MAX_LENGTH) {
          value = value.ToString().Substring(0, MAX_LENGTH);
          //xDialog.DisplayWarning(msg.TRUNCATED);
        }

        return new SQLiteParameter(ParameterName, dbType) {
          Value = value
        };
      }

      /// <summary>
      /// Begins a transaction.
      /// NOTE: SQLite ONLY SUPPORTS ONE TRANSACTION AT A TIME
      /// </summary>
      /// <returns>Returns whether the command was successful</returns>
      internal int BeginTransaction(bool SetGlobal = false)
      {
        int iRetVal = -1;

        if (!INGLOBALTRAN && !INLOCALTRAN) {
          INLOCALTRAN = true;
          INGLOBALTRAN = SetGlobal;
          if (this.HandledDisconnect()) {
            using (SQLiteCommand sqCmd = new SQLiteCommand("begin transaction", CONN)) {
              iRetVal = sqCmd.ExecuteNonQuery();
            }
          }
        }

        return iRetVal;
      }

      /// <summary>
      /// Ends the current transaction.
      /// </summary>
      /// <param name="Commit">Controls whether the transaction is committed or rolled back</param>
      /// <returns>Returns whether the command was successful</returns>
      internal int EndTransaction(bool Commit = true, bool EndGlobal = false)
      {
        int iRetVal = -1;
        if (INGLOBALTRAN) {
          INGLOBALTRAN = !EndGlobal;
        }

        if (!INGLOBALTRAN) {
          if (INLOCALTRAN) {
            INLOCALTRAN = false;
            if (this.HandledDisconnect()) {
              using (SQLiteCommand sqCmd = new SQLiteCommand((Commit ? "commit" : "rollback") + " transaction", CONN)) {
                iRetVal = sqCmd.ExecuteNonQuery();
              }
            }
          }
        }

        return iRetVal;
      }

      /// <summary>
      /// Wrapper around SQLite's ExecuteNonQuery for convenience
      /// </summary>
      /// <param name="CommandText">The SQL command to execute</param>
      /// <param name="cmd">The behaviour of the execution</param>
      /// <param name="sqParam">The parameters associated with the command</param>
      /// <returns>Returns the number of affected rows</returns>
      internal int ExecuteNonQuery(string CommandText, params SQLiteParameter[] sqParam)
      {
        int altered = -1;

        if (this.HandledDisconnect()) {
          using (SQLiteCommand sqCmd = new SQLiteCommand(CommandText, CONN)) {
            sqCmd.Parameters.AddRange(sqParam);

            try {
              altered = sqCmd.ExecuteNonQuery(CommandBehavior.Default);
            } catch (Exception) {
            }
          }
        }

        return altered;
      }

      /// <summary>
      /// Wrapper around SQLite's ExecuteQuery for convenience
      /// </summary>
      /// <param name="CommandText">The SQL command to execute</param>
      /// <param name="cmd">The behaviour of the execution</param>
      /// <param name="sqParam">The parameters associated with the command</param>
      /// <returns>Returns the results of the query command</returns>
      internal DataTable ExecuteQuery(string CommandText, params SQLiteParameter[] sqParam)
      {
        DataTable dt = new DataTable();

        if (this.HandledDisconnect()) {
          using (SQLiteCommand sqCmd = new SQLiteCommand(CommandText, CONN)) {
            sqCmd.Parameters.AddRange(sqParam);
            try {
              using (SQLiteDataReader dr = sqCmd.ExecuteReader(CommandBehavior.Default)) {
                dt.Load(dr);
              }
            } catch (Exception) {
            }
          }
        }

        return dt;
      }

      #endregion
    }

    /// <summary>
    /// Holds all the logic that updates, inserts, or queries information in the DB
    /// </summary>
    private static class SQLAccess
    {
      #region Search Entries

      /// <summary>
      /// Parses search terms based on an EH-like scheme and returns all results in the DB that match
      /// </summary>
      /// <param name="SearchTerms">The raw search string from the user</param>
      /// <param name="OnlyFav">Whether to only return results with a rating of 5.0</param>
      /// <param name="MangaID">Optional ability to only check a single manga</param>
      /// <returns>Returns a list of manga that match the criteria</returns>
      internal static DataTable Search(string SearchTerms, int MangaID = -1)
      {
        //validate and sanitize input
        SearchTerms = SQLBase.Sanitize(SearchTerms);
        if (string.IsNullOrWhiteSpace(SearchTerms)) {
          return GetManga(MangaID);
        }

        //set up variables
        Dictionary<string, Dictionary<string, bool>> dcTerms =
          new Dictionary<string, Dictionary<string, bool>>();

        #region split string into categories and entries
        if ((SearchTerms.Count(x => x == '"') % 2) != 0) {
          //xDialog.DisplayInfo(msg.SYNTAX_ERROR_APOSTROPHE);
        }
        else {
          bool bIsVerbatim = false;
          StringBuilder sb = new StringBuilder(SearchTerms);

          for (int i = 0; i < SearchTerms.Length; i++) {
            if (SearchTerms[i] == '"') {
              bIsVerbatim = !bIsVerbatim;
            }
            else if (bIsVerbatim) {
              sb[i] = Obfuscate(sb[i]);
            }
          }

          SearchTerms = sb.Replace("\"", "").ToString();
        }
        string[] asElements = Ext.Split(SearchTerms, " ");
        #endregion

        #region format sub entries
        for (int i = 0; i < asElements.Length; i++) {
          string[] asSubElements = Ext.Split(asElements[i], ":");
          string sKey = asSubElements.Length > 1 ? asSubElements[i] : "";

          if (!dcTerms.ContainsKey(sKey)) {
            dcTerms.Add(
              sKey
              , new Dictionary<string, bool>()
            );
          }

          string[] asSubSplit = Ext.Split(asSubElements[asSubElements.Length > 1 ? 1 : 0], ",");
          for (int x = 0; x < asSubSplit.Length; x++) {
            Unobfuscate(ref asSubSplit[x]);
            if (!dcTerms[sKey].ContainsKey(asSubSplit[x])) {
              dcTerms[sKey].Add(asSubSplit[x], asSubSplit[x].StartsWith("-"));
            }
          }
        }
        #endregion

        #region turn values into a SQL statement

        StringBuilder sbCmd = new StringBuilder(20000);
        sbCmd.AppendFormat("select * from vsManga where ({0} in (MangaID, -1)) "
          , MangaID
        );

        foreach (KeyValuePair<string, Dictionary<string, bool>> kvp in dcTerms) {
          foreach (KeyValuePair<string, bool> skvp in kvp.Value) {
            switch (kvp.Key) {
              case "artist":
              case "a":
                sbCmd.AppendFormat("and ifnull(Artists, '') {0} like '%{1}%' ESCAPE '\\' "
                  , skvp.Value ? "not" : ""
                  , skvp.Key);
                break;
              case "title":
              case "t":
                sbCmd.AppendFormat("and ifnull(Title, '') {0} like '%{1}%' ESCAPE '\\' "
                  , skvp.Value ? "not" : ""
                  , skvp.Key);
                break;
              case "tag":
              case "tags":
              case "g":
                sbCmd.AppendFormat("and ifnull(Tags, '') {0} like '%{1}%' ESCAPE '\\' "
                  , skvp.Value ? "not" : ""
                  , skvp.Key);
                break;
              case "description":
              case "desc":
              case "s":
                sbCmd.AppendFormat("and ifnull(Description, '') {0} like '%{1}%' ESCAPE '\\' "
                  , skvp.Value ? "not" : ""
                  , skvp.Key);
                break;
              case "type":
              case "y":
                sbCmd.AppendFormat("and ifnull(Type, '') {0} like '%{1}%' ESCAPE '\\' "
                  , skvp.Value ? "not" : ""
                  , skvp.Key);
                break;
              case "date":
              case "d":
                DateTime date = new DateTime();
                char c = !string.IsNullOrWhiteSpace(skvp.Key) ? skvp.Key[0] : ' ';

                if (DateTime.TryParse(skvp.Key.Substring(c != '<' && c != '>' ? 0 : 1), out date))
                  sbCmd.AppendFormat("and date(PublishedDate) {0} date('{1}') "
                    , skvp.Value ? '!' : (c == '<' || c == '>') ? c : '='
                    , date.ToString("yyyy-MM-dd"));
                break;
              case "rating":
              case "r":
                c = !string.IsNullOrWhiteSpace(skvp.Key) ? skvp.Key[0] : ' ';
                int rat;

                if (int.TryParse(skvp.Key.Substring(c != '<' && c != '>' ? 0 : 1), out rat))
                  sbCmd.AppendFormat("and Rating {0} {1} "
                    , skvp.Value ? '!' : (c == '<' || c == '>') ? c : '='
                    , rat);
                break;
              case "pages":
              case "page":
              case "p":
                c = !string.IsNullOrWhiteSpace(skvp.Key) ? skvp.Key[0] : ' ';
                int pg;

                if (int.TryParse(skvp.Key.Substring(c != '<' && c != '>' ? 0 : 1), out pg))
                  sbCmd.AppendFormat("and Pages {0} {1} "
                    , skvp.Value ? '!' : (c == '<' || c == '>') ? c : '='
                    , pg);
                break;
              default:
                if (skvp.Value) {
                  sbCmd.AppendFormat("and (ifnull(Tags, '') not like '%{0}%' ESCAPE '\\' and ifnull(Title, '') not like '%{0}%' ESCAPE '\\' and ifnull(Artists, '') not like '%{0}%' ESCAPE '\\' and ifnull(Description, '') not like '%{0}%' ESCAPE '\\' and ifnull(Type, '') not like '%{0}%' ESCAPE '\\' and date(PublishedDate) not like '%{0}%' ESCAPE '\\') "
                  , skvp.Key);
                }
                else {
                  sbCmd.AppendFormat("and (ifnull(Tags, '') like '%{0}%' ESCAPE '\\' or ifnull(Title, '') like '%{0}%' ESCAPE '\\' or ifnull(Artists, '') like '%{0}%' ESCAPE '\\' or ifnull(Description, '') like '%{0}%' ESCAPE '\\' or ifnull(Type, '') like '%{0}%' ESCAPE '\\' or date(PublishedDate) like '%{0}%' ESCAPE '\\') "
                  , skvp.Key);
                }

                break;
            }
          }
        }

        //sbCmd.Append(" ESCAPE '\\'");

        #endregion

        //execute statement
        return sqlConn.ExecuteQuery(sbCmd.ToString());
      }

      /// <summary>
      /// Preserve special characters by converting them into a 
      /// different char type before parsing the string
      /// </summary>
      /// <param name="RawChar">The char to obfuscate</param>
      /// <returns>Returns the obfuscated version of the char</returns>
      private static char Obfuscate(char RawChar)
      {
        switch (RawChar) {
          case ' ':
            RawChar = '╛';	//alt + 190 descending
            break;					//ugly but should be safe
          case ',':
            RawChar = '┐';
            break;
          case ':':
            RawChar = '└';
            break;
          case '_':
            RawChar = '┴';
            break;
          case '%':
            RawChar = '┬';
            break;
          case '?':
            RawChar = '├';
            break;
          case '*':
            RawChar = '─';
            break;
          default:
            break;
        }
        return RawChar;
      }

      /// <summary>
      /// Removes obfuscation and prepares a string for use in the SQL command
      /// </summary>
      /// <param name="RawString">The string to unobfuscate</param>
      private static void Unobfuscate(ref string RawString)
      {
        StringBuilder sb = new StringBuilder(RawString);
        RawString = sb
          .Replace('?', '_')
          .Replace('*', '%')
          .Replace('_', ' ')
          .Replace('╛', ' ')
          .Replace('┐', ',')
          .Replace('└', ':')
          .Replace("┴", "\\_")
          .Replace("┬", "\\%")
          .Replace('├', '?')
          .ToString();
      }

      #endregion

      #region Query Database

      /// <summary>
      /// Checks if a Table or View exists in the database
      /// </summary>
      /// <param name="ValueSource">The object to check for</param>
      /// <returns>Returns true if the object exists</returns>
      internal static DataTable GetValues(SQL.DB ValueSource)
      {
        if (SQLAccess.CheckExistence(ValueSource.ToString())) {
          string sCommandText = "select * from [" + ValueSource + "]";
          return sqlConn.ExecuteQuery(sCommandText);
        }
        else {
          return null;
        }
      }

      /// <summary>
      /// Selects out of the supplied data-source
      /// </summary>
      /// <param name="DatabaseObject">The object to select from</param>
      /// <returns>Returns the contents of a Table or View</returns>
      internal static bool CheckExistence(string DatabaseObject)
      {
        bool bExists = false;
        using (DataTable dt = sqlConn.ExecuteQuery(
              "select 1 from sqlite_master where name = @objectName and type in('table', 'view')",
              SQLBase.NewParameter("@objectName", DbType.String, DatabaseObject)
            )) {
          bExists = (dt.Rows.Count > 0);
        }
        return bExists;
      }

      /// <summary>
      /// Returns all manga entries
      /// </summary>
      /// <returns>Returns the results of a search against vsManga</returns>
      internal static DataTable GetManga(int iMangaID = -1)
      {
        string CommandText = string.Format("select * from vsManga ",
          iMangaID > -1 ? "where MangaID = " + iMangaID : "");
        return sqlConn.ExecuteQuery(CommandText);
      }

      /// <summary>
      /// Returns the full details of a specified manga
      /// </summary>
      /// <param name="mangaID">The PK of the manga record to find</param>
      /// <returns>Returns the results of vsManga for a specific manga</returns>
      internal static DataTable GetMangaEntry(int mangaID)
      {
        string sCommandText = "select * from vsManga where MangaID = @mangaID";

        return sqlConn.ExecuteQuery(sCommandText, new SQLiteParameter("@mangaID", DbType.Int32) {
          Value = mangaID
        }
        );
      }

      /// <summary>
      /// Check whether an entry already exists in the DB based on the Artist and Title
      /// </summary>
      /// <param name="sArtist">The Artist of the manga to check for</param>
      /// <param name="sTitle">The Title of the manga to check for</param>
      /// <returns>Returns whether an entry already exists with that artist and title</returns>
      internal static bool EntryExists(string sArtist, string sTitle)
      {
        bool bExists = false;
        string sCommandText = @"
					select 1 
					from vsManga 
					where
						Artists = @artist
					and
						Title = @title
				";

        using (DataTable dt = sqlConn.ExecuteQuery(sCommandText
            , SQLBase.NewParameter("@artist", DbType.String, sArtist)
            , SQLBase.NewParameter("@title", DbType.String, sTitle))) {
          bExists = dt.Rows.Count > 0;
        }

        return bExists;
      }

      /// <summary>
      /// Checks for existing records whose title has a 90% similarity to the value passed in
      /// </summary>
      /// <param name="RawTitle">The title of the manga to compare</param>
      /// <returns>Returns a list of potentially matching manga</returns>
      internal static DataTable EntryExists(string RawTitle)
      {
        DataTable dtMatch = new DataTable();
        using (DataTable dt = GetAllManga()) {
          for (int i = 0; i < dt.Rows.Count; i++) {
            if (Ext.SoerensonDiceCoef(RawTitle, dt.Rows[i]["Title"].ToString()) >= 0.90) {
              dtMatch.Rows.Add(dt.Rows[i]);
            }
          }
        }
        return dtMatch;
      }

      #endregion

      #region Update Database

      #region Update Manga Entry

      internal static int SavePageReadCount(int MangaID, int PageReadCount)
      {
        return sqlConn.ExecuteNonQuery("update Manga set PageReadCount = @pageReadCount where MangaID = @mangaID"
          , SQLBase.NewParameter("@pageReadCount", DbType.Int32, PageReadCount)
          , SQLBase.NewParameter("@mangaID", DbType.Int32, MangaID));
      }

      internal static int SaveMangaThumbnail(int MangaID, byte[] asBytes)
      {
        return sqlConn.ExecuteNonQuery("update Manga set Thumbnail = @thumbnail where MangaID = @mangaID"
          , SQLBase.NewParameter("@thumbnail", DbType.Object, asBytes)
          , SQLBase.NewParameter("@mangaID", DbType.Int32, MangaID));
      }

      /// <summary>
      /// Saves a manga record into the DB
      /// </summary>
      /// <param name="sArtist">The name of the Artist</param>
      /// <param name="sTitle">The title of the manga</param>
      /// <param name="dtPubDate">The date it was published</param>
      /// <param name="sTags">The tags associated with the manga</param>
      /// <param name="sLoc">The location on the HDD</param>
      /// <param name="iPages">The page count</param>
      /// <param name="sType">The manga type</param>
      /// <param name="dRating">The user's rating</param>
      /// <param name="sDesc">A description of the manga</param>
      /// <param name="sURL">The EH gallery the manga was downloaded from</param>
      /// <param name="iMangaID">The ID of the record if it already exists</param>
      internal static int DB_SaveEntry(string sArtist, string sTitle, DateTime dtPubDate,
          string sTags = null, string sLoc = null, decimal iPages = 0, int iPageReadCount = 0, string sType = null,
          decimal dRating = 0, string sDesc = null, string sURL = null, int iMangaID = -1)
      {
        if (!sqlConn.INGLOBALTRAN)
          sqlConn.BeginTransaction();

        //setup parameters
        StringBuilder sbCmd = new StringBuilder(10000);
        List<SQLiteParameter> lParam = new List<SQLiteParameter>(50);
        lParam.AddRange(new SQLiteParameter[12] {
        SQLBase.NewParameter("@mangaID", DbType.Int32, iMangaID)
          , SQLBase.NewParameter("@title", DbType.String, sTitle)
          , SQLBase.NewParameter("@name", DbType.String, sArtist)
          , SQLBase.NewParameter("@pages", DbType.Int32, Convert.ToInt32(iPages))
          , SQLBase.NewParameter("@pageReadCount", DbType.Int32, iPageReadCount)
          , SQLBase.NewParameter("@rating", DbType.Decimal, dRating)
          , SQLBase.NewParameter("@description", DbType.String, sDesc)
          , SQLBase.NewParameter("@location", DbType.String, sLoc)
          , SQLBase.NewParameter("@URL", DbType.String, sURL)
          , SQLBase.NewParameter("@pubDate", DbType.String, dtPubDate.ToString("yyyy-MM-dd"))
          , SQLBase.NewParameter("@name", DbType.String, sArtist)
          , SQLBase.NewParameter("@type", DbType.String, sType)
        });

        #region Update the base Manga record
        //determine whether to insert or update
        sbCmd.Append(@"
          update [Manga]
          set
            Title = @title
            ,PageCount = @pages
            ,PageReadCount = @pageReadCount
            ,Rating = @rating
            ,Description = case when @description <> '' then @description else null end
            ,Location = case when @location <> '' then @location else null end
            ,galleryURL = case when @URL <> '' then @URL else null end
            ,publishedDate = @pubDate
          where MangaID = @mangaID;

          insert into [Manga](title, pagecount, pagereadcount, rating, description, location, galleryURL, publishedDate)
          select
            @title
            ,@pages
            ,@pageReadCount
            ,@rating
            ,case when @description <> '' then @description else null end
            ,case when @location <> '' then @location else null end
            ,case when @URL <> '' then @URL else null end
            ,@pubDate
          where changes() = 0;
        ");
        #endregion

        #region Update the peripheral records
        //set the mangaID parameter if necessary
        if (iMangaID == -1) {
          sqlConn.ExecuteNonQuery(sbCmd.ToString(), lParam.ToArray());
          sbCmd.Clear();

          using (DataTable dt = sqlConn.ExecuteQuery("select max(MangaID) from Manga")) {
            iMangaID = Int32.Parse(dt.Rows[0][0].ToString());
            lParam[0].Value = iMangaID;
          }
        }

        //update the artist and manga type
        sbCmd.Append(@"
        --insert the manga artist
        insert into [Artist](Name)
        select @name
        where 
          @name <> ''
        and
          not exists(select 1 from [Artist] where Name = @name);

        insert into [MangaArtist](MangaID, ArtistID)
        select @mangaID, ArtistID
        from [Artist] art
        where 
          Name = @name 
        and 
          not exists(select 1 from [MangaArtist] mat where MangaID = @mangaID and mat.ArtistID = art.ArtistID);

        delete from [MangaArtist] 
        where MangaID = @mangaID and ArtistID <> (
          select ArtistID from [Artist] where Name = @name);

        --insert/update the manga type
        insert into [Type](Type)
        select @type
        where 
          @type <> ''
        and
          not exists(select 1 from [Type] where Type = @type);

        update [Manga]
        set TypeID = case when @type = '' then null else (select TypeID from [Type] where Type = @type) end 
        where 
            MangaID = @mangaID 
        and 
          (TypeID is null or TypeID <> (select TypeID from [Type] where Type = @type));"
        );

        //Update the tags
        string[] asTags = SQLBase.Sanitize(sTags).Split(',').Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
        sbCmd.AppendFormat("insert or ignore into [Tag](Tag) values {0};",
          string.Format("('{0}')", String.Join("'),('", asTags)));
        sbCmd.Append("delete from [MangaTag] where MangaID = @mangaID;");
        sbCmd.AppendFormat("insert into [MangaTag](MangaID, TagID) select @mangaID, TagID from [Tag] where Tag in ({0});",
          string.Format("'{0}'", String.Join("','", asTags)));

        //run the generated command statement
        sqlConn.ExecuteNonQuery(sbCmd.ToString(), lParam.ToArray());
        #endregion

        if (!sqlConn.INGLOBALTRAN)
          sqlConn.EndTransaction();
        return iMangaID;
      }

      /// <summary>
      /// Deletes an entry from the DB
      /// </summary>
      /// <param name="iMangaID">The PK of the record to delete</param>
      /// <returns>Returns the number of affected rows</returns>
      internal static int DeleteEntry(int iMangaID)
      {
        sqlConn.BeginTransaction();
        SQLiteParameter sqParam = SQLBase.NewParameter("@mangaID", DbType.Int32, iMangaID);

        string sCommandText = @"
					delete from MangaArtist
					where MangaID = @mangaID;
					delete from MangaTag
					where MangaID = @mangaID;
					delete from Manga
					where MangaID = @mangaID;";
        int altered = sqlConn.ExecuteNonQuery(sCommandText, sqParam);

        sqlConn.EndTransaction();
        return altered;
      }

      #endregion

      /// <summary>
      /// Updates a program setting
      /// </summary>
      /// <param name="DBSetting">The setting to update</param>
      /// <param name="value">The new value for the setting</param>
      /// <returns>Returns the number of affected rows</returns>
      internal static int UpdateSetting(Setting DBSetting, object value)
      {
        //setup parameters
        SQLiteParameter sqParam = null;

        switch (DBSetting) {
          case Setting.BackgroundColour:
          case Setting.DBversion:
          case Setting.member_id:
          case Setting.RowColourAlt:
          case Setting.RowColourHighlight:
          case Setting.ShowDate:
          case Setting.ShowGrid:
            sqParam = SQLBase.NewParameter("@value", DbType.Int32, int.Parse(value.ToString()));
            break;
          case Setting.FormPosition:
          case Setting.GallerySettings:
          case Setting.ImageBrowser:
          case Setting.Notes:
          case Setting.pass_hash:
          case Setting.RootPath:
          case Setting.SearchIgnore:
          case Setting.TagIgnore:
            sqParam = SQLBase.NewParameter("@value", DbType.String, value.ToString());
            break;
        }

        //run the command
        return sqlConn.ExecuteNonQuery(
          string.Format("UPDATE [Settings] SET {0} = @value", DBSetting.ToString())
          , sqParam
        );
      }

      /// <summary>
      /// Re-creates the MangaTag table to normalize the PK values
      /// </summary>
      /// <returns>Returns the number of affected rows</returns>
      internal static int RecycleMangaTag()
      {
        sqlConn.BeginTransaction();
        StringBuilder sbCmd = new StringBuilder(10000);
        sbCmd.Append("ALTER TABLE MangaTag RENAME TO TempMangaTag;");
        sbCmd.Append(SQLBase.Create_MangaTag);
        sbCmd.Append(@"
					INSERT INTO MangaTag(MangaID, TagID, TagNamespaceID)
					SELECT MangaID, TagID, TagNamespaceID
					FROM TempMangaTag;
				");
        sbCmd.Append("DROP TABLE TempMangaTag;");
        int iAltered = sqlConn.ExecuteNonQuery(sbCmd.ToString());
        sqlConn.EndTransaction();

        return iAltered;
      }

      /// <summary>
      /// Removes tags from the DB with no associated manga
      /// </summary>
      /// <returns>Returns the number of affected rows</returns>
      internal static int DeleteUnusedTags()
      {
        sqlConn.BeginTransaction();

        string sCommandText = @"
					delete from Tag
					where TagID not in 
					(select TagID from MangaTag)";
        int altered = sqlConn.ExecuteNonQuery(sCommandText);

        sqlConn.EndTransaction();
        return altered;
      }

      /// <summary>
      /// Removes tags from the DB with no associated manga
      /// </summary>
      /// <returns>Returns the number of affected rows</returns>
      internal static int DeleteUnusedArtists()
      {
        sqlConn.BeginTransaction();

        string sCommandText = @"
					delete from Artist
					where ArtistID not in 
					(select ArtistID from MangaArtist)";
        int altered = sqlConn.ExecuteNonQuery(sCommandText);

        sqlConn.EndTransaction();
        return altered;
      }

      #endregion
    }
  }
}
