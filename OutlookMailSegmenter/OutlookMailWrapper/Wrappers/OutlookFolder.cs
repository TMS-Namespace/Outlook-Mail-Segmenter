using Microsoft.Office.Interop.Outlook;

using System.Collections.Generic;

namespace TMS.Libraries.OutlookMailWrapper
{
    public class OutlookFolder : OutlookBase
    {

        #region Init

        internal Folder _COMFolder;
        internal OutlookFolder(OutlookEmailsStore store, OutlookFolder parent, Folder COMFolder)
        {
            _COMFolder = COMFolder;
            Parent = parent;
            Store = store;
            OutlookEntryID = COMFolder.EntryID;
        }

        #endregion

        #region Properties

        public string Name => _COMFolder.Name;

        public OutlookFolder Parent { get; private set; }

        public OutlookEmailsStore Store { get; private set; }


        private List<OutlookFolder> _Folders;
        public List<OutlookFolder> Folders
        {
            get
            {
                if (_Folders is null)
                {
                    _Folders = new List<OutlookFolder>();

                    foreach (Folder fd in _COMFolder.Folders)
                        _Folders.Add(new OutlookFolder(this.Store, this, fd));

                }

                return _Folders;

            }
        }

        private OutlookEmailsCollection _Emails;
        public OutlookEmailsCollection Emails
        {
            get
            {
                if (_Emails is null)
                    _Emails = new OutlookEmailsCollection(this);
                return _Emails;
            }

        }

        #endregion

        #region Help Methods

        internal OutlookFolder GetSubFolderByOutlookEntryID(string entryID)
        {
            foreach (OutlookFolder folder in this.Folders)
            {
                if (folder.OutlookEntryID == entryID)
                    return folder;
                else
                {
                    var res = folder.GetSubFolderByOutlookEntryID(entryID);
                    if (res != null)
                        return res;
                }
            }

            return null;
        }

        #endregion

    }
}
