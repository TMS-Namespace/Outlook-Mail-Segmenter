using System;
using System.Collections.Generic;

using TMS.Libraries.EmailSegmenter;

namespace TMS.Libraries.OutlookMailWrapper
{
    public class HeaderSegmentEx : BaseSegmentEx
    {

        #region Init

        private HeaderSegment Origin;
        internal HeaderSegmentEx(HeaderSegment origin) : base(origin) { Origin = origin; }

        string _From, _Subject;
        List<string> _To, _CC;
        DateTime _Date;

        internal HeaderSegmentEx(string from, List<string> to, List<string> cc, DateTime date, string subject) : base(null)
        {
            _From = from;
            _To = to;
            _CC = cc;
            _Date = date;
            _Subject = subject;
        }

        #endregion

        #region Properties

        public string Subject => (Origin == null) ? _Subject : Origin.Subject;

        public string From => (Origin == null) ? _From : Origin.From;


        /// <summary>
        /// Email addresses of "To" receivers.
        /// <para><strong>Note:</strong></para>
        /// <para>Emails are not always explicitly mentioned in the replay header, they can be replaced by receiver's contact name, so some receivers could be missed.</para>
        /// </summary>
        public List<string> To => (Origin == null) ? _To : Origin.To;

        public List<string> CC => (Origin == null) ? _CC : Origin.CC;

        public DateTime Date => (Origin == null) ? _Date : Origin.Date;

        #endregion


    }
}
