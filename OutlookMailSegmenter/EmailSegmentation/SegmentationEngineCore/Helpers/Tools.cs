using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;
using Wmhelp.XPath2;

namespace TMS.Libraries.EmailSegmentation.SegmentationEngineCore.Helpers
{
    public static class Tools
    {

        #region Help Methods

        public static List<HtmlNode> GetXPath2Nodes(HtmlDocument doc, string query)
        {
            return doc
                        .CreateNavigator()
                        .XPath2SelectNodes(query)
                        .Cast<HtmlNodeNavigator>()
                        .ToList()
                        .Select(n => n.CurrentNode).ToList();
        }

        #endregion

    }
}
