using System.Windows.Controls;

namespace MusicPlayer.MVVM.View
{
    public partial class HomeView : UserControl
    {
        public HomeView()
        {
            InitializeComponent();
        }

        // Ezt a metódust kereste a XAML!
        private void Track_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Egyelőre üresen hagyjuk, a lényeg, hogy a XAML megnyugodjon és leforduljon.
            // Később ide beköthetjük, hogy kattintásra egyből induljon is a zene!
        }
    }
}