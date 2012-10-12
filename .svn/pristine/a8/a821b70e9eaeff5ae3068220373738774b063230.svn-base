using System;
using System.Collections.Generic;
using System.Text;

namespace Nagru___Manga_Organizer
{
    /* Compare strings accurately (accounts for numbers)
       Author: Samuel Allen (2012)                         */
    public class TrueCompare : IComparer<string>
    {
        public int Compare(string sX, string sY)
        {
            if (sX == null || sY == null) return 0;
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

                if (result != 0) return result;
            }

            return iLenX - iLenY;
        }
    }
}
