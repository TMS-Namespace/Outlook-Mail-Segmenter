using System.Collections.Generic;

using static TMS.Libraries.ClassicalEmailSegmenter.Shared;

namespace TMS.Libraries.ClassicalEmailSegmenter
{
    public class BodySegment : BaseSegment
    {

        #region Init

        internal BodySegment(BaseSegment parent) : base(parent)
        {
            this.SegmentHmlDocument = parent.SegmentHmlDocument;
        }


        #endregion

        #region Properties

        private string _HTML;
        public string HTML
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_HTML))
                    _HTML = CleanHTML(OriginalHTML);


                return _HTML;
            }
        }

        private string _Text;
        public string Text
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_Text))
                    _Text = StripTextFromHTML(HTML);


                return _Text;
            }
        }

        // hide body from base
        private new BodySegment Body { get; set; }

        List<string> _EmailAddresses;
        public List<string> EmailAddresses
        {
            get
            {
                if (_EmailAddresses == null)
                    _EmailAddresses = ParseEmailAddresses(this.HTML);

                return _EmailAddresses;
            }
        }

        List<string> _Phones;
        public List<string> InternationalPhones
        {
            get
            {
                if (_Phones == null)
                    _Phones = ParsePhones(this.HTML);

                return _Phones;
            }
        }


        #endregion

    }
}
