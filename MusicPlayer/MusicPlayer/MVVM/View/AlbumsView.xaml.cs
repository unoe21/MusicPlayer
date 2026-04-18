using System.Windows.Controls;

namespace MusicPlayer.MVVM.View
{
    /// <summary>
    /// Interaction logic for AlbumsView.xaml.
    /// Following the strict MVVM pattern, this code-behind remains devoid of business logic,
    /// relying entirely on data binding via the ViewModel.
    /// </summary>
    public partial class AlbumsView : UserControl
    {
        public AlbumsView()
        {
            InitializeComponent();
        }
    }
}