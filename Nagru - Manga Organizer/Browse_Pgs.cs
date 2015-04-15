using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Nagru___Manga_Organizer
{
  public partial class BrowseTo : Form
	{
		#region Properties

    Browse_Img fmSource = null;
    bool bWideR, bWideL;
    float fWidth;

		#region Interface

		public int iPage
    {
      get;
      set;
    }

		#endregion

		#endregion

		#region Constructor

		public BrowseTo(Browse_Img fm)
    {
      InitializeComponent();
      this.DialogResult = DialogResult.Abort;
      fmSource = fm;
    }

		#endregion

		#region Events

		private void Browse_GoTo_Load(object sender, EventArgs e)
    {
      //set grid on/off
      LV_Pages.GridLines = SQL.GetSetting(SQL.Setting.ShowGrid) == "1";

      //add pages to listview
      ListViewItem[] lvi = new ListViewItem[fmSource.Files.Count];
      for (int i = 0; i < fmSource.Files.Count; i++) {
        lvi[i] = new ListViewItem(
          BetterFilename(fmSource.Files[i])
        );
      }
      LV_Pages.BeginUpdate();
      LV_Pages.Items.AddRange(lvi);
      LV_Pages.EndUpdate();

      if (iPage < 0) iPage = 0;
      Col_Page.Width = LV_Pages.DisplayRectangle.Width;
      LV_SelectPages();

      //set width of half-picbx
      fWidth = (float)(picbxPreview.Bounds.Width / 2.0);

      //show selected pages topmost
      LV_Pages.TopItem = LV_Pages.Items[iPage];
      LV_Pages.TopItem = LV_Pages.Items[iPage];
      LV_Pages.TopItem = LV_Pages.Items[iPage];
    }

    private void BrowseTo_Shown(object sender, EventArgs e)
    {
      Next(iPage);
    }
		
		private void BrowseTo_KeyDown(object sender, KeyEventArgs e)
    {
      switch (e.KeyCode) {
        case Keys.Enter:
          UpdatePage();
          break;
        case Keys.Up:
          iPage--;
          if (iPage < 0)
            iPage = LV_Pages.Items.Count - 1;
          LV_SelectPages();
          break;
        case Keys.Down:
          iPage++;
          if (iPage > LV_Pages.Items.Count)
            iPage = 0;
          LV_SelectPages();
          break;
        default:
          this.Close();
          break;
      }
      e.Handled = true;
    }
		
		private void BrowseTo_ResizeEnd(object sender, EventArgs e)
    {
      //re-calculate width of half-picbx
      fWidth = (float)(picbxPreview.Bounds.Width / 2.0);

      Next(iPage);
    }

    private void LV_Pages_Resize(object sender, EventArgs e)
    {
      Col_Page.Width = LV_Pages.DisplayRectangle.Width;
    }

    private void LV_Pages_Click(object sender, EventArgs e)
    {
      iPage = LV_Pages.SelectedItems[0].Index;
      LV_SelectPages();
    }

    private void LV_SelectPages()
    {
      int iNext = iPage + 1;
      if (iNext > LV_Pages.Items.Count - 1)
        iNext = 0;

      LV_Pages.BeginUpdate();
      LV_Pages.SelectedItems.Clear();
      LV_Pages.FocusedItem = LV_Pages.Items[iPage];
      LV_Pages.Items[iPage].Selected = true;
      LV_Pages.Items[iNext].Selected = true;
      LV_Pages.EndUpdate();
      Next(iPage);
    }

    private void LV_Pages_DoubleClick(object sender, EventArgs e)
    {
      UpdatePage();
    }

		#endregion

		#region Custom Methods

    /// <summary>
    /// Returns path names as {Parent Directory}\{File}
    /// </summary>
    /// <param name="Raw">The path to parse</param>
    /// <returns>The formatted version of the path</returns>
    private static string BetterFilename(string Raw)
    {
      string[] aSplit = Raw.Split('\\');
      if (aSplit.Length >= 2) {
        return string.Format("{0}\\{1}", aSplit[aSplit.Length - 2], aSplit[aSplit.Length - 1]);
      }
      else {
        return Raw;
      }
    }

		/// <summary>
		/// Get the next pair of images for the preview screen
		/// </summary>
		/// <param name="Page">The index of the last page browse to</param>
    private void Next(int Page)
    {
      Image imgL = null, imgR = null;		//holds the loaded image files
      byte byError = 0;									//holds the number of tries attempted to load an image
			Page--;

			//get the next valid, right-hand image file in the sequence
      do {
				byError++;
        if (++Page >= fmSource.Files.Count)
          Page = 0;
        imgR = TryLoad(Page);
			} while (imgR == null && byError < 10);

			//if the image is not multi-page, then load the next valid, left-hand image in the sequence
      if (imgR == null || !(bWideR = imgR.Height < imgR.Width)) {
				byError = 0;
        do {
					byError++;
					if (++Page >= fmSource.Files.Count)
            Page = 0;
          imgL = TryLoad(Page);
				} while (imgL == null && byError < 10);

				//if this image is multi-page, decrement the page value so the next page turn will catch it
        if (imgL != null && (bWideL = imgL.Height < imgL.Width))
          Page--;
      }
      Refresh(imgL, imgR);

      if (imgL != null) {
        imgL.Dispose();
      }
      if (imgR != null) {
        imgR.Dispose();
      }
    }

		/// <summary>
		/// Try to load the image at the indicated index
		/// </summary>
		/// <param name="iPos">The file index</param>
    private Bitmap TryLoad(int i)
    {
      Bitmap bmpTmp = null;
      MemoryStream ms = new MemoryStream();

      try {
        if (fmSource.Archive != null) {
          fmSource.Archive[fmSource.Sort[i]].WriteTo(ms);
        }
        else {
          FileStream fs = new FileStream(fmSource.Files[i], FileMode.Open);
          fs.CopyTo(ms);
          fs.Dispose();
        }

        bmpTmp = new Bitmap(ms);
        bmpTmp = Ext.ScaleImage(bmpTmp,
            (bmpTmp.Width > bmpTmp.Height) ? picbxPreview.Bounds.Width : fWidth
            , picbxPreview.Bounds.Height);
      } catch (Exception Ex) {
        Console.WriteLine(Ex.Message);
      } finally {
        ms.Dispose();
      }

      return bmpTmp;
    }

		/// <summary>
		/// Process which images to draw & how
		/// </summary>
		/// <param name="imgL"></param>
		/// <param name="imgR"></param>
    private void Refresh(Image imgL, Image imgR)
    {
      picbxPreview.Refresh();

      picbxPreview.SuspendLayout();
      using(Graphics g = picbxPreview.CreateGraphics())
			{
				g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

				if (!bWideL && !bWideR) {
					DrawImage(g, imgL, true);
					DrawImage(g, imgR, false);
				}
				else
					DrawImage(g, imgR, false);
			}
      picbxPreview.ResumeLayout();
    }

		/// <summary>
		/// Draw the image to the screen
		/// </summary>
		/// <param name="g">A graphics reference to the picturebox</param>
		/// <param name="imgL">The image to draw</param>
		private void DrawImage(Graphics g, Image img, bool bLeft)
		{
			if (img != null)
			{
				g.DrawImage(
					img
					, (bLeft ? bWideL : bWideR)
							? (int)(fWidth - img.Width / 2.0)
							: fWidth + (bLeft ? -(img.Width + 5) : 5)
					, (int)(picbxPreview.Height / 2.0 - img.Height / 2.0)
					, img.Width
					, img.Height
				);
			}
		}

		/// <summary>
		/// Return selected pages to Browse_Img
		/// </summary>
    private void UpdatePage()
    {
      this.DialogResult = DialogResult.OK;
      this.Close();
    }

		#endregion
  }
}