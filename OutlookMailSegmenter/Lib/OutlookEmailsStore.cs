using Microsoft.Office.Interop.Outlook;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TMS.Libraries.OutlookMailSegmenter
{
    public class OutlookEmailsStore : IOutlookEntity
    {


        #region Init

        private Folder _COMFolder;
        internal OutlookEmailsStore(Folder COMfolder)
        {
            _COMFolder = COMfolder;

            this.ID = Guid.NewGuid();

            OutlookEntryID = COMfolder.EntryID;

            Name = COMfolder.Name;
        }

        #endregion

        #region Properties

        public string Name { get; private set; }


        private List<OutlookFolder> _Folders;
        public List<OutlookFolder> Folders
        {
            get
            {
                if (_Folders is null)
                {
                    _Folders = new List<OutlookFolder>();

                    foreach (Folder fd in _COMFolder.Folders)
                        _Folders.Add(new OutlookFolder(this, null, fd));
                }

                return _Folders;
            }
        }

        public Guid ID { get; private set; }

        public string OutlookEntryID { get; private set; }



        #endregion

        #region Help Methods


        internal OutlookFolder GetFolderByOutlookEntryID(string entryID)
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
