using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace MediatR.Courier.Examples.Wpf.Core.ViewModels
{
    public interface IExampleViewModel : INotifyPropertyChanged, IDisposable
    {
        Task InitializeAsync();
        Task IncrementNotificationCountAsync();
        int NotificationCount { get; }
    }
}