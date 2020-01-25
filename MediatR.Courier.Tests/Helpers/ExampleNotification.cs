namespace MediatR.Courier.Tests.Helpers
{
    public sealed class ExampleNotification : INotification
    {
        public string Message { get; set; } = string.Empty;
    }
}