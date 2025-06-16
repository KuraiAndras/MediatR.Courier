using System.Windows;

namespace MediatR.Courier.Examples.Wpf.Core.View;

public partial class MainWindow : Window
{
    public MainWindow() => InitializeComponent();

    private void OpenWindowClicked(object sender, RoutedEventArgs e)
    {
        var mainControl = new MainControl();
        var window = new Window { Content = mainControl };

        window.Show();
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Critical Code Smell", "S1215:\"GC.Collect\" should not be called", Justification = "We want that for testing")]
    private async void OnRunGcClicked(object sender, RoutedEventArgs e)
    {
        await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(false);
        GC.Collect(2, GCCollectionMode.Forced, true);
    }
}
