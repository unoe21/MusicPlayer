using System.Collections.ObjectModel;

namespace MusicPlayer.MVVM.Model
{
    public class Playlist
    {
        public string Name { get; set; }
        public ObservableCollection<Track> Tracks { get; set; } = new ObservableCollection<Track>();
    }
}