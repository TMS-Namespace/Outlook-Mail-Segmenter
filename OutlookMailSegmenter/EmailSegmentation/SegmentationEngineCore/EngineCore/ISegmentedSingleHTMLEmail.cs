namespace TMS.Libraries.EmailSegmentation.SegmentationEngineCore
{
    public interface ISegmentedSingleHTMLEmail
    {
        string BodyHTML { get; }
        string HeaderHTML { get; }
        string SignatureHTML { get; }

        string UnsegmentedHTML { get; }

        void Segment();

        bool IsSegmented { get; }
    }
}