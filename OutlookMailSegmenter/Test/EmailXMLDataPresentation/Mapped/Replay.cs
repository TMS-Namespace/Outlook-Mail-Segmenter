using System;
using System.Xml.Serialization;

using TMS.Libraries.OutlookMailWrapper.Helpers;

namespace TMS.Libraries.EmailXMLDataPresentation
{
    [Serializable]
    [XmlRoot("Replays")]
    [XmlType(TypeName = "Replay")]
    public class Replay : Base
    {
        #region Init

        public Replay() { AllEmailParts.Add(this); }

        public Replay(IMessage replay) : base(replay)
        {

            Header = new Header(replay.Header);
            Signature = (replay.Signature == null) ? null : new Signature(replay.Signature);

            AllEmailParts.Add(this);
        }

        #endregion

        #region Properties

        public Signature Signature { get; set; }

        public Header Header { get; set; }

        #endregion

    }
}
