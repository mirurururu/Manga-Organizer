using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.IO;
using SCA = SharpCompress.Archive;

namespace Nagru___Manga_Organizer
{
	public partial class Browse_Img : Form, IDisposable
	{
		public Dictionary<int, int> Sort = new Dictionary<int, int>();
		public List<string> Files
		{
			get;
			set;
		}
		public SCA.IArchiveEntry[] Archive
		{
			get;
			set;
		}
		public int Page
		{
			get;
			set;
		}

		bool bWideL, bWideR, bNext;
		float fWidth;

		Timer trFlip;
		bool bAuto = false;

		public Browse_Img()
		{
			InitializeComponent();
		}

		private void Browse_Load(object sender, EventArgs e)
		{
			this.Location = Main.ActiveForm.Location;

			//Compensate for zip files being improperly sorted
			Files.Sort(new TrueCompare());
			if (Archive != null) {
				for (int x = 0; x < Archive.Length; x++) {
					for (int y = 0; y < Archive.Length; y++) {
						if (Archive[x].FilePath.Equals(Files[y])) {
							Sort.Add(y, x);
							break;
						}
					}
				}
			}

			//set up timer
			trFlip = new Timer();
			trFlip.Tick += trFlip_Tick;
			trFlip.Interval = Properties.Settings.Default.Interval;

#if !DEBUG
            Cursor.Hide();
            Bounds = Screen.GetWorkingArea(this.picBx);
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
#endif

			picBx.BackColor = Properties.Settings.Default.DefColour;
			fWidth = (float)(Bounds.Width / 2.0);

		}

		private void Browse_Shown(object sender, EventArgs e)
		{
			if (Page == -1)
				Next();
			else
				Prev();
		}

		private void Browse_KeyDown(object sender, KeyEventArgs e)
		{
			if (bAuto && e.Modifiers == Keys.Shift) {
				if (e.KeyCode == Keys.Oemplus) {
					bAuto = true;
					Console.Beep(700, 100);
					trFlip.Interval += 500;
					Tmr_Reset();
				}
				else if (e.KeyCode == Keys.OemMinus) {
					if (trFlip.Interval >= 1500) {
						bAuto = true;
						Console.Beep(100, 100);
						trFlip.Interval -= 500;
						Tmr_Reset();
					}
				}
				return;
			}

			switch (e.KeyCode) {
				#region Traversal
				case Keys.Left:
					Next();
					break;
				case Keys.Right:
					Prev();
					break;
				case Keys.Up:
					if ((Page += 7) > Files.Count)
						Page = Page - Files.Count;
					Next();
					break;
				case Keys.Down:
					if ((Page -= 7) < 0)
						Page = Files.Count + Page;
					Prev();
					break;
				case Keys.Home:
					Page = -1;
					Next();
					break;
				case Keys.End:
					Page = Files.Count + 1;
					Prev();
					break;
				#endregion
				#region Special Functions
				case Keys.F:
					if (bAuto)
						trFlip.Stop();
					Cursor.Show();
					BrowseTo fmGoTo = new BrowseTo(this);
					fmGoTo.iPage = (bWideR || bWideL) ?
							Page : Page - 1;

					if (fmGoTo.ShowDialog() == DialogResult.OK) {
						bNext = true;
						Page = fmGoTo.iPage - 1;
						Next();
					}

					fmGoTo.Dispose();
					if (bAuto)
						trFlip.Start();
					this.Select();
					Cursor.Hide();
					break;
				case Keys.S:
					bAuto = !bAuto;
					if (bAuto) {
						Console.Beep(500, 100);
						trFlip.Start();
					}
					else {
						Console.Beep(300, 100);
						trFlip.Stop();
					}
					break;
				#endregion
				#region Ignored Keys
				case Keys.PrintScreen:
				case Keys.MediaNextTrack:
				case Keys.MediaPreviousTrack:
				case Keys.MediaPlayPause:
				case Keys.MediaStop:
				case Keys.VolumeUp:
				case Keys.VolumeDown:
				case Keys.VolumeMute:
				case Keys.LWin:
				case Keys.RWin:
					break;
				#endregion
				default:
					Close();
					break;
			}
		}

		private void Next()
		{
			Image imgL = null, imgR = null;
			byte by = 0;
			Reset(true);

			do {
				by++;
				if (++Page >= Files.Count)
					Page = 0;
				imgR = TrySet(Page);
			} while (imgR == null && by < 10);

			if (imgR == null || !(bWideR = imgR.Height < imgR.Width)) {
				by = 0;
				do {
					by++;
					if (++Page >= Files.Count)
						Page = 0;
					imgL = TrySet(Page);
				} while (imgL == null && by < 10);

				if (imgL != null && (bWideL = imgL.Height < imgL.Width))
					Page--;
			}

			Refresh(imgL, imgR);
			if (imgL != null)
				imgL.Dispose();
			if (imgR != null)
				imgR.Dispose();
		}

		private void Prev()
		{
			if (Page != 0 && !(bWideR || bWideL))
				Page--;
			Image imgL = null, imgR = null;
			byte by = 0;
			Reset(false);

			do {
				by++;
				if (--Page < 0)
					Page = Files.Count - 1;
				else if (Page >= Files.Count)
					Page = 0;
				imgL = TrySet(Page);
			} while (imgL == null && by < 10);

			if (imgL == null || !(bWideL = imgL.Height < imgL.Width)) {
				by = 0;
				do {
					by++;
					if (--Page < 0)
						Page = Files.Count - 1;
					imgR = TrySet(Page);
				} while (imgR == null && by < 10);

				Page++;
				bWideR = (imgR != null) ? imgR.Height < imgR.Width : false;
			}

			Refresh(imgL, imgR);
			if (imgL != null)
				imgL.Dispose();
			if (imgR != null)
				imgR.Dispose();
		}

		private Bitmap TrySet(int i)
		{
			Bitmap bmpTmp = null;
			MemoryStream ms = new MemoryStream();

			try {
				if (Archive != null) {
					Archive[Sort[i]].WriteTo(ms);
				}
				else {
					FileStream fs = new FileStream(Files[i], FileMode.Open);
					fs.CopyTo(ms);
					fs.Dispose();
				}

				bmpTmp = new Bitmap(ms);
				bmpTmp = ExtImage.Scale(bmpTmp,
						(bmpTmp.Width > bmpTmp.Height) ? picBx.Width : fWidth
						, picBx.Height);
			} catch (Exception ex) {
				Console.WriteLine(ex.Message);
			} finally {
				ms.Dispose();
			}

			return bmpTmp;
		}

		private void Reset(bool bSet)
		{
			if (bAuto)
				Tmr_Reset();
			bWideL = false;
			bWideR = false;
			bNext = bSet;
			GC.Collect(0);
		}
		private void Tmr_Reset()
		{
			trFlip.Stop();
			trFlip.Start();
		}

		void trFlip_Tick(object sender, EventArgs e)
		{
			Next();
		}

		/* Process which images to draw & how */
		private void Refresh(Image imgL, Image imgR)
		{
			picBx.Refresh();

			picBx.SuspendLayout();
			Graphics g = picBx.CreateGraphics();
			g.InterpolationMode = InterpolationMode.HighQualityBicubic;

			if (!bWideL && !bWideR) {
				DrawImage_L(g, imgL);
				DrawImage_R(g, imgR);
			}
			else if (bNext)
				DrawImage_R(g, imgR);
			else
				DrawImage_L(g, imgL);
			picBx.ResumeLayout();
		}

		private void DrawImage_L(Graphics g, Image imgL)
		{
			if (imgL == null)
				return;
			g.DrawImage(imgL,
					(bWideL) ? (int)(fWidth - imgL.Width / 2.0) : fWidth - imgL.Width - 5,
					(int)(picBx.Height / 2.0 - imgL.Height / 2.0),
					imgL.Width, imgL.Height);
		}
		private void DrawImage_R(Graphics g, Image imgR)
		{
			if (imgR == null)
				return;
			g.DrawImage(imgR,
					(bWideR) ? (int)(fWidth - imgR.Width / 2.0) : fWidth + 5,
					(int)(picBx.Height / 2.0 - imgR.Height / 2.0),
					imgR.Width, imgR.Height);
		}

		private void Browse_MouseUp(object sender, MouseEventArgs e)
		{
			this.Close();
		}

		private void Browse_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (bAuto) {
				Properties.Settings.Default.Interval = trFlip.Interval;
				Properties.Settings.Default.Save();
				trFlip.Stop();
			}

			Cursor.Show();
			Page += 2;
		}

		~Browse_Img()
		{
			if (trFlip != null) {
				trFlip.Dispose();
			}
		}
	}
}
