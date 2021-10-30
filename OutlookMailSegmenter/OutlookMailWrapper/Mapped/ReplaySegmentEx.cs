using TMS.Libraries.EmailSegmenter;

namespace TMS.Libraries.OutlookMailWrapper
{
    public class ReplaySegmentEx : BaseSegmentEx
    {

        #region Init

        private ReplaySegment Origin;

        internal ReplaySegmentEx(ReplaySegment origin) : base(origin) { Origin = origin; }

        #endregion

        #region Properties

        public SignatureSegmentEx Signature => new SignatureSegmentEx(Origin.Signature);

        public HeaderSegmentEx Header => new HeaderSegmentEx(Origin.Header);

        #endregion

    }
}
