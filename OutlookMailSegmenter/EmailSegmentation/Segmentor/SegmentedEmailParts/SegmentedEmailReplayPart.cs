using TMS.Libraries.EmailSegmentation.SegmentationEngineCore;
using TMS.Libraries.EmailSegmentation.Segmentor.Segments;

namespace TMS.Libraries.EmailSegmentation.Segmentor.SegmentedEmailParts
{
    public class SegmentedEmailReplayPart : EmailChunk
    {

        #region Init

        //private ReplaySegment Origin;

        internal SegmentedEmailReplayPart() : base(null) { }

        internal SegmentedEmailReplayPart(EmailChunk parent,
                                            ISegmentedSingleHTMLEmail origin) : base(parent)
        {
            this.Origin = origin;

            //if (!string.IsNullOrWhiteSpace(bodyHTML))
            //    this.Body = new BodySegment(this, bodyHTML);

            //if (!string.IsNullOrWhiteSpace(headerHTML))
            //    this.Header = new HeaderSegment(this, headerHTML);



        }

        #endregion

        #region Properties


        // we need this to be able to override it in mainPart class that inherits this one
        private ISegmentedSingleHTMLEmail _Origin;

        internal ISegmentedSingleHTMLEmail Origin
        {
            get
            {
                if (_Origin == null)
                    this.Segment();

                return _Origin;
            }
            set
            { _Origin = value; }
        }

        private SignatureSegment _Signature;
        public SignatureSegment Signature
        {
            get
            {
                if (!Origin.IsSegmented)
                    Origin.Segment();

                if (_Signature == null && !string.IsNullOrWhiteSpace(Origin.SignatureHTML))
                    _Signature = new SignatureSegment(this, Origin.SignatureHTML);

                return _Signature;
            }
        }
        //{
        //    get
        //    {

        //        if (_Signature == null)
        //            _Signature = new SignatureSegmentEx(Origin.Signature, this);

        //        return _Signature;
        //    }
        //}

        private HeaderSegment _Header;
        public virtual HeaderSegment Header
        {
            get
            {
                if (!Origin.IsSegmented)
                    Origin.Segment();

                if (_Header == null && !string.IsNullOrWhiteSpace(Origin.HeaderHTML))
                    _Header = new HeaderSegment(this, Origin.HeaderHTML);

                return _Header;
            }
        }


        private BodySegment _Body;
        public override BodySegment Body
        {
            get
            {
                if (!Origin.IsSegmented)
                    Origin.Segment();

                if (_Body == null && !string.IsNullOrWhiteSpace(Origin.BodyHTML))
                    _Body = new BodySegment(this, Origin.BodyHTML);

                return _Body;
            }
        }

        #endregion


        // we need this to be able to override it in mainPart class that inherits this one
        internal virtual void Segment()
        {

            this.Origin?.Segment();
        }

    }
}
