using System;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Security.Permissions;

namespace Nagru___Manga_Organizer
{
    /* Fixes default's broken Update (it no longer acts as a refresh)
       Author: geekswithblogs.net (Feb 27, 2006) */
    class ListViewNF : System.Windows.Forms.ListView
    {
        public ListViewNF()
        {
            //Activate double buffering
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

            //Enable the OnNotifyMessage event so we get a chance to filter out 
            //Windows messages before they get to the form's WndProc
            this.SetStyle(ControlStyles.EnableNotifyMessage, true);
        }

        //Filter out the WM_ERASEBKGND message
        protected override void OnNotifyMessage(Message m)
        {
            if (m.Msg != 0x14) base.OnNotifyMessage(m);
        }
    }

    /* Fixes default's broken AutoWordSelection method
       Author: Hans Passant (Sep 9, 2010)                              */
    public class FixedRichTextBox : RichTextBox
    {
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (!base.AutoWordSelection)
            {
                base.AutoWordSelection = true;
                base.AutoWordSelection = false;
            }
        }
    }

    /* Implementation of custom serialization method
       Author: sdktsg, (Jan 9, 2008)                         */
    public static class FileSerializer
    {
        //serialize passed object
        public static void Serialize(string sFilepath, object Obj)
        {
            if (Obj == null)
                throw new ArgumentNullException("Object cannot be null");

            Stream stream = null;
            try
            {
                stream = File.Open(sFilepath, FileMode.Create);
                BinaryFormatter bFormatter = new BinaryFormatter();
                bFormatter.Serialize(stream, Obj);
            }
            finally { if (stream != null) stream.Close(); }
        }

        //deserialize passed file
        public static T Deserialize<T>(string sFilepath)
        {
            T obj = default(T);
            Stream stream = null;

            try
            {
                stream = File.Open(sFilepath, FileMode.Open);
                BinaryFormatter bFormatter = new BinaryFormatter();
                obj = (T)bFormatter.Deserialize(stream);
            }
            catch (Exception ex)
            {
                MessageBox.Show("The application failed to retrieve the inventory:\n"
                    + ex.Message);
            }
            finally { if (stream != null)stream.Close(); }

            return obj;
        }

    }

    /* Provides predicate with ability to accept parameters
       Author: Alex Perepletov (Aug 1, 2006)                     */
    public class Search
    {
        private string sTitle;

        /* Sets a different suffix to match   */
        public string Title
        {
            get { return sTitle; }
            set { sTitle = value; }
        }

        /* Name: Match
           Desc: Gets the predicate for use with custom entry   */
        public Predicate<Main.stEntry> Match
        { get { return IsMatch; } }

        /* Returns true if Entries match   */
        private bool IsMatch(Main.stEntry en)
        { return (en.sArtist + en.sTitle).Equals(sTitle); }

        /* Initialize with suffix we want to match   */
        public Search(string Title)
        { sTitle = Title; }
    }

    /* Handles sorting of ListView columns
       Author: Microsoft (March 13, 2008)          */
    public class LVSorter : IComparer
    {
        public int ColToSort;                    //specifies column to sort
        public SortOrder OrdOfSort;              //specifies order sorting

        /* Initializes column & order  */
        public LVSorter()
        {
            ColToSort = 0;
            OrdOfSort = SortOrder.Ascending;
        }

        /* Swaps current direction of column sorting   */
        public void SwapOrder()
        {
            if (OrdOfSort == SortOrder.Ascending) OrdOfSort = SortOrder.Descending;
            else OrdOfSort = SortOrder.Ascending;
        }

        /* Compare ListViewItems (accounts for numbers)
           Author: Samuel Allen (2012)                         */
        public int Compare(object x, object y)
        {
            if (x == null || y == null) return 0;
            ListViewItem lviX = (ListViewItem)x;
            ListViewItem lviY = (ListViewItem)y;

            //choose sorting style
            string sX, sY;
            switch (ColToSort)
            {
                case 0:
                    sX = lviX.SubItems[0].Text + lviX.SubItems[1].Text;
                    sY = lviY.SubItems[0].Text + lviY.SubItems[1].Text;
                    break;
                case 1:
                    sX = lviX.SubItems[1].Text + lviX.SubItems[0].Text;
                    sY = lviY.SubItems[1].Text + lviY.SubItems[0].Text;
                    break;
                default:
                    sX = lviX.SubItems[ColToSort].Text;
                    sY = lviY.SubItems[ColToSort].Text;
                    break;
            }
            int iLenX = sX.Length, iLenY = sY.Length;
            int iMarkX = 0, iMarkY = 0;

            //go through strings with two markers.
            while (iMarkX < iLenX && iMarkY < iLenY)
            {
                char cX = sX[iMarkX], cY = sY[iMarkY];

                //buffers for characters
                char[] acSpaceX = new char[iLenX], acSpaceY = new char[iLenY];
                int iLocX = 0, iLocY = 0;

                //walk through characters in both strings to fill char array
                do
                {
                    acSpaceX[iLocX++] = cX;
                    iMarkX++;

                    if (iMarkX < iLenX) cX = sX[iMarkX];
                    else break;
                }
                while (char.IsDigit(cX) == char.IsDigit(acSpaceX[0]));

                do
                {
                    acSpaceY[iLocY++] = cY;
                    iMarkY++;

                    if (iMarkY < iLenY) cY = sY[iMarkY];
                    else break;
                }
                while (char.IsDigit(cY) == char.IsDigit(acSpaceY[0]));

                //if numbers compare numerically, else compare alphabetically.
                string sCmpX = new string(acSpaceX), sCmpY = new string(acSpaceY);
                int result;

                if (char.IsDigit(acSpaceX[0]) && char.IsDigit(acSpaceY[0]))
                {
                    int iNumChunkX = int.Parse(sCmpX), iNumChunkY = int.Parse(sCmpY);
                    result = iNumChunkX.CompareTo(iNumChunkY);
                }
                else result = sCmpX.CompareTo(sCmpY);

                if (result != 0)
                {
                    if (OrdOfSort == SortOrder.Ascending) return result;
                    else return (-result);
                }
            }

            return iLenX - iLenY;
        }
    }

    /* Extends Directory.GetFiles to support multiple filters
       Author: Bean Software (2002-2008)                            */
    public static class ExtDirectory
    {
        public static List<string> GetFiles(string SourceFolder,
            string Filter = "*.jpg|*.png|*.bmp|*.jpeg",
            SearchOption SearchOption = SearchOption.AllDirectories)
        {
            List<string> lFiles = new List<string>();
            string[] sFilters = Filter.Split('|');

            //for each filter find matching file names
            for (int i = 0; i < sFilters.Length; i++)
                lFiles.AddRange(System.IO.Directory.GetFiles(SourceFolder,
                    sFilters[i], SearchOption));

            lFiles.Sort();
            return lFiles;
        }

        /* Delete passed directory, including all files/subfolders 
           Author: Jeremy Edwards (Nov 30, 2008)                */
        public static void Delete(Object obj)
        {
            string sPath = obj as string;

            //remove any readonly setting, then delete file
            string[] asFiles = Directory.GetFiles(sPath);
            for (int i = 0; i < asFiles.Length; i++)
            {
                File.SetAttributes(asFiles[i], FileAttributes.Normal);
                File.Delete(asFiles[i]);
            }

            //delete all subfolders
            string[] asDirs = Directory.GetDirectories(sPath);
            for (int i = 0; i < asDirs.Length; i++)
                Delete(asDirs[i]);

            //delete current folder
            Directory.Delete(sPath, false);
        }

        /* Ensure chosen folder is not protected before operating 
           Author: Me */
        public static bool Restricted(string Path)
        {
            try
            {
                string[] asDirs = Directory.GetDirectories(Path, "*", SearchOption.AllDirectories);
                FileIOPermission fp;

                for (int i = 0; i < asDirs.Length; i++)
                {
                    fp = new FileIOPermission(FileIOPermissionAccess.Read |
                        FileIOPermissionAccess.Write, asDirs[i]);
                    fp.Demand();
                }

                return false;
            }
            catch (Exception)
            { return true; }
        }
    }

    /* Extends String.Contains to support case options & multiple filters  */
    public static class ExtString
    {
        /* Author: JaredPar (Jan 14, 2009) */
        //public static bool Contains(this string source, string toCheck, StringComparison Comparer)
        //{ return source.IndexOf(toCheck, Comparer) >= 0; }

        public static bool Contains(this string sSource, string sCheck, StringComparison scComp)
        {
            string[] sFilters = sCheck.Split('\0');
            for (int i = 0; i < sFilters.Length; i++)
                if ((sSource.IndexOf(sFilters[i], scComp) >= 0))
                    return true;

            return false;
        }
    }
}
