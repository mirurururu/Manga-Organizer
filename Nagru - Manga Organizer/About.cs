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

        int iVer = 0;

        public About()
        { InitializeComponent(); }

        private void About_Load(object sender, EventArgs e)
        {
            delFini = Checked;
            string sVer = Properties.Settings.Default.Version;
            Text += string.Format("(v. {0})", sVer);
            iVer = Convert.ToInt32(sVer.Replace(".", ""));

            Lbl_P1.Text = "This program provides tagging, searching and other basic management\n" +
            "tools for manga. It is intended as a companion to the E-H website, and\n" +
            "optimally functions with directory names formatted as \"[Artist] Title\".\n" +
            "Copyright (C) 2012  Nagru\n\n\n" +
            "This program is free software; you can redistribute it and/or modify it\n" +
            "under the terms of the GNU General Public License as published by the Free\n" +
            "Software Foundation, either version 3 of the License, or (at your option)\n" +
            "any later version.\n\n" +
            "This program is distributed in the hope that it will be useful, but WITHOUT\n" +
            "ANY WARRANTY; without even the implied warranty of\n" +
            "MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.\n" +
            "See the GNU General Public License for more details:";
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

            try
            {
                ServicePointManager.DefaultConnectionLimit = 64;
                HttpWebRequest rq = (HttpWebRequest)WebRequest.Create(
                    "http://dl.dropboxusercontent.com/u/103899726/Version.txt");
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
                try { Invoke(delFini, sGet); } catch { }
            }
        }

        private void Checked(string sGet)
        {
            int iGet = Convert.ToInt32(sGet.Replace(".",""));
            Text = Text.Remove(Text.Length - 22);
            if (iGet > iVer)
                Text += string.Format(
                    " - New version available (v. {0})", sGet);
            else if (iGet == iVer)
                Text += " - Latest version";
            else if (iGet == 0)
                Text += " - Could not establish a connection";
            else Text += " - Pre-release version";

            this.Cursor = Cursors.Default;
        }

        private void LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (sender == LnkLbl_Git)
            {
                LnkLbl_Git.LinkVisited = true;
                System.Diagnostics.Process.Start("http://nagru.github.io/Manga-Organizer/");
            }
            else /*if (sender == LnkLbl_Gpl)*/
            {
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