using System.Windows;
using System.Windows.Input;
using MusicPlayer.MVVM.ViewModel;

namespace MusicPlayer.MVVM.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // Ablak mozgatása a felső sávnál fogva
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        // Ablakvezérlő gombok
        private void Ellipse_MouseDown_Red(object sender, MouseButtonEventArgs e) => Application.Current.Shutdown();
        private void Ellipse_MouseDown_Yellow(object sender, MouseButtonEventArgs e) => this.WindowState = WindowState.Minimized;
        private void Ellipse_MouseDown_Green(object sender, MouseButtonEventArgs e) { /* Maximize logic if needed */ }

        // Slider eseménykezelők, amik a ViewModel specializált metódusait hívják
        private void ProgressSlider_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.UserStartedDragging();
            }
        }

        private void ProgressSlider_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.UserFinishedDragging();
            }
        }
    }
}