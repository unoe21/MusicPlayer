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
                Title = "Zene kiválasztása",
                // JAVÍTVA: Nincs '|' a legvégén!
                Filter = "Audio fájlok (*.mp3, *.flac, *.wav)|*.mp3;*.flac;*.wav",
                FilterIndex = 1
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
