using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMS.Libraries.EmailSegmentation.SegmentationEngineCore;
using TMS.Libraries.EmailSegmentation.Segmentor.SegmentedEmailParts;
using TMS.Libraries.EmailSegmentation.Segmentor.Segments;
using static TMS.Libraries.EmailSegmentation.SegmentationEngineCore.Helpers.InfoParsers;

namespace TMS.Libraries.EmailSegmentation.Segmentor
{
    public class Factory
    {
        public List<SegmentedEmailMainPart> SegmentEmails(
                                            List<(HeaderInfo header, string html)> emails,
                                            ISegmentationEngine segmentationEngine)
        {
            if (emails?.Count > 0)
            {
                this.SegmentedEmails = new();
                // when in parallel we pre-reload all lazy properties
                if (this.ProcessInParallel)
                    emails
                        .AsParallel().
                        ForAll(m =>
                        {
                            var email = new SegmentedEmailMainPart(this,
                                                                                            m.html,
                                                                                            m.header,
                                                                                            segmentationEngine);
                            PreloadProperties(email);
                            this.SegmentedEmails.Add(email);
                        });
                else
                    emails.ForEach(m =>
                                    SegmentedEmails.Add(new SegmentedEmailMainPart(this,
                                                                                            m.html,
                                                                                            m.header,
                                                                                            segmentationEngine)));

                return this.SegmentedEmails;
            }

            return null;
        }

        public List<SegmentedEmailMainPart> SegmentedEmails { get; private set; }


        internal List<BodySegment> AllBodies { get; } = new List<BodySegment>();

        public bool CheckForIdenticalBodySegments { get; set; } = true;

        public bool GreedyHeadersProcessing { get; set; } = true;
        public bool GreedyReplaysProcessing { get; set; } = true;
        public bool GreedySignaturesProcessing { get; set; } = true;

        public bool ProcessInParallel { get; set; } = true;


        private void PreloadProperties(object obj)
        {
            if (obj is SegmentedEmailMainPart)
            {
                var casted = (SegmentedEmailMainPart)obj;
                PropertyInfo[] props = casted.GetType().GetProperties();
                // we exclude conversations, since they are not a parsing result
                props.ToList().ForEach(p =>
                {
                    var tmp = (p.Name != "Conversations") ? p.GetValue(casted) : null;
                }
                );
                PreloadProperties(casted.Body);
                PreloadProperties(casted.Signature);
                PreloadProperties(casted.Replays);

            }

            if (obj is HeaderSegment && this.GreedyHeadersProcessing)
            {
                var casted = (HeaderSegment)obj;
                PropertyInfo[] props = casted.GetType().GetProperties();
                props.ToList().ForEach(p => p.GetValue(casted));
                PreloadProperties(casted.Body);
            }

            if (obj is SignatureSegment && this.GreedySignaturesProcessing)
            {
                var casted = (SignatureSegment)obj;
                PropertyInfo[] props = casted.GetType().GetProperties();
                props.ToList().ForEach(p => p.GetValue(casted));
                PreloadProperties(casted.Body);
            }

            if (obj is SegmentedEmailReplayPart && this.GreedyReplaysProcessing)
            {
                var casted = (SegmentedEmailReplayPart)obj;
                PropertyInfo[] props = casted.GetType().GetProperties();
                props.ToList().ForEach(p => p.GetValue(casted));
                PreloadProperties(casted.Body);
                PreloadProperties(casted.Signature);
                PreloadProperties(casted.Header);
            }

            if (obj is BodySegment)
            {
                var casted = (BodySegment)obj;
                PropertyInfo[] props = casted.GetType().GetProperties();
                props.ToList().ForEach(p => p.GetValue(casted));
            }

        }

    }
}
