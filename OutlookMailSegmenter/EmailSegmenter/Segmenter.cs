using HtmlAgilityPack;

using System.Collections.Generic;
using System.Linq;

namespace TMS.Libraries.EmailSegmenter
{
    public class Segmenter : BaseSegment
    {

        #region Init

        public Segmenter(string html) : base(html, null) { }

        #endregion

        #region Properties


        private SignatureSegment _Signature;
        public SignatureSegment Signature
        {
            get
            {
                if (_Signature == null)
                    SegmentEmail(OriginalHTML);

                return
                    _Signature;
            }

        }

        private List<ReplaySegment> _Replays;
        public List<ReplaySegment> Replays
        {
            get
            {
                if (_Replays == null)
                    SegmentEmail(OriginalHTML);

                return _Replays;
            }
        }

        // hide properties
        private new BaseSegment Parent { get; set; }

        private BodySegment _Body;
        public override BodySegment Body
        {
            get
            {
                if (_Body == null)
                    SegmentEmail(OriginalHTML);

                return _Body;
            }
        }

        #endregion

        #region Help Methods

        private void SegmentEmail(string html)
        {

            // --- find replays
            var replaysHTML = SplitToReplays(html);

            // --- set the main email
            var mainDoc = new HtmlDocument();
            mainDoc.LoadHtml(replaysHTML[0]);

            // first we find and strip out the signature
            _Signature = new SignatureSegment(mainDoc, this);
            // Force signature segmenting
            var tmp = Signature.Body;
            // set main body
            _Body = new BodySegment(mainDoc, this);

            if (replaysHTML.Count > 0)
                _Replays = new List<ReplaySegment>();

            replaysHTML.
                        Skip(1).
                        ToList().
                        ForEach(r =>
                                _Replays.Add(new ReplaySegment(r, this))
                                );


        }


        private List<string> SplitToReplays(string html)
        {
            List<string> replayes = new List<string>();
            List<HtmlNode> res = null;

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            // we do not need whole html file, scope to body
            HtmlNode bodyND = FindBody(doc);

            // sometimes, outlook dose not return full html document! then whole msg is a body
            string bodyHTML = (bodyND == null) ? html : bodyND.InnerHtml;

            // re-create doc from body
            doc.LoadHtml(bodyHTML);

            // find separators
            res = FindReplaySeparators(doc);

            // if no replays are detected, the whole message is main message
            if (res == null || res.Count == 0)
                replayes.Add(bodyHTML);
            else
                // add the main message to conversations list
                replayes.Add(bodyHTML.Substring(0, res[0].OuterStartIndex));

            // generate replay list
            //if (ProcessReplays)
            //{
            for (int i = 0; i < res?.Count; i++)
            {
                // if this is the last replay
                if (i == res.Count - 1)
                    replayes.Add(bodyHTML.Substring(res[i].OuterStartIndex));
                else
                    replayes.Add(bodyHTML.Substring(
                        res[i].OuterStartIndex,
                        res[i + 1].OuterStartIndex - res[i].OuterStartIndex));
                //}
            }

            return replayes;
        }

        // From, to etc... keywords in different languages
        //static List<string> froms = new List<string>() { "from", "от", "من" };
        static string froms = string.Join("|", new List<string>() { "from", "от", "من" });

        /// <summary>
        /// Finds all replay dividers.
        /// <para><strong>Note:</strong></para>
        /// <para>It will search for all separators, even if we set ProcessAllReplaes=False, because we do not know which criteria corresponds to first separator.</para>
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        private List<HtmlNode> FindReplaySeparators(HtmlDocument doc)
        {

            var separators = new List<HtmlNode>();

            // create XPath v2.0 regex match condition for "from"
            var conditionsFrom = @"matches(.,'(" + froms + @")\s*\:', 'i')";

            //var res = doc.DocumentNode.SelectNodes($"//div/div[contains(@style,'border-top:solid')]/p/b/span[{conditionsFrom}]")?.ToList();
            var res = Shared.GetXPath2Nodes(doc, @"//div/div[contains(@style,'border-top:solid')]/p/b/span[" + conditionsFrom + "]");

            // we go up in DOM tree to get top most separator tag
            if (res != null)
                separators.AddRange(res?.Select(n => n.ParentNode?.ParentNode?.ParentNode?.ParentNode).ToList());

            // another possibility
            //res = doc.DocumentNode.SelectNodes($"//div/div[contains(@style,'border-top:solid')]/p/b[{conditionsFrom}]")?.ToList();

            res = Shared.GetXPath2Nodes(doc, @"//div/div[contains(@style,'border-top:solid')]/p/b[" + conditionsFrom + "]");

            if (res != null)
                separators.AddRange(res?.Select(n => n.ParentNode?.ParentNode?.ParentNode).ToList());

            // another possibility
            res = doc.DocumentNode.SelectNodes("//div/span/hr")?.Where(n => n.ParentNode?.ParentNode?.NextSibling?.SelectSingleNode($"//div/p/strong/span[contains(.,'from:') or contains(.,'From:')]") != null)?.ToList();

            if (res != null)
                separators.AddRange(res?.Select(n => n.ParentNode?.ParentNode).ToList());

            // another possibility, commented replay
            res = doc.DocumentNode.SelectNodes("//comment()[contains(.,'originalMessage')]")?.ToList();
            if (res != null)
                separators.AddRange(res?.Select(n => n.ParentNode).ToList());

            // another possibility, commented replay
            res = doc.DocumentNode.SelectNodes("//blockquote/div/div/div")?.ToList();
            if (res != null)
                separators.AddRange(res?.Select(n => n.ParentNode).ToList());

            // another possibility, commented replay
            res = doc.DocumentNode.SelectNodes("//div/p/span[contains(.,'ORIGINAL MESSAGE')]")?.ToList();
            if (res != null)
                separators.AddRange(res?.Select(n => n.ParentNode?.ParentNode).ToList());





            // since we splinting the replays sequentially, we need to make sure that our separators comes in order, also we make them unique, since different criteria may result in repeated separators
            separators = separators.Distinct().OrderBy(s => s.OuterStartIndex).ToList();

            return separators;
        }



        private HtmlNode FindBody(HtmlDocument doc)
        {
            return doc.DocumentNode.SelectSingleNode("/html/body");
        }

        #endregion

    }
}
