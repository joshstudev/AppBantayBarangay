# Xamarin.Forms to .NET MAUI Migration Guide

## 📋 Table of Contents
1. [Overview](#overview)
2. [Prerequisites](#prerequisites)
3. [Migration Strategy](#migration-strategy)
4. [Step-by-Step Migration](#step-by-step-migration)
5. [Breaking Changes](#breaking-changes)
6. [Firebase Migration](#firebase-migration)
7. [Testing & Validation](#testing--validation)
8. [Troubleshooting](#troubleshooting)

---

## 🎯 Overview

This guide will help you migrate **AppBantayBarangay** from Xamarin.Forms to .NET MAUI (Multi-platform App UI).

### Current State
- **Framework**: Xamarin.Forms 5.0.0.2196
- **Target**: .NET Standard 2.0
- **Android**: MonoAndroid 13.0
- **Firebase**: Xamarin-compatible packages (120.x)

### Target State
- **Framework**: .NET MAUI 8.0+
- **Target**: .NET 8.0
- **Android**: net8.0-android34.0
- **Firebase**: .NET MAUI-compatible packages (121.x+)

### Benefits of Migration
✅ **Single Project** - No more separate Android/iOS projects  
✅ **Better Performance** - Improved startup time and runtime performance  
✅ **Modern .NET** - Access to latest C# features and .NET libraries  
✅ **Hot Reload** - Better development experience  
✅ **Long-term Support** - Xamarin.Forms is deprecated  
✅ **Latest Firebase** - Access to newest Firebase features  

---

## 📦 Prerequisites

### Required Software
1. **Visual Studio 2022 (17.8 or later)**
   - Workload: .NET Multi-platform App UI development
   - Workload: Mobile development with .NET

2. **.NET 8.0 SDK**
   ```bash
   dotnet --version  # Should be 8.0.x or later
   ```

3. **Android SDK**
   - API Level 34 (Android 14)
   - Build Tools 34.0.0

### Recommended Tools
- **Visual Studio Extension**: .NET MAUI Extension
- **Visual Studio Extension**: XAML Styler
- **.NET Upgrade Assistant** (optional)

---

## 🗺️ Migration Strategy

### Option 1: Manual Migration (Recommended)
**Pros:**
- Full control over the process
- Better understanding of changes
- Cleaner result

**Cons:**
- More time-consuming
- Requires careful attention

### Option 2: .NET Upgrade Assistant
**Pros:**
- Automated process
- Faster initial migration

**Cons:**
- May require manual fixes
- Less control

**We recommend Option 1 for this project.**

---

## 🚀 Step-by-Step Migration

### Phase 1: Preparation

#### Step 1.1: Backup Your Project
```bash
# Create a backup
cd C:\Users\ASUS_VX16\source\repos\AppBantayBarangay
xcopy AppBantayBarangay AppBantayBarangay_Backup /E /I /H
```

#### Step 1.2: Document Current State
- [ ] List all NuGet packages and versions
- [ ] Document custom renderers
- [ ] Document platform-specific code
- [ ] List all dependencies

#### Step 1.3: Create New .NET MAUI Project
```bash
# Create new MAUI project
dotnet new maui -n AppBantayBarangay.Maui -o AppBantayBarangay.Maui
```

---

### Phase 2: Project Structure Migration

#### Step 2.1: Update Project File

**Create new `AppBantayBarangay.csproj`:**

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFrameworks>net8.0-android34.0</TargetFrameworks>
    <!-- Add iOS/Windows if needed -->
    <!-- <TargetFrameworks>net8.0-android34.0;net8.0-ios17.0;net8.0-windows10.0.19041.0</TargetFrameworks> -->
    
    <OutputType>Exe</OutputType>
    <RootNamespace>AppBantayBarangay</RootNamespace>
    <UseMaui>true</UseMaui>
    <SingleProject>true</SingleProject>
    <ImplicitUsings>enable</ImplicitUsings>
    
    <!-- Display name -->
    <ApplicationTitle>Bantay Barangay</ApplicationTitle>
    
    <!-- App Identifier -->
    <ApplicationId>com.companyname.appbantaybarangay</ApplicationId>
    <ApplicationIdGuid>B1102102-36CC-481B-A787-65CCB37FF4E3</ApplicationIdGuid>
    
    <!-- Versions -->
    <ApplicationDisplayVersion>1.0</ApplicationDisplayVersion>
    <ApplicationVersion>1</ApplicationVersion>
    
    <!-- Android -->
    <SupportedOSPlatformVersion Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'android'">21.0</SupportedOSPlatformVersion>
    <AndroidPackageFormat>apk</AndroidPackageFormat>
  </PropertyGroup>
  
  <ItemGroup>
    <!-- MAUI Core -->
    <PackageReference Include="Microsoft.Maui.Controls" Version="8.0.90" />
    <PackageReference Include="Microsoft.Maui.Controls.Compatibility" Version="8.0.90" />
    <PackageReference Include="Microsoft.Extensions.Logging.Debug" Version="8.0.0" />
    
    <!-- Firebase for .NET MAUI -->
    <PackageReference Include="Xamarin.Firebase.Auth" Version="121.0.8" />
    <PackageReference Include="Xamarin.Firebase.Database" Version="121.0.0" />
    <PackageReference Include="Xamarin.Firebase.Storage" Version="121.0.0" />
    <PackageReference Include="Xamarin.GooglePlayServices.Base" Version="118.5.0" />
    
    <!-- Maps -->
    <PackageReference Include="Microsoft.Maui.Controls.Maps" Version="8.0.90" />
    
    <!-- JSON -->
    <PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
  </ItemGroup>
  
  <ItemGroup>
    <!-- Android -->
    <GoogleServicesJson Include="Platforms\Android\google-services.json" />
  </ItemGroup>
</Project>
```

#### Step 2.2: Create MAUI Folder Structure

```
AppBantayBarangay/
├── Platforms/
│   ├── Android/
│   │   ├── MainActivity.cs
│   │   ├── MainApplication.cs
│   │   ├── AndroidManifest.xml
│   │   ├── google-services.json
│   │   └── Resources/
│   │       ├── values/
│   │       │   ├── colors.xml
│   │       │   └── styles.xml
│   │       └── mipmap-*/
│   ├── iOS/ (if needed)
│   └── Windows/ (if needed)
├── Resources/
│   ├── AppIcon/
│   ├── Fonts/
│   ├── Images/
│   ├── Raw/
│   └── Splash/
├── Models/
├── Services/
├── Views/
├── ViewModels/
├── App.xaml
├── App.xaml.cs
├── AppShell.xaml
├── AppShell.xaml.cs
└── MauiProgram.cs
```

---

### Phase 3: Code Migration

#### Step 3.1: Create MauiProgram.cs

```csharp
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;

namespace AppBantayBarangay
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                })
                .UseMauiMaps();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            // Register services
            builder.Services.AddSingleton<IFirebaseService, FirebaseService>();

            return builder.Build();
        }
    }
}
```

#### Step 3.2: Update App.xaml.cs

```csharp
namespace AppBantayBarangay
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }
    }
}
```

#### Step 3.3: Create AppShell.xaml

```xml
<?xml version="1.0" encoding="UTF-8" ?>
<Shell
    x:Class="AppBantayBarangay.AppShell"
    xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
    xmlns:local="clr-namespace:AppBantayBarangay.Views"
    Shell.FlyoutBehavior="Disabled">

    <ShellContent
        Title="Login"
        ContentTemplate="{DataTemplate local:LoginPage}"
        Route="LoginPage" />

</Shell>
```

#### Step 3.4: Create AppShell.xaml.cs

```csharp
namespace AppBantayBarangay
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
        }
    }
}
```

---

### Phase 4: Platform-Specific Code

#### Step 4.1: Create MainActivity.cs (Android)

**Location**: `Platforms/Android/MainActivity.cs`

```csharp
using Android.App;
using Android.Content.PM;
using Android.OS;
using Microsoft.Maui;

namespace AppBantayBarangay
{
    [Activity(
        Theme = "@style/Maui.SplashTheme",
        MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | 
                              ConfigChanges.Orientation | 
                              ConfigChanges.UiMode | 
                              ConfigChanges.ScreenLayout | 
                              ConfigChanges.SmallestScreenSize | 
                              ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            // Initialize Firebase
            Firebase.FirebaseApp.InitializeApp(this);
        }
    }
}
```

#### Step 4.2: Create MainApplication.cs (Android)

**Location**: `Platforms/Android/MainApplication.cs`

```csharp
using Android.App;
using Android.Runtime;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;

namespace AppBantayBarangay
{
    [Application]
    public class MainApplication : MauiApplication
    {
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}
```

#### Step 4.3: Update AndroidManifest.xml

**Location**: `Platforms/Android/AndroidManifest.xml`

```xml
<?xml version="1.0" encoding="utf-8"?>
<manifest xmlns:android="http://schemas.android.com/apk/res/android">
    <application 
        android:allowBackup="true" 
        android:icon="@mipmap/appicon" 
        android:roundIcon="@mipmap/appicon_round" 
        android:supportsRtl="true">
    </application>
    
    <uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />
    <uses-permission android:name="android.permission.INTERNET" />
    <uses-permission android:name="android.permission.ACCESS_FINE_LOCATION" />
    <uses-permission android:name="android.permission.ACCESS_COARSE_LOCATION" />
    
    <uses-sdk android:minSdkVersion="21" android:targetSdkVersion="34" />
</manifest>
```

---

### Phase 5: Migrate Services

#### Step 5.1: Update IFirebaseService

**No changes needed** - Interface remains the same

#### Step 5.2: Update FirebaseService (Android)

**Location**: `Platforms/Android/Services/FirebaseService.cs`

```csharp
using System;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Storage;
using AppBantayBarangay.Services;
using Newtonsoft.Json;

namespace AppBantayBarangay.Platforms.Android.Services
{
    public class FirebaseService : IFirebaseService
    {
        private FirebaseAuth _auth;
        private FirebaseDatabase _database;
        private FirebaseStorage _storage;

        public FirebaseService()
        {
            InitializeFirebase();
        }

        private void InitializeFirebase()
        {
            try
            {
                _auth = FirebaseAuth.Instance;
                _database = FirebaseDatabase.Instance;
                _storage = FirebaseStorage.Instance;
                _database.SetPersistenceEnabled(true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Firebase initialization error: {ex.Message}");
            }
        }

        // ... rest of the implementation (same as before)
    }
}
```

---

### Phase 6: Migrate Views

#### Step 6.1: Update XAML Namespace

**OLD (Xamarin.Forms):**
```xml
xmlns="http://xamarin.com/schemas/2014/forms"
xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
```

**NEW (.NET MAUI):**
```xml
xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
```

#### Step 6.2: Update Code-Behind

**Replace:**
- `Xamarin.Forms` → `Microsoft.Maui.Controls`
- `Xamarin.Essentials` → `Microsoft.Maui.Essentials` (or use built-in MAUI APIs)
- `DependencyService.Get<T>()` → Constructor injection or `Handler.MauiContext.Services.GetService<T>()`

---

### Phase 7: Update Resources

#### Step 7.1: Move Images

**OLD**: `Resources/drawable/image.png`  
**NEW**: `Resources/Images/image.png`

#### Step 7.2: Update Image References

**OLD**: `Source="image.png"`  
**NEW**: `Source="image.png"` (same, but file location changed)

---

## 🔥 Firebase Migration

### Update Package Versions

```xml
<!-- .NET MAUI Compatible Firebase Packages -->
<PackageReference Include="Xamarin.Firebase.Auth" Version="121.0.8" />
<PackageReference Include="Xamarin.Firebase.Database" Version="121.0.0" />
<PackageReference Include="Xamarin.Firebase.Storage" Version="121.0.0" />
<PackageReference Include="Xamarin.GooglePlayServices.Base" Version="118.5.0" />
<PackageReference Include="Xamarin.Google.Dagger" Version="2.51.1" />
```

### Move google-services.json

**OLD**: `AppBantayBarangay.Android/Assets/google-services.json`  
**NEW**: `Platforms/Android/google-services.json`

---

## 💥 Breaking Changes

### 1. Namespace Changes
| Xamarin.Forms | .NET MAUI |
|---------------|-----------|
| `Xamarin.Forms` | `Microsoft.Maui.Controls` |
| `Xamarin.Essentials` | `Microsoft.Maui.Essentials` or built-in |
| `Xamarin.Forms.Maps` | `Microsoft.Maui.Controls.Maps` |

### 2. DependencyService
**OLD**:
```csharp
var service = DependencyService.Get<IFirebaseService>();
```

**NEW** (Constructor Injection):
```csharp
public class MyPage : ContentPage
{
    private readonly IFirebaseService _firebaseService;
    
    public MyPage(IFirebaseService firebaseService)
    {
        _firebaseService = firebaseService;
        InitializeComponent();
    }
}
```

### 3. Custom Renderers → Handlers
Custom renderers need to be converted to handlers.

### 4. Effects → Behaviors/Handlers
Effects are replaced with behaviors or handlers.

---

## ✅ Testing & Validation

### Checklist
- [ ] App builds successfully
- [ ] App runs on Android emulator
- [ ] Firebase authentication works
- [ ] Firebase database operations work
- [ ] Firebase storage operations work
- [ ] Maps functionality works
- [ ] All views render correctly
- [ ] Navigation works
- [ ] All features tested

---

## 🐛 Troubleshooting

### Issue: Build Errors
**Solution**: Clean and rebuild
```bash
dotnet clean
dotnet build
```

### Issue: Firebase Not Initializing
**Solution**: Ensure `google-services.json` is in correct location with correct build action

### Issue: Package Conflicts
**Solution**: Clear NuGet cache
```bash
dotnet nuget locals all --clear
```

---

## 📚 Additional Resources

- [Official .NET MAUI Documentation](https://learn.microsoft.com/dotnet/maui/)
- [Xamarin.Forms to .NET MAUI Migration](https://learn.microsoft.com/dotnet/maui/migration/)
- [.NET Upgrade Assistant](https://dotnet.microsoft.com/platform/upgrade-assistant)
- [Firebase for .NET MAUI](https://github.com/xamarin/GooglePlayServicesComponents)

---

## 🎯 Next Steps

1. **Review this guide thoroughly**
2. **Backup your current project**
3. **Create a new branch for migration**
4. **Follow the step-by-step migration**
5. **Test thoroughly**
6. **Deploy to production**

---

*Migration Guide Version 1.0*  
*Last Updated: 2025*
