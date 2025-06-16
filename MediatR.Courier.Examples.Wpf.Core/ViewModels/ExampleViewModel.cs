using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

using MediatR.Courier.Examples.Wpf.Core.Notifications;
using MediatR.Courier.Examples.Wpf.Core.Requests;

namespace MediatR.Courier.Examples.Wpf.Core.ViewModels;

public sealed class ExampleViewModel : ViewModelBase, IExampleViewModel
{
    private int _notificationCount;

    public ExampleViewModel(IMediator mediator, ICourier courier)
        : base(mediator, courier)
    {
        if (Courier is null) throw new ArgumentNullException(nameof(courier));

        Courier.SubscribeWeak<ExampleNotification>(ExampleNotificationFired);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public int NotificationCount
    {
        get => _notificationCount;
        private set => SetAndNotifyPropertyChanged(ref _notificationCount, value);
    }

    public async Task InitializeAsync() => NotificationCount = await Mediator.Send(new NotificationCountQuery()).ConfigureAwait(false);

    public async Task IncrementNotificationCountAsync() => await Mediator.Send(new IncrementCallCountCommand()).ConfigureAwait(false);

    private void SetAndNotifyPropertyChanged<T>(ref T backingField, T value, [CallerMemberName] string? propertyName = default)
    {
        if (backingField?.Equals(value) == true) return;

        backingField = value;

        Application.Current.Dispatcher?.Invoke(() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)));
    }

    private void ExampleNotificationFired(ExampleNotification notification, CancellationToken _) => NotificationCount = notification.NotificationCount;
}
