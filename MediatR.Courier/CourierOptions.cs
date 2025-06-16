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

    /// <summary>
    /// Gets or sets a value indicating whether to use Task.WhenAll to await handler tasks concurrently.
    /// When set to true, all notification handlers are collected and awaited using Task.WhenAll.
    /// When set to false, notification handlers are awaited sequentially.
    /// Defaults to false.
    /// </summary>
    public bool UseTaskWhenAll { get; set; }
}
