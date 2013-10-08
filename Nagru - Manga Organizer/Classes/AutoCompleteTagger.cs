using System;
using System.Windows.Forms;
using System.Collections.Generic;
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
        protected bool bListBoxAdded = false;
        protected bool bHScrollAdded = false;
        protected readonly char cSep = ',';

        public string[] KeyWords {
            get { return lKeyWords.ToArray(); }
            set {
                lKeyWords.Clear();
                lKeyWords.AddRange(value);
            }
        }
        #endregion

        public AutoCompleteTagger()
        {
            sbHorz = new HScrollBar();
            lbSuggest = new ListBox();
            lKeyWords = new List<string>();

            sbHorz.Scroll += sbHorz_Scroll;
            sbHorz.Height = 15;
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

        /* Get bounds of keyword based on caret position */
        private int getPrevSepCharIndex() {
            int iPos = base.SelectionStart == base.Text.Length ? 
                base.Text.Length - 1 : base.SelectionStart - 1;
            for (int i = iPos; i > -1; i--) {
                if (base.Text[i] == cSep)
                    return i + 1;
            }
            return 0;
        }
        private int getNextSepCharIndex(int iStart = -1)
        {
            if (iStart == -1) iStart = base.SelectionStart;
            for (int i = iStart; i < base.Text.Length; i++) {
                if (base.Text[i] == cSep)
                    return i;
            }
            return base.Text.Length;
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
            
            //hide & exit if empty
            if(sKey == string.Empty) {
                lbSuggest.Hide();
            }
            else {
                //re-pop suggestions
                string[] asOpt = lKeyWords.Where(x => x.StartsWith(sKey)).ToArray();
                if (asOpt.Length == 0) {
                    lbSuggest.Hide();
                }
                else {
                    asOpt = asOpt.Distinct().ToArray();
                    Array.Sort(asOpt, new TrueCompare());
                    lbSuggest.Items.Clear();
                    lbSuggest.Items.AddRange(asOpt);
                    
                    //set lbSuggest location
                    if (!bListBoxAdded) {
                        Parent.Controls.Add(lbSuggest);
                        bListBoxAdded = true;
                    }
                    lbSuggest.Width = Width;
                    lbSuggest.Left = Left;
                    lbSuggest.Top = Bottom;
                    lbSuggest.Show();
                    lbSuggest.BringToFront();
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
                    }
                    e.Handled = e.SuppressKeyPress = true;
                    break;
            }

            base.OnKeyDown(e);
        }

        /* Show\Hide scrollbar as needed */
        public void SetScroll()
        {
            int iWidth = TextRenderer.MeasureText(base.Text, base.Font).Width;
            if (iWidth > base.Width) {
                sbHorz.Maximum = iWidth / 5;
                sbHorz.Value = base.SelectionStart;

                if (!bHScrollAdded) {
                    Parent.Controls.Add(sbHorz);
                    bHScrollAdded = true;
                }
                sbHorz.Width = Width;
                sbHorz.Left = Left;
                sbHorz.Top = Bottom;
                sbHorz.Show();
            }
            else sbHorz.Hide();
        }

        protected override void OnTextChanged(EventArgs e)
        {
            SetScroll();
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
            lbSuggest.Hide();
            base.OnClick(e);
        }

        ~AutoCompleteTagger()
        {
            lbSuggest.Dispose();
            sbHorz.Dispose();
        }
    }
}
