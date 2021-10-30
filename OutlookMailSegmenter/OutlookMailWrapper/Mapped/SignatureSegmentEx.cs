using TMS.Libraries.EmailSegmenter;

namespace TMS.Libraries.OutlookMailWrapper
{
    public class SignatureSegmentEx : BaseSegmentEx
    {
        #region Init

        private SignatureSegment Origin;

        internal SignatureSegmentEx(SignatureSegment origin) : base(origin) { Origin = origin; }

        #endregion

    }
}
