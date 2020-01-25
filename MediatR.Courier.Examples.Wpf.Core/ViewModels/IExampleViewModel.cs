using System.ComponentModel;
using System.Threading.Tasks;

namespace MediatR.Courier.Examples.Wpf.Core.ViewModels
{
    public interface IExampleViewModel : INotifyPropertyChanged
    {
        Task InitializeAsync();
        Task IncrementNotificationCountAsync();
        int NotificationCount { get; }
    }
}