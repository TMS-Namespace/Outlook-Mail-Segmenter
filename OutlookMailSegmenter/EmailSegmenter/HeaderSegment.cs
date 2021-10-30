using HtmlAgilityPack;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;

namespace TMS.Libraries.EmailSegmenter
{
    public class HeaderSegment : BaseSegment
    {

        #region Init

        internal HeaderSegment(HtmlDocument doc, BaseSegment parent) : base(doc, parent) { }

        #endregion

        #region Properties

        private string _Subject;
        public string Subject
        {
            get
            {
                if (!AlreadyCreated)
                    CreateHeader(Doc);

                return _Subject;
            }
        }

        private string _From;
        public string From
        {
            get
            {
                if (!AlreadyCreated)
                    CreateHeader(Doc);

                return _From;
            }
        }

        private BodySegment _Body;
        public override BodySegment Body
        {
            get
            {
                if (_Body == null)
                    CreateHeader(Doc);

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
                    CreateHeader(Doc);

                return _To;
            }
        }


        private List<string> _CC;
        public List<string> CC
        {
            get
            {
                if (!AlreadyCreated)
                    CreateHeader(Doc);

                return _CC;
            }
        }


        private DateTime _Date;
        public DateTime Date
        {
            get
            {
                if (!AlreadyCreated)
                    CreateHeader(Doc);

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
        private void CreateHeader(HtmlDocument doc)
        {
            var header = FindReplayHeader(doc);

            if (header != null)
            {
                // we need cleaned text
                _Body = new BodySegment(header.InnerHtml, true);

                var headInfo = SegmentReplayHeader(Body.Text);

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
            }

            AlreadyCreated = true;
        }

        private struct HeaderInfo
        {
            public string Subject;
            public DateTime Date;

            public List<string> To;
            public List<string> CC;

            public string From;

        }

        // header shared regex object
        private static Regex headerRegex;



        private HeaderInfo SegmentReplayHeader(string headerText)
        {
            // among many other possible ways to implement this, the below approach showed to be the most relabel

            if (headerRegex == null)
            {
                string patt = @"\b" +
                              string.Format("({0}|{1}|{2}|{3}|{4}|{5})",
                              Shared.froms,
                              Shared.tos,
                              Shared.ccs,
                              Shared.subjects,
                              Shared.sents,
                              Shared.importances)
                              + @"\b\s*\:";

                headerRegex = new Regex(patt,
                    RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
            }

            var res = headerRegex.Split(headerText);

            var h = new HeaderInfo();

            for (int i = 0; i < res.Count() - 1; i++)
            {
                if (Shared.froms.Contains(res[i], StringComparison.InvariantCultureIgnoreCase))
                    h.From = new BodySegment(res[i + 1]).EmailAddresses?[0];


                if (Shared.tos.Contains(res[i], StringComparison.InvariantCultureIgnoreCase))
                    h.To = new BodySegment(res[i + 1]).EmailAddresses;


                if (Shared.ccs.Contains(res[i], StringComparison.InvariantCultureIgnoreCase))
                    h.CC = new BodySegment(res[i + 1]).EmailAddresses;

                if (Shared.sents.Contains(res[i], StringComparison.InvariantCultureIgnoreCase))
                    DateTime.TryParse(new BodySegment(res[i + 1]).Text, out h.Date);

                if (Shared.subjects.Contains(res[i], StringComparison.InvariantCultureIgnoreCase))
                    h.Subject = new BodySegment(res[i + 1]).Text;
            }

            return h;

        }

        #endregion

    }
}
