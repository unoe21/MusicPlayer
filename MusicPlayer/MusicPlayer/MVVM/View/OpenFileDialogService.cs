using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.MVVM.View
{
    public interface IOpenFileDialogService
    {
        string OpenFile();
    }

    class OpenFileDialogService : IOpenFileDialogService
    {
        public string OpenFile()
        {
            var dialog = new OpenFileDialog
            {
                Multiselect = false,
                DefaultExt = ".mp3",
                Filter = "MP3 files (*.mp3)|*.mp3"
            };

            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                return dialog.FileName;
            }

            return null;
        }
    }
}
