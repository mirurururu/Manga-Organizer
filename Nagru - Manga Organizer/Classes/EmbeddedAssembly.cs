using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;

/// <summary>
/// Handles loading embedded DLLs
/// Written by adriancs, 16 January 2013
/// </summary>
public class EmbeddedAssembly
{
  // Version 1.3
  static Dictionary<string, Assembly> dic = null;

  public static void Load(string embeddedResource, string fileName)
  {
    if (dic == null)
      dic = new Dictionary<string, Assembly>();

    byte[] ba = null;
    Assembly asm = null
      , curAsm = Assembly.GetExecutingAssembly();

    using (Stream stm = curAsm.GetManifestResourceStream(embeddedResource)) {
      // Either the file doesn't exist or it isn't marked as an embedded resource
      if (stm == null)
        throw new Exception(embeddedResource + " is not found in Embedded Resources.");

      // Get byte[] from the embedded resource
      ba = new byte[(int)stm.Length];
      stm.Read(ba, 0, (int)stm.Length);
      try {
        asm = Assembly.Load(ba);

        // Add the assembly/dll into the dictionary
        dic.Add(asm.FullName, asm);
        return;
      } catch {
        // Purposely do nothing
        // Unmanaged dll or assembly cannot be loaded directly from byte[]
        // Let the process fall through for next part
      }
    }

    bool fileOk = false;
    string tempFile = "";

    using (SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider()) {
      string fileHash = BitConverter.ToString(sha1.ComputeHash(ba)).Replace("-", string.Empty);
      tempFile = Path.GetTempPath() + fileName;

      if (File.Exists(tempFile)) {
        byte[] bb = File.ReadAllBytes(tempFile);
        string fileHash2 = BitConverter.ToString(sha1.ComputeHash(bb)).Replace("-", string.Empty);

        fileOk = (fileHash == fileHash2);
      }
      else {
        fileOk = false;
      }
    }

    if (!fileOk) {
      System.IO.File.WriteAllBytes(tempFile, ba);
    }

    asm = Assembly.LoadFile(tempFile);

    dic.Add(asm.FullName, asm);
  }

  public static Assembly Get(string assemblyFullName)
  {
    if (dic != null && dic.Count > 0) {
      if (dic.ContainsKey(assemblyFullName))
        return dic[assemblyFullName];
    }

    return null;
  }
}