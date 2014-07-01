using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using SCA = SharpCompress.Archive;

namespace Nagru___Manga_Organizer
{
  public partial class BrowseTo : Form
  {
    public int iPage
    {
      get;
      set;
    }

    Browse_Img fmSource = null;
    bool bWideR, bWideL;
    float fWidth;

    public BrowseTo(Browse_Img fm)
    {
      InitializeComponent();
      this.DialogResult = DialogResult.Abort;
      fmSource = fm;
    }

    private void Browse_GoTo_Load(object sender, EventArgs e)
    {
      //set grid on/off
      LV_Pages.GridLines = SQL.GetSetting(SQL.Setting.ShowGrid) == "1";

      //add pages to listview
      for (int i = 0; i < fmSource.Files.Count; i++) {
        LV_Pages.Items.Add(new ListViewItem(
            Path.GetFileName(fmSource.Files[i])));
      }

      if (iPage < 0)
        iPage = 0;
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

    private void BrowseTo_KeyDown(object sender, KeyEventArgs e)
    {
      switch (e.KeyCode) {
        case Keys.Enter:
          UpdatePage();
          break;
        default:
          this.Close();
          break;
      }
      e.Handled = true;
    }

    private void LV_Pages_DoubleClick(object sender, EventArgs e)
    {
      UpdatePage();
    }

    /* Return selected pages to Browse_Img */
    private void UpdatePage()
    {
      this.DialogResult = DialogResult.OK;
      this.Close();
    }

    #region SetImagePreview

    private void BrowseTo_ResizeEnd(object sender, EventArgs e)
    {
      //re-calculate width of half-picbx
      fWidth = (float)(picbxPreview.Bounds.Width / 2.0);

      Next(iPage);
    }

    private void Next(int Page)
    {
      Image imgL = null, imgR = null;
      byte by = 0;
      Page--;

      do {
        by++;
        if (++Page >= fmSource.Files.Count)
          Page = 0;
        imgR = TrySet(Page);
      } while (imgR == null && by < 10);

      if (!(bWideR = imgR.Height < imgR.Width)) {
        by = 0;
        do {
          by++;
          if (++Page >= fmSource.Files.Count)
            Page = 0;
          imgL = TrySet(Page);
        } while (imgL == null && by < 10);

        if (bWideL = imgL.Height < imgL.Width)
          Page--;
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
        if (fmSource.Archive != null) {
          fmSource.Archive[fmSource.Sort[i]].WriteTo(ms);
        }
        else {
          FileStream fs = new FileStream(fmSource.Files[i], FileMode.Open);
          fs.CopyTo(ms);
          fs.Dispose();
        }

        bmpTmp = new Bitmap(ms);
        bmpTmp = ExtImage.Scale(bmpTmp,
            (bmpTmp.Width > bmpTmp.Height) ? picbxPreview.Bounds.Width : fWidth
            , picbxPreview.Bounds.Height);
      } catch (Exception Ex) {
        Console.WriteLine(Ex.Message);
      } finally {
        ms.Dispose();
      }

      return bmpTmp;
    }

    /* Process which images to draw & how */
    private void Refresh(Image imgL, Image imgR)
    {
      picbxPreview.Refresh();

      picbxPreview.SuspendLayout();
      Graphics g = picbxPreview.CreateGraphics();
      g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

      if (!bWideL && !bWideR) {
        DrawImage_L(g, imgL);
        DrawImage_R(g, imgR);
      }
      else
        DrawImage_R(g, imgR);
      picbxPreview.ResumeLayout();
    }

    private void DrawImage_L(Graphics g, Image imgL)
    {
      g.DrawImage(imgL,
          (bWideL) ? (int)(fWidth - imgL.Width / 2.0) : fWidth - imgL.Width - 5,
          (int)(picbxPreview.Height / 2.0 - imgL.Height / 2.0),
          imgL.Width, imgL.Height);
    }
    private void DrawImage_R(Graphics g, Image imgR)
    {
      g.DrawImage(imgR,
          (bWideR) ? (int)(fWidth - imgR.Width / 2.0) : fWidth + 5,
          (int)(picbxPreview.Height / 2.0 - imgR.Height / 2.0),
          imgR.Width, imgR.Height);
    }
    #endregion
  }
}