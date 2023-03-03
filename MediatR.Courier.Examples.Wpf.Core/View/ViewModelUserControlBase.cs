using System.Windows.Controls;

using MediatR.Courier.Examples.Wpf.Core.DependencyInjection;

namespace MediatR.Courier.Examples.Wpf.Core.View;

public abstract class ViewModelUserControlBase<TViewModel> : UserControl
{
    protected ViewModelUserControlBase()
    {
        ViewModel = CompositionRoot.Resolve<TViewModel>()!;
        DataContext = ViewModel;
    }

    protected TViewModel ViewModel { get; }
}