# Repository Guidelines

## Instructions

## Project Structure & Module Organization

- AppBantayBarangay/ — Shared Xamarin.Forms project
  - App.xaml, App.xaml.cs — application bootstrapping and resources
  - MainPage.xaml(.cs) — sample page
  - Models/ — domain models (empty in current snapshot)
  - Views/ — XAML pages (referenced by App.xaml.cs, e.g., LoginPage)
  - bin/, obj/ — build artifacts
- AppBantayBarangay.Android/ — Android head project
  - MainActivity.cs — Android entry point; initializes Xamarin.Essentials, Xamarin.Forms, Maps
  - Properties/, Resources/, Assets/ — Android-specific configuration and assets
  - bin/, obj/ — build artifacts
- Documentation files in root: IMPLEMENTATION_GUIDE.md, README_OFFICIAL_PAGE.md, DESIGN_* and QUICK_REFERENCE.md, etc.

## Build, Test, and Development Commands

```bash
# Build (from Visual Studio on Windows)
# Open the solution and Build Solution (Ctrl+Shift+B)

# Build via MSBuild (example)
msbuild AppBantayBarangay.Android/AppBantayBarangay.Android.csproj /t:Build /p:Configuration=Debug

# Run/Deploy (Visual Studio)
# Select AppBantayBarangay.Android as startup project and start (F5)
```

## Coding Style & Naming Conventions

- Indentation: 4 spaces
- File naming: PascalCase for C# files (e.g., MainActivity.cs, MainPage.xaml.cs). XAML pages match class names (MainPage.xaml)
- Classes/Methods: PascalCase for classes and methods; camelCase for local variables and parameters
- Namespaces: Company-style namespaces (AppBantayBarangay, AppBantayBarangay.Droid)
- XAML: Use ResourceDictionary in App.xaml for shared resources
- Linting: Use Visual Studio analyzers/default C# conventions; no explicit .editorconfig found

## Testing Guidelines

- Framework: No unit test projects detected in the repository
- Test files: Not configured
- Running tests: Not applicable in current state
- Coverage: Not specified

## Commit & Pull Request Guidelines

- Commit format: Conventional commits not enforced; follow clear, descriptive messages (e.g., "Add LoginPage and navigation setup", "Configure Xamarin.Forms.Maps in MainActivity")
- PR process: Open PRs with description of changes and testing steps; request review from maintainers
- Branch naming: Use feature/<short-name>, fix/<issue-id>, chore/<task>

---

# Repository Tour

## 🎯 What This Repository Does

AppBantayBarangay is a Xamarin.Forms mobile application with an Android head project, intended for barangay management workflows such as report handling and official dashboards.

Key responsibilities:
- Provide cross-platform UI using Xamarin.Forms
- Android-specific initialization and packaging
- Foundation for official dashboard and report management features

---

## 🏗️ Architecture Overview

### System Context
```
[User (Android)] → [Xamarin.Forms App] → [Optional Backend APIs]
                           ↓
                     [Maps Services]
```

### Key Components
- App (App.xaml.cs) — Initializes the application and sets MainPage to a NavigationPage with LoginPage (from Views)
- Pages (Views/*.xaml) — UI screens built in XAML with code-behind
- Android Host (AppBantayBarangay.Android/MainActivity.cs) — Android entry point; wires up Xamarin.Essentials, Xamarin.Forms, and Xamarin.Forms.Maps

### Data Flow
1. Android launches MainActivity (MainLauncher)
2. Xamarin.Forms is initialized; LoadApplication(new App())
3. App constructor sets MainPage = new NavigationPage(new LoginPage())
4. Navigation and page logic operate within Xamarin.Forms; optional backend calls per implementation guides

---

## 📁 Project Structure [Partial Directory Tree]

```
./
├── AppBantayBarangay/
│   ├── App.xaml
│   ├── App.xaml.cs
│   ├── MainPage.xaml
│   ├── MainPage.xaml.cs
│   ├── Models/
│   ├── Views/
│   ├── AppBantayBarangay.csproj
│   └── obj/, bin/
├── AppBantayBarangay.Android/
│   ├── MainActivity.cs
│   ├── Properties/
│   ├── Resources/
│   ├── Assets/
│   ├── AppBantayBarangay.Android.csproj
│   └── obj/, bin/
├── README_OFFICIAL_PAGE.md
├── IMPLEMENTATION_GUIDE.md
└── DESIGN_*.md, QUICK_REFERENCE.md, etc.
```

### Key Files to Know

| File | Purpose | When You'd Touch It |
|------|---------|---------------------|
| AppBantayBarangay/App.xaml.cs | App startup, sets MainPage and resources | Change startup page, global styles
| AppBantayBarangay/App.xaml | ResourceDictionary, app-level resources | Add theme colors, styles
| AppBantayBarangay/MainPage.xaml(.cs) | Sample page scaffold | Replace with real pages or keep as template
| AppBantayBarangay/Views/* | Feature pages (e.g., LoginPage) | Build UI, navigation targets
| AppBantayBarangay/AppBantayBarangay.csproj | Shared project configuration and NuGet packages | Manage dependencies (Xamarin.Forms, Essentials, Maps)
| AppBantayBarangay.Android/MainActivity.cs | Android entry point and platform init | Configure platform services, permissions, Maps
| AppBantayBarangay.Android/Properties/AndroidManifest.xml | Android configuration | Permissions, SDK levels, activities
| README_OFFICIAL_PAGE.md | Product overview and workflows | Understand UX and intended features
| IMPLEMENTATION_GUIDE.md | Backend integration guidance | Hook up services, API endpoints

---

## 🔧 Technology Stack

### Core Technologies
- Language: C# (netstandard2.0 for shared project)
- Framework: Xamarin.Forms 5.x for UI; Xamarin.Android head project (TargetFrameworkVersion v13.0)
- Mobile Services: Xamarin.Essentials, Xamarin.Forms.Maps

### Key Libraries
- Xamarin.Forms (5.0.0.2662 shared, 5.0.0.2196 Android ref) — cross-platform UI
- Xamarin.Essentials (1.8.1 shared, 1.7.0 Android ref) — device APIs
- Xamarin.Forms.Maps (5.0.0.2662) — map rendering and pins

### Development Tools
- Visual Studio 2019/2022 with Xamarin workload
- MSBuild for CLI builds
- Android SDKs and emulators

---

## 🌐 External Dependencies

- Google Play Services (maps/material via NuGet transitive deps) — required for Maps
- Android SDK platform v13.0 — per csproj TargetFrameworkVersion

### Environment Variables [Optional]

```
# Example
MAPS_API_KEY=        # Required by Maps if using Google Maps on Android
```

---

## 🔄 Common Workflows

### Run on Android Emulator
1. Open solution in Visual Studio
2. Restore NuGet packages
3. Set AppBantayBarangay.Android as Startup Project
4. Press F5 to deploy to chosen emulator

### Add a New Page
1. Create Views/MyPage.xaml + code-behind
2. Register navigation (e.g., navigate via Navigation.PushAsync(new MyPage()))
3. Update App resources as needed

---

## 📈 Performance & Scale

- Linking: AndroidLinkMode None in Debug, Embedded assemblies in Release; adjust for APK size
- Permissions: Location, Storage, Camera declared via attributes in MainActivity; validate runtime permission requests

---

## 🚨 Things to Be Careful About

### Security Considerations
- Do not hardcode API keys; use secure storage or build-time injection
- Request runtime permissions responsibly (location, storage, camera)
- Validate and sanitize data exchanged with backend services


*Updated at: 2025-10-21 (UTC)*
