using NAudio.Wave;
using System;

namespace MusicPlayer.MVVM.Services
{
    public class AudioPlayerService : IAudioPlayerService
    {
        private WaveOutEvent _outputDevice;
        private MediaFoundationReader _audioFile;
        private float _currentVolume = 0.5f; // <--- ÚJ: Eltároljuk a hangerőt, alapértelmezetten 50%

        public event Action PlaybackEnded;

        public bool IsPlaying => _outputDevice?.PlaybackState == PlaybackState.Playing;

        public double CurrentPosition => _audioFile?.CurrentTime.TotalSeconds ?? 0;

        public double TotalDuration => _audioFile?.TotalTime.TotalSeconds ?? 0;

        public void Play(string filePath)
        {
            CleanUp();

            // A MediaFoundationReader a modern Windows beépített kodekjeit használja,
            // így alapból viszi az mp3, m4a, wav, és a Windows 10/11-en a FLAC fájlokat is!
            _audioFile = new MediaFoundationReader(filePath);
            _outputDevice = new WaveOutEvent();

            // ÚJ: Átadjuk az elmentett hangerőt az új lejátszónak!
            _outputDevice.Volume = _currentVolume;

            _outputDevice.Init(_audioFile);
            _outputDevice.PlaybackStopped += OnPlaybackStopped;
            _outputDevice.Play();
        }

        public void Pause()
        {
            _outputDevice?.Pause();
        }

        public void Resume()
        {
            if (_outputDevice?.PlaybackState == PlaybackState.Paused)
            {
                _outputDevice.Play();
            }
        }

        public void Stop()
        {
            _outputDevice?.Stop();
            CleanUp();
        }

        public void SetPosition(double seconds)
        {
            if (_audioFile != null)
            {
                _audioFile.CurrentTime = TimeSpan.FromSeconds(seconds);
            }
        }

        // <--- ÚJ METÓDUS: Hangerő beállítása
        public void SetVolume(double volume)
        {
            // A dupla (double) értéket átalakítjuk lebegőpontossá (float), amit a NAudio vár.
            // Biztosítjuk, hogy az érték szigorúan 0.0 és 1.0 között maradjon.
            _currentVolume = (float)Math.Max(0.0, Math.Min(1.0, volume));

            if (_outputDevice != null)
            {
                _outputDevice.Volume = _currentVolume;
            }
        }

        private void OnPlaybackStopped(object sender, StoppedEventArgs e)
        {
            // Ezt akkor hívja meg a NAudio, ha megállt a lejátszás.
            // Leellenőrizzük, hogy a szám végére értünk-e (nem csak mi nyomtunk Pause/Stop-ot)
            if (_audioFile != null && _audioFile.Position >= _audioFile.Length)
            {
                PlaybackEnded?.Invoke(); // Szólunk a ViewModel-nek, hogy mehet a következő szám
            }
        }

        private void CleanUp()
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
    }
}