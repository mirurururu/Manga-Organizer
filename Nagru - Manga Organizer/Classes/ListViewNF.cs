using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Nagru___Manga_Organizer
{
  /// <summary>
  /// Fixes default's broken Update (it no longer acts as a refresh)
  /// </summary>
  /// <remarks>Author: geekswithblogs.net (Feb 27, 2006)</remarks>
  class ListViewNF : System.Windows.Forms.ListView
  {
    #region Properties

    public HashSet<int> staticColumns;
    private int ColRating = 0;
    private LVsorter lvSortObj;
    private static readonly Color cRowColorAlt;

    #region Interface

    /// <summary>
    /// Returns whether the object is being used on the Main form page
    /// Unlocks specific sorting behavior for the main page
    /// </summary>
    [DefaultValue(0)]
    [Description("Unlocks specific sorting behavior for the main form page")]
    public bool IsMain
    {
      get
      {
        return lvSortObj.IsMain;
      }
      set
      {
        lvSortObj.IsMain = value;
      }
    }

    /// <summary>
    /// The index of the Rating column, used for setting row colour
    /// </summary>
    [DefaultValue(0)]
    [Description("The index of the Rating column, used for setting row colour")]
    public int RatingColumn
    {
      get
      {
        return ColRating;
      }
      set
      {
        ColRating = value;
      }
    }

    /// <summary>
    /// Checks whether the control is in design mode
    /// This prevents the SQL call from breaking the VS designer
    /// </summary>
    private static bool InDesignMode()
    {
      try {
        return Process.GetCurrentProcess().ProcessName == "devenv";
      } catch {
        return false;
      }
    }

    #endregion

    #endregion

    #region Constructor

    /// <summary>
    /// Initialize the listview
    /// </summary>
    public ListViewNF()
    {
      staticColumns = new HashSet<int>();
      lvSortObj = new LVsorter();
      this.ListViewItemSorter = lvSortObj;

      //Activate double buffering
      this.SetStyle(ControlStyles.OptimizedDoubleBuffer
          | ControlStyles.AllPaintingInWmPaint, true);

      /* Enable the OnNotifyMessage event so we get a chance to filter out 
         Windows messages before they get to the form's WndProc   */
      this.SetStyle(ControlStyles.EnableNotifyMessage, true);
    }

    /// <summary>
    /// Set the row color to be used for alternating backgrounds
    /// This value is application-scoped, so it only needs to be loaded once
    /// </summary>
    static ListViewNF()
    {
      if (!InDesignMode()) {
        cRowColorAlt = Color.FromArgb(Int32.Parse(SQL.GetSetting(SQL.Setting.RowColourAlt)));
      }
      else {
        cRowColorAlt = Color.LightGray;
      }
    }

    #endregion

    #region Overrides

    /// <summary>
    /// Filter out the WM_ERASEBKGND message
    /// </summary>
    protected override void OnNotifyMessage(Message m)
    {
      if (m.Msg != 0x14)
        base.OnNotifyMessage(m);
    }

    /// <summary>
    /// Call custom sorting whenever a column is clicked
    /// </summary>
    protected override void OnColumnClick(ColumnClickEventArgs e)
    {
      //prevent sorting by tags
      if (staticColumns != null && staticColumns.Contains(e.Column))
        return;

      if (e.Column != lvSortObj.SortingColumn)
        lvSortObj.NewColumn(e.Column);
      else
        lvSortObj.SwapOrder();

      this.SortRows();
      base.OnColumnClick(e);
    }

    /// <summary>
    /// Give listview WindowsExplorer style
    /// </summary>
    /// <remarks>Author: Zach Johnson (Mar 27, 2010)</remarks>
    [DllImport("uxtheme.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
    internal static extern int SetWindowTheme(IntPtr hWnd, string appName, string partList);
    protected override void OnHandleCreated(EventArgs e)
    {
      base.OnHandleCreated(e);
      SetWindowTheme(this.Handle, "explorer", null);
    }

    #endregion

    #region Functions

    /// <summary>
    /// Alternate row colors in the listview
    /// </summary>
    public void Alternate()
    {
      this.BeginUpdate();
      for (int i = 0; i < this.Items.Count; i++) {
        if (IsMain) {
          if (this.Items[i].SubItems[ColRating].Text.Length == 5)
            continue;
        }
        else if (this.Items[i].BackColor == Color.MistyRose) {
          continue;
        }

        this.Items[i].BackColor = (i % 2 != 0) ?
          cRowColorAlt : SystemColors.Window;
      }
      this.EndUpdate();
    }

    /// <summary>
    /// Resets the alternating row colours after the Sort operation
    /// </summary>
    public void SortRows()
    {
      this.Sort();
      this.Alternate();
    }

    #endregion


    /// <summary>
    /// Handles sorting of ListView columns
    /// </summary>
    /// <remarks>Author: Microsoft (March 13, 2008)</remarks>
    private class LVsorter : IComparer
    {
      #region Properties

      #region Interface
      /// <summary>
      /// Enables custom sorting order for the main form
      /// </summary>
      public bool IsMain
      {
        get
        {
          return bIsMain;
        }
        set
        {
          bIsMain = value;
        }
      }

      /// <summary>
      /// Controls how to sort items
      /// </summary>
      public SortOrder SortingOrder
      {
        get
        {
          return OrdOfSort;
        }
        set
        {
          OrdOfSort = value;
        }
      }

      /// <summary>
      /// Controls which column to sort by
      /// </summary>
      public int SortingColumn
      {
        get
        {
          return ColToSort;
        }
        set
        {
          ColToSort = value;
        }
      }
      #endregion

      static TrueCompare tc = new TrueCompare();
      private int ColToSort;
      private SortOrder OrdOfSort;
      private bool bIsMain;
      #endregion

      #region Constructor

      public LVsorter()
      {
        ColToSort = 0;
        OrdOfSort = SortOrder.Ascending;
      }

      #endregion

      #region Functions

      /// <summary>
      /// Swap the sorting direction
      /// </summary>
      public void SwapOrder()
      {
        if (OrdOfSort == SortOrder.Ascending)
          OrdOfSort = SortOrder.Descending;
        else
          OrdOfSort = SortOrder.Ascending;
      }

      /// <summary>
      /// Set a new column to sort by
      /// </summary>
      /// <param name="Column">The index of the new column</param>
      /// <param name="Order">The new sort order to use</param>
      public void NewColumn(int Column, SortOrder Order = SortOrder.Ascending)
      {
        ColToSort = Column;
        OrdOfSort = Order;
      }

      /// <summary>
      /// Handles column comparisons during sorting
      /// </summary>
      public int Compare(object x, object y)
      {
        if (x == null || y == null)
          return 0;

        int Result = 0;
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

      #endregion
    }
  }
}
