namespace TMS.Libraries.ClassicalEmailSegmenter
{
    public class ReplaySegment : BaseSegment, IReplaySegment
    {

        #region Init

        internal ReplaySegment(string html, BaseSegment parent) : base(parent)
        {

            // as an exception, the original html here is not parent's one
            this.OriginalHTML = html;

        }

        #endregion

        #region Properties

        private SignatureSegment _Signature;
        public SignatureSegment Signature
        {
            get
            {
                if (_Signature == null)
                    Segment();

                return _Signature;

            }
        }

        private HeaderSegment _Header;
        public HeaderSegment Header
        {
            get
            {
                if (_Header == null)
                    Segment();

                return _Header;
            }
        }


        private BodySegment _Body;
        public override BodySegment Body
        {
            get
            {

                if (_Body == null)
                    Segment();

                return _Body;


            }
        }

        #endregion

        #region Help Methods

        private void Segment()
        {
            // we force segmenting by calling body of header and signature
            _Header = new HeaderSegment(this);
            var tmp1 = _Header.Body;

            _Signature = new SignatureSegment(this);
            var tmp2 = _Signature.Body;

            _Body = new BodySegment(this);
        }

        #endregion

    }
}
