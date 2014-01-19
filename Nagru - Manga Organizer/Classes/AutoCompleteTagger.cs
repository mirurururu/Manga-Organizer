using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Nagru___Manga_Organizer.Classes
{
    public class AutoCompleteTagger : TextBox
    {
        #region Properties
        protected HScrollBar sbHorz;
        protected ListBox lbSuggest;
        protected List<string> lKeyWords;
        protected char cSep = ',';

        [Description("Sets the terms to be predicted.")]
        public string[] KeyWords {
            get { return lKeyWords.ToArray(); }
            set {
                lKeyWords.Clear();
                lKeyWords.AddRange(value);
            }
        }

        [Description("Sets the delimeter character between tags.")]
        public char Seperator {
            get { return cSep; }
            set { cSep = value; }
        }

        //Hide relevant inherited properties
        [Browsable(false)]
        public new AutoCompleteStringCollection AutoCompleteCustomSource {
            get { return base.AutoCompleteCustomSource; }
            set { base.AutoCompleteCustomSource = value; }
        }
        [Browsable(false)]
        public new AutoCompleteMode AutoCompleteMode {
            get { return base.AutoCompleteMode; }
            set { base.AutoCompleteMode = value; }
        }
        [Browsable(false)]
        public new AutoCompleteSource AutoCompleteSource {
            get { return base.AutoCompleteSource; }
            set { base.AutoCompleteSource = value; }
        }
        [Browsable(false)]
        public new ScrollBars ScrollBars {
            get { return base.ScrollBars; }
            set { base.ScrollBars = value; }
        }
        #endregion

        public AutoCompleteTagger()
        {
            sbHorz = new HScrollBar();
            sbHorz.Scroll += sbHorz_Scroll;
            sbHorz.Height = 14;
            sbHorz.Hide();

            lbSuggest = new ListBox();
            lKeyWords = new List<string>();
            lbSuggest.MouseUp += lbSuggest_MouseUp;
            lbSuggest.MouseMove += lbSuggest_MouseMove;
            lbSuggest.Hide();
        }
        
        /* Hook child controls to parent */
        protected override void InitLayout()
        {
            Parent.Controls.Add(lbSuggest);
            Parent.Controls.Add(sbHorz);
            base.InitLayout();
        }
        
        protected override void OnKeyUp(KeyEventArgs e)
        {
            //filter out listbox controls
            switch(e.KeyCode) {
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
            if(sKey == string.Empty) {
                lbSuggest.Hide();
            }
            else {
                //re-pop suggestions
                string[] asOpt = lKeyWords.Where(x => x.StartsWith(sKey)).ToArray();
                switch(asOpt.Length) {
                    case 0: lbSuggest.Hide();
                        break;
                    case 1:
                        if (asOpt[0] == sKey) lbSuggest.Hide();
                        else {
                            lbSuggest.Items.Add(asOpt[0]);
                            lbSuggest.SelectedIndex = 0;
                            SetListboxPosition();
                        }
                        break;
                    default:
                        lbSuggest.Items.AddRange(
                            asOpt.OrderBy(x => x, new TrueCompare()).ToArray());
                        SetListboxPosition();
                        break;
                }
            }

            base.OnKeyUp(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            //select suggestion
            switch (e.KeyCode) {
                case Keys.Down:
                    if(lbSuggest.Visible 
                            && lbSuggest.Items.Count > 0) {
                        if (lbSuggest.SelectedIndex < lbSuggest.Items.Count - 1)
                            lbSuggest.SelectedIndex++;
                        else lbSuggest.SelectedIndex = 0;
                    }
                    e.Handled = e.SuppressKeyPress = true;
                    break;
                case Keys.Up:
                    if (lbSuggest.Visible
                            && lbSuggest.Items.Count > 0) {
                        if (lbSuggest.SelectedIndex > 0)
                            lbSuggest.SelectedIndex--;
                        else lbSuggest.SelectedIndex = lbSuggest.Items.Count - 1;
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
        
        /* Update scrollbar as user types */
        protected override void OnTextChanged(EventArgs e)
        {
            SetScroll();
            if (this.Text == "") lbSuggest.Hide();
            base.OnTextChanged(e);
        }

        /* Move TxBx_Tags cursor pos. based on ScrTags value */
        protected void sbHorz_Scroll(object sender, ScrollEventArgs e)
        {
            base.Select(sbHorz.Value, 0);
            base.ScrollToCaret();
        }

        /* Prevent autosuggest from blocking other inputs */
        protected override void OnLostFocus(EventArgs e)
        {
            lbSuggest.Hide();
            base.OnLostFocus(e);
        }
        
        /* Prevent listbox from showing when keyword changes */
        protected override void OnClick(EventArgs e)
        {
            SetScroll();
            lbSuggest.Hide();
            base.OnClick(e);
        }

        ~AutoCompleteTagger()
        {
            lbSuggest.Dispose();
            sbHorz.Dispose();
        }

        #region Mouse Handling
        /* Selects listbox item from mouse position */
        private void lbSuggest_MouseMove(object sender, MouseEventArgs e)
        {
            int indx = lbSuggest.IndexFromPoint(
                lbSuggest.PointToClient(Cursor.Position));

            if (indx >= 0)
                lbSuggest.SelectedIndex = indx;
        }
        
        /* Allow mouse clicks to select tags */
        private void lbSuggest_MouseUp(object sender, MouseEventArgs e)
        {
            int indx = lbSuggest.IndexFromPoint(
                lbSuggest.PointToClient(Cursor.Position));
            if (indx < 0) return;

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
        #endregion

        #region Custom Methods
        /* Get bounds of keyword based on caret position */
        private int getPrevSepCharIndex() {
            if (!base.Text.Contains(cSep)) return 0;
            int iPos = base.SelectionStart == base.Text.Length ? 
                base.Text.Length - 1 : base.SelectionStart - 1;
            for (int i = iPos; i > -1; i--) {
                if (base.Text[i] == cSep)
                    return i + 1;
            }
            return 0;
        }
        private int getNextSepCharIndex(int iStart = -1) {
            if (!base.Text.Contains(cSep)) return base.Text.Length;
            if (iStart == -1) iStart = base.SelectionStart;
            for (int i = iStart; i < base.Text.Length; i++) {
                if (base.Text[i] == cSep)
                    return i;
            }
            return base.Text.Length;
        }

        private void SetListboxPosition()
        {
            lbSuggest.Width = Width;
            lbSuggest.Left = Left;
            lbSuggest.Top = Bottom;
            lbSuggest.Show();
            lbSuggest.BringToFront();
        }
        
        /* Show\Hide scrollbar as needed */
        public void SetScroll()
        {
            int iWidth = TextRenderer.MeasureText(this.Text, this.Font).Width;
            if (iWidth > this.Width) {
                sbHorz.Maximum = this.Text.Length + 10;
                sbHorz.Value = this.SelectionStart;
                
                //set sbHorz location
                sbHorz.Width = Width;
                sbHorz.Left = Left;
                sbHorz.Top = Bottom;
                sbHorz.Show();
            }
            else if(sbHorz.Visible) {
				int iStart = this.SelectionStart;
				this.SelectionStart = 0;
				this.SelectionStart = iStart;
				sbHorz.Hide();
			}
        }

        /* Add new keywords if not contained */
        public void UpdateAutoComplete()
        {
            bool bUnsorted = false;
            string[] asTags = base.Text.Split(cSep);
            for(int i = 0; i < asTags.Length; i++) {
                asTags[i] = asTags[i].Trim();
                if(!lKeyWords.Contains(asTags[i])) {
                    lKeyWords.Add(asTags[i]);
                    bUnsorted = true;
                }
            }

            if (bUnsorted)
                lKeyWords.Sort(new TrueCompare());
        }
        #endregion
    }
}
