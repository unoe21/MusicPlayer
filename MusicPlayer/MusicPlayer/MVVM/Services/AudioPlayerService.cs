using System;
using NAudio.Wave;

namespace MusicPlayer.MVVM.Services
{
    /// <summary>
    /// Implementation of the audio playback engine using the NAudio library.
    /// Utilizes MediaFoundationReader to support a wide variety of modern audio codecs (MP3, M4A, WAV, FLAC natively on Windows).
    /// </summary>
    public class AudioPlayerService : IAudioPlayerService, IDisposable
    {
        private WaveOutEvent? _outputDevice;
        private MediaFoundationReader? _audioFile;

        private float _currentVolume = 0.5f;
        private bool _isDisposed;

        /// <inheritdoc />
        public event Action? PlaybackEnded;

        /// <inheritdoc />
        public bool IsPlaying => _outputDevice?.PlaybackState == PlaybackState.Playing;

        /// <inheritdoc />
        public double CurrentPosition => _audioFile?.CurrentTime.TotalSeconds ?? 0;

        /// <inheritdoc />
        public double TotalDuration => _audioFile?.TotalTime.TotalSeconds ?? 0;

        /// <inheritdoc />
        public void Play(string filePath)
        {
            DisposeCurrentMedia();

            _audioFile = new MediaFoundationReader(filePath);

            _outputDevice = new WaveOutEvent
            {
                Volume = _currentVolume
            };

            _outputDevice.Init(_audioFile);
            _outputDevice.PlaybackStopped += OnPlaybackStopped;
            _outputDevice.Play();
        }

        /// <inheritdoc />
        public void Pause()
        {
            _outputDevice?.Pause();
        }

        /// <inheritdoc />
        public void Resume()
        {
            if (_outputDevice?.PlaybackState == PlaybackState.Paused)
            {
                _outputDevice.Play();
            }
        }

        /// <inheritdoc />
        public void Stop()
        {
            _outputDevice?.Stop();
            DisposeCurrentMedia();
        }

        /// <inheritdoc />
        public void SetPosition(double seconds)
        {
            if (_audioFile != null)
            {
                _audioFile.CurrentTime = TimeSpan.FromSeconds(seconds);
            }
        }

        /// <inheritdoc />
        public void SetVolume(double volume)
        {
            _currentVolume = Math.Clamp((float)volume, 0f, 1f);

            if (_outputDevice != null)
            {
                _outputDevice.Volume = _currentVolume;
            }
        }

        /// <summary>
        /// Event handler triggered by NAudio when playback stops.
        /// </summary>
        private void OnPlaybackStopped(object? sender, StoppedEventArgs e)
        {

            if (_audioFile != null && _audioFile.Position >= _audioFile.Length)
            {
                PlaybackEnded?.Invoke();
            }
        }

        /// <summary>
        /// Safely unhooks events and disposes of the current audio file and output device.
        /// </summary>
        private void DisposeCurrentMedia()
        {
            if (_outputDevice != null)
            {
                _outputDevice.PlaybackStopped -= OnPlaybackStopped;
                _outputDevice.Dispose();
                _outputDevice = null;
            }

            if (_audioFile != null)
            {
                _audioFile.Dispose();
                _audioFile = null;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing unmanaged resources.
        /// Required to prevent memory leaks from the audio driver when the app closes.
        /// </summary>
        public void Dispose()
        {
            if (!_isDisposed)
            {
                DisposeCurrentMedia();
                _isDisposed = true;
                GC.SuppressFinalize(this);
            }
        }
    }
}