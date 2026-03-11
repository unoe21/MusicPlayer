using System.Windows;
using System.Windows.Input;

namespace MusicPlayer.MVVM.View
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // --- EZEKET KERESTE A XAML FORDÍTÓ, EMIATT NEM MŰKÖDÖTT SEMMI! ---

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Ez teszi lehetővé, hogy a felső sávnál fogva mozgasd az ablakot
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void Ellipse_MouseDown_Red(object sender, MouseButtonEventArgs e)
        {
            // Piros gomb: Bezárás
            Application.Current.Shutdown();
        }

        private void Ellipse_MouseDown_Yellow(object sender, MouseButtonEventArgs e)
        {
            // Sárga gomb: Tálcára rakás
            this.WindowState = WindowState.Minimized;
        }

        private void Ellipse_MouseDown_Green(object sender, MouseButtonEventArgs e)
        {
            // Zöld gomb: Teljes képernyő
            if (this.WindowState == WindowState.Normal)
                this.WindowState = WindowState.Maximized;
            else
                this.WindowState = WindowState.Normal;
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            // Ezt már a ViewModel intézi (Commandokkal), így ez maradhat teljesen üresen!
        }

        private void ProgressSlider_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            // Ha a ViewModel be van kötve, jelezzük neki, hogy a felhasználó elkezdte húzni a csúszkát
            if (DataContext is ViewModel.MainViewModel vm)
            {
                vm.IsUserDraggingSlider = true;
            }
        }

        private void ProgressSlider_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            // Elengedtük a csúszkát, tekerjünk oda!
            if (DataContext is ViewModel.MainViewModel vm)
            {
                vm.IsUserDraggingSlider = false;
                vm.SeekTo(vm.CurrentPositionSeconds);
            }
        }

    }
}