using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;

namespace MusicPlayer.MVVM.Model
{
    public partial class Album : ObservableObject
    {
        [ObservableProperty]
        private string _name;

        [ObservableProperty]
        private string _artist;

        [ObservableProperty]
        private BitmapImage _coverImage;

        public ObservableCollection<Track> Tracks { get; } = new();
    }
}