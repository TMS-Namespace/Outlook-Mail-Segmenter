using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using TMS.Libraries.EmailSegmentation.Segmentor.SegmentedEmailParts;

namespace TMS.Libraries.EmailsSources.XMLPresentation
{

    #region Enums

    //public enum Sources
    //{
    //    Agent,
    //    CAA,
    //    Client,
    //    Relative,
    //    Spam,
    //    Supplier,
    //    NewsLetter,
    //    Phishing,
    //    Proposal,
    //    Notam,

    //}

    //public enum Services
    //{
    //    Catering,
    //    Credit_Note,
    //    Flight_Plan,
    //    Fuel,
    //    Handling,
    //    Hotac,
    //    Invoice,
    //    Landing_Permit,
    //    Movment,
    //    Navigation,
    //    Overflight_Permit,
    //    Relative,
    //    Slot,
    //    Statement,

    //}

    //public enum Topics
    //{
    //    Acknowledge,
    //    Price,
    //    Reminder,
    //    Service,
    //    Information,
    //    Issue,
    //    Question,
    //    Confirmation,
    //    Relative

    //}

    //public enum Actions
    //{
    //    Cancelation,
    //    Correction,
    //    Delay,
    //    Delivering,
    //    Relative,
    //    Request,
    //    Revision,
    //    Resolve,

    //}

    //public enum Types
    //{
    //    Email,
    //    SITA,
    //    AFTN,

    //}

    #endregion

    [Serializable]
    [XmlRoot("Emails")]
    public class Email : Replay
    {
        #region Init

        public Email() { }

        public Email(SegmentedEmailMainPart segmentedEmail,
                        string entryID,
                        string conversationID,
                        string conversationIndex,
                        int attachmentsCount) : base(segmentedEmail)
        {

            //var em= email as SegmentedEmailMainPart;

            EntryID = entryID;
            ConversationID = conversationID;
            ConversationIndex = conversationIndex;
            //Replays = segmentedEmail.Replays?.Select(c => new Replay(c)).ToList();
            //Header = new Header(segmentedEmail.Header);
            //Signature = (segmentedEmail.Signature == null) ? null : new Signature(segmentedEmail.Signature);
            AttachmentsCount = attachmentsCount;
        }

        #endregion

        #region Properties

        public string EntryID { get; set; }

        public string ConversationID { get; set; }

        public string ConversationIndex { get; set; }

        private List<Replay> _Replays;
        [XmlArray]
        public List<Replay> Replays
        {
            get
            {

                if (Origin != null && _Replays == null)
                    _Replays = ((SegmentedEmailMainPart)Origin).Replays?.Select(c => new Replay(c)).ToList();

                return _Replays;


            }
            set { _Replays = value; }
        }
        public int AttachmentsCount { get; set; }

        //#region WAP properties

        //public bool HasParsingError { get; set; }

        //public Types? Type { get; set; }

        //public Sources? Source { get; set; }

        //[XmlArray]
        //public List<Services> Services { get; set; }

        //[XmlArray]
        //public List<Topics> Topics { get; set; }

        //[XmlArray]
        //public List<Actions> Actions { get; set; }

        //#endregion

        #endregion

    }
}
