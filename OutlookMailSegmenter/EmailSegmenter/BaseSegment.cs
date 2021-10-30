using HtmlAgilityPack;

namespace TMS.Libraries.EmailSegmenter
{
    public abstract class BaseSegment
    {

        #region Init

        internal BaseSegment(HtmlDocument doc, BaseSegment parent)
        {
            Doc = doc;
            OriginalHTML = Doc.DocumentNode.OuterHtml;
            Parent = parent;
        }

        internal BaseSegment(string html, BaseSegment parent)
        {
            OriginalHTML = html;
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
        /// Segment's HTML before any manipulations.
        /// </summary>
        public string OriginalHTML { get; private set; }

        private HtmlDocument _Doc;
        internal HtmlDocument Doc
        {
            get
            {
                if (_Doc == null)
                {
                    _Doc = new HtmlDocument();
                    _Doc.LoadHtml(OriginalHTML);
                }

                return _Doc;

            }
            set => _Doc = value;
        }

        #endregion

    }
}
