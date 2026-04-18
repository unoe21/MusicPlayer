using CommunityToolkit.Mvvm.ComponentModel;

namespace MusicPlayer.MVVM.Model
{
    /// <summary>
    /// A UI-specific wrapper class for a <see cref="Model.Track"/>.
    /// Used in lists where the user needs to select/deselect tracks (e.g., creating a playlist) 
    /// without polluting the core Track data model with UI state logic.
    /// </summary>
    public partial class SelectableTrack : ObservableObject
    {
        /// <summary>
        /// Gets the underlying track data.
        /// The 'required' and 'init' modifiers ensure that a SelectableTrack cannot be created 
        /// without a valid Track, and the Track cannot be swapped out after initialization.
        /// </summary>
        public required Track Track { get; init; }

        /// <summary>
        /// Gets or sets a value indicating whether this track is currently selected in the UI.
        /// Generates the 'IsSelected' property and notifies the UI upon changes.
        /// </summary>
        [ObservableProperty]
        private bool _isSelected;
    }
}