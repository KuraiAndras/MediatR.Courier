namespace MediatR.Courier.Examples.Wpf.Core.ViewModels
{
    public abstract class ViewModelBase
    {
        protected ViewModelBase(IMediator mediator, ICourier courier)
        {
            Mediator = mediator;
            Courier = courier;
        }

        protected IMediator Mediator { get; }
        protected ICourier Courier { get; }
    }
}
