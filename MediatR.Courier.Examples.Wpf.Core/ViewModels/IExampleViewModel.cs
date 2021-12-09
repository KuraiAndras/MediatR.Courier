using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace MediatR.Courier.Examples.Wpf.Core.ViewModels
{
    public interface IExampleViewModel : INotifyPropertyChanged
    {
        int NotificationCount { get; }

        Task InitializeAsync();

        Task IncrementNotificationCountAsync();
    }
}
