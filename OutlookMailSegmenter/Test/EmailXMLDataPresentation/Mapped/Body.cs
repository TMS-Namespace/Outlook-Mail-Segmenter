using System;
using System.Collections.Generic;
using System.Xml.Serialization;

using TMS.Libraries.OutlookMailWrapper;

namespace TMS.Libraries.EmailXMLDataPresentation
{

    [Serializable]
    public class Body : Base
    {

        #region Init

        public Body() { AllEmailParts.Add(this); }

        public Body(BodySegmentEx body) : base(body)
        {

            HTML = body.HTML;
            Text = body.Text;
            EmailAddresses = body.EmailAddresses;
            InternationalPhones = body.InternationalPhones;

            BaseBodySegmentID = body.BaseBodySegment?.ID;

            AllEmailParts.Add(this);
        }


        #endregion

        #region Properties

        public string Text { get; set; }

        public string HTML { get; set; }

        public Guid? BaseBodySegmentID { get; set; }

        public List<string> EmailAddresses { get; set; }

        public List<string> InternationalPhones { get; set; }


        [XmlIgnore]
        public Body BaseBodySegment => (Body)AllEmailParts.FindByID(this.BaseBodySegmentID);

        #endregion

    }
}
