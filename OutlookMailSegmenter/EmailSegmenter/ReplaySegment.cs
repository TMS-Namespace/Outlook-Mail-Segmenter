namespace TMS.Libraries.EmailSegmenter
{
    public class ReplaySegment : BaseSegment
    {

        #region Init

        //// this is needed for MainEmail class that inherits this one
        //internal EmailReplay() { }

        //bool ProcessHeaders;

        internal ReplaySegment(string html, BaseSegment parent) : base(html, parent) { }
        //{
        //    OriginalHTML = html;

        //    this.Doc = new HtmlDocument();
        //    Doc.LoadHtml(html);

        //    // process header
        //    if (MySegmenter.ProcessHeader)
        //        this.Header = new EmailHeader(doc, segmenter, this);

        //    // process signature
        //    if (MySegmenter.ProcessSignatures)
        //        this.Signature = new EmailSignature(doc, segmenter, this);

        //    // process what left from body after we striped out header and signature
        //    this.Body = new EmailBody(doc.DocumentNode.OuterHtml, this);

        //}

        #endregion

        #region Properties

        private SignatureSegment _Signature;
        public SignatureSegment Signature
        {
            get
            {
                if (_Signature == null)
                    SegmentReplay();

                return _Signature;

            }
        }

        private HeaderSegment _Header;
        public HeaderSegment Header
        {
            get
            {
                if (_Header == null)
                    SegmentReplay();

                return _Header;
            }
        }


        private BodySegment _Body;
        public override BodySegment Body
        {
            get
            {

                if (_Body == null)
                    SegmentReplay();

                return _Body;


            }
        }

        #endregion

        #region Help Methods

        private void SegmentReplay()
        {
            // we force segmenting by calling body of header and signature
            _Header = new HeaderSegment(Doc, this);
            var tmp1 = _Header.Body;

            _Signature = new SignatureSegment(Doc, this);
            var tmp2 = _Signature.Body;

            _Body = new BodySegment(Doc.DocumentNode.InnerHtml, this);
        }

        #endregion

    }
}
