using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace MusicPlayer.MVVM.Model
{
    // A 'partial' kulcsszó kötelező, mert a keretrendszer a háttérben kiegészíti ezt az osztályt.
    public partial class Track : ObservableObject
    {
        [ObservableProperty]
        private string _title;

        [ObservableProperty]
        private string _filePath;

        [ObservableProperty]
        private TimeSpan _duration;
    }
}