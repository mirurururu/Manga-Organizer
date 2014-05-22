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
			EmbeddedAssembly.Load(
				"Nagru___Manga_Organizer.Resources.System.Data.SQLite.dll"
				, "System.Data.SQLite.dll");
			EmbeddedAssembly.Load(
				"Nagru___Manga_Organizer.Resources.SharpCompress.dll"
				, "SharpCompress.dll");

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
