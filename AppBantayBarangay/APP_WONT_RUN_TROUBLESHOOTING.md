# App Won't Run - Troubleshooting Guide

## 🚨 Quick Fixes (Try These First!)

### Fix 1: Clean and Rebuild
```
1. Build → Clean Solution
2. Close Visual Studio
3. Delete these folders:
   - AppBantayBarangay.Android\bin
   - AppBantayBarangay.Android\obj
   - AppBantayBarangay\bin
   - AppBantayBarangay\obj
4. Reopen Visual Studio
5. Build → Rebuild Solution
6. Press F5 to run
```

### Fix 2: Restore NuGet Packages
```
1. Right-click Solution → Restore NuGet Packages
2. Wait for completion
3. Build → Rebuild Solution
4. Press F5 to run
```

### Fix 3: Set Startup Project
```
1. Right-click "AppBantayBarangay.Android" in Solution Explorer
2. Select "Set as Startup Project"
3. Press F5 to run
```

### Fix 4: Select Android Emulator/Device
```
1. Click the dropdown next to the green "Play" button
2. Select an Android emulator or connected device
3. If no emulator exists, create one:
   - Tools → Android → Android Device Manager
   - Click "New Device"
   - Select a device template (e.g., Pixel 5)
   - Click "Create"
4. Press F5 to run
```

---

## 🔍 Detailed Troubleshooting

### Issue 1: Build Errors

#### Symptom
- Error messages in Error List window
- Build fails before running

#### Solution
```
1. View → Error List
2. Check for errors
3. Common errors and fixes:

   Error: "Package restore failed"
   Fix: Tools → Options → NuGet Package Manager → Clear All NuGet Cache(s)
        Then: Right-click Solution → Restore NuGet Packages

   Error: "Could not find a part of the path"
   Fix: Delete bin and obj folders, rebuild

   Error: "The type or namespace name could not be found"
   Fix: Ensure all NuGet packages are restored
        Check that all files are included in the project

   Error: "Resource not found"
   Fix: Clean solution, rebuild
```

---

### Issue 2: No Emulator/Device Available

#### Symptom
- No device shown in device dropdown
- "No compatible Android device found" message

#### Solution A: Create Android Emulator
```
1. Tools → Android → Android Device Manager
2. Click "+ New Device"
3. Select device definition (e.g., Pixel 5 - API 33)
4. Click "Create"
5. Wait for emulator to be created
6. Select the emulator from dropdown
7. Press F5
```

#### Solution B: Use Physical Device
```
1. Enable Developer Options on your Android phone:
   - Settings → About Phone
   - Tap "Build Number" 7 times
   
2. Enable USB Debugging:
   - Settings → Developer Options
   - Enable "USB Debugging"
   
3. Connect phone via USB
4. Allow USB debugging when prompted
5. Select your device from dropdown
6. Press F5
```

---

### Issue 3: App Crashes on Startup

#### Symptom
- App builds successfully
- App deploys to device/emulator
- App crashes immediately or shows error

#### Solution
```
1. Check Output window for error messages:
   - View → Output
   - Select "Debug" from "Show output from:" dropdown
   
2. Common crash causes:

   Cause: Firebase initialization error
   Fix: Ensure google-services.json is in Assets folder
        Build Action should be "GoogleServicesJson"
   
   Cause: Missing permissions
   Fix: Check AndroidManifest.xml has required permissions
   
   Cause: LoginPage not found
   Fix: Ensure LoginPage.xaml and LoginPage.xaml.cs exist in Views folder
   
   Cause: Resource not found
   Fix: Clean and rebuild solution
```

---

### Issue 4: Deployment Fails

#### Symptom
- Build succeeds
- Deployment to device/emulator fails
- Error: "Deployment failed"

#### Solution
```
1. Restart the emulator:
   - Close emulator
   - Tools → Android → Android Device Manager
   - Click "Start" on your emulator
   - Wait for emulator to fully boot
   - Try running again

2. If using physical device:
   - Disconnect and reconnect USB cable
   - Revoke USB debugging authorization:
     Settings → Developer Options → Revoke USB Debugging Authorizations
   - Reconnect and allow again

3. Clear previous deployment:
   - Uninstall app from device/emulator
   - Try deploying again
```

---

### Issue 5: "The application could not be started"

#### Symptom
- Error message: "The application could not be started"
- App doesn't launch

#### Solution
```
1. Check minimum Android version:
   - Your app requires Android 5.0 (API 21) or higher
   - Ensure emulator/device meets this requirement

2. Check AndroidManifest.xml:
   - Verify package name is correct
   - Verify permissions are declared

3. Rebuild with clean:
   - Build → Clean Solution
   - Build → Rebuild Solution
   - Deploy again
```

---

## 🛠️ Step-by-Step Comprehensive Fix

If nothing above works, follow these steps:

### Step 1: Complete Clean
```
1. Close Visual Studio
2. Navigate to project folder
3. Delete these folders:
   - AppBantayBarangay.Android\bin
   - AppBantayBarangay.Android\obj
   - AppBantayBarangay\bin
   - AppBantayBarangay\obj
   - .vs (hidden folder)
```

### Step 2: Clear NuGet Cache
```
1. Open Command Prompt
2. Run: dotnet nuget locals all --clear
```

### Step 3: Reopen and Restore
```
1. Open Visual Studio
2. Open solution
3. Right-click Solution → Restore NuGet Packages
4. Wait for completion (check status bar)
```

### Step 4: Rebuild
```
1. Build → Clean Solution
2. Build → Rebuild Solution
3. Check for errors in Error List
```

### Step 5: Configure Startup
```
1. Right-click "AppBantayBarangay.Android"
2. Set as Startup Project
3. Select Android emulator from dropdown
```

### Step 6: Run
```
1. Press F5 or click green "Play" button
2. Wait for deployment
3. Check Output window for messages
```

---

## 📋 Pre-Run Checklist

Before pressing Run, verify:

- [ ] **Startup Project**: AppBantayBarangay.Android is set as startup project (bold in Solution Explorer)
- [ ] **Device Selected**: Android emulator or device is selected in dropdown
- [ ] **Build Succeeds**: Solution builds without errors (Build → Build Solution)
- [ ] **Packages Restored**: All NuGet packages are restored (no yellow warning icons)
- [ ] **Emulator Running**: If using emulator, it's fully booted (not just starting)
- [ ] **USB Debugging**: If using device, USB debugging is enabled and authorized

---

## 🔧 Common Error Messages and Fixes

### Error: "Deployment failed. Could not find a part of the path"
**Fix:**
```
1. Delete bin and obj folders
2. Rebuild solution
```

### Error: "Package restore failed"
**Fix:**
```
1. Tools → Options → NuGet Package Manager
2. Click "Clear All NuGet Cache(s)"
3. Right-click Solution → Restore NuGet Packages
```

### Error: "The type or namespace name 'LoginPage' could not be found"
**Fix:**
```
1. Ensure LoginPage.xaml exists in Views folder
2. Check LoginPage.xaml.cs has correct namespace
3. Rebuild solution
```

### Error: "Resource not found"
**Fix:**
```
1. Clean solution
2. Delete bin/obj folders
3. Rebuild solution
```

### Error: "Firebase initialization failed"
**Fix:**
```
1. Verify google-services.json is in Assets folder
2. Right-click google-services.json → Properties
3. Build Action should be "GoogleServicesJson"
4. Rebuild
```

### Error: "The application could not be started. Press OK to close."
**Fix:**
```
1. Check Output window for actual error
2. Uninstall app from device/emulator
3. Clean and rebuild
4. Deploy again
```

---

## 🎯 Specific Fixes for Your Project

### Fix: LoginPage Not Loading
```csharp
// Verify App.xaml.cs has this:
MainPage = new NavigationPage(new LoginPage())
{
    BarBackgroundColor = Color.FromHex("#007AFF"),
    BarTextColor = Color.White
};
```

### Fix: Firebase Not Initializing
```
1. Check google-services.json location:
   - Should be in: AppBantayBarangay.Android\Assets\
   
2. Check Build Action:
   - Right-click google-services.json
   - Properties
   - Build Action: GoogleServicesJson
   
3. Verify package name matches:
   - AndroidManifest.xml: com.companyname.appbantaybarangay
   - google-services.json: same package name
```

---

## 📱 Emulator-Specific Issues

### Emulator Won't Start
```
1. Check Hyper-V is enabled (Windows):
   - Control Panel → Programs → Turn Windows features on or off
   - Enable "Hyper-V"
   - Restart computer

2. Check BIOS virtualization:
   - Restart computer
   - Enter BIOS (usually F2, F10, or Del)
   - Enable Intel VT-x or AMD-V
   - Save and exit

3. Use Android Device Manager:
   - Tools → Android → Android Device Manager
   - Delete problematic emulator
   - Create new emulator
```

### Emulator is Slow
```
1. Allocate more RAM:
   - Tools → Android → Android Device Manager
   - Edit emulator
   - Increase RAM to 2048 MB or more

2. Enable hardware acceleration:
   - Ensure Hyper-V is enabled
   - Use x86_64 system image (not ARM)
```

---

## 🔍 How to Check Output Window

```
1. View → Output (or Ctrl+Alt+O)
2. In "Show output from:" dropdown, select:
   - "Debug" - for runtime errors
   - "Build" - for build errors
   - "Package Manager" - for NuGet errors
3. Look for error messages in red
4. Copy error message and search for solution
```

---

## 📞 Still Not Working?

### Collect Information
```
1. Visual Studio version:
   - Help → About Microsoft Visual Studio
   
2. .NET version:
   - Open Command Prompt
   - Run: dotnet --version
   
3. Android SDK version:
   - Tools → Android → Android SDK Manager
   
4. Error messages:
   - Copy from Output window
   - Copy from Error List
```

### Try Alternative Approach
```
1. Create new Xamarin.Forms project
2. Copy your code files to new project
3. Add NuGet packages
4. Try running new project
```

---

## ✅ Success Checklist

When app runs successfully, you should see:
- [ ] Build succeeds with 0 errors
- [ ] App deploys to device/emulator
- [ ] Emulator/device shows app icon
- [ ] App launches and shows LoginPage
- [ ] No crash or error messages
- [ ] Output window shows "Firebase initialized successfully"

---

## 🎯 Quick Command Reference

```bash
# Clean NuGet cache
dotnet nuget locals all --clear

# Restore packages
dotnet restore

# Build solution
dotnet build

# Clean build
dotnet clean
```

---

*Troubleshooting guide for AppBantayBarangay*  
*Last updated: 2025*
