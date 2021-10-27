using Microsoft.Office.Interop.Outlook;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace TMS.Libraries.OutlookMailSegmenter
{
    public static class Outlook
    {

        #region Methods

        public static void Connect()
        {
            _EmailStores?.Clear();

            Application outlookApplication = new Application();

            outlookNameSpace = outlookApplication.GetNamespace("MAPI");

            outlookNameSpace.Logon("", "", Missing.Value, Missing.Value);

        }

        #endregion

        #region Properties

        private static NameSpace outlookNameSpace;

        private static List<OutlookEmailsStore> _EmailStores;
        public static List<OutlookEmailsStore> Stores
        {
            get
            {
                // for simplicity, we will use store's root folder as a store directly

                if (_EmailStores is null)
                {
                    _EmailStores = new List<OutlookEmailsStore>();

                    foreach (Folder fd in outlookNameSpace.Folders)
                        _EmailStores.Add(new OutlookEmailsStore(fd));
                }

                // or, if we need original stores...
                //foreach (Store s in OutlookNameSpace.Stores)
                //{
                //    _EmailSources.Add(new OutlookEmailsSource(s.GetRootFolder());
                //}

                return _EmailStores;
            }

        }

        public static bool ProcessAllReplays { get; set; } = true;

        public static bool ProcessSignatures { get; set; } = true;

        public static bool CheckForIdenticalChunks { get; set; } = true;

        public static bool ProcessHeaders { get; set; } = true;

        public static bool ProcessInParallel { get; set; } = true;

        #endregion

        #region Help Methods


        internal static OutlookEmailsStore GetStoreByOutlookEntryID(string entryID)
        {
            return Outlook.Stores.SingleOrDefault(s => s.OutlookEntryID == entryID);
        }

        public static OutlookEmail GetEmailByOutlookEntryID(string entryID)
        {
            var comMail = outlookNameSpace.GetItemFromID(entryID) as MailItem;
            var comFolder = comMail?.Parent as Folder;
            var comStore = comFolder?.Store.GetRootFolder() as Folder;

            var store = Outlook.GetStoreByOutlookEntryID(comStore.EntryID);
            var folder = store?.GetFolderByOutlookEntryID(comFolder.EntryID);

            var email = folder?.Emails.GetEmailByOutlookEntryID(comMail.EntryID);

            if (email == null && folder != null)
            {
                email = new OutlookEmail(folder, comMail);
                folder.Emails.Add(email);
            }

            return email;

        }

        #endregion

    }
}
