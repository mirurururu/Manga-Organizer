using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Security.Permissions;

/* Explorer-style file\folder browser 
   Based on MSDN Example (retrieved Oct 12, 2013) */
namespace Nagru___Manga_Organizer
{
    public partial class Explorer : Form
    {
        #region Properties
        TreeNode tnNew = null;
        List<List<string>> lFilters;

        public string Title = "Select the location of the current entry:";
        public string InitialDirectory = ".";
        public string Selection = "";
        public string Filter {
            private get {
                if (lFilters == null || lFilters.Count == 0)
                    return null;
                else return "NotEmpty";
            }
            set {
                lFilters = new List<List<string>>();
                string[] asBase = value.Split('|');

                int x = 0;
                for (int i = 0; i < asBase.Length; i++) {
                    if (i % 2 == 0) {
                        CmbBx_Filters.Items.Add(asBase[i]);
                    } else {
                        string[] asSub = asBase[i].Split(';');
                        lFilters.Add(new List<string>());
                        lFilters[x++].AddRange(asSub);
                    }
                }
                CmbBx_Filters.SelectedIndex = 0;
            }
        }
        #endregion

        /* Set up Form according to passed values */
        public Explorer()
        {
            InitializeComponent();
            this.Icon = Properties.Resources.dbIcon;
        }
        private void Explorer_Load(object sender, EventArgs e)
        {
            lvCurr.SuspendLayout();

            if (!Directory.Exists(InitialDirectory))
                InitialDirectory = ".";

            Text = Title;
            PopulateTreeView();
            tvDir.Nodes[0].Toggle();
            tvDir.SelectedNode = tvDir.Nodes[0];
            tvDir.Select();
            PopulateListView(InitialDirectory);

            if (Selection == "" || Selection == InitialDirectory) {
                TxBx_Path.Text = tvDir.Nodes[0].FullPath;
            } 
            else {
                List<string> sNodes = Selection.Split('\\').ToList<string>();
                sNodes.RemoveAt(0);
                string sLast = sNodes.Last();

                if (Selection.Contains('.'))
                    sNodes.RemoveAt(sNodes.Count - 1);

                FindNode(tvDir.Nodes[0], tvDir.Nodes[0].Text
                    + "\\" + String.Join("\\", sNodes));
                tvDir.SelectedNode = tnNew ?? tvDir.Nodes[0];
                tvDir.SelectedNode.Expand();
                tvDir.SelectedNode.EnsureVisible();
                PopulateListView(tvDir.SelectedNode.Tag.ToString());
                TxBx_Path.Text = tvDir.SelectedNode.FullPath;

                int iIndx = FindLVI(sLast);
                if(iIndx != -1) {
                    lvCurr.FocusedItem = lvCurr.Items[iIndx];
                    lvCurr.Items[iIndx].Selected = true;

                    /* Compensate for broken scroll-to function */
                    lvCurr.TopItem = lvCurr.Items[iIndx];
                    lvCurr.TopItem = lvCurr.Items[iIndx];
                    lvCurr.TopItem = lvCurr.Items[iIndx];
                }
            }
            
            if (Filter == null) Filter = "All Files (*.*)|*.*";
            ApplyFilter();
            lvCurr.ResumeLayout();
        }

        /* More convenient control selection */
        private void lvCurr_MouseHover(object sender, EventArgs e)
        { lvCurr.Select(); }
        private void tvDir_MouseHover(object sender, EventArgs e)
        { tvDir.Select(); }

        /* Ensure combobox path is accurate */
        private void tvDir_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            TxBx_Path.Text = e.Node.FullPath;
            PopulateListView(e.Node.Tag.ToString());
        }
        private void lvCurr_ItemSelectionChanged(object sender,
            ListViewItemSelectionChangedEventArgs e)
        {
            TxBx_Path.Text = tvDir.SelectedNode.FullPath +
                '\\' + e.Item.SubItems[0].Text;
        }

        /* Allow user to traverse nodes using listview  */
        private void lvCurr_DoubleClick(object sender, EventArgs e)
        {
            if (lvCurr.SelectedItems[0].SubItems[0].Text.Contains('.')) return;
            string sPath = tvDir.SelectedNode.FullPath + "\\"
                + lvCurr.SelectedItems[0].SubItems[0].Text;

            FindNode(tvDir.Nodes[0], sPath);
            if (tnNew == null) return;

            tvDir.SelectedNode = tnNew;
            tvDir.SelectedNode.Expand();
            tvDir.SelectedNode.EnsureVisible();

            PopulateListView(tvDir.SelectedNode.Tag.ToString());
        }

        private void Btn_Open_Click(object sender, EventArgs e)
        {
            Selection = InitialDirectory + 
                TxBx_Path.Text.Replace(tvDir.Nodes[0].Text, "");
        }

        #region CustomMethods
        /* Fake regex to remove non-matching files */
        private void ApplyFilter()
        {
            lvCurr.BeginUpdate();
            int x = CmbBx_Filters.SelectedIndex;
            bool bKeep, bLeft, bRight;

            for (int i = lvCurr.Items.Count - 1; i > -1; i--) {
                if (!lvCurr.Items[i].SubItems[0].Text.Contains('.')) continue;
                bKeep = false;

                for(int y = 0; y < lFilters[x].Count; y++) {
                    bLeft = lFilters[x][y].StartsWith("*");
                    bRight = lFilters[x][y].EndsWith("*");

                    if ((bLeft && bRight)
                        || (!bLeft && lvCurr.Items[i].SubItems[0].Text.
                            StartsWith(lFilters[x][y].Split('.').ElementAt(0)))
                        || (!bRight && lvCurr.Items[i].SubItems[0].Text.
                            EndsWith(lFilters[x][y].Split('.').ElementAt(1)))) {
                        bKeep = true;
                        continue;
                    }
                }

                if (!bKeep) 
                    lvCurr.Items.RemoveAt(i);
            }
            lvCurr.EndUpdate();
        }
        
        private int FindLVI(string sItem)
        {
            for(int i = 0; i < lvCurr.Items.Count; i++) {
                if (lvCurr.Items[i].Text.Length == sItem.Length
                        && lvCurr.Items[i].Text == sItem)
                    return i;
            }

            return -1;
        }
        
        private void FindNode(TreeNode atn, string sPath)
        {
            foreach (TreeNode n in atn.Nodes) {
                if (n.FullPath.Length == sPath.Length
                        && String.CompareOrdinal(n.FullPath, sPath) == 0) {
                    tnNew = n;
                    break;
                }
                FindNode(n, sPath);
            }
        }

        /* Recursively traverse directory */
        private void GetDirectories(string[] asDirs, TreeNode tnBase)
        {
            //breaks on processing 
            //C:\Windows\System32\LogFiles\WMI\RtBackup
            //(locked)

            TreeNode tnCurr;
            string[] asSubDirs = new string[0];
            DirectoryInfo diCurr;

            for (int i = 0; i < asDirs.Length; i++) {
                //if(asDirs[i] == @"C:\Windows\System32") continue;
                //if(IsRestricted(asDirs[i])) continue;
                
                diCurr = new DirectoryInfo(asDirs[i]);
                tnCurr = new TreeNode(diCurr.Name, 0, 0);
                tnCurr.Tag = diCurr;
                tnCurr.ImageKey = "folder";

                try {
                    asSubDirs = Directory.GetDirectories(asDirs[i]);
                } catch (UnauthorizedAccessException) {
                    //Console.WriteLine(asDirs[i]);
                } finally {
                    if (asSubDirs.Length > 0) {
                        GetDirectories(asSubDirs, tnCurr);
                        tnBase.Nodes.Add(tnCurr);
                    }
                }
            }
        }

        private static bool IsRestricted(string sPath)
        {
            return (File.GetAttributes(sPath) & FileAttributes.Hidden) != 0;
        }

        /*Fill listview using passed directory */
        private void PopulateListView(string sPath)
        {
            lvCurr.BeginUpdate();
            DirectoryInfo diNode = new DirectoryInfo(sPath);
            ListViewItem.ListViewSubItem[] alvsi;
            ListViewItem lvi = null;
            lvCurr.Items.Clear();

            DirectoryInfo[] adi = diNode.GetDirectories();
            for (int i = 0; i < adi.Length; i++) {
                if (IsRestricted(adi[i].FullName)) continue;

                lvi = new ListViewItem(adi[i].Name, 0);
                alvsi = new ListViewItem.ListViewSubItem[] {
                    new ListViewItem.ListViewSubItem(lvi, "Directory"),
                    new ListViewItem.ListViewSubItem(lvi,
                        adi[i].LastAccessTime.ToShortDateString())
                };
                lvi.SubItems.AddRange(alvsi);
                lvCurr.Items.Add(lvi);
            }
            FileInfo[] afi = diNode.GetFiles();
            for (int i = 0; i < afi.Length; i++) {
                lvi = new ListViewItem(afi[i].Name, 1);
                alvsi = new ListViewItem.ListViewSubItem[] {
                    new ListViewItem.ListViewSubItem(lvi, "File"),
                    new ListViewItem.ListViewSubItem(lvi,
                        afi[i].LastAccessTime.ToShortDateString())};

                lvi.SubItems.AddRange(alvsi);
                lvCurr.Items.Add(lvi);
            }

            lvCurr.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            ApplyFilter();
            lvCurr.EndUpdate();
        }

        /* Fill tvDir using InitialDirectory */
        private void PopulateTreeView()
        {
            DirectoryInfo diInit;
            TreeNode tnRoot;

            diInit = new DirectoryInfo(@InitialDirectory);
            if (diInit.Exists) {
                tnRoot = new TreeNode(diInit.Name);
                tnRoot.Tag = diInit;
                GetDirectories(Directory.GetDirectories(@InitialDirectory), tnRoot);
                tvDir.Nodes.Add(tnRoot);
            }
        }
        #endregion
    }
}
