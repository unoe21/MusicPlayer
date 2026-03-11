using System;

namespace MusicPlayer.MVVM.Services
{
    public interface IAudioPlayerService
    {
        void Play(string filePath);
        void Pause();
        void Resume();
        void Stop();
        void SetPosition(double seconds);
        void SetVolume(double volume); // <--- EZT ADTUK HOZZÁ

        bool IsPlaying { get; }
        double CurrentPosition { get; }
        double TotalDuration { get; }

        event Action PlaybackEnded;
    }
}