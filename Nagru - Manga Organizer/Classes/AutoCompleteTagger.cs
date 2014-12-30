using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace Nagru___Manga_Organizer
{
	/// <summary>
	/// An extended combo box that allows for multiple dropdown events based on a separator keyword
	/// </summary>
  public class AutoCompleteTagger : TextBox, IDisposable
  {
		#region Properties
		private bool bDisposed = false;
    protected HScrollBar sbHorz;
    protected ListBox lbSuggest;
    protected string[] asKeyWords;
    protected char cSep = ',';
    protected readonly int ScrollModifier = 7;

    [Description("Sets the terms to be predicted.")]
    public string[] KeyWords
    {
      get
      {
        return asKeyWords;
      }
      set
      {
				asKeyWords = value.ToArray();
      }
    }

    [Description("Sets the delimiter character between tags.")]
    public char Seperator
    {
      get
      {
        return cSep;
      }
      set
      {
        cSep = value;
      }
    }

		#region Hide relevant inherited properties
		[Browsable(false)]
    public new AutoCompleteStringCollection AutoCompleteCustomSource
    {
      get
      {
        return base.AutoCompleteCustomSource;
      }
      set
      {
        base.AutoCompleteCustomSource = value;
      }
    }
    [Browsable(false)]
    public new AutoCompleteMode AutoCompleteMode
    {
      get
      {
        return base.AutoCompleteMode;
      }
      set
      {
        base.AutoCompleteMode = value;
      }
    }
    [Browsable(false)]
    public new AutoCompleteSource AutoCompleteSource
    {
      get
      {
        return base.AutoCompleteSource;
      }
      set
      {
        base.AutoCompleteSource = value;
      }
    }
    [Browsable(false)]
    public new ScrollBars ScrollBars
    {
      get
      {
        return base.ScrollBars;
      }
      set
      {
        base.ScrollBars = value;
      }
    }
		#endregion

		#endregion

		#region Constructor

		public AutoCompleteTagger()
    {
      asKeyWords = new string[0];

      sbHorz = new HScrollBar();
      sbHorz.Scroll += sbHorz_Scroll;
      sbHorz.Height = 14;
      sbHorz.Hide();

      lbSuggest = new ListBox();
      lbSuggest.MouseUp += lbSuggest_MouseUp;
      lbSuggest.MouseMove += lbSuggest_MouseMove;
      lbSuggest.VisibleChanged += lbSuggest_VisibleChanged;
      lbSuggest.Hide();
    }
		
		#endregion

		#region Events

		/// <summary>
    /// Hook child controls to parent
    /// </summary>
    protected override void InitLayout()
    {
      Parent.Controls.Add(lbSuggest);
      Parent.Controls.Add(sbHorz);
      base.InitLayout();
    }

    /// <summary>
    /// Align the scrollbar relative to the textbox
    /// </summary>
    protected override void OnLayout(LayoutEventArgs levent)
    {
      sbHorz.Width = Width;
      sbHorz.Left = Left;
      sbHorz.Top = Bottom;
      base.OnLayout(levent);
    }

		/// <summary>
		/// Whenever the list box becomes visible, adjust its position
		/// </summary>
    protected void lbSuggest_VisibleChanged(object sender, EventArgs e)
    {
      if (lbSuggest.Visible) {
        SetListboxPosition();
      }
    }
		
		/// <summary>
		/// Protected implementation of Dispose
		/// </summary>
		/// <param name="Disposing">Whether we are calling the method from the Dispose override</param>
    protected override void Dispose(bool Disposing)
    {
			if (bDisposed)
				return;

      if (Disposing) {
        lbSuggest.Dispose();
        sbHorz.Dispose();
			}

			bDisposed = true;
      base.Dispose();
    }

		/// <summary>
		/// Destructor
		/// </summary>
		~AutoCompleteTagger()
		{
			Dispose(true);
		}

		#endregion

		#region Keyboard Handling
		
		/// <summary>
		/// Whenever the user types, show the list box if there are any auto-complete possibilities
		/// </summary>
    protected override void OnKeyUp(KeyEventArgs e)
    {
      //filter out list box controls
      switch (e.KeyCode) {
        case Keys.Down:
        case Keys.Up:
        case Keys.Enter:
          base.OnKeyUp(e);
          return;
      }

      //get current keyword
      int iStart = getPrevSepCharIndex();
      int iEnd = getNextSepCharIndex();
      string sKey = base.Text.Substring(iStart, iEnd - iStart).Trim();

      //hide & exit if empty, else show possible tags
      lbSuggest.Items.Clear();
      if (sKey == string.Empty) {
        lbSuggest.Hide();
      }
      else {
        //re-pop suggestions
        string[] asOpt = asKeyWords.Where(x => x.StartsWith(sKey)).ToArray();
        switch (asOpt.Length) {
          case 0:
            lbSuggest.Hide();
            break;
          case 1:
            if (asOpt[0] == sKey)
              lbSuggest.Hide();
            else {
              lbSuggest.Items.Add(asOpt[0]);
              lbSuggest.SelectedIndex = 0;
              lbSuggest.Show();
            }
            break;
          default:
            lbSuggest.Items.AddRange(
                asOpt.OrderBy(x => x, new TrueCompare()).ToArray());
            lbSuggest.Show();
            break;
        }
      }

      base.OnKeyUp(e);
    }

		/// <summary>
		/// Handles the user selecting items in the list box with the arrow & enter keys
		/// </summary>
    protected override void OnKeyDown(KeyEventArgs e)
    {
      //select suggestion
      switch (e.KeyCode) {
        case Keys.Down:
          if (lbSuggest.Visible
                  && lbSuggest.Items.Count > 0) {
            if (lbSuggest.SelectedIndex < lbSuggest.Items.Count - 1)
              lbSuggest.SelectedIndex++;
            else
              lbSuggest.SelectedIndex = 0;
          }
          e.Handled = e.SuppressKeyPress = true;
          break;
        case Keys.Up:
          if (lbSuggest.Visible
                  && lbSuggest.Items.Count > 0) {
            if (lbSuggest.SelectedIndex > 0)
              lbSuggest.SelectedIndex--;
            else
              lbSuggest.SelectedIndex = lbSuggest.Items.Count - 1;
          }
          e.Handled = e.SuppressKeyPress = true;
          break;
        case Keys.Enter:
          if (lbSuggest.Visible
                  && lbSuggest.Items.Count > 0
                  && lbSuggest.SelectedItem != null) {
            int iStart = getPrevSepCharIndex();
            int iEnd = getNextSepCharIndex();
            base.Text = base.Text.Remove(iStart, iEnd - iStart);
            base.Text = base.Text.Insert(iStart, (iStart == 0 ? "" : " ")
                + lbSuggest.SelectedItem.ToString());
            base.Select(getNextSepCharIndex(iEnd), 0);
            lbSuggest.Hide();
            SetScroll();
          }
          e.Handled = e.SuppressKeyPress = true;
          break;
      }

      base.OnKeyDown(e);
    }
		
		/// <summary>
    /// Update scrollbar as the user types
    /// </summary>
    protected override void OnTextChanged(EventArgs e)
    {
      SetScroll();
      if (string.IsNullOrWhiteSpace(this.Text))
        lbSuggest.Hide();
      base.OnTextChanged(e);
    }

		#endregion

		#region Scroll Tags handling
		
		/// <summary>
    /// Move TxBx_Tags cursor pos. based on ScrTags value
    /// </summary>
    protected void sbHorz_Scroll(object sender, ScrollEventArgs e)
    {
      base.Select(sbHorz.Value * (ScrollModifier * 2), 0);
      base.ScrollToCaret();
    }
		
		/// <summary>
    /// Show\Hide scrollbar as needed
    /// </summary>
    public void SetScroll()
    {
      if (TextRenderer.MeasureText(this.Text, this.Font).Width > this.Width) {
        int iValue = this.SelectionStart / ScrollModifier;
        sbHorz.Maximum = (this.Text.Length + (ScrollModifier * 2)) / ScrollModifier;
        sbHorz.Value = (iValue <= sbHorz.Value) ? iValue : sbHorz.Value;
        sbHorz.Show();
      }
      else if (sbHorz.Visible) {
        sbHorz.Hide();
      }
    }

		#endregion

    #region Mouse Handling

    /// <summary>
    /// Highlights listbox item from mouse position
    /// </summary>
    private void lbSuggest_MouseMove(object sender, MouseEventArgs e)
    {
      int indx = lbSuggest.IndexFromPoint(
          lbSuggest.PointToClient(Cursor.Position));

      if (indx >= 0)
        lbSuggest.SelectedIndex = indx;
    }

    /// <summary>
    /// Allow mouse clicks to select tags
    /// </summary>
    private void lbSuggest_MouseUp(object sender, MouseEventArgs e)
    {
      int indx = lbSuggest.IndexFromPoint(
          lbSuggest.PointToClient(Cursor.Position));
      if (indx < 0)
        return;

      lbSuggest.SelectedIndex = indx;
      int iStart = getPrevSepCharIndex();
      int iEnd = getNextSepCharIndex();
      base.Text = base.Text.Remove(iStart, iEnd - iStart);
      base.Text = base.Text.Insert(iStart, (iStart == 0 ? "" : " ")
          + lbSuggest.SelectedItem.ToString());
      base.Select(getNextSepCharIndex(iEnd), 0);
      lbSuggest.Hide();
      SetScroll();
      Select();
    }
		
		/// <summary>
    /// Prevent listbox from showing when keyword changes
    /// </summary>
    protected override void OnClick(EventArgs e)
    {
      SetScroll();
      lbSuggest.Hide();
      base.OnClick(e);
    }
		
		/// <summary>
    /// Prevent autosuggest from blocking other inputs
    /// </summary>
    protected override void OnLostFocus(EventArgs e)
    {
      lbSuggest.Hide();
      base.OnLostFocus(e);
    }

    #endregion

    #region Custom Methods

    /// <summary>
    /// Get leftmost bounds of keyword based on caret position
    /// </summary>
    /// <returns>First instance of the separator char, left from the current pos</returns>
    private int getPrevSepCharIndex()
    {
			if (base.Text.IndexOf(cSep) == -1)
        return 0;

      int iPos = base.SelectionStart == base.Text.Length ?
				base.Text.Length - 1 : base.SelectionStart - 1;

      for (int i = iPos; i > -1; i--) {
        if (base.Text[i] == cSep)
          return i + 1;
      }
      return 0;
    }

    /// <summary>
    /// Get rightmost bounds of keyword based on caret position
    /// </summary>
    /// <param name="iStart">Can override search position</param>
    /// <returns>First instance of the separator char, right from the current pos</returns>
    private int getNextSepCharIndex(int iStart = -1)
    {
      if (base.Text.IndexOf(cSep) == -1)
        return base.Text.Length;

      if (iStart == -1)
				iStart = base.SelectionStart;

      for (int i = iStart; i < base.Text.Length; i++) {
        if (base.Text[i] == cSep)
          return i;
      }
      return base.Text.Length;
    }

    /// <summary>
    /// Updates display position of the suggestions listbox
    /// </summary>
    private void SetListboxPosition()
    {
      lbSuggest.Width = Width;
      lbSuggest.Left = Left;
      lbSuggest.Top = Bottom;
      lbSuggest.BringToFront();
    }

    #endregion
  }
}
