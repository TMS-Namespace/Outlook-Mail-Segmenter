using HtmlAgilityPack;

using System.Collections.Generic;
using System.Linq;

namespace TMS.Libraries.ClassicalEmailSegmenter
{
    public class SignatureSegment : BaseSegment
    {

        #region init

        internal SignatureSegment(BaseSegment parent) : base(parent) { }


        #endregion

        #region Properties

        private BodySegment _Body;
        public override BodySegment Body
        {
            get
            {
                if (_Body == null)
                    SegmentSignature();

                return _Body;
            }

        }

        #endregion

        #region Help Methods

        static string gratings = string.Join("|", new List<string>() { "best regards", "sincerely yours", "с уважением", "Regards" });

        /// <summary>
        /// Tries to get the signature by using last div tag.
        /// <para>It assumes that the header in the passed doc, is already striped out.</para>
        /// </summary>
        /// <param name="doc">Html Agility document, that represents the replay body with striped header.</param>
        /// <returns></returns>
        private List<HtmlNode> FindSignature(HtmlDocument doc)
        {
            // another option
            var divs = doc.DocumentNode.SelectNodes("//div[contains(@id,'signature')]");
            if (divs != null)
            {
                var lastDiv = divs.ToList().Last();
                return new List<HtmlNode>() { divs.ToList().Last() };
            }

            // Usually, last div represents a signature
            // NOTE: this requires that we already stripped out the conversation header, otherwise it may delete the header it self
            divs = doc.DocumentNode.SelectNodes("//div");
            if (divs != null)
            {
                var lastDiv = divs.ToList().Last();
                // make sure that this is not a wrapping div, otherwise we may remove the whole message, so we simulate removing by cloning
                if (lastDiv != doc.DocumentNode.FirstChild)
                {
                    var docClone = new HtmlDocument();
                    docClone.LoadHtml(doc.DocumentNode.OuterHtml);
                    docClone.DocumentNode.SelectNodes("//div").ToList().Last().Remove();
                    if (!string.IsNullOrWhiteSpace(docClone.DocumentNode.InnerText))
                        return new List<HtmlNode>() { divs.ToList().Last() };
                }
            }


            // if none is found, we try segment by "best regards"
            var greting = FindGreting(doc);
            if (greting != null)
            {

                List<HtmlNode> res = new List<HtmlNode>();

                // add gratings to signature
                res.Add(greting);

                // all below nodes are the signature
                var next = greting.NextSibling;

                while (next != null)
                {
                    res.Add(next);
                    next = next.NextSibling;
                }

                return res;
            }

            return null;

        }

        private HtmlNode FindGreting(HtmlDocument doc)
        {
            var gretings = Shared.GetXPath2Nodes(doc, "//span[matches(.,'" + gratings.Replace(" ", @"\s*") + "','i')]");

            if (gretings.Count > 0)
            {   // go up till we get a node with text
                var parent = gretings.Last().ParentNode;
                while (Parent != null)
                {
                    if (parent.ParentNode == null || parent.ParentNode.Name == "div" || parent.ParentNode.Name == "table")
                        return parent;

                    parent = parent.ParentNode;
                }
            }

            return null;
        }

        private void SegmentSignature()
        {
            var signs = FindSignature(this.Parent.SegmentHmlDocument);

            // it could be that we detected the signature wrong, for example, when a person writes the message inside the signature itself!, so save temp doc
            if (signs?.Count == 1)
            {
                // strip out signature from parent's Doc
                signs[0].Remove();

                // set signature doc
                if (this.SegmentHmlDocument == null)
                    this.SegmentHmlDocument = new HtmlDocument();

                this.SegmentHmlDocument.LoadHtml(signs[0].OuterHtml);

            }
            else if (signs?.Count > 1)
            {
                // remove from parent doc
                signs?.ForEach(s => s.Remove());

                // join sign tags to create signature doc
                var signHtml = string.Join(string.Empty, signs.Select(s => s.OuterHtml));

                // set signature doc
                if (this.SegmentHmlDocument == null)
                    this.SegmentHmlDocument = new HtmlDocument();

                this.SegmentHmlDocument.LoadHtml(signHtml);
            }

            _Body = new BodySegment(this);

        }

        #endregion

    }
}
