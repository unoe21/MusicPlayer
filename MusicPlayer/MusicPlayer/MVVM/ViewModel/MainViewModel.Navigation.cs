using System.Linq;
using CommunityToolkit.Mvvm.Input;
using MusicPlayer.MVVM.Model;
using MusicPlayer.MVVM.View;

namespace MusicPlayer.MVVM.ViewModel
{
    /// <summary>
    /// Partial class handling application routing, view switching, and list filtering based on user selection.
    /// </summary>
    public partial class MainViewModel
    {
        /// <summary>
        /// Navigates the main content area to the specified view and resets state if necessary.
        /// </summary>
        /// <param name="viewName">The string identifier of the target view (e.g., "Home", "Songs").</param>
        [RelayCommand]
        public void NavigateTo(string viewName)
        {
            switch (viewName)
            {
                case "Home":
                    CurrentView = new HomeView();
                    break;
                case "Playlists":
                    CurrentView = new PlaylistsView();
                    break;
                case "Artists":
                    CurrentView = new ArtistsView();
                    break;
                case "Albums":
                    CurrentView = new AlbumsView();
                    break;
                case "Songs":
                    // Reset all filters when manually navigating to the raw Songs view
                    LoadAllTracks();
                    SelectedArtist = string.Empty;
                    SelectedAlbum = null;
                    SelectedPlaylist = null;
                    SongsTitle = "Songs";
                    CurrentView = new SongsView();
                    break;
            }
        }

        /// <summary>
        /// Automatically triggered by the MVVM Toolkit when the SelectedMenu property changes.
        /// </summary>
        partial void OnSelectedMenuChanged(string value)
        {
            NavigateTo(value);
        }

        /// <summary>
        /// Automatically triggered when the user clicks on an Artist in the ArtistsView.
        /// Filters the tracks to only show songs by the selected artist and navigates to the SongsView.
        /// </summary>
        partial void OnSelectedArtistChanged(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return;

            SongsTitle = $"{value} - Songs";
            AllTracks.Clear();

            var artistAlbums = Albums.Where(a => a.Artist == value);
            foreach (var album in artistAlbums)
            {
                foreach (var track in album.Tracks)
                {
                    AllTracks.Add(track);
                }
            }

            CurrentView = new SongsView();
        }

        /// <summary>
        /// Automatically triggered when the user clicks on an Album in the AlbumsView.
        /// Filters the tracks to only show songs from the selected album and navigates to the SongsView.
        /// </summary>
        partial void OnSelectedAlbumChanged(Album? value)
        {
            if (value == null) return;

            SongsTitle = $"{value.Name} - Songs";
            AllTracks.Clear();

            foreach (var track in value.Tracks)
            {
                AllTracks.Add(track);
            }

            CurrentView = new SongsView();
        }

        /// <summary>
        /// Automatically triggered when the user clicks on a Playlist in the PlaylistsView.
        /// Loads the custom tracks associated with the playlist and navigates to the SongsView.
        /// </summary>
        partial void OnSelectedPlaylistChanged(Playlist? value)
        {
            if (value == null) return;

            SongsTitle = $"{value.Name} - Playlist";
            AllTracks.Clear();

            foreach (var track in value.Tracks)
            {
                AllTracks.Add(track);
            }

            CurrentView = new SongsView();
        }
    }
}