using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace MusicPlayer.MVVM.Model
{
    public class Album
    {
        public string Name { get; set; }              
        public string Artist { get; set; }             
        public ObservableCollection<Track> Tracks { get; set; } = new();
        public BitmapImage Cover { get; set; }      
    }
}
