using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TMS.Libraries.EmailSegmentation.EngineCore
{
    public interface ISegmentationEngine : ISegmentationResults
    {
        ReadOnlyCollection<ISegmentationResults> Replays { get; }
    
    }
}