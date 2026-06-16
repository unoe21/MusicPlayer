# 🎵 MusicPlayer MVVM

<!-- Badges (These render as nice visual tags on GitHub) -->
![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square&logo=dotnet)
![WPF](https://img.shields.io/badge/WPF-Desktop_App-blue?style=flat-square)
![License](https://img.shields.io/badge/License-MIT-green.svg?style=flat-square)

> A WPF-based desktop audio player built with .NET 8 focusing on Clean Code principles and strict MVVM architectural separation.

---

## 📸 Preview

<!-- IMPORTANT: Add a screenshot or a short GIF of your app working! -->
![MusicPlayer Screenshot](docs/screenshot.png) 
*(Note: Replace with actual screenshot of your application)*

---

## ✨ Features

* 🎶 **Library Browsing:** Artist, Album, and Song-based library browsing with UI virtualization for high-performance listing.
* 📋 **Playlist Management:** Local playlist management system with track selection overlay and persistent data binding.
* 🎧 **Integrated Audio Engine:** Supports real-time playback tracking, seeking, and volume control.
* 🧩 **Service-Oriented Architecture:** Uses dependency inversion for audio playback and OS dialogs.

---

## 🏗️ Architecture and Clean Code

This project was built with a strong emphasis on maintainability, scalability, and clean architecture:

* **Strict MVVM Pattern:** ViewModels are split into partial classes (`Playback`, `Navigation`, `Playlists`) using source generators to maintain the Single Responsibility Principle.
* **Dependency Inversion (IoC):** High-level modules depend on abstractions (e.g., `IAudioPlayerService`, `IOpenFileDialogService`) rather than concrete UI implementations, making the core logic highly testable.
* **Resource Management:** Optimized XAML grid structures ensure consistent layout behavior and efficient memory usage without visual stuttering.

---

## 📚 Dependencies / Tech Stack

* **[C# / .NET 8.0](https://dotnet.microsoft.com/)** - Core framework.
* **[CommunityToolkit.Mvvm](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/)** - Used for source-generated observable properties and commands, keeping boilerplate code to a minimum.
* **[NAudio](https://github.com/naudio/NAudio)** - Primary audio engine for robust playback and stream management.
* **[MahApps.Metro.IconPacks.Material](https://github.com/MahApps/MahApps.Metro.IconPacks)** - Vector-based UI iconography for crisp, scalable visuals.

---

## 🚀 Getting Started

### Prerequisites
* [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
* Visual Studio 2022 (or your preferred C# IDE)
