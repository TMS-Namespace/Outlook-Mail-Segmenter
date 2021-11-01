using System;

using TMS.Libraries.ClassicalEmailSegmenter;
using TMS.Libraries.OutlookMailWrapper.Helpers;

namespace TMS.Libraries.OutlookMailWrapper
{
    public class BaseSegmentEx : IEmailPart
    {
        #region Init

        private BaseSegment Origin;

        internal BaseSegmentEx(BaseSegment origin, IEmailPart parent)
        {

            Origin = origin;
            ID = Guid.NewGuid();
            this.Parent = parent;
        }

        #endregion

        #region Properties

        public Guid ID { get; private set; }

        private BodySegmentEx _Body;
        public BodySegmentEx Body
        {
            get
            {

                if (_Body == null && !(Origin == null || Origin.Body == null))
                    _Body = new BodySegmentEx(Origin.Body, this);

                return _Body;

            }
        }

        public IEmailPart Parent { get; private set; }


        /// <summary>
        /// Segment's HTML before any manipulations.
        /// </summary>
        public string OriginalHTML => Origin?.OriginalHTML;

        #endregion

    }
}
