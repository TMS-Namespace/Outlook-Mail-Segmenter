using System.Collections.ObjectModel;

namespace TMS.Libraries.EmailSegmentation.SegmentationEngineCore
{
    public interface ISegmentationEngine
    {
        ReadOnlyCollection<ISegmentedSingleHTMLEmail> SingleEmailsSegments { get; }

        void Segment(string html);
        bool IsSegmented { get; }


    }
}