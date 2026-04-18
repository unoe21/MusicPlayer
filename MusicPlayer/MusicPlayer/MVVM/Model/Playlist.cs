using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MusicPlayer.MVVM.Model
{
    /// <summary>
    /// Represents a user-created collection of tracks.
    /// Inherits from ObservableObject to support real-time UI data binding.
    /// </summary>
    public partial class Playlist : ObservableObject
    {
        /// <summary>
        /// Gets or sets the display name of the playlist.
        /// The ObservableProperty attribute automatically generates the public 'Name' property 
        /// and notifies the UI whenever the name changes.
        /// </summary>
        [ObservableProperty]
        private string _name = string.Empty;

        /// <summary>
        /// Gets the collection of tracks included in this playlist.
        /// The setter is intentionally omitted to prevent accidental overwriting of the collection instance,
        /// which would break existing XAML bindings. Use .Add(), .Remove(), or .Clear() to modify contents.
        /// </summary>
        public ObservableCollection<Track> Tracks { get; } = new();
    }
}