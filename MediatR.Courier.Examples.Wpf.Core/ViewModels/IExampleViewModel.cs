using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace MediatR.Courier.Examples.Wpf.Core.ViewModels
{
    public interface IExampleViewModel : INotifyPropertyChanged, IDisposable
    {
        int NotificationCount { get; }

        Task InitializeAsync();

        Task IncrementNotificationCountAsync();
    }
}
