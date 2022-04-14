using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using TMS.Libraries.EmailSegmentation.Segmentor.Segments;

namespace TMS.Libraries.EmailsSources.XMLPresentation
{

    [Serializable]
    public class BodyContents : Base
    {

        #region Init

        public BodyContents() { EmailsXML.Add(this); }

        public BodyContents(BodySegment body) : base(body)
        {

            HTML = body.HTML;
            Text = body.Text;
            EmailAddresses = body.EmailAddresses;
            InternationalPhones = body.InternationalPhones;

            BaseBodySegmentID = body.BaseBodySegment?.ID;

            EmailsXML.Add(this);
        }


        #endregion

        #region Properties

        [Obsolete("Don't use this", true)]
        public override BodyContents Body { get; set; }

        private string _Text;
        public string Text
        {
            get
            {

                if (Origin != null && _Text == null)
                    _Text = ((BodySegment)Origin).Text;

                return _Text;

            }
            set { _Text = value; }
        }

        private string _HTML;
        public string HTML
        {
            get
            {

                if (Origin != null && _HTML == null)
                    _HTML = ((BodySegment)Origin).HTML;
                return _HTML;

            }
            set { _HTML = value; }
        }

        private Guid? _BaseBodySegmentID;
        public Guid? BaseBodySegmentID
        {
            get
            {

                if (Origin != null && _BaseBodySegmentID == null)
                {
                    _BaseBodySegmentID = ((BodySegment)Origin).BaseBodySegment?.ID;

                    if (_BaseBodySegmentID == Guid.Empty)
                        _BaseBodySegmentID = null;
                }

                return _BaseBodySegmentID;

            }
            set { _BaseBodySegmentID = value; }
        }

        private List<string> _EmailAddresses;
        public List<string> EmailAddresses
        {
            get
            {
                if (Origin != null && _EmailAddresses == null)
                    _EmailAddresses = ((BodySegment)Origin).EmailAddresses;

                return _EmailAddresses;

            }
            set { _EmailAddresses = value; }
        }

        private List<string> _InternationalPhones;
        public List<string> InternationalPhones
        {
            get
            {
                if (Origin != null && _InternationalPhones == null)
                    _InternationalPhones = ((BodySegment)Origin).InternationalPhones;

                return _InternationalPhones;

            }
            set { _InternationalPhones = value; }
        }


        [XmlIgnore]
        public BodyContents BaseBodySegment => (BodyContents)EmailsXML.FindByID(this.BaseBodySegmentID);

        #endregion

    }
}
