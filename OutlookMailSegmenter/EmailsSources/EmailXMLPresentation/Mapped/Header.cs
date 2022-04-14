using System;
using System.Collections.Generic;
using TMS.Libraries.EmailSegmentation.Segmentor.Segments;

namespace TMS.Libraries.EmailsSources.XMLPresentation
{
    [Serializable]
    public class Header : Base
    {

        #region Init

        public Header() { EmailsXML.Add(this); }

        public Header(HeaderSegment header) : base(header)
        {

            Subject = header.Subject;
            From = header.From;
            To = header.To;
            CC = header.CC;
            Date = header.Date;

            EmailsXML.Add(this);
        }

        #endregion

        #region Properties

        public string Subject { get; set; }

        public string From { get; set; }

        public List<string> To { get; set; }

        public List<string> CC { get; set; }

        public DateTime? Date { get; set; }

        #endregion

    }
}
