using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using MusicPlayer.MVVM.Model;

namespace MusicPlayer.MVVM.Services
{
    /// <summary>
    /// Implements the media library service by scanning local storage directories.
    /// Utilizes TagLib-Sharp to extract ID3 tags, FLAC metadata, and embedded cover art.
    /// </summary>
    public class LocalMediaLibraryService : IMediaLibraryService
    {
        /// <summary>
        /// A highly optimized, case-insensitive collection of supported audio file extensions.
        /// </summary>
        private static readonly HashSet<string> SupportedExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".mp3", ".flac", ".wav", ".m4a", ".aac"
        };

        /// <inheritdoc />
        public async Task<IEnumerable<Album>> LoadAlbumsAsync(string folderPath)
        {
            var albums = new List<Album>();

            if (!Directory.Exists(folderPath))
                return albums;

            await Task.Run(() =>
            {
                var albumDirs = Directory.GetDirectories(folderPath, "*", SearchOption.AllDirectories);

                var allDirs = new List<string>(albumDirs) { folderPath };

                foreach (var dir in allDirs)
                {
                    var files = Directory.EnumerateFiles(dir)
                        .Where(file => SupportedExtensions.Contains(Path.GetExtension(file)))
                        .ToList();

                    if (files.Count == 0) continue;

                    var album = new Album();

                    using (var firstFile = TagLib.File.Create(files[0]))
                    {
                        album.Name = firstFile.Tag.Album ?? Path.GetFileName(dir);
                        album.Artist = firstFile.Tag.FirstPerformer ?? "Unknown Artist";
                        album.CoverImage = GetCoverImage(firstFile);
                    }

                    foreach (var file in files)
                    {
                        using var tagFile = TagLib.File.Create(file);

                        album.Tracks.Add(new Track
                        {
                            Title = tagFile.Tag.Title ?? Path.GetFileNameWithoutExtension(file),
                            FilePath = file,
                            Duration = tagFile.Properties.Duration
                        });
                    }

                    albums.Add(album);
                }
            });

            return albums;
        }

        /// <summary>
        /// Attempts to extract the cover art for an album. It checks for embedded ID3 pictures first,
        /// and falls back to looking for common cover image files in the directory.
        /// </summary>
        /// <param name="tagFile">The TagLib file instance containing the metadata.</param>
        /// <returns>A frozen BitmapImage safe for cross-thread UI binding, or null if no art is found.</returns>
        private BitmapImage? GetCoverImage(TagLib.File tagFile)
        {
            if (tagFile.Tag.Pictures != null && tagFile.Tag.Pictures.Length > 0)
            {
                try
                {
                    var pic = tagFile.Tag.Pictures[0];
                    byte[] imageData = pic.Data.Data;
                    BitmapImage? image = null;

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        using var mem = new MemoryStream(imageData);
                        image = new BitmapImage();
                        image.BeginInit();
                        image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                        image.CacheOption = BitmapCacheOption.OnLoad;
                        image.StreamSource = mem;
                        image.EndInit();
                        image.Freeze();
                    });

                    return image;
                }
                catch
                {
                    // Intentional swallow: If embedded extraction fails, fall through to local folder search
                }
            }

            try
            {
                string? directoryPath = Path.GetDirectoryName(tagFile.Name);
                if (!string.IsNullOrEmpty(directoryPath))
                {
                    string[] possibleNames = ["folder.jpg", "cover.jpg", "folder.png", "cover.png"];

                    foreach (var name in possibleNames)
                    {
                        string imagePath = Path.Combine(directoryPath, name);
                        if (File.Exists(imagePath))
                        {
                            BitmapImage? folderImage = null;
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                folderImage = new BitmapImage();
                                folderImage.BeginInit();
                                folderImage.CacheOption = BitmapCacheOption.OnLoad;
                                folderImage.UriSource = new Uri(imagePath);
                                folderImage.EndInit();
                                folderImage.Freeze();
                            });

                            return folderImage;
                        }
                    }
                }
            }
            catch
            {
                // Intentional swallow: If file system access fails, return null
            }

            return null;
        }
    }
}