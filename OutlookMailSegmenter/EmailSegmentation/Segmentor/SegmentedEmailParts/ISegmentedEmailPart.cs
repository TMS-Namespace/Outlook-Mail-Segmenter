using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.Libraries.EmailSegmentation.Segmenter;

namespace TMS.Libraries.EmailSegmentation.Segmentor.SegmentedEmailParts
{

    /// <summary>
    /// An interface, that unifies main email, and any of it's replays, so, it represents a full distinct messages.
    /// </summary>
    public interface ISegmentedEmailPart : IEmailSegment
    {
        public SignatureSegmentEx Signature { get; }
        public HeaderSegmentEx Header { get; }

    }
}
