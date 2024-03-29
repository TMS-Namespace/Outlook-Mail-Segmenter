﻿using System;
using TMS.Libraries.EmailSegmentation.Segmentor.Segments;
using TMS.Libraries.OutlookMailWrapper;

namespace TMS.Libraries.EmailXMLDataPresentation
{
    [Serializable]
    public class Signature : Base
    {
        #region Init

        public Signature() { AllEmailParts.Add(this); }

        public Signature(SignatureSegment origin) : base(origin)
        {
            AllEmailParts.Add(this);
        }

        #endregion

    }
}
