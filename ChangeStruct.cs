
List<stEntry> lNew = new List<stEntry>(lData.Count);
foreach (stEntry en in lData)
{
    string sTags = "";
    string[] sOld = en.sTags.Split(';');

    for (int i = 0; i < sOld.Length; i++)
        if (sOld[i] != string.Empty)
        {
            if (i != 0) sTags += ',';
            sTags += sOld[i];
        }

    lNew.Add(new stEntry(en.sTitle, en.sArtist, en.sLoc, en.sDesc,
    sTags, en.sType, en.dtDate, en.iPages, en.bFav));
}


bool bError = false;
            string sPath = lData[indx].sLoc.ToLower();
            List<string> lNoGo = new List<string>() { "c:", @"c:\program files", @"c:\program files (x86)" };

            foreach (string sFolder in Directory.GetLogicalDrives())
            {
                lNoGo.Add(sFolder);
            }

            foreach (Environment.SpecialFolder spFolder in
                Enum.GetValues(typeof(Environment.SpecialFolder)))
            {
                lNoGo.Add(spFolder.ToString());
            }

            for (int i = 0; i < lNoGo.Count; i++)
                if (Regex.IsMatch(sPath, lNoGo[i]))
                {
                    MessageBox.Show("This is a protected directory, deletion cancelled.",
                        "Manga Organizer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }


//nickstips.wordpress.com/2010/11/10/c-listview-dynamically-sizing-columns-to-fill-whole-control/
private bool Resizing = false;
private void ListView_SizeChanged(object sender, EventArgs e)
{
    // Don't allow overlapping of SizeChanged calls
    if (!Resizing)
    {
        Resizing = true;
        float totalColumnWidth = 0;

        // Get the sum of all column tags
        for (int i = 0; i < listView.Columns.Count; i++)
            totalColumnWidth += Convert.ToInt32(listView.Columns[i].Tag);

        // Calculate the percentage of space each column should 
        // occupy in reference to the other columns and then set the 
        // width of the column to that percentage of the visible space.
        for (int i = 0; i < listView.Columns.Count; i++)
        {
            float colPercentage = (Convert.ToInt32(listView.Columns[i].Tag) / totalColumnWidth);
            listView.Columns[i].Width = (int)(colPercentage * listView.ClientRectangle.Width);             
        }

        Resizing = false;
    }
}
