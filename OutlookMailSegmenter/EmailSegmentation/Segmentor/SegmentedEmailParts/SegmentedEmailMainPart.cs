using System.Collections.Generic;
using System.Collections.ObjectModel;
using TMS.Libraries.EmailSegmentation.SegmentationEngineCore;
using TMS.Libraries.EmailSegmentation.Segmentor.Segments;
using static TMS.Libraries.EmailSegmentation.SegmentationEngineCore.Helpers.InfoParsers;

namespace TMS.Libraries.EmailSegmentation.Segmentor.SegmentedEmailParts
{
    public class SegmentedEmailMainPart : SegmentedEmailReplayPart
    {
        ISegmentationEngine _SegmentationEngine;
        HeaderInfo _MainEmailHeader;
        internal SegmentedEmailMainPart(Factory factory, string html,
                                         HeaderInfo mainEmailHeader,
                                         ISegmentationEngine segmentationEngine)
        {
            Factory = factory;
            _SegmentationEngine = segmentationEngine;
            _MainEmailHeader = mainEmailHeader;
            this.OriginalHTML = html;
        }

        //public string OriginalHTML { get; private set; }

        ReadOnlyCollection<SegmentedEmailReplayPart> _Replays;
        public ReadOnlyCollection<SegmentedEmailReplayPart> Replays
        {
            get
            {
                if (!_EmailProcessed)
                    ProcessEmail();

                return _Replays;
            }
            set { _Replays = value; }
        }


        //public new BodySegment Body { 
        //    get
        //    {
        //        if (!_EmailProcessed)
        //            ProcessEmail();

        //        return base.Body;
        //    }

        //}

        internal override void Segment()
        {
            ProcessEmail();
        }

        private HeaderSegment _Header;

        public override HeaderSegment Header
        {
            get
            {
                if (_Header is null)
                    _Header = new HeaderSegment(_MainEmailHeader.From,
                                                _MainEmailHeader.To,
                                                _MainEmailHeader.CC,
                                                _MainEmailHeader.Date,
                                                _MainEmailHeader.Subject,
                                                this);

                return _Header;

            }
        }

        private bool _EmailProcessed;
        internal void ProcessEmail()
        {
            if (!_EmailProcessed && !string.IsNullOrWhiteSpace(this.OriginalHTML))
            {
                _SegmentationEngine.Segment(this.OriginalHTML);

                if (_SegmentationEngine.SingleEmailsSegments != null)
                {

                    this.Origin = _SegmentationEngine.SingleEmailsSegments[0];



                    //// build the segments of the main message
                    //if (!string.IsNullOrWhiteSpace(cur.BodyHTML))
                    //    this.Body = new BodySegment(this, cur.BodyHTML);

                    //if (!string.IsNullOrWhiteSpace(cur.SignatureHTML))
                    //    this.Signature = new SignatureSegment(this, cur.SignatureHTML);

                    //build the segments of the replays
                    List<SegmentedEmailReplayPart> replayes = new();


                    //ISegmentedSingleHTMLEmail cur;// = _SegmentationEngine.SingleEmailsSegments[0];

                    for (int i = 1; i < _SegmentationEngine.SingleEmailsSegments?.Count; i++)
                    {
                        //cur = _SegmentationEngine.SingleEmailsSegments[i];
                        //cur.Segment();
                        replayes.Add(new SegmentedEmailReplayPart(this,
                                                                      _SegmentationEngine.SingleEmailsSegments[i]));
                    }

                    if (replayes.Count > 0)
                        this.Replays = replayes.AsReadOnly();
                }

                _EmailProcessed = true;

            }
        }

    }
}
