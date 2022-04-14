namespace TMS.Libraries.EmailSegmentation.Segmentor.Segments
{
    public class SignatureSegment : EmailChunk
    {
        #region Init

        private SignatureSegment Origin;

        internal SignatureSegment(EmailChunk parent, string originalHTML) : base(parent)
        {
            if (!string.IsNullOrWhiteSpace(originalHTML))
            {
                this.Body = new BodySegment(this, originalHTML);
                this.OriginalHTML = originalHTML;
            }

        }

        #endregion

    }
}
