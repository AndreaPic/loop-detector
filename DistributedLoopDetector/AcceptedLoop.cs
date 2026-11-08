namespace DistributedLoopDetector
{
    /// <summary>Request differences that may allow a detected distributed loop to continue.</summary>
    [Flags]
    public enum AcceptedLoop
    {
        /// <summary>Stop every detected loop (the default behaviour).</summary>
        None = 0,
        /// <summary>Allow a different body, route parameters, or query parameters.</summary>
        WithDifferentPayload = 1,
        /// <summary>Allow different request headers, excluding loop and tracing metadata.</summary>
        WithDifferentHeaders = 2
    }
}
