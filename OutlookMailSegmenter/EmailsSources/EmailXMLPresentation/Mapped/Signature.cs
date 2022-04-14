using System;
using TMS.Libraries.EmailSegmentation.Segmentor.Segments;

namespace TMS.Libraries.EmailsSources.XMLPresentation
{
    [Serializable]
    public class Signature : Base
    {
        #region Init

        public Signature() { EmailsXML.Add(this); }

        public Signature(SignatureSegment origin) : base(origin)
        {
            EmailsXML.Add(this);
        }

        #endregion

    }
}
