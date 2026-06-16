using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using MusicPlayer.MVVM.Model;
using MusicPlayer.MVVM.Services;
namespace MusicPlayer.MVVM.ViewModel
{
    /// <summary>
    /// The central orchestrator of the application.
    /// This file contains the primary state properties, dependencies, and initialization logic.
    /// Commands and specific feature logic are divided into partial classes (Playback, Playlists, Navigation).
    /// </summary>
    public partial class MainViewModel : ObservableObject
    {
        #region Fields & Services

        private readonly IMediaLibraryService _mediaLibraryService;
        private readonly IAudioPlayerService _audioPlayerService;
        private readonly IOpenFileDialogService _fileDialogService;
        private readonly DispatcherTimer _timer = new();

        private int _currentTrackIndex = -1;
        private bool _wasPlayingBeforeDrag;

        #endregion

        #region Observable Properties (Application State)

        [ObservableProperty] private object? _currentView;
        [ObservableProperty] private ObservableCollection<Album> _albums = new();
        [ObservableProperty] private ObservableCollection<Track> _allTracks = new();
        [ObservableProperty] private ObservableCollection<Track> _dailyAlbum = new();
        [ObservableProperty] private ObservableCollection<string> _artists = new();
        [ObservableProperty] private double _volume = 50;
        [ObservableProperty] private string _selectedArtist = string.Empty;
        [ObservableProperty] private Album? _selectedAlbum;
        [ObservableProperty] private string _songsTitle = "Songs";
        [ObservableProperty] private string _artist = string.Empty;
        [ObservableProperty] private string _title = string.Empty;
        [ObservableProperty] private BitmapImage? _coverImage;
        [ObservableProperty] private string _selectedMenu = string.Empty;
        [ObservableProperty] private string _dailyArtist = string.Empty;
        [ObservableProperty] private string _dailyAlbumName = string.Empty;

        private Track? _currentTrack;

        /// <summary>
        /// Gets or sets the currently playing track.
        /// Features a custom setter to update the track index and automatically trigger playback when a new track is selected.
        /// </summary>
        public Track? CurrentTrack
        {
            get => _currentTrack;
            set
            {
                if (SetProperty(ref _currentTrack, value) && value != null)
                {
                    if (SelectedAlbum != null && SelectedAlbum.Tracks.Contains(value))
                    {
                        _currentTrackIndex = SelectedAlbum.Tracks.IndexOf(value);
                    }
                    PlaySelectedTrack(value); // Declared in MainViewModel.Playback.cs
                }
            }
        }

        #endregion

        #region Observable Properties (Playback & Timeline)

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(PlayPauseIconKind))]
        private bool _isPlaying;

        [ObservableProperty] private string _currentTimeString = "0:00";
        [ObservableProperty] private string _totalTimeString = "0:00";
        public string PlayPauseIconKind => IsPlaying ? "Pause" : "Play";
        [ObservableProperty] private double _currentPositionSeconds;
        [ObservableProperty] private double _currentTrackDurationSeconds;
        [ObservableProperty] private bool _isUserDraggingSlider;

        #endregion

        #region Observable Properties (Playlists)

        [ObservableProperty] private ObservableCollection<Playlist> _playlists = new();
        [ObservableProperty] private Playlist? _selectedPlaylist;
        [ObservableProperty] private bool _isCreatingPlaylist;
        [ObservableProperty] private string _newPlaylistName = string.Empty;
        [ObservableProperty] private ObservableCollection<SelectableTrack> _availableTracksForPlaylist = new();

        #endregion

        #region Constructor

        public MainViewModel(
            IMediaLibraryService mediaLibraryService,
            IAudioPlayerService audioPlayerService,
            IOpenFileDialogService fileDialogService)
        {
            _mediaLibraryService = mediaLibraryService;
            _audioPlayerService = audioPlayerService;
            _fileDialogService = fileDialogService;

            _audioPlayerService.PlaybackEnded += OnPlaybackEnded; // Declared in MainViewModel.Playback.cs

            _timer.Interval = TimeSpan.FromMilliseconds(500);
            _timer.Tick += Timer_Tick; // Declared in MainViewModel.Playback.cs

            _ = InitializeLibraryAsync();

            NavigateTo("Home"); // Declared in MainViewModel.Navigation.cs
        }

        #endregion

        #region Initialization & Data Loading

        private async Task InitializeLibraryAsync()
        {
            string musicFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            var loadedAlbums = await _mediaLibraryService.LoadAlbumsAsync(musicFolder);

            Albums = new ObservableCollection<Album>(loadedAlbums);
            LoadAllTracks();
            LoadDailyMix();
            LoadArtists();
        }

        private void LoadAllTracks()
        {
            AllTracks.Clear();
            foreach (var album in Albums)
            {
                foreach (var track in album.Tracks)
                {
                    AllTracks.Add(track);
                }
            }
        }

        private void LoadDailyMix()
        {
            if (Albums.Count == 0) return;

            // Modern .NET uses Random.Shared for better performance and thread-safety
            int randomIndex = Random.Shared.Next(Albums.Count);

            Album daily = Albums[randomIndex];
            DailyAlbum = daily.Tracks;
            DailyArtist = daily.Artist;
            DailyAlbumName = daily.Name;
        }

        private void LoadArtists()
        {
            Artists.Clear();
            var uniqueArtists = Albums
                .Select(a => a.Artist)
                .Where(artist => !string.IsNullOrWhiteSpace(artist))
                .Distinct()
                .OrderBy(artist => artist)
                .ToList();

            foreach (var artist in uniqueArtists)
            {
                Artists.Add(artist);
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Attempts to extract the embedded cover art from an audio file.
        /// Falls back to local folder images if no embedded art is found.
        /// </summary>
        private BitmapImage? GetCoverImage(TagLib.File tagFile)
        {
            // Try extracting embedded ID3 picture
            if (tagFile.Tag.Pictures != null && tagFile.Tag.Pictures.Length > 0)
            {
                try
                {
                    var pic = tagFile.Tag.Pictures[0];
                    byte[] imageData = pic.Data.Data;
                    BitmapImage? image = null;

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        using var mem = new MemoryStream(imageData);
                        image = new BitmapImage();
                        image.BeginInit();
                        image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                        image.CacheOption = BitmapCacheOption.OnLoad;
                        image.StreamSource = mem;
                        image.EndInit();
                        image.Freeze();
                    });

                    return image;
                }
                catch
                {
                    // Intentional swallow: Fall back to checking folder for images
                }
            }

            // Try extracting from the local directory
            try
            {
                string? directoryPath = Path.GetDirectoryName(tagFile.Name);
                if (!string.IsNullOrEmpty(directoryPath))
                {
                    string[] possibleNames = ["folder.jpg", "cover.jpg", "folder.png", "cover.png"];

                    foreach (var name in possibleNames)
                    {
                        string imagePath = Path.Combine(directoryPath, name);
                        if (File.Exists(imagePath))
                        {
                            BitmapImage? folderImage = null;
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                folderImage = new BitmapImage();
                                folderImage.BeginInit();
                                folderImage.CacheOption = BitmapCacheOption.OnLoad;
                                folderImage.UriSource = new Uri(imagePath);
                                folderImage.EndInit();
                                folderImage.Freeze();
                            });

                            return folderImage;
                        }
                    }
                }
            }
            catch
            {
                // Intentional swallow: Return null if file access fails
            }

            return null;
        }

        #endregion
    }
}