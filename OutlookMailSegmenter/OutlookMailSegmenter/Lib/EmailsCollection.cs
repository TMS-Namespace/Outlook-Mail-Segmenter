using Microsoft.Office.Interop.Outlook;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TMS.Libraries.OutlookMailSegmenter
{
    public class EmailsCollection : List<OutlookEmail>
    {
        #region Init

        internal Folder _COMFolder;
        internal EmailsCollection(OutlookFolder folder)
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
            EmailChunk.BaseMailChunks?.Clear();
            GC.Collect();
        }

        internal new void Add(OutlookEmail mail)
        {
            base.Add(mail);
        }

        public void Fetch(int from, int to)
        {
            if (from > to || from < 0 || to < 0 || to > TotalCount - 1)
                throw new System.Exception("Invalid Fetch' boundaries are specified.");


            var mails = _COMFolder.Items.Cast<MailItem>().Skip(from).Take(to - from + 1).ToList();

            if (Outlook.ProcessInParallel)
                mails.AsParallel().ForAll(m => this.Add(new OutlookEmail(Folder, m)));
            else
                mails.ForEach(m => this.Add(new OutlookEmail(Folder, m)));
        }

        internal OutlookEmail GetEmailByOutlookEntryID(string entryID)
        {
            return this.SingleOrDefault(s => s.OutlookEntryID == entryID);
        }

        #endregion

    }
}