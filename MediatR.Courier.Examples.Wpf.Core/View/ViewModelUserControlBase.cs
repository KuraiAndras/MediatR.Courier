using MediatR.Courier.Examples.Wpf.Core.DependencyInjection;
using System.Windows.Controls;

namespace MediatR.Courier.Examples.Wpf.Core.View
{
    public abstract class ViewModelUserControlBase<TViewModel> : UserControl
    {
        protected ViewModelUserControlBase()
        {
            ViewModel = CompositionRoot.Resolve<TViewModel>()!;
            DataContext = ViewModel;
        }

        protected TViewModel ViewModel { get; }
    }
}
