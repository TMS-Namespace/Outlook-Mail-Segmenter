namespace TMS.Libraries.ClassicalEmailSegmenter
{
    public interface IBaseSegment
    {
        BodySegment Body { get; }
        string OriginalHTML { get; }
        BaseSegment Parent { get; }
    }
}