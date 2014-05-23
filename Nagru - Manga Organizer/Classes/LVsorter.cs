using System;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Generic;

namespace Nagru___Manga_Organizer
{
	/// <summary>
	/// Handles sorting of ListView columns
	/// </summary>
	/// <remarks>Author: Microsoft (March 13, 2008)</remarks>
	public class LVsorter : IComparer
	{
		static TrueCompare tc = new TrueCompare();
		public int ColToSort;
		public SortOrder OrdOfSort;

		public LVsorter()
		{
			ColToSort = 0;
			OrdOfSort = SortOrder.Ascending;
		}

		public void SwapOrder()
		{
			if (OrdOfSort == SortOrder.Ascending)
				OrdOfSort = SortOrder.Descending;
			else
				OrdOfSort = SortOrder.Ascending;
		}

		public void NewColumn(int Column, SortOrder Order)
		{
			ColToSort = Column;
			OrdOfSort = Order;
		}

		public int Compare(object x, object y)
		{
			if (x == null || y == null)
				return 0;
			ListViewItem lviX = (ListViewItem)x;
			ListViewItem lviY = (ListViewItem)y;

			//sort by art-tit, tit-art or custom-single
			string sX, sY;
			switch (ColToSort) {
				case 0:
					sX = lviX.SubItems[0].Text + lviX.SubItems[1].Text;
					sY = lviY.SubItems[0].Text + lviY.SubItems[1].Text;
					break;
				case 1:
					sX = lviX.SubItems[1].Text + lviX.SubItems[0].Text;
					sY = lviY.SubItems[1].Text + lviY.SubItems[0].Text;
					break;
				case 4:
					sX = Convert.ToDateTime(lviX.SubItems[4].Text).ToString("yyyy MM dd");
					sY = Convert.ToDateTime(lviY.SubItems[4].Text).ToString("yyyy MM dd");
					break;
				default:
					sX = lviX.SubItems[ColToSort].Text;
					sY = lviY.SubItems[ColToSort].Text;
					break;
			}
			int iResult = tc.Compare(sX, sY);
			return (OrdOfSort == SortOrder.Ascending) ? iResult : -iResult;
		}
	}
}
