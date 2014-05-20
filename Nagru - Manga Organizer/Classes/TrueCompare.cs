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
			if (sX == null || sY == null)
				return 0;
			int iMarkX = 0, iMarkY = 0;

			//go through strings with two markers.
			while (iMarkX < sX.Length && iMarkY < sY.Length) {
				char cX = sX[iMarkX], cY = sY[iMarkY];

				//buffers for characters
				char[] acSpaceX = new char[sX.Length], acSpaceY = new char[sY.Length];
				int iLocX = 0, iLocY = 0;

				//walk through characters in both strings to fill char array
				do {
					acSpaceX[iLocX++] = cX;
					iMarkX++;

					if (iMarkX < sX.Length)
						cX = sX[iMarkX];
					else
						break;
				}
				while (char.IsDigit(cX) == char.IsDigit(acSpaceX[0]));

				do {
					acSpaceY[iLocY++] = cY;
					iMarkY++;

					if (iMarkY < sY.Length)
						cY = sY[iMarkY];
					else
						break;
				}
				while (char.IsDigit(cY) == char.IsDigit(acSpaceY[0]));

				//if numbers then compare numerically, else compare alphabetically.
				string sCmpX = new string(acSpaceX), sCmpY = new string(acSpaceY);
				int iResult;

				if (char.IsDigit(acSpaceX[0])
								&& char.IsDigit(acSpaceY[0])) {
					int iNumChunkX = 0, iNumChunkY = 0;
					if (int.TryParse(sCmpX, out iNumChunkX)
									&& int.TryParse(sCmpY, out iNumChunkY))
						iResult = iNumChunkX.CompareTo(iNumChunkY);
					else
						iResult = sCmpX.CompareTo(sCmpY);
				}
				else
					iResult = sCmpX.CompareTo(sCmpY);

				if (iResult != 0)
					return iResult;
			}

			return sX.Length - sY.Length;
		}
	}
}
