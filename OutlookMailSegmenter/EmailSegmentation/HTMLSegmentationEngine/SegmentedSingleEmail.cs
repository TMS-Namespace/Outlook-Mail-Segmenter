using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;
using TMS.Libraries.EmailSegmentation.SegmentationEngineCore;
using TMS.Libraries.EmailSegmentation.SegmentationEngineCore.Helpers;

namespace TMS.Libraries.EmailSegmentation.HTMLSegmentationEngine
{
    public sealed class SegmentedSingleEmail : ISegmentedSingleHTMLEmail
    {
        internal SegmentedSingleEmail(string html) { this.UnsegmentedHTML = html; }

        public string BodyHTML { get; private set; }

        public string HeaderHTML { get; private set; }

        public string SignatureHTML { get; private set; }

        public string UnsegmentedHTML { get; private set; }

        public bool IsSegmented { get; private set; }

        //private string UnsegmentedHTML;

        private HtmlDocument RunningHTMLDocument;

        public void Segment()
        {
            if (!IsSegmented)
            {
                RunningHTMLDocument = new HtmlDocument();
                RunningHTMLDocument.LoadHtml(UnsegmentedHTML);

                SegmentHeader();
                SegmentSignature();

                this.BodyHTML = RunningHTMLDocument.DocumentNode.OuterHtml;

                this.IsSegmented = true;
            }

        }



        #region Header business

        private HtmlNode FindReplayHeader(HtmlDocument doc)
        {
            return doc.DocumentNode.SelectSingleNode("//div/div")?.ParentNode;
        }

        //private bool AlreadyCreated;
        private void SegmentHeader()
        {
            var header = FindReplayHeader(RunningHTMLDocument);

            if (header != null)
            {

                var headInfo = InfoParsers.ParseReplayHeader(Cleaners.StripTextFromHTML(header.InnerHtml));

                // if from not found, and at least one another header entry is not found, we consider as if header is not detected 
                if (!string.IsNullOrWhiteSpace(headInfo.From) & (!string.IsNullOrWhiteSpace(headInfo.Subject) | headInfo.To != null | headInfo.CC != null | headInfo.Date != null))
                {

                    //_CC = headInfo.CC;
                    //_From = headInfo.From;
                    //_To = headInfo.To;
                    //_Date = headInfo.Date;
                    //_Subject = headInfo.Subject;

                    // before removing header, we will remove everything above it (like lines)
                    header.PreviousSibling?.PreviousSibling?.PreviousSibling?.Remove();
                    header.PreviousSibling?.PreviousSibling?.Remove();
                    header.PreviousSibling?.Remove();
                    header.Remove(); // strip out the header

                    // we need cleaned text
                    //this.SegmentHmlDocument = new HtmlDocument();
                    //this.SegmentHmlDocument.LoadHtml(header.InnerHtml);
                    //_Body = new BodySegment(this);
                    this.HeaderHTML = header.InnerHtml;
                }

            }

            // AlreadyCreated = true;
        }

        #endregion


        #region Signature business

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
                // also, we may find an empty div, then do noting
                if (lastDiv != doc.DocumentNode.FirstChild && !string.IsNullOrWhiteSpace(lastDiv.InnerText))
                {
                    var docClone = new HtmlDocument();
                    docClone.LoadHtml(doc.DocumentNode.OuterHtml);
                    docClone.DocumentNode.SelectNodes("//div").ToList().Last().Remove();
                    if (!string.IsNullOrWhiteSpace(docClone.DocumentNode.InnerText))
                        return new List<HtmlNode>() { divs.ToList().Last() };
                }
            }


            // if none is found, we try segment by email closing
            var greting = FindClosing(doc);
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

        private HtmlNode FindClosing(HtmlDocument doc)
        {
            // check for closing words, flowed by couple or no charters (like comma for say)
            var gretings = Tools.GetXPath2Nodes(doc, @"//span[matches(.,'\s*(" + KeyWords.closingKW.Replace(" ", @"\s*") + @")\w*','i')]");

            if (gretings.Count > 0)
            {   // go up till we get a node with text
                var parent = gretings.Last().ParentNode;
                while (parent != null)
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
            var signs = FindSignature(this.RunningHTMLDocument);

            // it could be that we detected the signature wrong, for example, when a person writes the message inside the signature itself!, so save temp doc
            if (signs?.Count == 1)
            {
                // strip out signature from parent's Doc
                signs[0].Remove();

                //// set signature doc
                //if (this.SegmentHmlDocument == null)
                //    this.SegmentHmlDocument = new HtmlDocument();

                this.SignatureHTML = signs[0].OuterHtml;

            }
            else if (signs?.Count > 1)
            {
                // remove from parent doc
                signs?.ForEach(s => s.Remove());

                // join sign tags to create signature doc
                var signHtml = string.Join(string.Empty, signs.Select(s => s.OuterHtml));

                // set signature doc
                //if (this.SegmentHmlDocument == null)
                //    this.SegmentHmlDocument = new HtmlDocument();

                this.SignatureHTML = signHtml;
            }

            //_Body = new BodySegment(this);

        }

        #endregion

    }
}
