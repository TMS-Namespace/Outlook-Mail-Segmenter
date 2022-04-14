using System;
using System.Xml.Serialization;
using TMS.Libraries.EmailSegmentation.Segmentor;
using TMS.Libraries.EmailSegmentation.Segmentor.SegmentedEmailParts;

namespace TMS.Libraries.EmailXMLDataPresentation
{
    [Serializable]
    [XmlRoot("Replays")]
    [XmlType(TypeName = "Replay")]
    public class Replay : Base
    {
        #region Init

        public Replay() { AllEmailParts.Add(this); }

        public Replay(EmailChunk replay) : base(replay)
        {
            var rep = replay as SegmentedEmailReplayPart;
            Header = (rep.Header == null) ? null : new Header(rep.Header);
            Signature = (rep.Signature == null) ? null : new Signature(rep.Signature);

            AllEmailParts.Add(this);
        }

        #endregion

        #region Properties

        public Signature Signature { get; set; }

        public Header Header { get; set; }

        #endregion

    }
}
