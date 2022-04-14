using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.Libraries.EmailSegmentation.Segmenter.Segments
{

    /// <summary>
    /// An interface, that unifies main email part, and any of it segments/parts.
    /// </summary>
    public interface IEmailSegment
    {
        public Guid ID { get; }
         
        public BodySegment Body { get; }

        IEmailSegment Parent { get; }


    }
}
