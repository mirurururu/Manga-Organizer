using System;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Generic;

namespace Nagru___Manga_Organizer
{
    /* Handles sorting of ListView columns
       Author: Microsoft (March 13, 2008)          */
    public class LVsorter : IComparer
    {
        public int ColToSort;       //specifies column to sort
        public SortOrder OrdOfSort; //specifies order sorting

        /* Initializes column & order  */
        public LVsorter()
        {
            ColToSort = 0;
            OrdOfSort = SortOrder.Ascending;
        }

        /* Swaps current direction of column sorting   */
        public void SwapOrder()
        {
            if (OrdOfSort == SortOrder.Ascending) OrdOfSort = SortOrder.Descending;
            else OrdOfSort = SortOrder.Ascending;
        }

        /* Compare ListViewItems (accounts for numbers)
           Author: Samuel Allen (2012)                         */
        public int Compare(object x, object y)
        {
            if (x == null || y == null) return 0;
            ListViewItem lviX = (ListViewItem)x;
            ListViewItem lviY = (ListViewItem)y;

            //choose sorting style
            string sX, sY;
            switch (ColToSort)
            {
                case 0:
                    sX = lviX.SubItems[0].Text + lviX.SubItems[1].Text;
                    sY = lviY.SubItems[0].Text + lviY.SubItems[1].Text;
                    break;
                case 1:
                    sX = lviX.SubItems[1].Text + lviX.SubItems[0].Text;
                    sY = lviY.SubItems[1].Text + lviY.SubItems[0].Text;
                    break;
                default:
                    sX = lviX.SubItems[ColToSort].Text;
                    sY = lviY.SubItems[ColToSort].Text;
                    break;
            }
            int iResult = (new TrueCompare()).Compare(sX, sY);
            if (OrdOfSort == SortOrder.Ascending) return iResult;
            else return (-iResult);
        }
    }
}
