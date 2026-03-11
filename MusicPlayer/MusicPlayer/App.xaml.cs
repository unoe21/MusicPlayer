using Microsoft.Extensions.DependencyInjection;
using MusicPlayer.MVVM.Services;
using MusicPlayer.MVVM.ViewModel;
using MusicPlayer.MVVM.View;
using System;
using System.Configuration;
using System.Data;
using System.Windows;

namespace MusicPlayer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IServiceProvider Services { get; }

        public new static App Current => (App)Application.Current;

        public App()
        {
            Services = ConfigureServices();
        }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // 1. Szolgáltatások (Services) regisztrálása
            services.AddSingleton<IMediaLibraryService, LocalMediaLibraryService>();
            services.AddSingleton<IAudioPlayerService, AudioPlayerService>();

            // Ha az OpenFileDialogService a View mappában van (a képed alapján), akkor azt regisztráljuk:
            // Ha időközben áttetted a Services mappába, akkor a usingokat fent igazítsd hozzá!
            services.AddTransient<IOpenFileDialogService, OpenFileDialogService>();

            // 2. ViewModel-ek regisztrálása
            services.AddTransient<MainViewModel>();

            // 3. Ablakok (Views) regisztrálása
            services.AddTransient<MainWindow>();

            return services.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Itt kötjük össze a MainWindow-t a MainViewModel-lel és indítjuk el a programot
            var mainWindow = Services.GetRequiredService<MainWindow>();
            mainWindow.DataContext = Services.GetRequiredService<MainViewModel>();
            mainWindow.Show();
        }
    }
}