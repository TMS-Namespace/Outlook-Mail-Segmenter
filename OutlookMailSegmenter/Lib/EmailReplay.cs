using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace TMS.Libraries.OutlookMailSegmenter
{

    public class EmailReplay
    {

        #region Init

        // this is needed for OutlookEmail that inherits this one
        internal EmailReplay() { }

        internal EmailReplay(string html, OutlookEmail parent)
        {

            ID = Guid.NewGuid();

            ParentEmail = parent;

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            // process header
            if (Outlook.ProcessHeaders)
                this.Header = new EmailHeader(doc);

            // process signature
            if (Outlook.ProcessSignatures)
                CreateSignature(doc);

            // process what left from body after we striped out header and signature
            this.Body = new EmailChunk(doc.DocumentNode.OuterHtml, this);

        }

        #endregion

        #region Properties

        public Guid ID { get; internal set; }

        public OutlookEmail ParentEmail { get; private set; }

        public EmailChunk Body { get; internal set; }

        public EmailChunk Signature { get; internal set; }

        public EmailHeader Header { get; internal set; }


        #endregion

        #region Help Methods



        /// <summary>
        /// Tries to get the signature by using last div tag.
        /// <para>It assumes that the header in the passed doc, is already striped out.</para>
        /// </summary>
        /// <param name="doc">Html Agility document, that represents the replay body with striped header.</param>
        /// <returns></returns>
        protected HtmlNode FindSignature(HtmlDocument doc)
        {
            // Usually, last div represents a signature
            // NOTE: this requires that we already stripped out the conversation header, otherwise it may delete the header it self

            var divs = doc.DocumentNode.SelectNodes("//div");
            if (divs != null)
                return divs.ToList().Last();

            return null;

        }


        protected void CreateSignature(HtmlDocument doc)
        {
            var sign = FindSignature(doc);

            // it could be that we detected the signature wrong, for example, when a person writes the message inside the signature itself!, so save temp doc
            var tmp = doc.DocumentNode.OuterHtml;
            sign?.Remove(); // strip out the signature

            // if removing the signature removed whole document, revert
            if (string.IsNullOrWhiteSpace(doc.DocumentNode.OuterHtml))
                doc.LoadHtml(tmp);
            else if (sign != null && Outlook.ProcessSignatures)
                this.Signature = new EmailChunk(sign.OuterHtml, this);
        }

        #endregion

    }
}
