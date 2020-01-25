using MediatR.Courier.Examples.Shared.Notifications;
using MediatR.Courier.Examples.Shared.Requests;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace MediatR.Courier.Examples.Wpf.Core.ViewModels
{
    public sealed class ExampleViewModel : ViewModelBase, IExampleViewModel
    {
        private int _notificationCount;

        public ExampleViewModel(
            IMediator mediator,
            ICourier courier)
            : base(mediator, courier) =>
            Courier.Subscribe<ExampleNotification>(ExampleNotificationFired);

        private void ExampleNotificationFired(ExampleNotification notification, CancellationToken _) => NotificationCount = notification.NotificationCount;

        public async Task InitializeAsync() => NotificationCount = await Mediator.Send(new NotificationCountQuery()).ConfigureAwait(false);
        public async Task IncrementNotificationCountAsync() => await Mediator.Send(new IncrementCallCountCommand()).ConfigureAwait(false);

        public int NotificationCount
        {
            get => _notificationCount;
            private set => SetAndNotifyPropertyChanged(ref _notificationCount, value);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void SetAndNotifyPropertyChanged<T>(ref T backingField, T value, [CallerMemberName] string propertyName = default)
        {
            if (backingField.Equals(value)) return;

            backingField = value;

            Application.Current.Dispatcher?.Invoke(() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)));
        }

        public void Dispose() => Courier.UnSubscribe<ExampleNotification>(ExampleNotificationFired);
    }
}
