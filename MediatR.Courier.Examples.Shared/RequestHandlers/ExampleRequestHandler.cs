using MediatR.Courier.Examples.Shared.Notifications;
using MediatR.Courier.Examples.Shared.Requests;
using System.Threading;
using System.Threading.Tasks;

namespace MediatR.Courier.Examples.Shared.RequestHandlers
{
    /// <summary>
    /// Demonstrates modifying and querying some shared state. Call count could be replaced for example with a DbContext.
    /// </summary>
    public sealed class ExampleRequestHandler : AsyncRequestHandler<IncrementCallCountCommand>, IRequestHandler<NotificationCountQuery, int>
    {
        private static readonly SemaphoreSlim Semaphore = new SemaphoreSlim(1, 1);
        private static int _callCount;

        private readonly IMediator _mediator;

        public ExampleRequestHandler(IMediator mediator) => _mediator = mediator;

        public async Task<int> Handle(NotificationCountQuery request, CancellationToken cancellationToken)
        {
            await Semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);

            var response = _callCount;

            Semaphore.Release();

            return response;
        }

        protected override async Task Handle(IncrementCallCountCommand request, CancellationToken cancellationToken)
        {
            await Semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);

#pragma warning disable S2696 // Instance members should not write to "static" fields
            _callCount++;
#pragma warning restore S2696 // Instance members should not write to "static" fields

            await _mediator.Publish(new ExampleNotification(_callCount), cancellationToken).ConfigureAwait(false);

            Semaphore.Release();
        }
    }
}
