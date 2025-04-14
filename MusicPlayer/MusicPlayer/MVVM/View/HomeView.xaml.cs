using MusicPlayer.MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MusicPlayer.MVVM.Model;



namespace MusicPlayer.MVVM.View
{
    /// <summary>
    /// Interaction logic for HomeView.xaml
    /// </summary>
    public partial class HomeView : UserControl
    {
        public HomeView()
        {
            InitializeComponent();
            this.DataContext = App.Current.MainWindow.DataContext;
        }

        private void Track_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            if (SongsListBox.SelectedItem != null)
            {
                var selectedTrack = (Track)SongsListBox.SelectedItem;

                Debug.WriteLine($"Selected track: {selectedTrack.Title}");

                var viewModel = this.DataContext as MainViewModel;
                if (viewModel != null)
                {

                    viewModel.CurrentTrack = selectedTrack;

                    viewModel.mediaPlayer.Open(new Uri(selectedTrack.FilePath));

                    viewModel.PlayPause();

                    viewModel.LoadCoverImage(selectedTrack.FilePath);
                    var tagFile = TagLib.File.Create(selectedTrack.FilePath);
                    viewModel.Artist = tagFile.Tag.FirstPerformer ?? "Unknown Artist";
                    viewModel.Title = selectedTrack.Title;

                }
            }
            else
            {
                Debug.WriteLine("No track selected");
            }
        }
    }
}
