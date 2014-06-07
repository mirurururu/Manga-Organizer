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
		#region Properties

		#region Settings
		/// <summary>
		/// Enables custom sorting order for the main form
		/// </summary>
		public bool IsMain {
			get {
				return bIsMain;
			}
			set {
				bIsMain = value;
			}
		}

		/// <summary>
		/// Controls how to sort items
		/// </summary>
		public SortOrder SortingOrder {
			get {
				return OrdOfSort;
			}
			set {
				OrdOfSort = value;
			}
		}

		/// <summary>
		/// Controls which column to sort by
		/// </summary>
		public int SortingColumn {
			get {
				return ColToSort;
			}
			set {
				ColToSort = value;
			}
		}
		#endregion

		static TrueCompare tc = new TrueCompare();
		private int ColToSort, Result;
		private SortOrder OrdOfSort;
		private bool bIsMain;
		#endregion

		public LVsorter(bool IsMain = false)
		{
			ColToSort = 0;
			bIsMain = IsMain;
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

			if (bIsMain) {
				switch (ColToSort) {
					case 0: //artist
						Result = tc.Compare(
							lviX.SubItems[0].Text + lviX.SubItems[1].Text
							, lviY.SubItems[0].Text + lviY.SubItems[1].Text);
						break;
					case 1: //title
						Result = tc.Compare(
							lviX.SubItems[1].Text + lviX.SubItems[0].Text
							, lviY.SubItems[1].Text + lviY.SubItems[0].Text);
						break;
					case 2: //pages
						Result = (Int32.Parse(lviX.SubItems[2].Text))
							.CompareTo
							(Int32.Parse(lviY.SubItems[2].Text));
						break;
					case 4: //date: re-order MM/DD/YY into YYMMDD for search
						Result = (lviX.SubItems[4].Text.Substring(6, 2)
								+ lviX.SubItems[4].Text.Substring(0, 2)
								+ lviX.SubItems[4].Text.Substring(3, 2))
							.CompareTo
							(lviY.SubItems[4].Text.Substring(6, 2)
								+ lviY.SubItems[4].Text.Substring(0, 2)
								+ lviY.SubItems[4].Text.Substring(3, 2));
						break;
					case 5: //type
						Result = tc.Compare(
							lviX.SubItems[5].Text
							, lviY.SubItems[5].Text);
						break;
					case 6: //rating
						Result =
							(lviX.SubItems[6].Text.Length)
							.CompareTo
							(lviY.SubItems[6].Text.Length);
						break;
				}
			}
			else {
				Result = tc.Compare(
					lviX.SubItems[ColToSort].Text
					, lviY.SubItems[ColToSort].Text);
			}
			
			return (OrdOfSort == SortOrder.Ascending) ? Result : -Result;
		}
	}
}
