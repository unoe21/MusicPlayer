using MusicPlayer.MVVM.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows;

namespace MusicPlayer.MVVM.Services
{
    public class LocalMediaLibraryService : IMediaLibraryService
    {
        // Támogatott kiterjesztések listája (itt jön be a FLAC és WAV támogatás!)
        private readonly string[] _supportedExtensions = { ".mp3", ".flac", ".wav", ".m4a", ".aac" };

        public async Task<IEnumerable<Album>> LoadAlbumsAsync(string folderPath)
        {
            var albums = new List<Album>();

            if (!Directory.Exists(folderPath))
                return albums;

            await Task.Run(() =>
            {
                var albumDirs = Directory.GetDirectories(folderPath, "*", SearchOption.AllDirectories);
                // Hozzáadjuk a gyökérmappát is, hátha ott is vannak zenék
                var allDirs = new List<string>(albumDirs) { folderPath };

                foreach (var dir in allDirs)
                {
                    // Megkeressük az összes támogatott fájlt az adott mappában
                    var files = Directory.GetFiles(dir)
                        .Where(file => _supportedExtensions.Contains(Path.GetExtension(file).ToLower()))
                        .ToList();

                    if (files.Count == 0) continue;

                    var firstFile = TagLib.File.Create(files[0]);
                    var album = new Album
                    {
                        Name = firstFile.Tag.Album ?? Path.GetFileName(dir),
                        Artist = firstFile.Tag.FirstPerformer ?? "Unknown Artist",
                        CoverImage = GetCoverImage(firstFile)
                    };

                    foreach (var file in files)
                    {
                        var tagFile = TagLib.File.Create(file);
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

        private BitmapImage GetCoverImage(TagLib.File tagFile)
        {
            // 1. LÉPÉS: Próbáljuk meg kiolvasni a fájlba beágyazott képet
            if (tagFile.Tag.Pictures != null && tagFile.Tag.Pictures.Length > 0)
            {
                try
                {
                    var pic = tagFile.Tag.Pictures[0];
                    byte[] imageData = pic.Data.Data;
                    BitmapImage image = null;

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        using (var mem = new MemoryStream(imageData))
                        {
                            image = new BitmapImage();
                            image.BeginInit();
                            image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                            image.CacheOption = BitmapCacheOption.OnLoad;
                            image.StreamSource = mem;
                            image.EndInit();
                            image.Freeze();
                        }
                    });
                    return image;
                }
                catch { } // Ha hiba van, megyünk a 2. lépésre
            }

            // 2. LÉPÉS: Ha nincs beágyazott kép, nézzünk szét a zene mappájában!
            try
            {
                string directoryPath = Path.GetDirectoryName(tagFile.Name);
                if (!string.IsNullOrEmpty(directoryPath))
                {
                    // A leggyakoribb borítókép nevek, amiket a Windows és a letöltők használnak
                    string[] possibleNames = { "folder.jpg", "cover.jpg", "folder.png", "cover.png"};

                    foreach (var name in possibleNames)
                    {
                        string imagePath = Path.Combine(directoryPath, name);
                        if (File.Exists(imagePath))
                        {
                            BitmapImage folderImage = null;
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                folderImage = new BitmapImage();
                                folderImage.BeginInit();
                                folderImage.CacheOption = BitmapCacheOption.OnLoad;
                                folderImage.UriSource = new Uri(imagePath); // Betöltjük a külső fájlt
                                folderImage.EndInit();
                                folderImage.Freeze();
                            });
                            return folderImage;
                        }
                    }
                }
            }
            catch { }

            // 3. Ha végképp nincs sehol semmi, üresen hagyjuk
            return null;
        }
    }
}