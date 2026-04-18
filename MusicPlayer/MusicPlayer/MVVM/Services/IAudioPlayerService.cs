using System;

namespace MusicPlayer.MVVM.Services
{
    /// <summary>
    /// Defines the contract for the application's audio playback engine.
    /// Abstracts the underlying media player implementation away from the ViewModels,
    /// allowing for easy unit testing and future engine replacements.
    /// </summary>
    public interface IAudioPlayerService
    {
        /// <summary>
        /// Loads and starts playing the audio file from the specified path.
        /// </summary>
        /// <param name="filePath">The absolute file path to the audio track.</param>
        void Play(string filePath);

        /// <summary>
        /// Pauses the currently playing audio track.
        /// Retains the current playback position.
        /// </summary>
        void Pause();

        /// <summary>
        /// Resumes playback of a paused audio track from its current position.
        /// </summary>
        void Resume();

        /// <summary>
        /// Stops playback entirely and resets the audio engine's position to zero.
        /// </summary>
        void Stop();

        /// <summary>
        /// Seeks to a specific time position within the currently loaded track.
        /// </summary>
        /// <param name="seconds">The target playback position in seconds.</param>
        void SetPosition(double seconds);

        /// <summary>
        /// Adjusts the master volume of the audio engine.
        /// </summary>
        /// <param name="volume">The desired volume level, typically represented as a value between 0.0 (mute) and 1.0 (max).</param>
        void SetVolume(double volume);

        /// <summary>
        /// Gets a value indicating whether the audio engine is currently playing a track.
        /// </summary>
        bool IsPlaying { get; }

        /// <summary>
        /// Gets the current playback position of the track in seconds.
        /// Returns 0 if no track is loaded.
        /// </summary>
        double CurrentPosition { get; }

        /// <summary>
        /// Gets the total duration of the currently loaded track in seconds.
        /// Returns 0 if no track is loaded.
        /// </summary>
        double TotalDuration { get; }

        /// <summary>
        /// Occurs when the currently loaded track finishes playing naturally (reaches the end).
        /// Used to trigger features like auto-playing the next track in a playlist.
        /// </summary>
        event Action PlaybackEnded;
    }
}