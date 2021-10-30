using Microsoft.Office.Interop.Outlook;

using System.Collections.Generic;
using System.Linq;

using TMS.Libraries.EmailSegmenter;

namespace TMS.Libraries.OutlookMailWrapper
{
    public class OutlookEmail : OutlookBase
    {

        #region Init

        private MailItem _COMEmail;
        internal OutlookEmail(OutlookFolder parentFolder, MailItem COMEmail)
        {
            _COMEmail = COMEmail;
            //ID = Guid.NewGuid();
            Folder = parentFolder;

            OutlookEntryID = COMEmail.EntryID;
            OutlookConversationID = COMEmail.ConversationID;
            OutlookConversationIndex = COMEmail.ConversationIndex;

            this.AttachmentsCount = COMEmail.Attachments.Count;

        }

        #endregion

        #region Properties

        private Segmenter SegmentedEmail;

        private HeaderSegmentEx _Header;
        public HeaderSegmentEx Header
        {
            get
            {
                if (_Header == null)
                {

                    var subject = new BodySegment(_COMEmail.Subject?.Trim()).Text;
                    var date = _COMEmail.SentOn.ToUniversalTime();
                    var from = _COMEmail.SenderEmailAddress.Trim();
                    List<string> to = null, cc = null;

                    // outlook may return contact name in CC & To properties, to get email address we use the following:
                    // from https://stackoverflow.com/questions/25758491/getting-an-email-address-instead-of-a-name

                    foreach (Recipient recip in _COMEmail.Recipients)
                    {
                        switch (recip.Type)
                        {
                            case (int)OlMailRecipientType.olTo:
                                {
                                    if (to == null)
                                        to = new List<string>();
                                    to.Add(recip.Address);
                                    break;
                                }
                            case (int)OlMailRecipientType.olCC:
                                {
                                    if (cc == null)
                                        cc = new List<string>();
                                    cc.Add(recip.Address);
                                    break;
                                }
                        };
                    }

                    _Header = new HeaderSegmentEx(from, to, cc, date, subject);
                }

                return _Header;

            }
        }

        public string OutlookEntryID { get; private set; }

        public string OutlookConversationID { get; private set; }

        public string OutlookConversationIndex { get; private set; }

        public int AttachmentsCount { get; private set; }

        public OutlookFolder Folder { get; private set; }

        // Hide this property
        private OutlookEmail ParentEmail { get; set; }

        public List<ReplaySegmentEx> Replays
        {
            get
            {
                if (SegmentedEmail == null)
                    SegmentedEmail = new Segmenter(_COMEmail.HTMLBody);

                return SegmentedEmail.Replays.Select(r => new ReplaySegmentEx(r)).ToList();
            }
        }

        public SignatureSegmentEx Signature
        {
            get
            {
                if (SegmentedEmail == null)
                    SegmentedEmail = new Segmenter(_COMEmail.HTMLBody);

                return new SignatureSegmentEx(SegmentedEmail.Signature);
            }
        }

        public BodySegmentEx Body
        {
            get
            {
                if (SegmentedEmail == null)
                    SegmentedEmail = new Segmenter(_COMEmail.HTMLBody);

                return new BodySegmentEx(SegmentedEmail.Body);
            }
        }

        List<OutlookEmail> _Conversations;

        /// <summary>
        /// Returns a list of emails that part of the current conversation as per Outlook.
        /// <para><strong>Note:</strong></para>
        /// <para>The returned emails can be located across different stores and folders.</para>
        /// </summary>
        public List<OutlookEmail> Conversations
        {
            get
            {
                if (_Conversations == null)
                {

                    _Conversations = new List<OutlookEmail>();

                    foreach (object comItem in _COMEmail.GetConversation()?.GetRootItems())
                    {
                        if (comItem is MailItem)
                        {
                            var comMail = comItem as MailItem;

                            var mail = Outlook.GetEmailByOutlookEntryID(comMail.EntryID);

                            if (mail != null)
                                _Conversations.Add(mail);

                        }
                    }
                }

                return _Conversations;
            }
        }

        #endregion

    }
}
