using System;
using System.Windows.Forms;

namespace Nagru___Manga_Organizer
{
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();
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
            Lbl_P2.Text = "For a copy of the source code, visit:";
        }

        private void LnkLbl_Gpl_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LnkLbl_Gpl.LinkVisited = true;
            System.Diagnostics.Process.Start("http://www.gnu.org/licenses/gpl.html"); 
        }

        private void LnkLbl_Git_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LnkLbl_Git.LinkVisited = true;
            System.Diagnostics.Process.Start("http://nagru.github.com/Manga-Organizer"); 
        }
    }
}