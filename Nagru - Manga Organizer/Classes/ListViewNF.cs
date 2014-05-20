using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Nagru___Manga_Organizer
{
	/* Fixes default's broken Update (it no longer acts as a refresh)
		 Author: geekswithblogs.net (Feb 27, 2006) */
	class ListViewNF : System.Windows.Forms.ListView
	{
		public ListViewNF()
		{
			//Activate double buffering
			this.SetStyle(ControlStyles.OptimizedDoubleBuffer
					| ControlStyles.AllPaintingInWmPaint, true);

			/* Enable the OnNotifyMessage event so we get a chance to filter out 
				 Windows messages before they get to the form's WndProc   */
			this.SetStyle(ControlStyles.EnableNotifyMessage, true);
		}

		//Filter out the WM_ERASEBKGND message
		protected override void OnNotifyMessage(Message m)
		{
			if (m.Msg != 0x14)
				base.OnNotifyMessage(m);
		}

		/* Give listview WindowsExplorer style
			 Author: Zach Johnson (Mar 27, 2010) */
		[DllImport("uxtheme.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		internal static extern int SetWindowTheme(IntPtr hWnd, string appName, string partList);
		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e);
			SetWindowTheme(this.Handle, "explorer", null);
		}
	}
}
