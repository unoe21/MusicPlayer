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
using MahApps.Metro.IconPacks;
using MusicPlayer.MVVM.Model;
using MusicPlayer.MVVM.ViewModel;


namespace MusicPlayer.MVVM.View
{
    /// <summary>
    /// Interaction logic for SongsView.xaml
    /// </summary>
    public partial class SongsView : UserControl
    {
        public SongsView()
        {
            InitializeComponent();
            this.DataContext = App.Current.MainWindow.DataContext;  // Feltevés, hogy a MainWindow a MainViewModel-t használja

        }

        // Track kiválasztásának eseménykezelője
        private void Track_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SongsListBox.SelectedItem != null)
            {
                var selectedTrack = (Track)SongsListBox.SelectedItem;

                Debug.WriteLine($"Selected track: {selectedTrack.Title}");

                var viewModel = this.DataContext as MainViewModel;
                if (viewModel != null)
                {
                    
                    // Az aktuális track-et beállítjuk
                    viewModel.CurrentTrack = selectedTrack;

                    // Betöltjük az új track fájlt a mediaPlayer-be
                    viewModel.mediaPlayer.Open(new Uri(selectedTrack.FilePath));

                    viewModel.LoadCoverImage(selectedTrack.FilePath);

                    var tagFile = TagLib.File.Create(selectedTrack.FilePath);
                    viewModel.Artist = tagFile.Tag.FirstPerformer ?? "Unknown Artist";
                    viewModel.Title = selectedTrack.Title;

                    // Lejátszás indítása
                    viewModel.PlayPause();
                }
            }
            else
            {
                Debug.WriteLine("No track selected");
            }
        }


    }
}
