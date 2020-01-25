using MediatR.Courier.Examples.Wpf.Core.ViewModels;

namespace MediatR.Courier.Examples.Wpf.Core.View
{
    public partial class MainControl : ViewModelUserControlBase<IExampleViewModel>
    {
        public MainControl() => InitializeComponent();
    }
}
