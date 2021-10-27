using System;
using System.Windows.Forms;
using TMS.Libraries.OutlookMailSegmenter;

namespace TMS.Apps.OutlookMailSegmenter.Test
{
    public partial class FrmTest : Form
    {
        public FrmTest()
        {
            InitializeComponent();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {

                Outlook.Connect();
                foreach (OutlookEmailsStore fd in Outlook.Stores)
                {
                    cbDataSource.Items.Add(fd.Name);

                }

                lbConnect.Text = "Success!";

            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void cbDataSource_SelectedIndexChanged(object sender, EventArgs e)
        {

            tvFolders.Nodes.Clear();

            foreach (OutlookFolder fld in Outlook.Stores[cbDataSource.SelectedIndex].Folders)
            {

                var nd = new TreeNode(fld.Name);
                nd.Tag = fld;

                tvFolders.Nodes.Add(nd);

                PopulateSubTree(nd, fld);
            }
        }

        private void PopulateSubTree(TreeNode parentNode, OutlookFolder parentFolder)
        {
            foreach (OutlookFolder fld in parentFolder.Folders)
            {
                var nd = new TreeNode(fld.Name);
                nd.Tag = fld;

                parentNode.Nodes.Add(nd);


                PopulateSubTree(nd, fld);
            }
        }

        private OutlookFolder _SelectedFolder;

        private void tvFolders_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var nd = (TreeNode)tvFolders.SelectedNode;
            _SelectedFolder = (OutlookFolder)nd.Tag;

            lbCount.Text = "Email count: " + _SelectedFolder.Emails.TotalCount;

        }


        private void btnFetch_Click(object sender, EventArgs e)
        {

            if (_SelectedFolder != null)
            {
                lbProgress.Text = "processing..";

                int from = int.Parse(tbFrom.Text) - 1;
                int to = int.Parse(tbTo.Text) - 1;

                _SelectedFolder.Emails.Clear();

                var watch = System.Diagnostics.Stopwatch.StartNew();

                _SelectedFolder.Emails.Fetch(from, to);

                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;

                lbProgress.Text = string.Format("Done! in {0} sec", Math.Round((double)elapsedMs / 1000, 2));

            }

        }



        private void chbProcessAllReplaes_CheckedChanged(object sender, EventArgs e)
        {
            Outlook.ProcessAllReplays = chbProcessAllReplaes.Checked;
        }

        private void chbProcessSignitures_CheckedChanged(object sender, EventArgs e)
        {
            Outlook.ProcessSignatures = chbProcessSignitures.Checked;
        }

        private void chbProcessHeaders_CheckedChanged(object sender, EventArgs e)
        {
            Outlook.ProcessHeaders = chbCheckForIdenticalChunks.Checked;

        }

        private void chbCheckForIdenticalChunks_CheckedChanged_1(object sender, EventArgs e)
        {
            Outlook.CheckForIdenticalChunks = chbProcessHeaders.Checked;
        }

        private void chbProcessInParallel_CheckedChanged(object sender, EventArgs e)
        {
            Outlook.ProcessInParallel = chbProcessInParallel.Checked;

        }
    }
}
