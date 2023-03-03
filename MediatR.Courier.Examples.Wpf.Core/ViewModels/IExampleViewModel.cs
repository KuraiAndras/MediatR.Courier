using System.ComponentModel;

namespace MediatR.Courier.Examples.Wpf.Core.ViewModels;

public interface IExampleViewModel : INotifyPropertyChanged
{
    int NotificationCount { get; }

    Task InitializeAsync();

    Task IncrementNotificationCountAsync();
}