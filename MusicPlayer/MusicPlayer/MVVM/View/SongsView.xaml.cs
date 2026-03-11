using System.Windows.Controls;

namespace MusicPlayer.MVVM.View
{
    public partial class SongsView : UserControl
    {
        public SongsView()
        {
            InitializeComponent();
        }

        // Ezt a metódust kereste a XAML!
        private void Track_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Egyelőre üresen hagyjuk, hogy leforduljon a kód.
        }
    }
}