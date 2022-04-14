using HtmlAgilityPack;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using TMS.Libraries.EmailSegmentation.SegmentationEngineCore;
using TMS.Libraries.EmailSegmentation.SegmentationEngineCore.Helpers;

namespace TMS.Libraries.EmailSegmentation.HTMLSegmentationEngine
{
    public sealed class SegmentationEngine : ISegmentationEngine
    {

        //#region Init

        //public Segmenter(string html) : base(null) { this.OriginalHTML = html; }

        //#endregion

        //#region Properties

        //private SignatureSegment _Signature;
        //public SignatureSegment Signature
        //{
        //    get
        //    {
        //        if (_Signature == null)
        //            Segment();

        //        return
        //            _Signature;
        //    }

        //}

        //private List<ReplaySegment> _Replays;
        //public List<ReplaySegment> Replays
        //{
        //    get
        //    {
        //        if (_Replays == null)
        //            Segment();

        //        return _Replays;
        //    }
        //}

        //// hide properties
        //private new BaseSegment Parent { get; set; }

        //private BodySegment _Body;
        //public override BodySegment Body
        //{
        //    get
        //    {
        //        if (_Body == null)
        //            Segment();

        //        return _Body;
        //    }
        //}

        //#endregion

        #region Help Methods

        public void Segment(string html)
        {
            if (!IsSegmented)
            {
                //this.UnsegmentedHTML = html;

                // --- find replays
                var replaysHTML = SplitToReplays(html);

                //// --- set the main email
                //this.HTMLDocument = new HtmlDocument();
                //SegmentHmlDocument.LoadHtml(replaysHTML[0]);

                //// for main body part, we first we find and strip out the signature
                //_Signature = new SignatureSegment(this);

                //// Force signature segmenting
                //var tmp = Signature.Body;

                //// set main body
                //_Body = new BodySegment(this);

                //if (replaysHTML.Count > 0)
                //{
                var emails = new List<ISegmentedSingleHTMLEmail>();

                if (replaysHTML.Count > 0)
                    replaysHTML.ForEach(r => emails.Add(new SegmentedSingleEmail(r)));
                //}

                this.SingleEmailsSegments = emails.AsReadOnly();
                this.IsSegmented = true;
            }
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
        //static string froms = string.Join("|", new List<string>() { "from", "от", "من" });

        //public ReadOnlyCollection<ISingleEmail> Replays => throw new System.NotImplementedException();

        //public string BodyHTML { get; private set; }

        //public string HeaderHTML  { get; private set; }

        //public string SignatureHTML  { get; private set; }

        public ReadOnlyCollection<ISegmentedSingleHTMLEmail> SingleEmailsSegments { get; private set; }

        public bool IsSegmented { get; private set; }


        //public string UnsegmentedHTML   { get; private set; }

        // private HtmlDocument HTMLDocument { get; set; }

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
            var conditionsFrom = @"matches(.,'(" + KeyWords.fromKW + @")\s*\:', 'i')";

            //var res = doc.DocumentNode.SelectNodes($"//div/div[contains(@style,'border-top:solid')]/p/b/span[{conditionsFrom}]")?.ToList();
            var res = Tools.GetXPath2Nodes(doc, @"//div/div[contains(@style,'border-top:solid')]/p/*[self::b or self::strong]/span[" + conditionsFrom + "]");

            // we go up in DOM tree to get top most separator tag
            if (res != null)
                separators.AddRange(res?.Select(n => n.ParentNode?.ParentNode?.ParentNode?.ParentNode).ToList());

            // another possibility
            //res = doc.DocumentNode.SelectNodes($"//div/div[contains(@style,'border-top:solid')]/p/b[{conditionsFrom}]")?.ToList();

            res = Tools.GetXPath2Nodes(doc, @"//div/div[contains(@style,'border-top:solid')]/p/b[" + conditionsFrom + "]");

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

            // another possibility, blockquote, however, they are nasty, since they are nested!
            res = doc.DocumentNode.SelectNodes("//blockquote/div/div/div")?.ToList();
            if (res != null)
                separators.AddRange(res?.Select(n => n.ParentNode?.ParentNode).ToList());

            // another hr option
            res = doc.DocumentNode.SelectNodes("//div/span/hr[contains(@style,'color:')]")?.ToList();
            if (res != null)
                separators.AddRange(res?.Select(n => n.ParentNode?.ParentNode).ToList());


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
