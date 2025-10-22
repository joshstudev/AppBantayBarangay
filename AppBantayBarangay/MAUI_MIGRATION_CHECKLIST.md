# .NET MAUI Migration Checklist

## 📋 Pre-Migration

- [ ] **Backup entire project**
  ```bash
  xcopy AppBantayBarangay AppBantayBarangay_Backup /E /I /H
  ```

- [ ] **Install Visual Studio 2022 (17.8+)**
  - [ ] .NET Multi-platform App UI development workload
  - [ ] Mobile development with .NET workload

- [ ] **Install .NET 8.0 SDK**
  ```bash
  dotnet --version  # Verify 8.0.x
  ```

- [ ] **Document current packages**
  - [ ] List all NuGet packages
  - [ ] Note custom renderers
  - [ ] Note platform-specific code

- [ ] **Create new Git branch**
  ```bash
  git checkout -b feature/maui-migration
  ```

---

## 🏗️ Project Setup

- [ ] **Create new .NET MAUI project structure**
  ```bash
  dotnet new maui -n AppBantayBarangay.Maui
  ```

- [ ] **Create folder structure**
  - [ ] `Platforms/Android/`
  - [ ] `Platforms/Android/Resources/`
  - [ ] `Platforms/Android/Services/`
  - [ ] `Resources/Images/`
  - [ ] `Resources/Fonts/`
  - [ ] `Resources/AppIcon/`
  - [ ] `Resources/Splash/`
  - [ ] `Models/`
  - [ ] `Services/`
  - [ ] `Views/`
  - [ ] `ViewModels/`

- [ ] **Update .csproj file**
  - [ ] Change to `<Project Sdk="Microsoft.NET.Sdk">`
  - [ ] Set `<TargetFrameworks>net8.0-android34.0</TargetFrameworks>`
  - [ ] Add `<UseMaui>true</UseMaui>`
  - [ ] Add `<SingleProject>true</SingleProject>`
  - [ ] Update package references

---

## 📦 Package Migration

- [ ] **Update core packages**
  - [ ] Remove `Xamarin.Forms` → Add `Microsoft.Maui.Controls` (8.0.90)
  - [ ] Remove `Xamarin.Essentials` → Built into MAUI
  - [ ] Remove `Xamarin.Forms.Maps` → Add `Microsoft.Maui.Controls.Maps` (8.0.90)

- [ ] **Update Firebase packages**
  - [ ] `Xamarin.Firebase.Auth` → 121.0.8
  - [ ] `Xamarin.Firebase.Database` → 121.0.0
  - [ ] `Xamarin.Firebase.Storage` → 121.0.0
  - [ ] `Xamarin.GooglePlayServices.Base` → 118.5.0
  - [ ] Add `Xamarin.Google.Dagger` → 2.51.1

- [ ] **Add MAUI packages**
  - [ ] `Microsoft.Extensions.Logging.Debug` (8.0.0)

---

## 🔧 Code Migration

- [ ] **Create MauiProgram.cs**
  - [ ] Configure app builder
  - [ ] Register services
  - [ ] Configure fonts
  - [ ] Add maps support

- [ ] **Update App.xaml**
  - [ ] Change namespace to `http://schemas.microsoft.com/dotnet/2021/maui`
  - [ ] Update resource dictionaries

- [ ] **Update App.xaml.cs**
  - [ ] Change namespace from `Xamarin.Forms` to `Microsoft.Maui.Controls`
  - [ ] Set `MainPage = new AppShell();`

- [ ] **Create AppShell.xaml**
  - [ ] Define shell structure
  - [ ] Add routes
  - [ ] Configure flyout

- [ ] **Create AppShell.xaml.cs**
  - [ ] Initialize shell
  - [ ] Register routes

---

## 📱 Platform-Specific Code

### Android

- [ ] **Create MainActivity.cs**
  - [ ] Location: `Platforms/Android/MainActivity.cs`
  - [ ] Inherit from `MauiAppCompatActivity`
  - [ ] Initialize Firebase in `OnCreate`

- [ ] **Create MainApplication.cs**
  - [ ] Location: `Platforms/Android/MainApplication.cs`
  - [ ] Inherit from `MauiApplication`
  - [ ] Override `CreateMauiApp()`

- [ ] **Update AndroidManifest.xml**
  - [ ] Location: `Platforms/Android/AndroidManifest.xml`
  - [ ] Update permissions
  - [ ] Set min/target SDK versions

- [ ] **Move google-services.json**
  - [ ] From: `AppBantayBarangay.Android/Assets/`
  - [ ] To: `Platforms/Android/`
  - [ ] Update build action to `GoogleServicesJson`

- [ ] **Move Resources**
  - [ ] Move `Resources/values/styles.xml`
  - [ ] Move `Resources/values/colors.xml`
  - [ ] Move `Resources/mipmap-*/` icons

---

## 🎨 Views Migration

- [ ] **Update all XAML files**
  - [ ] Change namespace: `xmlns="http://schemas.microsoft.com/dotnet/2021/maui"`
  - [ ] Update any Xamarin.Forms-specific controls

- [ ] **Update code-behind files**
  - [ ] Change `using Xamarin.Forms;` to `using Microsoft.Maui.Controls;`
  - [ ] Update `DependencyService` to constructor injection
  - [ ] Update `Device.RuntimePlatform` to `DeviceInfo.Platform`

- [ ] **Migrate Views**
  - [ ] MainPage
  - [ ] LoginPage
  - [ ] FirebaseLoginExample
  - [ ] (Add other pages as needed)

---

## 🔥 Firebase Migration

- [ ] **Update FirebaseService**
  - [ ] Move to `Platforms/Android/Services/`
  - [ ] Update namespace
  - [ ] Verify all methods work

- [ ] **Update IFirebaseService**
  - [ ] Keep in shared `Services/` folder
  - [ ] No changes needed

- [ ] **Update FirebaseConfig**
  - [ ] Keep in shared `Services/` folder
  - [ ] No changes needed

- [ ] **Register Firebase service**
  - [ ] Add to `MauiProgram.cs`:
    ```csharp
    builder.Services.AddSingleton<IFirebaseService, FirebaseService>();
    ```

---

## 🖼️ Resources Migration

- [ ] **Move images**
  - [ ] From: `Resources/drawable/`
  - [ ] To: `Resources/Images/`

- [ ] **Add fonts**
  - [ ] Add OpenSans-Regular.ttf
  - [ ] Add OpenSans-Semibold.ttf
  - [ ] Register in MauiProgram.cs

- [ ] **Update app icon**
  - [ ] Create `Resources/AppIcon/appicon.svg`
  - [ ] Or use PNG files in `Resources/AppIcon/`

- [ ] **Update splash screen**
  - [ ] Create `Resources/Splash/splash.svg`
  - [ ] Or use PNG in `Resources/Splash/`

---

## 🧪 Testing

- [ ] **Build project**
  ```bash
  dotnet build
  ```

- [ ] **Run on Android emulator**
  ```bash
  dotnet build -t:Run -f net8.0-android
  ```

- [ ] **Test Firebase Authentication**
  - [ ] Sign up
  - [ ] Sign in
  - [ ] Sign out

- [ ] **Test Firebase Database**
  - [ ] Save data
  - [ ] Retrieve data

- [ ] **Test Firebase Storage**
  - [ ] Upload file
  - [ ] Download file

- [ ] **Test Maps**
  - [ ] Map displays
  - [ ] Location works

- [ ] **Test Navigation**
  - [ ] All pages accessible
  - [ ] Back navigation works

- [ ] **Test UI**
  - [ ] All views render correctly
  - [ ] Styles applied correctly
  - [ ] Images display

---

## 🐛 Troubleshooting

- [ ] **If build fails**
  ```bash
  dotnet clean
  dotnet nuget locals all --clear
  dotnet restore
  dotnet build
  ```

- [ ] **If Firebase doesn't initialize**
  - [ ] Verify `google-services.json` location
  - [ ] Check build action is `GoogleServicesJson`
  - [ ] Verify package name matches

- [ ] **If packages conflict**
  - [ ] Clear NuGet cache
  - [ ] Remove bin/obj folders
  - [ ] Restore packages

---

## 📝 Documentation

- [ ] **Update README**
  - [ ] Change Xamarin.Forms to .NET MAUI
  - [ ] Update build instructions
  - [ ] Update package versions

- [ ] **Update Firebase docs**
  - [ ] Update package versions
  - [ ] Update file locations
  - [ ] Update code examples

- [ ] **Create migration notes**
  - [ ] Document issues encountered
  - [ ] Document solutions
  - [ ] Document breaking changes

---

## 🚀 Deployment

- [ ] **Test on physical device**
  - [ ] Deploy to Android device
  - [ ] Test all features

- [ ] **Create release build**
  ```bash
  dotnet publish -f net8.0-android -c Release
  ```

- [ ] **Test release build**
  - [ ] Install on device
  - [ ] Verify all features work

- [ ] **Update version numbers**
  - [ ] ApplicationDisplayVersion
  - [ ] ApplicationVersion

---

## ✅ Final Checks

- [ ] **Code quality**
  - [ ] No compiler warnings
  - [ ] No deprecated APIs
  - [ ] Code follows MAUI best practices

- [ ] **Performance**
  - [ ] App starts quickly
  - [ ] No memory leaks
  - [ ] Smooth navigation

- [ ] **Functionality**
  - [ ] All features work
  - [ ] No regressions
  - [ ] Firebase fully functional

- [ ] **Documentation**
  - [ ] All docs updated
  - [ ] Migration notes complete
  - [ ] README accurate

---

## 🎉 Post-Migration

- [ ] **Commit changes**
  ```bash
  git add .
  git commit -m "Migrate from Xamarin.Forms to .NET MAUI"
  ```

- [ ] **Create pull request**
  - [ ] Review changes
  - [ ] Get team approval

- [ ] **Merge to main**
  ```bash
  git checkout main
  git merge feature/maui-migration
  ```

- [ ] **Tag release**
  ```bash
  git tag -a v2.0.0-maui -m "Migrated to .NET MAUI"
  git push origin v2.0.0-maui
  ```

- [ ] **Celebrate! 🎊**

---

## 📊 Progress Tracking

**Overall Progress**: ☐☐☐☐☐☐☐☐☐☐ 0%

- Pre-Migration: ☐☐☐☐☐ 0/5
- Project Setup: ☐☐☐☐☐ 0/5
- Package Migration: ☐☐☐☐☐ 0/5
- Code Migration: ☐☐☐☐☐ 0/5
- Platform Code: ☐☐☐☐☐ 0/5
- Views Migration: ☐☐☐☐☐ 0/5
- Firebase Migration: ☐☐☐☐☐ 0/5
- Resources: ☐☐☐☐☐ 0/5
- Testing: ☐☐☐☐☐ 0/5
- Deployment: ☐☐☐☐☐ 0/5

---

*Use this checklist to track your migration progress*  
*Check off items as you complete them*  
*Estimated time: 2-5 days depending on project complexity*
