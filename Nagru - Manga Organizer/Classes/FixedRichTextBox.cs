using System;
using System.Windows.Forms;

namespace Nagru___Manga_Organizer
{
    /* Fixes default's broken AutoWordSelection method
       Author: Hans Passant (Sep 9, 2010)                              */
    public class FixedRichTextBox : RichTextBox
    {
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (!base.AutoWordSelection) {
                base.AutoWordSelection = true;
                base.AutoWordSelection = false;
            }
        }
    }
}
