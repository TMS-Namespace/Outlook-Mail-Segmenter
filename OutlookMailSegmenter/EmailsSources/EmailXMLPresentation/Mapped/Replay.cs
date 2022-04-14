using System;
using System.Xml.Serialization;
using TMS.Libraries.EmailSegmentation.Segmentor;
using TMS.Libraries.EmailSegmentation.Segmentor.SegmentedEmailParts;

namespace TMS.Libraries.EmailsSources.XMLPresentation
{
    [Serializable]
    [XmlRoot("Replays")]
    [XmlType(TypeName = "Replay")]
    public class Replay : Base
    {
        #region Init

        public Replay() { EmailsXML.Add(this); }

        public Replay(EmailChunk replay) : base(replay)
        {
            //var rep = replay as SegmentedEmailReplayPart;
            //Header = (rep.Header == null) ? null : new Header(rep.Header);
            //Signature = (rep.Signature == null) ? null : new Signature(rep.Signature);

            EmailsXML.Add(this);
        }

        #endregion

        #region Properties

        private Signature _Signature;
        public Signature Signature
        {
            get
            {
                if (Origin != null && _Signature == null)
                {
                    var rep = Origin as SegmentedEmailReplayPart;
                    if (rep.Signature != null)
                        _Signature = new Signature(rep.Signature);
                }
                return _Signature;
            }
            set { _Signature = value; }
        }


        private Header _Header;
        public Header Header
        {
            get
            {
                if (Origin != null && _Header == null)
                {
                    var rep = Origin as SegmentedEmailReplayPart;
                    if (rep.Header != null)
                        _Header = new Header(rep.Header);
                }
                return _Header;
            }
            set { _Header = value; }
        }

        #endregion

    }
}
