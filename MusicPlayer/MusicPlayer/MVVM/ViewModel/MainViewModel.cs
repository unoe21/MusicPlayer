using MusicPlayer.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MusicPlayer.MVVM.View;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using Microsoft.Win32;
using MahApps.Metro.IconPacks;
using MusicPlayer.MVVM.ViewModel;
using System.Windows.Threading;


namespace MusicPlayer.MVVM.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel() { }  
        public MainViewModel(IOpenFileDialogService fileDialogService)
        {

            InitializeCommands();
            _fileDialogService = fileDialogService;


            LoadAlbums();
            LoadAllTracks();
            LoadDaily();


            mediaPlayer.MediaOpened += MediaPlayer_MediaOpened;

            _timer.Interval = TimeSpan.FromMilliseconds(500);
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void InitializeCommands()
        {
            PlayPauseCommand = new RelayCommand(PlayPause);
            NextTrackCommand = new RelayCommand(NextTrack);
            PreviousTrackCommand = new RelayCommand(PlayPreviousTrack);
            OpenFileCommand = new RelayCommand(OpenFile);
        }


        //Albums
        public ObservableCollection<Album> Albums { get; set; } = new ObservableCollection<Album>();
        public ObservableCollection<Track> AllTracks { get; set; } = new ObservableCollection<Track>();
        public ObservableCollection<Track> DailyAlbum { get; set; } = new ObservableCollection<Track>();
        public ObservableCollection<BitmapImage> AlbumCovers =>
            new ObservableCollection<BitmapImage>(Albums.Select(a => a.Cover));
        public ObservableCollection<string> Artists =>
            new ObservableCollection<string>(Albums.Select(a => a.Artist));

        private string _dailyArtist;
        public string DailyArtist
        {
            get => _dailyArtist;
            set
            {
                if (_dailyArtist != value)
                {
                    _dailyArtist = value;
                    OnPropertyChanged(nameof(DailyArtist));
                }
            }
        }
        public string DailyAlbumName { get; set; }
        private Album _selectedAlbum;
        public Album SelectedAlbum
        {
            get => _selectedAlbum;
            set
            {
                _selectedAlbum = value;
                OnPropertyChanged();
            }
        }


        //C:Users/Your user name/Music
        string musicFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
        //Loading Albums
        public void LoadAlbums()
        {
            if (!Directory.Exists(musicFolder))
            {
                MessageBox.Show($"A {musicFolder} mappa nem található.");
                return;
            }

            Albums.Clear();

            var albumDirs = Directory.GetDirectories(musicFolder,"*");

            if (albumDirs.Length == 0)
            {
                MessageBox.Show("Nincsenek albumok a mappában.");
                return;
            }

            foreach (var albumDir in albumDirs)
            {
                var mp3Files = Directory.GetFiles(albumDir, "*.mp3");
                if (mp3Files.Length == 0) continue;

                var firstFile = TagLib.File.Create(mp3Files[0]);
                var album = new Album
                {
                    Name = firstFile.Tag.Album ?? Path.GetFileName(albumDir),
                    Artist = firstFile.Tag.FirstPerformer ?? "Unknown Artist",
                    Cover = GetCoverImage(firstFile)
                };

                foreach (var file in mp3Files)
                {
                    var tagFile = TagLib.File.Create(file);
                    album.Tracks.Add(new Track
                    {
                        Title = tagFile.Tag.Title ?? Path.GetFileNameWithoutExtension(file),
                        FilePath = file,
                        Duration = tagFile.Properties.Duration
                    });
                }

                Albums.Add(album);
            }

        }
        public void LoadAllTracks()
        {
            AllTracks.Clear(); // Az előző trackek törlése

            foreach (var album in Albums)
            {
                foreach (var track in album.Tracks)
                {
                    AllTracks.Add(track); // Az összes album számát hozzáadjuk
                }
            }
        }
        public void LoadDaily()
        {

            if (Albums.Count != 0)
            {
                Random random = new Random();
                int randomIndex = random.Next(Albums.Count);  // Véletlenszerű index kiválasztása

                Album daily = Albums[randomIndex]; // Visszaadjuk a véletlenszerűen kiválasztott albumot
                DailyAlbum = daily.Tracks;
                DailyArtist = daily.Artist;
                DailyAlbumName = daily.Name;
                SelectedAlbum = daily;
            }

        }

        //Loading Files
        private readonly IOpenFileDialogService _fileDialogService;
        public ICommand OpenFileCommand { get; private set; }
        private string currentFileName;

        private string _artist;
        private string _title;

        public string Artist
        {
            get => _artist;
            set
            {
                if (_artist != value)
                {
                    _artist = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Title
        {
            get => _title;
            set
            {
                if (_title != value)
                {
                    _title = value;
                    OnPropertyChanged();
                }
            }
        }

        private void OpenFile()
        {
            OpenFileDialog fileDialog = new OpenFileDialog
            {
                Multiselect = false,
                DefaultExt = ".mp3"
            };

            bool? dialogOk = fileDialog.ShowDialog();
            if (dialogOk == true)
            {
                mediaPlayer.Close();
                var filename = fileDialog.FileName;
                mediaPlayer.Open(new Uri(filename));

                var tagFile = TagLib.File.Create(filename);
                var artist = tagFile.Tag.FirstPerformer ?? "Unknown Artist";
                var title = tagFile.Tag.Title ?? Path.GetFileNameWithoutExtension(filename);

                // Beállítjuk a megfelelő adatokat a ViewModel-ben
                Artist = artist;
                Title = title;
                CoverImage = GetCoverImage(tagFile); // Borítókép beállítása

                // Lejátszás
                mediaPlayer.Play();

                OnPropertyChanged(nameof(CurrentTrack));
                OnPropertyChanged(nameof(CoverImage));
            }
        }




        public MediaPlayer mediaPlayer = new MediaPlayer(); // MediaPlayer példány a lejátszáshoz
        private Track _currentTrack;
        private int _currentTrackIndex = -1; // Az aktuális szám indexe
        public Track CurrentTrack
        {
            get => _currentTrack;
            set
            {
                if (_currentTrack != value)
                {
                    _currentTrack = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _isPlaying;
        public bool IsPlaying
        {
            get => _isPlaying;
            set
            {
                _isPlaying = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(PlayPauseIconKind));
            }
        }
        public string PlayPauseIconKind => IsPlaying ? "Pause" : "Play";


        // Commands
        private ICommand _playPauseCommand;
        public ICommand PlayPauseCommand
        {
            get
            {
                if (_playPauseCommand == null)
                {
                    _playPauseCommand = new RelayCommand(PlayPause);
                }
                return _playPauseCommand;
            }
            private set
            {
            }
        }
        private ICommand _nextTrackCommand;
        public ICommand NextTrackCommand
        {
            get
            {
                if (_nextTrackCommand == null)
                {
                    _nextTrackCommand = new RelayCommand(NextTrack);
                }
                return _nextTrackCommand;
            }
            private set
            {
            }
        }
        public ICommand PreviousTrackCommand { get; private set; }
        public void PlayPause()
        {
            if (_isPlaying)
            {
                mediaPlayer.Pause();
                _isPlaying = false;
                _timer.Stop();
                OnPropertyChanged(nameof(IsPlaying));
                OnPropertyChanged(nameof(PlayPauseIconKind));
            }
            else
            {


                // Ha nincs fájl betöltve, akkor betöltjük az aktuális számot
                if (mediaPlayer.Source == null)
                {
                    mediaPlayer.Open(new Uri(CurrentTrack.FilePath)); // Az aktuális track fájlja
                }
                mediaPlayer.Play();
                _isPlaying = true;
                _timer.Start();
                OnPropertyChanged(nameof(IsPlaying));
                OnPropertyChanged(nameof(PlayPauseIconKind));
            }
        }
        public void NextTrack()
        {
            if (_currentTrackIndex + 1 < SelectedAlbum.Tracks.Count)
            {
                _currentTrackIndex++;
                CurrentTrack = SelectedAlbum.Tracks[_currentTrackIndex];

                mediaPlayer.Open(new Uri(CurrentTrack.FilePath));

                var tagFile = TagLib.File.Create(CurrentTrack.FilePath);
                Artist = tagFile.Tag.FirstPerformer ?? "Unknown Artist";
                Title = CurrentTrack.Title;
                CoverImage = GetCoverImage(tagFile);

                mediaPlayer.Play();
                _isPlaying = true;
                OnPropertyChanged(nameof(CurrentTrack));
            }
        }
        public void PlayPreviousTrack()
        {
            if (_currentTrackIndex > 0)
            {
                _currentTrackIndex--;
                CurrentTrack = SelectedAlbum.Tracks[_currentTrackIndex];
                mediaPlayer.Open(new Uri(CurrentTrack.FilePath));

                var tagFile = TagLib.File.Create(CurrentTrack.FilePath);
                Artist = tagFile.Tag.FirstPerformer ?? "Unknown Artist";
                Title = CurrentTrack.Title;
                CoverImage = GetCoverImage(tagFile);

                mediaPlayer.Play();

                _isPlaying = true;
                OnPropertyChanged(nameof(CurrentTrack));
            }
        }

        //Image creator
        private BitmapImage _coverImage;
        public BitmapImage CoverImage
        {
            get { return _coverImage; }
            set
            {
                _coverImage = value;
                OnPropertyChanged(nameof(CoverImage)); // PropertyChanged hívása
            }
        }
        public void LoadCoverImage(string filePath)
        {
            var tagFile = TagLib.File.Create(filePath); // Betöltjük a fájlt a TagLib segítségével
            CoverImage = GetCoverImage(tagFile); // A CoverImage beállítása a kinyert borítóképpel
        }
        private BitmapImage GetCoverImage(TagLib.File tagFile)
        {
            if (tagFile.Tag.Pictures == null || tagFile.Tag.Pictures.Length == 0)
                return null;

            try
            {
                var pic = tagFile.Tag.Pictures[0];
                using var ms = new MemoryStream(pic.Data.Data);

                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = ms;
                image.EndInit();
                image.Freeze(); // Fontos UI-hoz

                return image;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Borítókép betöltési hiba: {ex.Message}");
                return null;
            }
        }

        //Silder
        private DispatcherTimer _timer = new DispatcherTimer();

        private double _currentPositionSeconds;
        public double CurrentPositionSeconds
        {
            get => _currentPositionSeconds;
            set
            {
                if (Math.Abs(value - _currentPositionSeconds) > 0.1) // Csak ha tényleg változott
                {
                    _currentPositionSeconds = value;
                    mediaPlayer.Position = TimeSpan.FromSeconds(value); // Csak akkor állítsuk be, ha felhasználó állítja
                    OnPropertyChanged();
                }
            }
        }

        private double _currentTrackDurationSeconds;
        public double CurrentTrackDurationSeconds
        {
            get => _currentTrackDurationSeconds;
            set
            {
                _currentTrackDurationSeconds = value;
                OnPropertyChanged();
            }
        }

        private bool _isUserDraggingSlider;
        public bool IsUserDraggingSlider
        {
            get => _isUserDraggingSlider;
            set
            {
                _isUserDraggingSlider = value;
                OnPropertyChanged();
            }
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            // Csak akkor frissítsük a pozíciót, ha nem húzzuk a Slider-t
            if (!_isUserDraggingSlider)
            {
                if (mediaPlayer.Source != null && mediaPlayer.NaturalDuration.HasTimeSpan)
                {
                    CurrentPositionSeconds = mediaPlayer.Position.TotalSeconds;
                }
            }
        }
        private void MediaPlayer_MediaOpened(object sender, EventArgs e)
        {
            if (mediaPlayer.NaturalDuration.HasTimeSpan)
            {
                CurrentTrackDurationSeconds = mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                CurrentPositionSeconds = 0; // Itt indul a slider az elején
                mediaPlayer.Position = TimeSpan.Zero;
            }
        }

        //View Navigator
        private UserControl _currentView;
        public UserControl CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged(nameof(CurrentView));
            }
        }
        private string _selectedMenu;
        public string SelectedMenu
        {
            get => _selectedMenu;
            set
            {
                if (_selectedMenu != value)
                {
                    _selectedMenu = value;
                    OnPropertyChanged(nameof(SelectedMenu));
                    NavigateTo(_selectedMenu); // Navigálás az új menüre
                }
            }
        }
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
                    CurrentView = new SongsView();
                    break;


            }
        }
        public void SeekTo(double seconds)
        {
            mediaPlayer.Position = TimeSpan.FromSeconds(seconds);
        }

        // INotifyPropertyChanged implementáció
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }


}