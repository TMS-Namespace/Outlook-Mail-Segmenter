using Microsoft.Office.Interop.Outlook;

using System.Collections.Generic;
using System.Linq;

using TMS.Libraries.ClassicalEmailSegmenter;
using TMS.Libraries.OutlookMailWrapper.Helpers;

namespace TMS.Libraries.OutlookMailWrapper
{
    public class OutlookEmail : OutlookBase, IMessage
    {

        #region Init

        private MailItem _COMEmail;
        internal OutlookEmail(OutlookFolder parentFolder, MailItem COMEmail)
        {
            _COMEmail = COMEmail;
            Folder = parentFolder;
            OutlookEntryID = COMEmail.EntryID;

        }

        #endregion

        #region Properties

        private Segmenter ClassicalEmailSegmenter;

        private HeaderSegmentEx _Header;
        public HeaderSegmentEx Header
        {
            get
            {
                if (_Header == null)
                {

                    var subject = Shared.FixBadCharacters(_COMEmail.Subject?.Trim());
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

                    _Header = new HeaderSegmentEx(from, to, cc, date, subject, this);
                }

                return _Header;

            }
        }

        public string OutlookConversationID => _COMEmail.ConversationID;

        public string OutlookConversationIndex => _COMEmail.ConversationIndex;

        public int AttachmentsCount => _COMEmail.Attachments.Count;

        public OutlookFolder Folder { get; private set; }

        public IEmailPart Parent => null;


        private List<ReplaySegmentEx> _Replays;
        public List<ReplaySegmentEx> Replays
        {
            get
            {
                if (_Replays == null)
                {
                    if (ClassicalEmailSegmenter == null)
                        ClassicalEmailSegmenter = new Segmenter(_COMEmail.HTMLBody);

                    _Replays = ClassicalEmailSegmenter.Replays.Select(r => new ReplaySegmentEx(r, this)).ToList();
                }

                return _Replays;
            }
        }


        private SignatureSegmentEx _Signature;
        public SignatureSegmentEx Signature
        {
            get
            {
                if (_Signature == null)
                {
                    if (ClassicalEmailSegmenter == null)
                        ClassicalEmailSegmenter = new Segmenter(_COMEmail.HTMLBody);

                    _Signature = new SignatureSegmentEx(ClassicalEmailSegmenter.Signature, this);
                }
                return _Signature;
            }
        }


        private BodySegmentEx _Body;
        public BodySegmentEx Body
        {
            get
            {
                if (ClassicalEmailSegmenter == null)
                    ClassicalEmailSegmenter = new Segmenter(_COMEmail.HTMLBody);

                if (_Body == null)
                    _Body = new BodySegmentEx(ClassicalEmailSegmenter.Body, this);

                return _Body;
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
