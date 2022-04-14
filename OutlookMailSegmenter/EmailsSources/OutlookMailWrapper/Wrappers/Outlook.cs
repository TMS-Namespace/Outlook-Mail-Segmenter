using Microsoft.Office.Interop.Outlook;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TMS.Libraries.EmailsSources.OutlookMailWrapper
{

    // note that this library can be referenced and used only form the .net 5 projects that has also the 
    // windows forms framework as a reference, this is because office interop is a windows com object and
    // can't be loaded from a a cross platform framework like .net 5 or core, although it will work fine with 
    // older non core frameworks.
    public static class Outlook
    {


        #region Properties

        //internal static List<BodySegmentEx> AllBodies { get; } = new List<BodySegmentEx>();

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



        //public static bool CheckForIdenticalBodySegments { get; set; } = true;

        //public static bool GreedyHeadersProcessing { get; set; } = true;
        //public static bool GreedyReplaysProcessing { get; set; } = true;
        //public static bool GreedySignaturesProcessing { get; set; } = true;

        //public static bool ProcessInParallel { get; set; } = true;

        #endregion

        #region Help Methods


        internal static OutlookEmailsStore GetStoreByOutlookEntryID(string entryID)
        {
            return Outlook.Stores.SingleOrDefault(s => s.OutlookEntryID == entryID);
        }

        #endregion

        #region Methods
        public static void Connect()
        {
            _EmailStores?.Clear();

            Application outlookApplication = new Application();

            outlookNameSpace = outlookApplication.GetNamespace("MAPI");

            outlookNameSpace.Logon("", "", Missing.Value, Missing.Value);

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
