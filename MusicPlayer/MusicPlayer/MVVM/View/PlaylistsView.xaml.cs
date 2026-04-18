using System.Windows.Controls;

namespace MusicPlayer.MVVM.View
{
    /// <summary>
    /// Interaction logic for PlaylistsView.xaml.
    /// In line with the MVVM pattern, this file contains no business logic.
    /// All playlist operations (creation, selection, display) are handled via DataBinding
    /// through the MainViewModel.
    /// </summary>
    public partial class PlaylistsView : UserControl
    {
        public PlaylistsView()
        {
            InitializeComponent();
        }
    }
}