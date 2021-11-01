using System;

using TMS.Libraries.OutlookMailWrapper;

namespace TMS.Libraries.EmailXMLDataPresentation
{
    [Serializable]
    public class Signature : Base
    {
        #region Init

        public Signature() { AllEmailParts.Add(this); }

        public Signature(SignatureSegmentEx origin) : base(origin)
        {
            AllEmailParts.Add(this);
        }

        #endregion

    }
}
