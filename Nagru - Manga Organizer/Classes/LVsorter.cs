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
		#region Column IDs
		const int colArtist = 0;
		const int colTitle	= 1;
		const int colPages	= 2;
		const int colTags		= 3;
		const int colDate		= 4;
		const int colType		= 5;
		const int colRating = 6;
		#endregion

		static TrueCompare tc = new TrueCompare();
		public int ColToSort;
		public SortOrder OrdOfSort;
		private int Result;

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

			Result = 0;
			ListViewItem lviX = (ListViewItem)x;
			ListViewItem lviY = (ListViewItem)y;

			switch (ColToSort) {
				case colArtist:
					Result = tc.Compare(
						lviX.SubItems[colArtist].Text + lviX.SubItems[colTitle].Text
						, lviY.SubItems[colArtist].Text + lviY.SubItems[colTitle].Text);
					break;
				case colTitle:
					Result = tc.Compare(
						lviX.SubItems[colTitle].Text + lviX.SubItems[colArtist].Text
						, lviY.SubItems[colTitle].Text + lviY.SubItems[colArtist].Text);
					break;
				case colPages:
					Result = (Int32.Parse(lviX.SubItems[colPages].Text))
						.CompareTo
						(Int32.Parse(lviY.SubItems[colPages].Text));
					break;
				case colDate: //re-order MM/DD/YY into YYMMDD for search
					Result = (lviX.SubItems[colDate].Text.Substring(6, 2) 
							+ lviX.SubItems[colDate].Text.Substring(0, 2) 
							+ lviX.SubItems[colDate].Text.Substring(3, 2))
						.CompareTo
						(lviY.SubItems[colDate].Text.Substring(6, 2) 
							+ lviY.SubItems[colDate].Text.Substring(0, 2) 
							+ lviY.SubItems[colDate].Text.Substring(3, 2));
					break;
				case colType:
					Result = tc.Compare(
						lviX.SubItems[colType].Text
						, lviY.SubItems[colType].Text);
					break;
				case colRating:
					Result = 
						(lviX.SubItems[colRating].Text.Length)
						.CompareTo
						(lviY.SubItems[colRating].Text.Length);
					break;
			}
			return (OrdOfSort == SortOrder.Ascending) ? Result : -Result;
		}
	}
}
