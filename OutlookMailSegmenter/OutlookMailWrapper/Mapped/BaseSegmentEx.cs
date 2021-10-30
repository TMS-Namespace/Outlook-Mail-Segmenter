using System;

using TMS.Libraries.EmailSegmenter;

namespace TMS.Libraries.OutlookMailWrapper
{
    public class BaseSegmentEx
    {
        #region Init

        private BaseSegment Origin;

        internal BaseSegmentEx(BaseSegment origin)
        {

            Origin = origin;
            ID = Guid.NewGuid();
        }

        #endregion

        #region Properties

        public Guid ID { get; private set; }

        public BodySegmentEx Body => (Origin == null || Origin.Body == null) ? null : new BodySegmentEx(Origin.Body);

        public BaseSegmentEx Parent => (Origin == null || Origin.Body == null) ? null : new BaseSegmentEx(Origin.Parent);

        /// <summary>
        /// Segment's HTML before any manipulations.
        /// </summary>
        public string OriginalHTML => Origin?.OriginalHTML;

        #endregion

    }
}
