using HtmlAgilityPack;

using System;
using System.Collections.Generic;
using System.Linq;

namespace TMS.Libraries.EmailSegmenter
{
    public class SignatureSegment : BaseSegment
    {

        #region init

        internal SignatureSegment(HtmlDocument doc, BaseSegment parent) : base(doc, parent) { }


        #endregion

        #region Properties

        private BodySegment _Body;
        public override BodySegment Body
        {
            get
            {
                if (_Body == null)
                    SegmentSignature(Doc);

                return _Body;
            }

        }

        #endregion

        #region Help Methods

        static string gratings = string.Join("|", new List<string>() { "best regards", "sincerely yours", "с уважением" });

        /// <summary>
        /// Tries to get the signature by using last div tag.
        /// <para>It assumes that the header in the passed doc, is already striped out.</para>
        /// </summary>
        /// <param name="doc">Html Agility document, that represents the replay body with striped header.</param>
        /// <returns></returns>
        private List<HtmlNode> FindSignature(HtmlDocument doc)
        {
            // Usually, last div represents a signature
            // NOTE: this requires that we already stripped out the conversation header, otherwise it may delete the header it self


            var divs = doc.DocumentNode.SelectNodes("//div");
            if (divs != null)
                return new List<HtmlNode>() { divs.ToList().Last() };
            else
            {
                // if none is found, we try segment by "best regards"
                var grating = Shared.GetXPath2Nodes(doc, "//span[matches(.,'" + gratings.Replace(" ", @"\s*") + "','i')]");
                if (grating.Count > 0)
                {
                    // all below nodes are the signature
                    var next = grating[0].NextSibling;

                    List<HtmlNode> res = new List<HtmlNode>();
                    while (next != null)
                    {
                        res.Add(next);
                        next = next.NextSibling;
                    }

                    return res;
                }
            }

            return null;

        }


        private void SegmentSignature(HtmlDocument doc)
        {
            var signs = FindSignature(doc);

            // it could be that we detected the signature wrong, for example, when a person writes the message inside the signature itself!, so save temp doc
            // var tmp = doc.DocumentNode.OuterHtml;
            signs?.ForEach(s => s.Remove()); // strip out the signature

            // if removing the signature removed whole document, revert
            if (string.IsNullOrWhiteSpace(doc.DocumentNode.OuterHtml))
                doc.LoadHtml(OriginalHTML);
            else if (signs != null)
                _Body = new BodySegment(string.Join(Environment.NewLine, signs.Select(s => s.OuterHtml)), this);

        }

        #endregion

    }
}
