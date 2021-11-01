using TMS.Libraries.ClassicalEmailSegmenter;
using TMS.Libraries.OutlookMailWrapper.Helpers;

namespace TMS.Libraries.OutlookMailWrapper
{
    public class ReplaySegmentEx : BaseSegmentEx, IMessage
    {

        #region Init

        private ReplaySegment Origin;

        internal ReplaySegmentEx(ReplaySegment origin, IEmailPart parent) : base(origin, parent)
        {
            Origin = origin;

        }

        #endregion

        #region Properties

        private SignatureSegmentEx _Signature;
        public SignatureSegmentEx Signature
        {
            get
            {

                if (_Signature == null)
                    _Signature = new SignatureSegmentEx(Origin.Signature, this);

                return _Signature;
            }
        }

        private HeaderSegmentEx _Header;
        public HeaderSegmentEx Header
        {
            get
            {
                if (_Header == null)
                    _Header = new HeaderSegmentEx(Origin.Header, this);

                return _Header;
            }
        }


        #endregion

    }
}
