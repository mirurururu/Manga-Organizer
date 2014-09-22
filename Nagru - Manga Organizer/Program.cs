using System;
using System.Windows.Forms;
using System.Threading;
using System.Reflection;

namespace Nagru___Manga_Organizer
{
  static class Program
  {
    [STAThread]
    static void Main(string[] args)
    {
      //load in embedded dlls
      const string resx = "Nagru___Manga_Organizer.Resources.";
      string[] dll = new string[3] { 
        "System.Data.SQLite.dll", 
        "SharpCompress.dll", 
        "Newtonsoft.Json.dll"
			};
      for (int i = 0; i < dll.Length; i++) {
        EmbeddedAssembly.Load(resx + dll[i], dll[i]);
      }

      AppDomain.CurrentDomain.AssemblyResolve
        += new ResolveEventHandler(CurrentDomain_AssemblyResolve);

      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Application.Run(new Main(args));
    }

    static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
    {
      return EmbeddedAssembly.Get(args.Name);
    }
  }
}
