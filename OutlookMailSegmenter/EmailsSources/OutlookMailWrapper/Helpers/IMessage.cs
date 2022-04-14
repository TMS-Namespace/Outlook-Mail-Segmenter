using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.Libraries.OutlookMailWrapper.Helpers
{

    /// <summary>
    /// An interface, that unifies main email, and any of it's replays, so, it represents a full distinct messages.
    /// </summary>
    public interface IMessage : IEmailPart
    {
        public SignatureSegmentEx Signature { get; }
        public HeaderSegmentEx Header { get; }

    }
}
