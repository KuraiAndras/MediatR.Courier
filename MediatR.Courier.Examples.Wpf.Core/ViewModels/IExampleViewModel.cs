using System.ComponentModel;
using System.Threading.Tasks;

namespace MediatR.Courier.Examples.Wpf.Core.ViewModels
{
    public interface IExampleViewModel : INotifyPropertyChanged
    {
        Task Initialize();
        Task IncrementNotificationCount();
        int NotificationCount { get; }
    }
}