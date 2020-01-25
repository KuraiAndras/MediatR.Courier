using MediatR.Courier.Examples.Shared.Notifications;
using MediatR.Courier.Examples.Shared.Requests;
using System.Threading;
using System.Threading.Tasks;

namespace MediatR.Courier.Examples.Shared.RequestHandlers
{
    public sealed class ExampleRequestHandler : AsyncRequestHandler<ExampleRequest>
    {
        private readonly IMediator _mediator;

        public ExampleRequestHandler(IMediator mediator) => _mediator = mediator;

        protected override async Task Handle(ExampleRequest request, CancellationToken cancellationToken) => await _mediator.Publish(new ExampleNotification(), cancellationToken);
    }
}
