using Microsoft.Office.Interop.Outlook;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TMS.Libraries.OutlookMailWrapper
{
    public class OutlookEmailsCollection : List<OutlookEmail>
    {
        #region Init

        internal Folder _COMFolder;
        internal OutlookEmailsCollection(OutlookFolder folder)
        {
            ID = Guid.NewGuid();
            Folder = folder;
            _COMFolder = folder._COMFolder;
        }

        #endregion

        #region Properties

        public Guid ID { get; private set; }

        public OutlookFolder Folder { get; private set; }

        public int TotalCount => _COMFolder.Items.Count;

        #endregion

        #region Methods

        // Override, to reset the fetched data
        public new void Clear()
        {
            base.Clear();

            Outlook.AllBodies.Clear();

            GC.Collect();
        }

        internal new void Add(OutlookEmail mail)
        {
            base.Add(mail);
        }

        public void FetchLatest(int from, int to)
        {
            if (from > to || from < 0 || to < 0 || to > TotalCount - 1)
                throw new System.Exception("Invalid Fetch' boundaries are specified.");

            var items = _COMFolder.Items;
            items.Sort("[ReceivedTime]", true);
            var mails = items.Cast<MailItem>().Skip(from).Take(to - from + 1).ToList();

            // when in parallel we pre-reload all lazy properties
            if (Outlook.ProcessInParallel)
                mails
                    .AsParallel().
                    ForAll(m =>
                    {
                        var email = new OutlookEmail(Folder, m);
                        PreloadProperties(email);
                        this.Add(email);
                    });
            else
                mails.ForEach(m => this.Add(new OutlookEmail(Folder, m)));

        }

        internal OutlookEmail GetEmailByOutlookEntryID(string entryID)
        {
            return this.SingleOrDefault(s => s.OutlookEntryID == entryID);
        }


        private void PreloadProperties(object obj)
        {
            if (obj is OutlookEmail)
            {
                var casted = (OutlookEmail)obj;
                PropertyInfo[] props = casted.GetType().GetProperties();
                // we exclude conversations, since they are not a parsing result
                props.ToList().ForEach(p =>
                {
                    var tmp = (p.Name != "Conversations") ? p.GetValue(casted) : null;
                }
                );
                PreloadProperties(casted.Body);
                PreloadProperties(casted.Signature);
                PreloadProperties(casted.Replays);

            }

            if (obj is HeaderSegmentEx && Outlook.GreedyHeadersProcessing)
            {
                var casted = (HeaderSegmentEx)obj;
                PropertyInfo[] props = casted.GetType().GetProperties();
                props.ToList().ForEach(p => p.GetValue(casted));
                PreloadProperties(casted.Body);
            }

            if (obj is SignatureSegmentEx && Outlook.GreedySignaturesProcessing)
            {
                var casted = (SignatureSegmentEx)obj;
                PropertyInfo[] props = casted.GetType().GetProperties();
                props.ToList().ForEach(p => p.GetValue(casted));
                PreloadProperties(casted.Body);
            }

            if (obj is ReplaySegmentEx && Outlook.GreedyReplaysProcessing)
            {
                var casted = (ReplaySegmentEx)obj;
                PropertyInfo[] props = casted.GetType().GetProperties();
                props.ToList().ForEach(p => p.GetValue(casted));
                PreloadProperties(casted.Body);
                PreloadProperties(casted.Signature);
                PreloadProperties(casted.Header);
            }

            if (obj is BodySegmentEx)
            {
                var casted = (BodySegmentEx)obj;
                PropertyInfo[] props = casted.GetType().GetProperties();
                props.ToList().ForEach(p => p.GetValue(casted));
            }

        }

        #endregion

    }
}