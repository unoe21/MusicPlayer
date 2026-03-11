using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MusicPlayer.MVVM.Model;
using MusicPlayer.MVVM.Services;
using MusicPlayer.MVVM.View;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace MusicPlayer.MVVM.ViewModel
{
    public partial class MainViewModel : ObservableObject
    {
        // --- 1. MEZŐK ÉS SZOLGÁLTATÁSOK ---

        private readonly IMediaLibraryService _mediaLibraryService;
        private readonly IAudioPlayerService _audioPlayerService;
        private readonly IOpenFileDialogService _fileDialogService;
        private DispatcherTimer _timer = new DispatcherTimer();

        // --- 2. PROPERTY-K (TULAJDONSÁGOK) ---

        [ObservableProperty]
        private object _currentView;

        [ObservableProperty]
        private ObservableCollection<Album> _albums = new();

        [ObservableProperty]
        private ObservableCollection<Track> _allTracks = new();

        [ObservableProperty]
        private ObservableCollection<Track> _dailyAlbum = new();

        [ObservableProperty]
        private ObservableCollection<string> _artists = new();

        [ObservableProperty]
        private ObservableCollection<string> _playlists = new();

        // ÚJ VÁLTOZÓ: Hangerő (Alapértelmezetten 50%-on indul)
        [ObservableProperty]
        private double _volume = 50;

        [ObservableProperty]
        private string _selectedArtist;

        [ObservableProperty]
        private Album _selectedAlbum;

        private Track _currentTrack;
        [ObservableProperty]
        private string _songsTitle = "Songs";
        public Track CurrentTrack
        {
            get => _currentTrack;
            set
            {
                if (SetProperty(ref _currentTrack, value))
                {
                    if (value != null)
                    {
                        if (SelectedAlbum != null && SelectedAlbum.Tracks.Contains(value))
                        {
                            _currentTrackIndex = SelectedAlbum.Tracks.IndexOf(value);
                        }
                        PlaySelectedTrack(value);
                    }
                }
            }
        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(PlayPauseIconKind))]
        private bool _isPlaying;

        public string PlayPauseIconKind => IsPlaying ? "Pause" : "Play";

        [ObservableProperty]
        private double _currentPositionSeconds;

        [ObservableProperty]
        private double _currentTrackDurationSeconds;

        [ObservableProperty]
        private bool _isUserDraggingSlider;

        [ObservableProperty]
        private string _artist;

        [ObservableProperty]
        private string _title;

        [ObservableProperty]
        private BitmapImage _coverImage;

        [ObservableProperty]
        private string _selectedMenu;

        [ObservableProperty]
        private string _dailyArtist;

        [ObservableProperty]
        private string _dailyAlbumName;

        private int _currentTrackIndex = -1;

        // --- 3. KONSTRUKTOR ---

        public MainViewModel(
            IMediaLibraryService mediaLibraryService,
            IAudioPlayerService audioPlayerService,
            IOpenFileDialogService fileDialogService)
        {
            _mediaLibraryService = mediaLibraryService;
            _audioPlayerService = audioPlayerService;
            _fileDialogService = fileDialogService;

            _audioPlayerService.PlaybackEnded += OnPlaybackEnded;

            _timer.Interval = TimeSpan.FromMilliseconds(500);
            _timer.Tick += Timer_Tick;

            _ = InitializeLibraryAsync();

            NavigateTo("Home");
        }

        // --- 4. INICIALIZÁLÁS ÉS ADATBETÖLTÉS ---

        private async Task InitializeLibraryAsync()
        {
            string musicFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);

            var loadedAlbums = await _mediaLibraryService.LoadAlbumsAsync(musicFolder);

            Albums = new ObservableCollection<Album>(loadedAlbums);
            LoadAllTracks();
            LoadDaily();
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

        private void LoadDaily()
        {
            if (Albums.Count != 0)
            {
                Random random = new Random();
                int randomIndex = random.Next(Albums.Count);

                Album daily = Albums[randomIndex];
                DailyAlbum = daily.Tracks;
                DailyArtist = daily.Artist;
                DailyAlbumName = daily.Name;
                SelectedAlbum = daily;
            }
        }

        private void LoadArtists()
        {
            Artists.Clear();

            var uniqueArtists = Albums
                .Select(a => a.Artist)
                .Where(artist => !string.IsNullOrEmpty(artist))
                .Distinct()
                .OrderBy(artist => artist)
                .ToList();

            foreach (var artist in uniqueArtists)
            {
                Artists.Add(artist);
            }
        }

        public void LoadCoverImage(string filePath)
        {
            var tagFile = TagLib.File.Create(filePath);
            CoverImage = GetCoverImage(tagFile);
        }

        private BitmapImage GetCoverImage(TagLib.File tagFile)
        {
            if (tagFile.Tag.Pictures != null && tagFile.Tag.Pictures.Length > 0)
            {
                try
                {
                    var pic = tagFile.Tag.Pictures[0];
                    byte[] imageData = pic.Data.Data;
                    BitmapImage image = null;

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        using (var mem = new MemoryStream(imageData))
                        {
                            image = new BitmapImage();
                            image.BeginInit();
                            image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                            image.CacheOption = BitmapCacheOption.OnLoad;
                            image.StreamSource = mem;
                            image.EndInit();
                            image.Freeze();
                        }
                    });
                    return image;
                }
                catch { }
            }

            try
            {
                string directoryPath = Path.GetDirectoryName(tagFile.Name);
                if (!string.IsNullOrEmpty(directoryPath))
                {
                    string[] possibleNames = { "folder.jpg", "cover.jpg", "folder.png", "cover.png", "AlbumArtSmall.jpg" };

                    foreach (var name in possibleNames)
                    {
                        string imagePath = Path.Combine(directoryPath, name);
                        if (File.Exists(imagePath))
                        {
                            BitmapImage folderImage = null;
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
            catch { }

            return null;
        }



        // --- 5. NAVIGÁCIÓ ---

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
                    LoadAllTracks(); // <--- ÚJ: Visszatölti az összes dalt a memóriából
                    SelectedArtist = null; // <--- ÚJ: Töröljük a korábbi előadó kijelölést
                    SelectedAlbum = null;
                    SongsTitle = "Songs";
                    CurrentView = new SongsView();
                    break;
            }
        }

        partial void OnSelectedMenuChanged(string value)
        {
            NavigateTo(value);
        }



        // --- 6. LEJÁTSZÁSI LOGIKA ÉS PARANCSOK ---

        [RelayCommand]
        private void PlayPause()
        {
            if (_audioPlayerService.IsPlaying)
            {
                _audioPlayerService.Pause();
                IsPlaying = false;
                _timer.Stop();
            }
            else
            {
                if (CurrentTrack != null)
                {
                    if (_audioPlayerService.CurrentPosition == 0)
                    {
                        PlaySelectedTrack(CurrentTrack);
                    }
                    else
                    {
                        _audioPlayerService.Resume();
                        IsPlaying = true;
                        _timer.Start();
                    }
                }
            }
        }

        [RelayCommand]
        private void NextTrack()
        {
            if (SelectedAlbum != null && _currentTrackIndex + 1 < SelectedAlbum.Tracks.Count)
            {
                _currentTrackIndex++;
                CurrentTrack = SelectedAlbum.Tracks[_currentTrackIndex];
            }
        }

        [RelayCommand]
        private void PreviousTrack()
        {
            if (SelectedAlbum != null && _currentTrackIndex > 0)
            {
                _currentTrackIndex--;
                CurrentTrack = SelectedAlbum.Tracks[_currentTrackIndex];
            }
        }

        private void PlaySelectedTrack(Track track)
        {
            _audioPlayerService.Play(track.FilePath);

            // ÚJ: Indításkor beállítjuk a kezdeti hangerőt is a motornak!
            _audioPlayerService.SetVolume(Volume / 100.0);

            var tagFile = TagLib.File.Create(track.FilePath);
            Artist = tagFile.Tag.FirstPerformer ?? "Unknown Artist";
            Title = track.Title;
            CoverImage = GetCoverImage(tagFile);

            CurrentTrackDurationSeconds = _audioPlayerService.TotalDuration;

            IsPlaying = true;
            _timer.Start();
        }

        private void OnPlaybackEnded()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                NextTrack();
            });
        }

        // --- 7. SLIDER ÉS HANGERŐ KEZELÉSE ---

        public void SeekTo(double seconds)
        {
            _audioPlayerService.SetPosition(seconds);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (!IsUserDraggingSlider && _audioPlayerService.IsPlaying)
            {
                CurrentPositionSeconds = _audioPlayerService.CurrentPosition;
            }
        }

        [RelayCommand]
        private void OpenFile()
        {
            // 1. Megnyitjuk a tallózó ablakot
            string filePath = _fileDialogService.OpenFile();

            // 2. Ha a felhasználó kiválasztott egy fájlt (nem nyomott Mégsem-et)
            if (!string.IsNullOrEmpty(filePath))
            {
                // Csinálunk egy "virtuális" zeneszámot a betallózott fájlból
                var newTrack = new Track
                {
                    FilePath = filePath,
                    Title = Path.GetFileNameWithoutExtension(filePath) // Alapból a fájlnév lesz a címe
                };

                // Megpróbáljuk kiolvasni a valódi címét a fájl metaadataiból (TagLib)
                try
                {
                    var tagFile = TagLib.File.Create(filePath);
                    if (!string.IsNullOrEmpty(tagFile.Tag.Title))
                    {
                        newTrack.Title = tagFile.Tag.Title;
                    }
                }
                catch { } // Ha nincs benne metaadat, akkor marad a fájlnév

                // 3. Átadjuk a kész számot a lejátszó logikánknak!
                PlaySelectedTrack(newTrack);
            }
        }

        [RelayCommand]
        private void AddPlaylist()
        {
            int newNumber = _playlists.Count + 1;
            _playlists.Add($"Új lista {newNumber}");
        }

        partial void OnCurrentPositionSecondsChanged(double value)
        {
            if (IsUserDraggingSlider)
            {
                _audioPlayerService.SetPosition(value);
            }
        }

        // ÚJ: Ha húzod a hangerő csúszkát, ez szól a motornak!
        partial void OnVolumeChanged(double value)
        {
            _audioPlayerService.SetVolume(value / 100.0);
        }


        // Ez automatikusan lefut, amikor az Artists nézetben rákattintasz egy előadóra
        partial void OnSelectedArtistChanged(string value)
        {
            if (string.IsNullOrEmpty(value)) return;

            SongsTitle = $"{value} - Songs";
            // 1. Kitöröljük a dalok listáját
            AllTracks.Clear();

            // 2. Megkeressük a kiválasztott előadó albumait, és csak az ő számait töltjük be
            var artistAlbums = Albums.Where(a => a.Artist == value);
            foreach (var album in artistAlbums)
            {
                foreach (var track in album.Tracks)
                {
                    AllTracks.Add(track);
                }
            }

            // 3. Átnavigálunk a Songs (Dalok) nézetre, ami most már csak ezt a szűrt listát fogja mutatni!
            CurrentView = new SongsView();
        }

        // Ez automatikusan lefut, amikor az Albums nézetben rákattintasz egy albumra
        partial void OnSelectedAlbumChanged(Album value)
        {
            if (value == null) return;

            // 1. Átírjuk a címet az album nevére!
            SongsTitle = $"{value.Name} - Songs";

            // 2. Kitöröljük a dalok listáját
            AllTracks.Clear();

            // 3. Csak a kiválasztott album dalait töltjük be
            foreach (var track in value.Tracks)
            {
                AllTracks.Add(track);
            }

            // 4. Átnavigálunk a Songs (Dalok) nézetre
            CurrentView = new SongsView();
        }
    }
}