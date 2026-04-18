using System.Windows.Controls;

namespace MusicPlayer.MVVM.View
{
    /// <summary>
    /// Interaction logic for SongsView.xaml.
    /// 
    /// Clean Code Note: 
    /// Selection handling is managed via DataBinding in the ViewModel.
    /// This keeps the code-behind clean and ensures the UI remains logic-free.
    /// </summary>
    public partial class SongsView : UserControl
    {
        public SongsView()
        {
            InitializeComponent();
        }
    }
}