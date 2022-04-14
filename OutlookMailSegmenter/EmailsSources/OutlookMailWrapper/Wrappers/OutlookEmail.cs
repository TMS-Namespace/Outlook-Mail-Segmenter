using Microsoft.Office.Interop.Outlook;
using System;
using System.Collections.Generic;

namespace TMS.Libraries.EmailsSources.OutlookMailWrapper
{
    public class OutlookEmail : OutlookBase
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

        //private Segmenter ClassicalEmailSegmenter;

        //public struct HeaderInfo
        //{
        //    public string Subject;
        //    public DateTime? Date;

        //    public List<string> To;
        //    public List<string> CC;

        //    public string From;

        //}

        public string Subject => _COMEmail.Subject?.Trim();
        public DateTime Date => _COMEmail.SentOn.ToUniversalTime();

        public string From => _COMEmail.SenderEmailAddress.Trim();
        public List<string> To
        {
            get
            {
                if (_To is null) FindRecipients();

                return _To;

            }
        }

        public List<string> CC
        {
            get
            {
                if (_CC is null) FindRecipients();

                return _CC;

            }
        }


        List<string> _To = null, _CC = null;
        private void FindRecipients()
        {

            _To ??= new();
            _CC ??= new();


            foreach (Recipient recip in _COMEmail.Recipients)
            {
                switch (recip.Type)
                {
                    case (int)OlMailRecipientType.olTo:
                        {
                            _To.Add(recip.Address);
                            break;
                        }
                    case (int)OlMailRecipientType.olCC:
                        {
                            _CC.Add(recip.Address);
                            break;
                        }
                };
            }
        }

        //private HeaderInfo _Header;
        //public HeaderInfo Header
        //{
        //    get
        //    {
        //        if (_Header)
        //        {

        //            var subject = _COMEmail.Subject?.Trim();// Shared.FixBadCharacters(_COMEmail.Subject?.Trim());
        //            var date = _COMEmail.SentOn.ToUniversalTime();
        //            var from = _COMEmail.SenderEmailAddress.Trim();
        //            List<string> to = null, cc = null;

        //            // outlook may return contact name in CC & To properties, to get email address we use the following:
        //            // from https://stackoverflow.com/questions/25758491/getting-an-email-address-instead-of-a-name

        //            foreach (Recipient recip in _COMEmail.Recipients)
        //            {
        //                switch (recip.Type)
        //                {
        //                    case (int)OlMailRecipientType.olTo:
        //                        {
        //                            if (to == null)
        //                                to = new List<string>();
        //                            to.Add(recip.Address);
        //                            break;
        //                        }
        //                    case (int)OlMailRecipientType.olCC:
        //                        {
        //                            if (cc == null)
        //                                cc = new List<string>();
        //                            cc.Add(recip.Address);
        //                            break;
        //                        }
        //                };
        //            }

        //            _Header = new HeaderSegmentEx(from, to, cc, date, subject, this);
        //        }

        //        return _Header;

        //    }
        //}

        public string OutlookConversationID => _COMEmail.ConversationID;

        public string OutlookConversationIndex => _COMEmail.ConversationIndex;

        public int AttachmentsCount => _COMEmail.Attachments.Count;

        public OutlookFolder Folder { get; private set; }

        //public IEmailPart Parent => null;


        //private List<ReplaySegmentEx> _Replays;
        //public List<ReplaySegmentEx> Replays
        //{
        //    get
        //    {
        //        if (_Replays == null)
        //        {
        //            if (ClassicalEmailSegmenter == null)
        //                ClassicalEmailSegmenter = new Segmenter(_COMEmail.HTMLBody);

        //            _Replays = ClassicalEmailSegmenter.Replays.Select(r => new ReplaySegmentEx(r, this)).ToList();
        //        }

        //        return _Replays;
        //    }
        //}


        //private SignatureSegmentEx _Signature;
        //public SignatureSegmentEx Signature
        //{
        //    get
        //    {
        //        if (_Signature == null)
        //        {
        //            if (ClassicalEmailSegmenter == null)
        //                ClassicalEmailSegmenter = new Segmenter(_COMEmail.HTMLBody);

        //            _Signature = new SignatureSegmentEx(ClassicalEmailSegmenter.Signature, this);
        //        }
        //        return _Signature;
        //    }
        //}


        //private string _HTML;
        public string HTML => _COMEmail.HTMLBody;
        //{
        //    get
        //    {
        //        if (ClassicalEmailSegmenter == null)
        //            ClassicalEmailSegmenter = new Segmenter(_COMEmail.HTMLBody);

        //        if (_Body == null)
        //            _Body = new BodySegmentEx(ClassicalEmailSegmenter.Body, this);

        //        return _HTML;
        //    }
        //}


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

        #region Methods

        public OutlookEmail CopyTo(OutlookFolder folder)
        {
            MailItem mailCopy = (MailItem)_COMEmail.Copy();
            mailCopy.Move(folder._COMFolder);

            return new OutlookEmail(folder, mailCopy);
        }

        public void MoveTo(OutlookFolder folder)
        {
            _COMEmail.Move(folder._COMFolder);
            this.Folder = folder;

        }

        #endregion

    }
}
