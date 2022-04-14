using System;
using System.Collections.Generic;
using TMS.Libraries.EmailSegmentation.SegmentationEngineCore.Helpers;

namespace TMS.Libraries.EmailSegmentation.Segmentor.Segments
{
    public class HeaderSegment : EmailChunk
    {

        #region Init

        //private HeaderSegment Origin;
        internal HeaderSegment(EmailChunk parent,
                                string originalHTML) : base(parent)
        {

            if (!string.IsNullOrWhiteSpace(originalHTML))
            {
                this.Body = new BodySegment(this, originalHTML);
                this.OriginalHTML = originalHTML;
            }
        }

        private bool _headerCreated;
        private void CreateHeader()
        {
            if (_headerCreated == false)
            {
                var header = InfoParsers.ParseReplayHeader(this.OriginalHTML);

                _From = header.From;
                _To = header.To;
                _CC = header.CC;
                _Date = header.Date;
                _Subject = Cleaners.FixBadCharacters(header.Subject); // sometimes the subject may contain bad chars

                this._headerCreated = true;

            }
        }

        string _From, _Subject;
        List<string> _To, _CC;
        DateTime? _Date;

        internal HeaderSegment(string from,
                                List<string> to,
                                List<string> cc,
                                DateTime? date,
                                string subject,
                                EmailChunk parent) : base(parent)
        {

            _From = from;
            _To = to;
            _CC = cc;
            _Date = date;
            _Subject = subject;

            this._headerCreated = true;

        }

        #endregion

        #region Properties

        public string Subject
        {
            get
            {

                if (!_headerCreated)
                    CreateHeader();

                return _Subject;
            }
        }

        public string From
        {
            get
            {

                if (!_headerCreated)
                    CreateHeader();

                return _From;
            }
        }



        /// <summary>
        /// Email addresses of "To" receivers.
        /// <para><strong>Note:</strong></para>
        /// <para>Emails are not always explicitly mentioned in the replay header, they can be replaced by receiver's contact name, so some receivers could be missed.</para>
        /// </summary>
        public List<string> To
        {
            get
            {

                if (!_headerCreated)
                    CreateHeader();

                return _To;
            }
        }

        public List<string> CC
        {
            get
            {

                if (!_headerCreated)
                    CreateHeader();

                return _CC;
            }
        }

        public DateTime? Date
        {
            get
            {

                if (!_headerCreated)
                    CreateHeader();

                return _Date;
            }
        }

        #endregion


    }
}
