using System;
using System.Windows.Forms;

namespace Nagru___Manga_Organizer
{
    /* Fixes default's broken Update (it no longer acts as a refresh)
       Author: geekswithblogs.net (Feb 27, 2006) */
    class ListViewNF : System.Windows.Forms.ListView
    {
        public ListViewNF()
        {
            //Activate double buffering
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

            /* Enable the OnNotifyMessage event so we get a chance to filter out 
               Windows messages before they get to the form's WndProc   */
            this.SetStyle(ControlStyles.EnableNotifyMessage, true);
        }

        //Filter out the WM_ERASEBKGND message
        protected override void OnNotifyMessage(Message m)
        {
            if (m.Msg != 0x14) base.OnNotifyMessage(m);
        }
    }
}
