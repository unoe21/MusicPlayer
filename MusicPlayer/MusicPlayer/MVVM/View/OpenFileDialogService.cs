using Microsoft.Win32;

namespace MusicPlayer.MVVM.Services
{
    public interface IOpenFileDialogService
    {
        string? OpenFile();
    }

    /// <summary>
    /// Windows-specific implementation of the file dialog service.
    /// Uses the standard Microsoft.Win32.OpenFileDialog to interact with the OS.
    /// </summary>
    public class OpenFileDialogService : IOpenFileDialogService
    {
        /// <summary>
        /// Displays an Open File Dialog filtered for supported audio formats.
        /// </summary>
        /// <returns>The path of the selected file, or null if the user cancelled.</returns>
        public string? OpenFile()
        {
            var dialog = new OpenFileDialog
            {
                Multiselect = false,
                Title = "Select Music File",
                // A zenelejátszó által támogatott formátumok szűrése
                Filter = "Audio Files (*.mp3;*.flac;*.wav;*.m4a)|*.mp3;*.flac;*.wav;*.m4a|All Files (*.*)|*.*",
                FilterIndex = 1
            };

            // A ShowDialog() true értéket ad vissza, ha a felhasználó a "Megnyitás" gombra kattint
            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                return dialog.FileName;
            }

            return null;
        }
    }
}