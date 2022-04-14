namespace TMS.Libraries.EmailSegmentation.EngineCore
{
    public interface ISegmentationResults
    { 
        string BodyHTML { get; }
        string HeaderHTML { get; }
        string SignatureHTML { get; }

        string UnsegmentedHTML { get; }

        void Segment(string html);
    }
}