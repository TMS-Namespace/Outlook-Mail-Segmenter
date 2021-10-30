using System;

namespace TMS.Libraries.OutlookMailWrapper
{
    /// <summary>
    /// Outlook objects, has EntryID, so we distinguish our wrappers that represents real outlook object by naming it as Outlook{Name}, and they will inherit this interface.
    /// Also, we use ID as an internal, not related to outlook, identifier.
    /// </summary>
    public class OutlookBase
    {

        #region Init

        public OutlookBase() { ID = Guid.NewGuid(); }

        #endregion

        #region Properties

        public Guid ID { get; private set; }

        string OutlookEntryID { get; }

        #endregion

    }
}
