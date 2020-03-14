using System.Windows;

namespace MediatR.Courier.Examples.Wpf.Core.View
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Closed += (o, args) =>
            {
                MainControl0.Dispose();
                MainControl1.Dispose();
                MainControl2.Dispose();
            };
        }

        private void OpenWindowClicked(object sender, RoutedEventArgs e)
        {
            var mainControl = new MainControl();
            var window = new Window { Content = mainControl };

            window.Closed += (o, args) => mainControl.Dispose();
            window.Show();
        }
    }
}
