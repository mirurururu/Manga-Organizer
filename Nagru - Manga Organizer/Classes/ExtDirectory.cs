using System;
using System.IO;
using System.Collections.Generic;
using System.Security.Permissions;

namespace Nagru___Manga_Organizer
{
    public static class ExtDirectory
    {
        /* Extends Directory.GetFiles to support multiple filters
           Author: Bean Software (2002-2008)                            */
        public static string[] GetFiles(string SourceFolder,
            SearchOption SearchOption = SearchOption.AllDirectories,
            string Filter = "*.jpg|*.png|*.jpeg")
        {
            if (!Directory.Exists(SourceFolder)) return new string[0];
            List<string> lFiles = new List<string>();
            string[] sFilters = Filter.Split('|');

            try
            {
                //for each filter find matching file names
                for (int i = 0; i < sFilters.Length; i++)
                    lFiles.AddRange(System.IO.Directory.GetFiles(SourceFolder,
                        sFilters[i], SearchOption));
            }
            catch (UnauthorizedAccessException) { }

            lFiles.Sort(new TrueCompare());
            return lFiles.ToArray();
        }

        /* Delete passed directory, including all files/subfolders  */
        public static void Delete(Object obj)
        {
            string sPath = obj as string;

            //delete all sub-files
            string[] asSub = Directory.GetFiles(sPath);
            for (int i = 0; i < asSub.Length; i++)
                File.Delete(asSub[i]);

            //delete all subfolders
            asSub = Directory.GetDirectories(sPath);
            for (int i = 0; i < asSub.Length; i++)
                Delete(asSub[i]);

            //delete current folder
            Directory.Delete(sPath);
        }

        /* Ensure chosen folder is not protected before operating */
        public static bool Restricted(string Path)
        {
            if (!Directory.Exists(Path)) return true;

            try
            {
                string[] asDirs = Directory.GetDirectories(Path, "*", SearchOption.TopDirectoryOnly);
                FileIOPermission fp;

                for (int i = 0; i < asDirs.Length; i++)
                {
                    fp = new FileIOPermission(FileIOPermissionAccess.Read |
                        FileIOPermissionAccess.Write, asDirs[i]);
                    fp.Demand();
                }
            }
            catch (Exception) { return true; }
            
            return false;
        }
    }
}
