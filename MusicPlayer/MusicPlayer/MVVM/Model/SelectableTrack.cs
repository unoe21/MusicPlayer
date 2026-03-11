using CommunityToolkit.Mvvm.ComponentModel;

namespace MusicPlayer.MVVM.Model
{
    public partial class SelectableTrack : ObservableObject
    {
        public Track Track { get; set; }

        [ObservableProperty]
        private bool _isSelected;
    }
}