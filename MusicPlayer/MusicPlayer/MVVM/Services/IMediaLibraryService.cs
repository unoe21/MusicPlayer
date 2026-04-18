using System.Collections.Generic;
using System.Threading.Tasks;
using MusicPlayer.MVVM.Model;

namespace MusicPlayer.MVVM.Services
{
    /// <summary>
    /// Defines the contract for scanning and loading the user's local music library.
    /// Abstracts file system operations and metadata extraction away from the ViewModels.
    /// </summary>
    public interface IMediaLibraryService
    {
        /// <summary>
        /// Asynchronously scans the specified directory and its subdirectories for supported audio files, 
        /// extracts their ID3 tags (metadata), and organizes them into a collection of albums.
        /// </summary>
        /// <param name="folderPath">The absolute path to the root directory containing the music files.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. 
        /// The task result contains an enumerable collection of <see cref="Album"/> objects.
        /// </returns>
        Task<IEnumerable<Album>> LoadAlbumsAsync(string folderPath);
    }
}