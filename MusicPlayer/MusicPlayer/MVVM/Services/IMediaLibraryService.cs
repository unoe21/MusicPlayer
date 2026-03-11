using MusicPlayer.MVVM.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MusicPlayer.MVVM.Services
{
    public interface IMediaLibraryService
    {
        Task<IEnumerable<Album>> LoadAlbumsAsync(string folderPath);
    }
}