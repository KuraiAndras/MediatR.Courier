using System.Threading;
using System.Threading.Tasks;

namespace MediatR.Courier.Tests.Helpers
{
    public sealed class ExampleRequestHandler : AsyncRequestHandler<ExampleRequest>
    {
        private readonly IMediator _mediator;

        public ExampleRequestHandler(IMediator mediator) => _mediator = mediator;

        protected override async Task Handle(ExampleRequest request, CancellationToken cancellationToken) =>
            await _mediator.Publish(new ExampleNotification { Message = request.Message + "Handle sent Notification" }, cancellationToken);
    }
}