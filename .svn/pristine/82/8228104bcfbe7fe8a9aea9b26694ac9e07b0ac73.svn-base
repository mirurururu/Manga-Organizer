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
            int iLenX = sX.Length, iLenY = sY.Length;
            int iMarkX = 0, iMarkY = 0;

            //go through strings with two markers.
            while (iMarkX < iLenX && iMarkY < iLenY)
            {
                char cX = sX[iMarkX], cY = sY[iMarkY];

                //buffers for characters
                char[] acSpaceX = new char[iLenX], acSpaceY = new char[iLenY];
                int iLocX = 0, iLocY = 0;

                //walk through characters in both strings to fill char array
                do
                {
                    acSpaceX[iLocX++] = cX;
                    iMarkX++;

                    if (iMarkX < iLenX) cX = sX[iMarkX];
                    else break;
                }
                while (char.IsDigit(cX) == char.IsDigit(acSpaceX[0]));

                do
                {
                    acSpaceY[iLocY++] = cY;
                    iMarkY++;

                    if (iMarkY < iLenY) cY = sY[iMarkY];
                    else break;
                }
                while (char.IsDigit(cY) == char.IsDigit(acSpaceY[0]));

                //if numbers compare numerically, else compare alphabetically.
                string sCmpX = new string(acSpaceX), sCmpY = new string(acSpaceY);
                int result;

                if (char.IsDigit(acSpaceX[0]) && char.IsDigit(acSpaceY[0]))
                {
                    int iNumChunkX = int.Parse(sCmpX), iNumChunkY = int.Parse(sCmpY);
                    result = iNumChunkX.CompareTo(iNumChunkY);
                }
                else result = sCmpX.CompareTo(sCmpY);

                if (result != 0)
                {
                    if (OrdOfSort == SortOrder.Ascending) return result;
                    else return (-result);
                }
            }

            return iLenX - iLenY;
        }
    }
}
