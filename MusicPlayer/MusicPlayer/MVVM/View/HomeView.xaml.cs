using System.Windows.Controls;

namespace MusicPlayer.MVVM.View
{
    /// <summary>
    /// Interaction logic for HomeView.xaml.
    /// 
    /// Clean Code Note: 
    /// Selection handling is managed via DataBinding in the ViewModel (CurrentTrack property).
    /// This keeps the View entirely focused on visuals, making the application 
    /// easier to maintain and test.
    /// </summary>
    public partial class HomeView : UserControl
    {
        public HomeView()
        {
            InitializeComponent();
        }
    }
}