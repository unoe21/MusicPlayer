using MahApps.Metro.IconPacks;
using Microsoft.Win32;
using MusicPlayer.MVVM.View;
using MusicPlayer.MVVM.ViewModel;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MusicPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var dialogService = new OpenFileDialogService();
            var viewModel = new MainViewModel(dialogService);
            DataContext = viewModel;

            viewModel.LoadAlbums();
            viewModel.LoadAllTracks();


        }


        MediaPlayer mediaPlayer = new MediaPlayer();
        string filename;

        private bool isPlaying = false;




        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }
        private void Ellipse_MouseDown_Red(object sender, MouseButtonEventArgs e)
        {
            if(e.ChangedButton == MouseButton.Left)
            {
                this.Close();
            }
        }
        private void Ellipse_MouseDown_Yellow(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.WindowState = WindowState.Normal;
            }
        }
        private void Ellipse_MouseDown_Green(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.WindowState = WindowState.Maximized;
            }
        }
        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel viewModel && sender is Button button)
            {
                string viewName = button.Name;
                if (!string.IsNullOrEmpty(viewName))
                {
                    viewModel.NavigateTo(viewName);  // a kiválasztott nézet betöltése
                }
            }
        }
        private void ProgressSlider_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            // Amikor elkezdjük húzni, állítsuk be, hogy a felhasználó húzza a Slider-t
            var viewModel = DataContext as MainViewModel;
            if (viewModel != null)
            {
                viewModel.IsUserDraggingSlider = true;
            }
        }
        private void ProgressSlider_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            // Amikor elengedjük, állítsuk vissza, hogy már nem húzunk
            var viewModel = DataContext as MainViewModel;
            if (viewModel != null)
            {
                viewModel.IsUserDraggingSlider = false;
                viewModel.SeekTo(viewModel.CurrentPositionSeconds);  // A pozíció frissítése, miután elengedtük
            }
        }
    }
}