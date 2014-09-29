using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Nagru___Manga_Organizer
{
  /// <summary>
  /// Implementation of custom serialization method
  /// </summary>
  /// <remarks>Author: sdktsg, (Jan 9, 2008)</remarks>
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
        if (stream != null)
          stream.Close();
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
  }
}
