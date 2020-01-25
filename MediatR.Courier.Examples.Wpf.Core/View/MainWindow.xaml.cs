using System.Windows;

namespace MediatR.Courier.Examples.Wpf.Core.View
{
    public partial class MainWindow : Window
    {
        public MainWindow() => InitializeComponent();

        private void OpenWindowClicked(object sender, RoutedEventArgs e)
        {
            var window = new Window
            {
                Content = new MainControl()
            };

            window.Show();
        }
    }
}
