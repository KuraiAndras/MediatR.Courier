namespace MediatR.Courier.Examples.Wpf.Core.ViewModels
{
    public sealed class ExampleViewModel : ViewModelBase, IExampleViewModel
    {
        public ExampleViewModel(
            IMediator mediator,
            ICourier courier)
            : base(mediator, courier)
        {
        }
    }
}
