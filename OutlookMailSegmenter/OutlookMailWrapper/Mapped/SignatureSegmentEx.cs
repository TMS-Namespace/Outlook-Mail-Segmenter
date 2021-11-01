using TMS.Libraries.ClassicalEmailSegmenter;
using TMS.Libraries.OutlookMailWrapper.Helpers;

namespace TMS.Libraries.OutlookMailWrapper
{
    public class SignatureSegmentEx : BaseSegmentEx
    {
        #region Init

        private SignatureSegment Origin;

        internal SignatureSegmentEx(SignatureSegment origin, IEmailPart parent) : base(origin, parent)
        {
            Origin = origin;
        }

        #endregion

    }
}
