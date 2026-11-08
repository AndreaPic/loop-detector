namespace DistributedLoopDetector
{
    /// <summary>The request differences accepted by this application's loop detector.</summary>
    public sealed class LoopDetectorOptions
    {
        public LoopDetectorOptions(AcceptedLoop acceptedLoop = AcceptedLoop.None)
        {
            if ((acceptedLoop & ~(AcceptedLoop.WithDifferentPayload | AcceptedLoop.WithDifferentHeaders)) != 0)
                throw new ArgumentOutOfRangeException(nameof(acceptedLoop));
            AcceptedLoop = acceptedLoop;
        }

        public AcceptedLoop AcceptedLoop { get; }
    }
}
