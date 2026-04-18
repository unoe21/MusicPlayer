using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MusicPlayer.MVVM.Model
{
    /// <summary>
    /// Represents an album entity within the music library.
    /// Inherits from ObservableObject to support data binding and UI updates in WPF.
    /// </summary>
    public partial class Album : ObservableObject
    {
        /// <summary>
        /// Gets or sets the title of the album.
        /// The ObservableProperty attribute automatically generates the public 'Name' property 
        /// and handles INotifyPropertyChanged events.
        /// </summary>
        [ObservableProperty]
        private string _name = string.Empty;

        /// <summary>
        /// Gets or sets the name of the artist who released the album.
        /// </summary>
        [ObservableProperty]
        private string _artist = string.Empty;

        /// <summary>
        /// Gets or sets the cover art image of the album.
        /// This can be null if the album does not have embedded or local cover art.
        /// </summary>
        [ObservableProperty]
        private BitmapImage? _coverImage;

        /// <summary>
        /// Gets the collection of tracks belonging to this album.
        /// Uses ObservableCollection to automatically notify the UI when tracks are added or removed.
        /// </summary>
        public ObservableCollection<Track> Tracks { get; } = new();
    }
}