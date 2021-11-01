using System;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.IO;

using TMS.Libraries.EmailXMLDataPresentation;
using TMS.Libraries.OutlookMailWrapper;

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
                    cbDataSource.Items.Add(fd.Name);


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

        private EmailsData Data = new EmailsData();

        private void btnFetch_Click(object sender, EventArgs e)
        {
            Outlook.ProcessInParallel = chbProcessInParallel.Checked;
            Outlook.CheckForIdenticalBodySegments = chbCheckForIdenticalChunks.Checked;

            Outlook.GreedyHeadersProcessing = chbGreedyHeaders.Checked;
            Outlook.GreedySignaturesProcessing = chbGreedySignatures.Checked;
            Outlook.GreedyReplaysProcessing = chbGreedyReplays.Checked;

            if (_SelectedFolder != null)
            {
                lbProgress.Text = "processing..";

                int from = int.Parse(tbFrom.Text) - 1;
                int to = int.Parse(tbTo.Text) - 1;

                _SelectedFolder.Emails.Clear();
                Data.Emails.Clear();

                var watch = System.Diagnostics.Stopwatch.StartNew();

                _SelectedFolder.Emails.FetchLatest(from, to);

                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;

                _SelectedFolder
                    .Emails
                    .ToList()
                    .ForEach(e => Data.Emails.Add(new Email(e)));

                Data.Emails = Data.Emails.OrderByDescending(e => e.Header.Date).ToList();

                lbProgress.Text = string.Format("Done! in {0} sec", Math.Round((double)elapsedMs / 1000, 2));
            }

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SFD.Filter = "XML Files (*.xml)|*.xml";

            if (SFD.ShowDialog(this) == DialogResult.OK)
            {
                XmlSerializer xs = new XmlSerializer(typeof(EmailsData));
                using (TextWriter tw = new StreamWriter(SFD.FileName))
                {
                    xs.Serialize(tw, Data);

                    tw.Close();
                }
            }
        }
    }
}
