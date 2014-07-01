using System;
using System.Net;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Nagru___Manga_Organizer
{
  public partial class Tutorial : Form
  {
    byte byPanel = 0;

    public Tutorial()
    {
      InitializeComponent();
      this.Icon = Properties.Resources.dbIcon;

      SetTutorialText();
    }

    private void Tutorial_Shown(object sender, EventArgs e)
    {
      SwitchPanel();
    }

    #region Text
    private void SetTutorialText()
    {
      lbl_Exp0.Text = @"New manga can be added in three ways:
- Scanning: Press 'Scan' to check a directory for new items, then highlight your choices and press 'Add'
- Drag/Drop: Drop a selection of folders, or archives, onto the column view
- Manually: Add them individually from the 'View' tab, then press the 'Save' button";

      lbl_Exp1.Text = @"The most important thing is the metadata

Tagging can be done more conveniently by:
- An EH Gallery Address: Open 'Menu -> Get from URL', which will auto-check your clipboard
- Dragging text from the EH gallery page into the relevant local field
- Setting the location will also set the Artist and Title";

      lbl_Exp2.Text = @"Searches work similarly to EH's:
- Separate terms with spaces
- Combine multiple terms with '_'
- Prepend a term with '-' to filter
- Limit search scale by setting a tag type
artist, title, tag, desc, type, date, or pages
- Use '<' or '>'  with 'date' or 'pages' for range
- Chain terms by placing '&' between them

Example:
title:celeb_wife date:>01/01/12 -vanilla";

      lbl_Exp3.Text = @"This program has a built-in image-browser intended to mimic manga page layout

Click on the cover-image to use this browser

Keyboard Shortcuts:
'Left/Right Arrows' - Next/previous page
'Up/Down Arrow' - Skip 4 pages ahead/behind
'Home' - Skip to beginning\r\n'End' - Skip to end
'F' - Find new pages
'S' - Start/stop auto-browse
'Shift + Plus' - Increase auto-browse speed
'Shift + Minus' - Decrease auto-browse speed
All other keys close the browser";

      lbl_Exp4.Text = @"Updated versions of this program can be automatically checked for by visiting:
'View -> Menu -> About'

Thank you for reading this, I hope it was helpful and that this program proves useful for you.

Feel free to contact me at nagru@live.ca";
    }
    #endregion

    private void SwitchPanel()
    {
      this.SuspendLayout();
      switch (byPanel) {
        case 0:
          btnPrev.Enabled = false;
          picBx_Ex.Image = Properties.Resources.AddManga;
          break;
        case 1:
          btnPrev.Enabled = true;
          picBx_Ex.Image = Properties.Resources.Tags;
          break;
        case 2:
          picBx_Ex.Image = Properties.Resources.Search;
          break;
        case 3:
          btnNxt.Text = "Next >>";
          picBx_Ex.Image = Properties.Resources.Browse;
          break;
        case 4:
          btnNxt.Text = "Finish";
          picBx_Ex.Image = Properties.Resources.Update;
          break;
        default:
          this.Close();
          return;
      }
      Controls["pnl" + byPanel].BringToFront();
      progBar.Step = byPanel + 1;
      this.ResumeLayout();
    }

    private void btnPrev_Click(object sender, EventArgs e)
    {
      byPanel--;
      SwitchPanel();
    }

    private void btnNxt_Click(object sender, EventArgs e)
    {
      byPanel++;
      SwitchPanel();
      btnPrev.Enabled = true;
    }

    private void progBar_Click(object sender, EventArgs e)
    {
      byPanel = (byte)progBar.Step;

      if (btnPrev.Enabled && byPanel == 0)
        btnPrev.Enabled = false;
      else
        btnPrev.Enabled = true;
      if (byPanel == 4 && btnNxt.Text != "Finish")
        btnNxt.Text = "Finish";
      else
        btnNxt.Text = "Next >>";

      SwitchPanel();
    }
  }
}