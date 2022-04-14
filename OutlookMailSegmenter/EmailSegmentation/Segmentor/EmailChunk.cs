using System;
using TMS.Libraries.EmailSegmentation.Segmentor.Segments;

namespace TMS.Libraries.EmailSegmentation.Segmentor
{
    public abstract class EmailChunk
    {
        #region Init

        //private EmailChunk Origin;

        internal EmailChunk(EmailChunk parent)//, string originalHTML)
        {

            //Origin = origin;
            ID = Guid.NewGuid();
            //this.OriginalHTML = originalHTML;
            //if (!string.IsNullOrWhiteSpace(originalHTML))
            //    Body = new BodySegment(this, originalHTML);

            this.Parent = parent;
            this.Factory = parent?.Factory;
        }

        #endregion

        #region Properties

        public Guid ID { get; private set; }

        //private BodySegment _Body;
        public virtual BodySegment Body { get; internal set; }
        public string OriginalHTML { get; internal set; }

        //{
        //    get
        //    {

        //        //if (_Body == null && !(Origin == null || Origin.Body == null))
        //        //    _Body = new BodySegmentEx(Origin.Body, this);

        //        _Body ??= new BodySegment(this);
        //        return _Body;

        //    }
        //}

        public EmailChunk Parent { get; private set; }


        /// <summary>
        /// Segment's HTML before any manipulations.
        /// </summary>
        //private string _HTML;
        //public string HTML
        //{
        //    get
        //    {
        //        if (string.IsNullOrWhiteSpace(_HTML))
        //            _HTML = Cleaners.CleanHTML(OriginalHTML);


        //        return _HTML;
        //    }
        //}

        //public string OriginalHTML { get; internal set; }

        public Factory Factory { get; internal set; }

        #endregion

    }
}
