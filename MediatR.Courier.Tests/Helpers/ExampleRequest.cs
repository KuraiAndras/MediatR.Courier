namespace MediatR.Courier.Tests.Helpers
{
    public sealed class ExampleRequest : IRequest
    {
        public string Message { get; set; } = string.Empty;
    }
}