using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Nagru___Manga_Organizer
{
    /* Implementation of custom serialization method
       Author: sdktsg, (Jan 9, 2008)                         */
    public static class FileSerializer
    {
        public static void Serialize(string sFilepath, object obj)
        {
            if (obj == null)
                throw new ArgumentNullException("Object cannot be null");

            Stream stream = null;
            try {
                stream = File.Open(sFilepath, FileMode.Create);
                BinaryFormatter bFormatter = new BinaryFormatter();
                bFormatter.Serialize(stream, obj);
            } catch {
            } finally { 
                if (stream != null) stream.Close(); 
            }
        }

        public static T Deserialize<T>(string sFilepath)
        {
            T obj = default(T);
            Stream stream = null;

            try {
                stream = File.Open(sFilepath, FileMode.Open);
                BinaryFormatter bFormatter = new BinaryFormatter();
                obj = (T)bFormatter.Deserialize(stream);
            } catch {
            } finally { 
                if (stream != null) 
                    stream.Close(); 
            }

            return obj;
        }

        public static List<Main.csEntry> ConvertDB(string sPath)
        {
            List<Main.stEntry> lOld = Deserialize<List<Main.stEntry>>(sPath);
            if (lOld.Count == 0) return null;
            
            //backup old database
            Serialize(sPath.Substring(0, sPath.Length - 4) + "_OldBK.bin", lOld);

            //convert to new format
            List<Main.csEntry> lNew = new List<Main.csEntry>(lOld.Count);
            for(int i = 0; i < lOld.Count; i++) {
                int iRating = lOld[i].bFav ? 5 : 3;
                lNew.Add(new Main.csEntry(lOld[i].sArtist, lOld[i].sTitle, 
                    lOld[i].sLoc, lOld[i].sDesc, lOld[i].sTags, lOld[i].sType, 
                    lOld[i].dtDate, lOld[i].iPages, iRating));
            }
            return lNew;
        }
    }
}
