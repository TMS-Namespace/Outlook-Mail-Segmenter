using HtmlAgilityPack;

using System;

namespace TMS.Libraries.ClassicalEmailSegmenter
{
    public abstract class BaseSegment : IBaseSegment
    {

        #region Init

        internal BaseSegment(BaseSegment parent)
        {
            if (parent == null & this is not Segmenter)
                throw new Exception("Parent can't be null");

            OriginalHTML = parent?.SegmentHmlDocument.DocumentNode.OuterHtml;
            Parent = parent;
        }


        #endregion

        #region Properties

        /// <summary>
        /// The Body object of the Header.
        /// <para><strong>Note:</strong></para>
        /// <para>In some cases the body is null, for example when it represents the header of the main part of email.</para>
        /// </summary>
        public virtual BodySegment Body { get; internal set; }

        public BaseSegment Parent { get; private set; }


        /// <summary>
        /// Segment's HTML before any manipulations, obtained once the object is created.
        /// </summary>
        public string OriginalHTML { get; internal set; }

        private HtmlDocument _SegmentHmlDocument;
        internal HtmlDocument SegmentHmlDocument
        {
            get
            {
                if (_SegmentHmlDocument == null)
                {
                    _SegmentHmlDocument = new HtmlDocument();
                    _SegmentHmlDocument.LoadHtml(OriginalHTML);
                }

                return _SegmentHmlDocument;

            }
            set => _SegmentHmlDocument = value;
        }

        #endregion

    }
}
