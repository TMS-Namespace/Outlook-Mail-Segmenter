using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.Libraries.OutlookMailWrapper.Helpers
{

    /// <summary>
    /// An interface, that unifies main email part, and any of it segments/parts.
    /// </summary>
    public interface IEmailPart
    {
        public Guid ID { get; }

        public BodySegmentEx Body { get; }

        IEmailPart Parent { get; }


    }
}
