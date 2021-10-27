using System;

namespace TMS.Libraries.OutlookMailSegmenter
{
    /// <summary>
    /// Outlook objects, has EntryID, so we distinguish our wrappers that represents real outlook object by naming it as Outlook{Name}, and they will inherit this interface.
    /// Also, we use ID as an internal, not related to outlook, identifier.
    /// </summary>
    public interface IOutlookEntity
    {

        Guid ID { get; }

        string OutlookEntryID { get; }

    }
}
