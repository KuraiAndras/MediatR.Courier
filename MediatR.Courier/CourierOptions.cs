namespace MediatR.Courier;

public sealed class CourierOptions
{
    public bool CaptureThreadContext { get; set; }
    public bool UseTaskWhenAll { get; set; }
}