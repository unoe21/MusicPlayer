using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.Input;
using MusicPlayer.MVVM.Model;

namespace MusicPlayer.MVVM.ViewModel
{
    /// <summary>
    /// Partial class responsible for the creation, management, and saving of custom playlists.
    /// </summary>
    public partial class MainViewModel
    {
        /// <summary>
        /// Initializes the playlist creation process by clearing previous inputs,
        /// loading all available tracks into the selectable list, and opening the overlay UI.
        /// </summary>
        [RelayCommand]
        private void StartCreatePlaylist()
        {
            NewPlaylistName = string.Empty;
            AvailableTracksForPlaylist.Clear();

            foreach (var track in AllTracks)
            {
                AvailableTracksForPlaylist.Add(new SelectableTrack { Track = track, IsSelected = false });
            }

            IsCreatingPlaylist = true;
        }

        /// <summary>
        /// Saves the newly created playlist if a valid name is provided.
        /// Gathers all checked tracks, creates a new Playlist object, and adds it to the user's collection.
        /// </summary>
        [RelayCommand]
        private void SavePlaylist()
        {
            // Guard clause to prevent saving a playlist with an empty or whitespace-only name
            if (string.IsNullOrWhiteSpace(NewPlaylistName)) return;

            // Extract only the tracks that the user selected via the checkboxes
            var selectedTracks = AvailableTracksForPlaylist
                .Where(t => t.IsSelected)
                .Select(t => t.Track);

            var newPlaylist = new Playlist
            {
                Name = NewPlaylistName
            };

            foreach (var track in selectedTracks)
            {
                newPlaylist.Tracks.Add(track);
            }

            Playlists.Add(newPlaylist);
            IsCreatingPlaylist = false;
        }

        /// <summary>
        /// Cancels the playlist creation process and hides the overlay without saving any data.
        /// </summary>
        [RelayCommand]
        private void CancelCreatePlaylist()
        {
            IsCreatingPlaylist = false;
        }
    }
}