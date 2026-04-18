MusicPlayer MVVM
A WPF-based desktop audio player built with .NET 8 focusing on Clean Code principles and strict MVVM architectural separation.

Features
Artist, Album, and Song-based library browsing with UI virtualization for high-performance listing.

Local playlist management system with track selection overlay and persistent data binding.

Integrated audio engine supporting real-time playback tracking, seeking, and volume control.

Service-oriented architecture using dependency inversion for audio playback and OS dialogs.

Installation
Prerequisites
.NET 8.0 SDK

Visual Studio 2022

Build from source
Clone the repository:

Bash
git clone https://github.com/yourusername/MusicPlayer-MVVM.git
Restore NuGet packages:

Bash
dotnet restore
Build the solution:

Bash
dotnet build --configuration Release
Run the executable found in MusicPlayer/bin/Release/net8.0-windows/.

Dependencies
CommunityToolkit.Mvvm: Used for source-generated observable properties and commands.

NAudio: Primary audio engine for playback and stream management.

MahApps.Metro.IconPacks.Material: Vector-based UI iconography.

Architecture and Clean Code
MVVM Pattern: ViewModels are split into partial classes (Playback, Navigation, Playlists) to maintain single responsibility.

Dependency Inversion: High-level modules depend on abstractions (IAudioPlayerService, IOpenFileDialogService) rather than concrete UI implementations.

Resource Management: Optimized XAML grid structures ensure consistent layout behavior and efficient memory usage.

License
This project is licensed under the MIT License.