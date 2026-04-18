using System;
using System.IO;
using System.Windows;
using CommunityToolkit.Mvvm.Input;
using MusicPlayer.MVVM.Model;

namespace MusicPlayer.MVVM.ViewModel
{
    /// <summary>
    /// Partial class handling all media playback logic, timeline dragging, and volume control.
    /// </summary>
    public partial class MainViewModel
    {
        /// <summary>
        /// Toggles between playing and pausing the current track.
        /// If playback is at position 0, it initializes and starts the current track.
        /// </summary>
        [RelayCommand]
        private void PlayPause()
        {
            if (_audioPlayerService.IsPlaying)
            {
                _audioPlayerService.Pause();
                IsPlaying = false;
                _timer.Stop();
            }
            else if (CurrentTrack != null)
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

        /// <summary>
        /// Skips to the next track in the currently selected album.
        /// </summary>
        [RelayCommand]
        private void NextTrack()
        {
            if (SelectedAlbum != null && _currentTrackIndex + 1 < SelectedAlbum.Tracks.Count)
            {
                _currentTrackIndex++;
                CurrentTrack = SelectedAlbum.Tracks[_currentTrackIndex];
            }
        }

        /// <summary>
        /// Returns to the previous track in the currently selected album.
        /// </summary>
        [RelayCommand]
        private void PreviousTrack()
        {
            if (SelectedAlbum != null && _currentTrackIndex > 0)
            {
                _currentTrackIndex--;
                CurrentTrack = SelectedAlbum.Tracks[_currentTrackIndex];
            }
        }

        /// <summary>
        /// Opens a file dialog to allow the user to select and play a local audio file directly.
        /// </summary>
        [RelayCommand]
        private void OpenFile()
        {
            string filePath = _fileDialogService.OpenFile();

            if (!string.IsNullOrEmpty(filePath))
            {
                var newTrack = new Track
                {
                    FilePath = filePath,
                    Title = Path.GetFileNameWithoutExtension(filePath)
                };

                try
                {
                    using var tagFile = TagLib.File.Create(filePath);
                    if (!string.IsNullOrWhiteSpace(tagFile.Tag.Title))
                    {
                        newTrack.Title = tagFile.Tag.Title;
                    }
                }
                catch
                {
                    // Intentional swallow: Fallback to the raw filename if ID3 tags are missing or corrupted 
                }

                PlaySelectedTrack(newTrack);
            }
        }

        /// <summary>
        /// Loads a track into the audio service, extracts its metadata/cover art, and begins playback.
        /// </summary>
        private void PlaySelectedTrack(Track track)
        {
            _audioPlayerService.Play(track.FilePath);
            _audioPlayerService.SetVolume(Volume / 100.0);

            try
            {
                using var tagFile = TagLib.File.Create(track.FilePath);
                Artist = tagFile.Tag.FirstPerformer ?? "Unknown Artist";
                Title = track.Title;
                CoverImage = GetCoverImage(tagFile);
            }
            catch
            {
                // Intentional swallow: Provide default values if metadata extraction fails
                Artist = "Unknown Artist";
                Title = track.Title;
                CoverImage = null;
            }

            CurrentTrackDurationSeconds = _audioPlayerService.TotalDuration;
            IsPlaying = true;
            _timer.Start();
        }

        /// <summary>
        /// Event handler triggered when a track naturally finishes playing.
        /// </summary>
        private void OnPlaybackEnded()
        {
            // Must be invoked on the UI dispatcher since it modifies observable collections and properties
            Application.Current.Dispatcher.Invoke(NextTrack);
        }

        /// <summary>
        /// Automatically formats the current playback position into a readable timestamp (m:ss).
        /// </summary>
        partial void OnCurrentPositionSecondsChanged(double value)
        {
            TimeSpan time = TimeSpan.FromSeconds(value);
            CurrentTimeString = time.ToString(@"m\:ss");

            if (IsUserDraggingSlider)
            {
                _audioPlayerService.SetPosition(value);
            }
        }

        /// <summary>
        /// Automatically formats the total track duration into a readable timestamp (m:ss).
        /// </summary>
        partial void OnCurrentTrackDurationSecondsChanged(double value)
        {
            TimeSpan time = TimeSpan.FromSeconds(value);
            TotalTimeString = time.ToString(@"m\:ss");
        }

        /// <summary>
        /// Automatically updates the audio engine volume when the user changes the volume slider.
        /// </summary>
        partial void OnVolumeChanged(double value)
        {
            _audioPlayerService.SetVolume(value / 100.0);
        }

        /// <summary>
        /// Periodic timer tick to update the UI timeline slider based on the actual audio engine position.
        /// </summary>
        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (!IsUserDraggingSlider && _audioPlayerService.IsPlaying)
            {
                CurrentPositionSeconds = _audioPlayerService.CurrentPosition;
            }
        }

        /// <summary>
        /// Invoked by the UI when the user clicks and holds the timeline slider.
        /// Pauses the audio to prevent stuttering during the drag operation.
        /// </summary>
        public void UserStartedDragging()
        {
            IsUserDraggingSlider = true;
            _wasPlayingBeforeDrag = IsPlaying;

            if (IsPlaying)
            {
                _audioPlayerService.Pause();
                _timer.Stop();
            }
        }

        /// <summary>
        /// Invoked by the UI when the user releases the timeline slider.
        /// Resumes audio playback from the new position.
        /// </summary>
        public void UserFinishedDragging()
        {
            IsUserDraggingSlider = false;
            _audioPlayerService.SetPosition(CurrentPositionSeconds);

            if (_wasPlayingBeforeDrag)
            {
                _audioPlayerService.Resume();
                _timer.Start();
            }
        }
    }
}