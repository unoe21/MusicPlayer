using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MusicPlayer.MVVM.Model
{
    /// <summary>
    /// Represents a single playable audio track within the music library.
    /// Inherits from ObservableObject to ensure any changes to the track's properties 
    /// (like updating the title from ID3 tags) are instantly reflected in the UI.
    /// </summary>
    public partial class Track : ObservableObject
    {
        /// <summary>
        /// Gets or sets the display title of the track.
        /// Generated automatically by the [ObservableProperty] attribute.
        /// </summary>
        [ObservableProperty]
        private string _title = string.Empty;

        /// <summary>
        /// Gets or sets the absolute file path to the audio file on the local file system.
        /// Initialized to string.Empty to prevent null reference exceptions before a file is loaded.
        /// </summary>
        [ObservableProperty]
        private string _filePath = string.Empty;

        /// <summary>
        /// Gets or sets the total playback duration of the track.
        /// Uses TimeSpan to easily format the duration into minutes and seconds for the UI.
        /// </summary>
        [ObservableProperty]
        private TimeSpan _duration;
    }
}