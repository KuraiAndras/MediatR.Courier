namespace MediatR.Courier;

/// <summary>
/// Configuration options for MediatR Courier behavior.
/// </summary>
public sealed class CourierOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether to capture the thread context when awaiting tasks.
    /// This is used like: ConfigureAwait(CaptureThreadContext).
    /// Defaults to false.
    /// </summary>
    public bool CaptureThreadContext { get; set; }
    public bool UseTaskWhenAll { get; set; }
}