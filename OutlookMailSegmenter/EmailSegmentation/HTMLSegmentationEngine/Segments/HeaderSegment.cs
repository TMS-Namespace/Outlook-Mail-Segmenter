using HtmlAgilityPack;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace TMS.Libraries.ClassicalEmailSegmenter
{
    public class HeaderSegment : BaseSegment, IHeaderSegment
    {

        #region Init

        internal HeaderSegment(BaseSegment parent) : base(parent) { }

        #endregion

        #region Properties

        private string _Subject;
        public string Subject
        {
            get
            {
                if (!AlreadyCreated)
                    Segment();

                return _Subject;
            }
        }

        private string _From;
        public string From
        {
            get
            {
                if (!AlreadyCreated)
                    Segment();

                return _From;
            }
        }

        private BodySegment _Body;
        public override BodySegment Body
        {
            get
            {
                if (_Body == null)
                    Segment();

                return _Body;
            }

        }

        private List<string> _To;

        /// <summary>
        /// Email addresses of "To" receivers.
        /// <para><strong>Note:</strong></para>
        /// <para>Emails are not always explicitly mentioned in the replay header, they can be replaced by receiver's contact name, so some receivers could be missed.</para>
        /// </summary>
        public List<string> To
        {
            get
            {
                if (!AlreadyCreated)
                    Segment();

                return _To;
            }
        }


        private List<string> _CC;
        public List<string> CC
        {
            get
            {
                if (!AlreadyCreated)
                    Segment();

                return _CC;
            }
        }


        private DateTime? _Date;
        public DateTime? Date
        {
            get
            {
                if (!AlreadyCreated)
                    Segment();

                return _Date;
            }
        }

        #endregion

        #region Help Methods

        private HtmlNode FindReplayHeader(HtmlDocument doc)
        {
            return doc.DocumentNode.SelectSingleNode("//div/div")?.ParentNode;
        }

        private bool AlreadyCreated;
        private void Segment()
        {
            var header = FindReplayHeader(Parent.SegmentHmlDocument);

            if (header != null)
            {

                var headInfo = ParseReplayHeader(Shared.StripTextFromHTML(header.InnerHtml));

                // if from not found, and at least one another header entry is not found, we consider as if header is not detected 
                if (!string.IsNullOrWhiteSpace(headInfo.From) & (!string.IsNullOrWhiteSpace(headInfo.Subject) | headInfo.To != null | headInfo.CC != null | headInfo.Date != null))
                {

                    _CC = headInfo.CC;
                    _From = headInfo.From;
                    _To = headInfo.To;
                    _Date = headInfo.Date;
                    _Subject = headInfo.Subject;

                    // before removing header, we will remove everything above it (like lines)
                    header.PreviousSibling?.PreviousSibling?.PreviousSibling?.Remove();
                    header.PreviousSibling?.PreviousSibling?.Remove();
                    header.PreviousSibling?.Remove();
                    header.Remove(); // strip out the header

                    // we need cleaned text
                    this.SegmentHmlDocument = new HtmlDocument();
                    this.SegmentHmlDocument.LoadHtml(header.InnerHtml);
                    _Body = new BodySegment(this);
                }

            }

            AlreadyCreated = true;
        }

        private struct HeaderInfo
        {
            public string Subject;
            public DateTime? Date;

            public List<string> To;
            public List<string> CC;

            public string From;

        }

        // header shared regex object
        private static Regex headerRegex;


        private HeaderInfo ParseReplayHeader(string headerText)
        {
            // among many other possible ways to implement this, the below approach showed to be the most relabel

            if (headerRegex == null)
            {
                string patt = @"\b" +
                              string.Format("({0}|{1}|{2}|{3}|{4}|{5})",
                              Shared.fromKeyWords,
                              Shared.toKeyWords,
                              Shared.ccKeyWords,
                              Shared.subjectKeyWords,
                              Shared.sentKeyWords,
                              Shared.importantKeyWords)
                              + @"\b\s*\:";

                headerRegex = new Regex(patt,
                    RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
            }

            var res = headerRegex.Split(headerText);

            var h = new HeaderInfo();

            for (int i = 0; i < res.Count() - 1; i++)
            {
                if (Shared.fromKeyWords.Contains(res[i], StringComparison.InvariantCultureIgnoreCase))
                    h.From = Shared.ParseEmailAddresses(res[i + 1])?[0];


                if (Shared.toKeyWords.Contains(res[i], StringComparison.InvariantCultureIgnoreCase))
                    h.To = Shared.ParseEmailAddresses(res[i + 1]);


                if (Shared.ccKeyWords.Contains(res[i], StringComparison.InvariantCultureIgnoreCase))
                    h.CC = Shared.ParseEmailAddresses(res[i + 1]);

                if (Shared.sentKeyWords.Contains(res[i], StringComparison.InvariantCultureIgnoreCase))
                {
                    DateTime dt;

                    if (
                        DateTime.TryParse(
                        Shared.FixBadCharacters(Shared.StripTextFromHTML(res[i + 1]))
                        , out dt))

                        h.Date = dt;
                }



                if (Shared.subjectKeyWords.Contains(res[i], StringComparison.InvariantCultureIgnoreCase))
                    h.Subject = Shared.FixBadCharacters(Shared.StripTextFromHTML(res[i + 1]));
            }

            return h;

        }

        #endregion

    }
}
