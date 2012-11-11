using System;
using System.Windows.Forms;

namespace Nagru___Manga_Organizer
{
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();
            this.TopMost = true;
            Lbl_License.Text = "This program provides tagging, searching and other basic management for a\n" +
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
            "See the GNU General Public License for more details.\n\n" +
            "You should have received a copy of the GNU General Public License along\n" +
            "with this program; If not, see http://www.gnu.org/licenses/gpl.html \n\n\n" +
            "For a copy of the source code, visit:\n" +
            "http://nagru.github.com/Manga-Organizer/";
        }
    }
}