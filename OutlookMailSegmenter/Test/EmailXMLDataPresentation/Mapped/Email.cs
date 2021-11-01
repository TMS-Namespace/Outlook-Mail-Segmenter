using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

using TMS.Libraries.OutlookMailWrapper;

namespace TMS.Libraries.EmailXMLDataPresentation
{

    #region Enums

    public enum Sources
    {
        Agent,
        CAA,
        Client,
        Relative,
        Spam,
        Supplier,
        NewsLetter,
        Phishing,
        Proposal,
        Notam,

    }

    public enum Services
    {
        Catering,
        Credit_Note,
        Flight_Plan,
        Fuel,
        Handling,
        Hotac,
        Invoice,
        Landing_Permit,
        Movment,
        Navigation,
        Overflight_Permit,
        Relative,
        Slot,
        Statement,

    }

    public enum Topics
    {
        Acknowledge,
        Price,
        Reminder,
        Service,
        Information,
        Issue,
        Question,
        Confirmation,
        Relative

    }

    public enum Actions
    {
        Cancelation,
        Correction,
        Delay,
        Delivering,
        Relative,
        Request,
        Revision,
        Resolve,

    }

    public enum Types
    {
        Email,
        SITA,
        AFTN,

    }

    #endregion

    [Serializable]
    [XmlRoot("Emails")]
    public class Email : Replay
    {
        #region Init

        public Email() { }

        public Email(OutlookEmail email) : base(email)
        {

            OutlookEntryID = email.OutlookEntryID;
            OutlookConversationID = email.OutlookConversationID;
            OutlookConversationIndex = email.OutlookConversationIndex;
            Replays = email.Replays?.Select(c => new Replay(c)).ToList();
            Header = new Header(email.Header);
            Signature = (email.Signature == null) ? null : new Signature(email.Signature);
            AttachmentsCount = email.AttachmentsCount;
        }

        #endregion

        #region Properties

        public string OutlookEntryID { get; set; }

        public string OutlookConversationID { get; set; }

        public string OutlookConversationIndex { get; set; }

        [XmlArray]
        public List<Replay> Replays { get; set; }
        public int AttachmentsCount { get; set; }

        #region WAP properties

        public bool HasParsingError { get; set; }

        public Types? Type { get; set; }

        public Sources? Source { get; set; }

        [XmlArray]
        public List<Services> Services { get; set; }

        [XmlArray]
        public List<Topics> Topics { get; set; }

        [XmlArray]
        public List<Actions> Actions { get; set; }

        #endregion

        #endregion

    }
}
