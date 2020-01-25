using MediatR.Courier.Examples.Wpf.Core.ViewModels;
using System;
using System.Windows;

namespace MediatR.Courier.Examples.Wpf.Core.View
{
    public partial class MainControl : ViewModelUserControlBase<IExampleViewModel>
    {
        public MainControl() => InitializeComponent();

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            ViewModel.InitializeAsync();
        }

        private void IncrementClicked(object sender, RoutedEventArgs e) => ViewModel.IncrementNotificationCountAsync();
    }
}
