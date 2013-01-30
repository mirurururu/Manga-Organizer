using System;
using System.Net;
using System.IO;
using System.Windows.Forms;

namespace Nagru___Manga_Organizer
{
    public partial class About : Form
    {
        delegate void DelVoidInt(int iResult);
        DelVoidInt delFini = null;

        public About()
        { InitializeComponent(); }

        private void About_Load(object sender, EventArgs e)
        {
            delFini = Checked;
            Text = string.Format("About ({0})", Properties.Settings.Default.Version);

            Lbl_P1.Text = "This program provides tagging, searching and other basic management for a\n" +
            "folder directory. It is intended as a companion to the EH website, and\n" +
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
            int iEvent = 0;

            HtmlAgilityPack.HtmlWeb htmlWeb = new HtmlAgilityPack.HtmlWeb();
            HtmlAgilityPack.HtmlDocument htmlDoc = htmlWeb.Load("http://nagru.github.com/Manga-Organizer/");

            //grab version info
            if (htmlWeb.StatusCode == System.Net.HttpStatusCode.OK) {
                try {
                    if (Properties.Settings.Default.Version ==
                        htmlDoc.DocumentNode.SelectNodes("//*[text()[contains(., 'v. ')]]")[0].InnerText)
                        iEvent = 1;
                    else iEvent = 2;
                }
                catch { }
            }

            try { Invoke(delFini, iEvent); } catch { }
        }

        private void Checked(int iResult)
        {
            Text = "About (" + Properties.Settings.Default.Version + ") - ";
            switch (iResult) {
                case 0: Text += "Could not establish a connection";
                    break;
                case 1: Text += "Latest version";
                    break;
                case 2: Text += "New version available";
                    break;
            }

            this.Cursor = Cursors.Default;
        }

        private void LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (sender == LnkLbl_Git)
            {
                LnkLbl_Git.LinkVisited = true;
                System.Diagnostics.Process.Start("http://nagru.github.com/Manga-Organizer");
            }
            else /*if (sender == LnkLbl_Gpl)*/
            {
                LnkLbl_Gpl.LinkVisited = true;
                System.Diagnostics.Process.Start("http://www.gnu.org/licenses/gpl.html");
            }
        }
    }
}