using System;
using System.IO;
using System.Collections.Generic;
using System.Security.Permissions;

namespace Nagru___Manga_Organizer
{
	public static class ExtDir
	{
		/// <summary>
		/// Extends Directory.GetFiles to support multiple filters
		/// </summary>
		/// <remarks>Inspiration: Bean Software (2002-2008)</remarks>
		/// <param name="SourceFolder"></param>
		/// <param name="SearchOption"></param>
		/// <param name="Filter"></param>
		/// <returns></returns>
		public static string[] GetFiles(string SourceFolder,
				SearchOption SearchOption = SearchOption.AllDirectories,
				string Filter = "*.jpg|*.png|*.jpeg|*.gif")
		{
			if (!Directory.Exists(SourceFolder))
				return new string[0];
			List<string> lFiles = new List<string>(10000);
			string[] sFilters = Filter.Split('|');

			try {
				for (int i = 0; i < sFilters.Length; i++)
					lFiles.AddRange(Directory.EnumerateFiles(SourceFolder,
							sFilters[i], SearchOption));
			} catch (ArgumentException) {
				Console.WriteLine("Invalid characters in path:\n" + SourceFolder);
			} catch (UnauthorizedAccessException) {
				Console.WriteLine("User does not have access to:\n" + SourceFolder);
			}

			lFiles.Sort(new TrueCompare());
			return lFiles.ToArray();
		}

		/// <summary>
		/// Ensure chosen folder is not protected before operating
		/// </summary>
		/// <param name="Path"></param>
		/// <returns></returns>
		public static bool Accessible(string Path)
		{
			if (!Directory.Exists(Path))
				return false;

			try {
				string[] asDirs = Directory.GetDirectories(Path, "*",
						SearchOption.TopDirectoryOnly);
				FileIOPermission fp;

				for (int i = 0; i < asDirs.Length; i++) {
					fp = new FileIOPermission(FileIOPermissionAccess.Read |
							FileIOPermissionAccess.Write, asDirs[i]);
					fp.Demand();
				}
			} catch {
				return false;
			}

			return true;
		}
	}
}
