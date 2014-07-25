using System;
using System.Net;
using System.IO;
using System.Windows.Forms;

namespace Nagru___Manga_Organizer
{
  public partial class About : Form
  {
    delegate void DelVoidInt(string sGet);
    DelVoidInt delFini = null;
		readonly int CURRENT_VERSION = 0;

    public About()
    {
      InitializeComponent();
			delFini = Checked;

      string sVer = Application.ProductVersion.Substring(0, 
				Application.ProductVersion.LastIndexOf('.'));
      Text += string.Format("(v. {0})", sVer);
			CURRENT_VERSION = Int32.Parse(sVer.Replace(".", ""));
    }

    private void About_Load(object sender, EventArgs e)
    {
			Lbl_P1.Text = 
@"This program provides tagging, searching and other basic management
tools for manga. It is intended as a companion to the E-H website, and
optimally functions with directory names formatted as '[Artist] Title'.
Copyright (C) 2012  Nagru


his program is free software; you can redistribute it and/or modify it
under the terms of the GNU General Public License as published by the Free
Software Foundation, either version 3 of the License, or (at your option)
any later version.

This program is distributed in the hope that it will be useful, but WITHOUT
ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU General Public License for more details:";
      Lbl_P2.Text = "For updates, or a copy of the source code, visit:";
    }

    private void About_Shown(object sender, EventArgs e)
    {
      this.Cursor = Cursors.WaitCursor;
      Text += " - Checking version...";

      System.Threading.ThreadPool.QueueUserWorkItem(CheckLatest);
    }

    private void CheckLatest(object Null)
    {
      string sGet = "0";

      //exit if there (probably) isn't an internet connection
      if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable()) {
        Invoke(delFini, sGet);
        return;
      }

      try {
        ServicePointManager.DefaultConnectionLimit = 64;
        HttpWebRequest rq = (HttpWebRequest)WebRequest.Create(
            "http://cloud.openmailbox.org/public.php?service=files&t=dbf99c611dad4ccd8627d37e6fdb4045&download");
        rq.UserAgent = "Mozilla/5.0 (Windows; WIndows NT 9.0; en-US))";
        rq.Method = "GET";
        rq.Timeout = 5000;
        rq.KeepAlive = false;
        rq.Proxy = null;

        using (StreamReader sr = new StreamReader(
						rq.GetResponse().GetResponseStream())) {
          sGet = sr.ReadToEnd();
        }
        rq.Abort();
      } catch (Exception e) {
        MessageBox.Show("A connection could not be established:\n" + e.Message,
					Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
      } finally {
        Invoke(delFini, sGet);
      }
    }

    private void Checked(string sGet)
    {
      int iGet;
      int.TryParse(sGet.Replace(".", ""), out iGet);

      Text = Text.Remove(Text.Length - 22);
			if (iGet > CURRENT_VERSION)
        Text += string.Format(
            " - New version available (v. {0})", sGet);
			else if (iGet == CURRENT_VERSION)
        Text += " - Latest version";
      else if (iGet == 0)
        Text += " - Could not establish a connection";
      else
        Text += " - Pre-release version";

      this.Cursor = Cursors.Default;
    }

    private void LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
      if (sender == LnkLbl_Git) {
        LnkLbl_Git.LinkVisited = true;
        System.Diagnostics.Process.Start("http://nagru.github.io/Manga-Organizer/");
      }
      else if (sender == LnkLbl_Gpl) {
        LnkLbl_Gpl.LinkVisited = true;
        System.Diagnostics.Process.Start("http://www.gnu.org/licenses/gpl.html");
      }
    }

    private void About_FormClosing(object sender, FormClosingEventArgs e)
    {
      this.DialogResult = DialogResult.OK;
    }
  }
}