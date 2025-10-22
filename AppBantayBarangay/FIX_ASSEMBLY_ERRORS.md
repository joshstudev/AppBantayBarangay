# Fix Assembly Reference Errors

## 🚨 The Problem

You're seeing errors like:
```
The name 'JsonConvert' does not exist in the current context
The name 'FirebaseAuth' does not exist in the current context
The type or namespace name 'Firebase' could not be found
The type or namespace name 'Gms' does not exist in the namespace 'Android'
The type or namespace name 'Newtonsoft' could not be found
```

## 🔍 Root Cause

These errors mean that **NuGet packages are not properly restored** or **assemblies are not being referenced**.

---

## ✅ SOLUTION 1: Automated Fix (Fastest)

### Run the Fix Script
```powershell
# 1. Close Visual Studio
# 2. Run this script
.\Fix-BuildErrors.ps1

# 3. Reopen Visual Studio
# 4. Build → Rebuild Solution
```

---

## ✅ SOLUTION 2: Manual Fix (Step-by-Step)

### Step 1: Close Visual Studio
```
Close Visual Studio completely
```

### Step 2: Delete bin and obj Folders
```
Navigate to:
C:\Users\ASUS_VX16\source\repos\AppBantayBarangay\AppBantayBarangay

Delete these folders:
- AppBantayBarangay.Android\bin
- AppBantayBarangay.Android\obj
- AppBantayBarangay\bin
- AppBantayBarangay\obj
```

### Step 3: Clear NuGet Cache
```powershell
# Open PowerShell or Command Prompt
dotnet nuget locals all --clear
```

### Step 4: Restore Packages
```powershell
cd C:\Users\ASUS_VX16\source\repos\AppBantayBarangay\AppBantayBarangay
dotnet restore
```

### Step 5: Reopen Visual Studio
```
1. Open Visual Studio
2. Open the solution
3. Wait for IntelliSense to load
```

### Step 6: Restore Packages in Visual Studio
```
1. Right-click Solution in Solution Explorer
2. Select "Restore NuGet Packages"
3. Wait for completion (check status bar)
```

### Step 7: Rebuild
```
1. Build → Clean Solution
2. Build → Rebuild Solution
```

---

## ✅ SOLUTION 3: If Still Not Working

### Option A: Clear All NuGet Caches in Visual Studio
```
1. Tools → Options
2. NuGet Package Manager → General
3. Click "Clear All NuGet Cache(s)"
4. Click OK
5. Right-click Solution → Restore NuGet Packages
6. Rebuild
```

### Option B: Reinstall Packages
```
1. Tools → NuGet Package Manager → Package Manager Console
2. Run these commands:

Update-Package Newtonsoft.Json -Reinstall -ProjectName AppBantayBarangay.Android
Update-Package Xamarin.Firebase.Auth -Reinstall -ProjectName AppBantayBarangay.Android
Update-Package Xamarin.Firebase.Database -Reinstall -ProjectName AppBantayBarangay.Android
Update-Package Xamarin.Firebase.Storage -Reinstall -ProjectName AppBantayBarangay.Android
Update-Package Xamarin.GooglePlayServices.Base -Reinstall -ProjectName AppBantayBarangay.Android
```

### Option C: Check Package References
```
1. Right-click AppBantayBarangay.Android project
2. Select "Manage NuGet Packages"
3. Go to "Installed" tab
4. Verify these packages are installed:
   - Newtonsoft.Json (13.0.3)
   - Xamarin.Firebase.Auth (120.0.4)
   - Xamarin.Firebase.Database (120.0.1)
   - Xamarin.Firebase.Storage (120.0.1)
   - Xamarin.GooglePlayServices.Base (117.6.0)
   - Xamarin.Forms (5.0.0.2196)
```

---

## 🔍 Verify Package Installation

### Check if Packages are Restored

1. **In Solution Explorer:**
   - Expand "Dependencies" or "References" under AppBantayBarangay.Android
   - Look for yellow warning icons
   - If you see warnings, packages are not restored

2. **In Package Manager Console:**
   ```powershell
   Get-Package -ProjectName AppBantayBarangay.Android
   ```
   Should list all installed packages

3. **Check packages folder:**
   ```
   C:\Users\ASUS_VX16\.nuget\packages\
   ```
   Should contain folders for:
   - newtonsoft.json
   - xamarin.firebase.auth
   - xamarin.firebase.database
   - xamarin.firebase.storage
   - xamarin.googleplayservices.base

---

## 🎯 Specific Error Fixes

### Error: "The name 'JsonConvert' does not exist"
**Cause:** Newtonsoft.Json package not restored  
**Fix:**
```powershell
dotnet add package Newtonsoft.Json --version 13.0.3
```

### Error: "The name 'FirebaseAuth' does not exist"
**Cause:** Firebase packages not restored  
**Fix:**
```powershell
dotnet add package Xamarin.Firebase.Auth --version 120.0.4
dotnet add package Xamarin.Firebase.Database --version 120.0.1
dotnet add package Xamarin.Firebase.Storage --version 120.0.1
```

### Error: "The type or namespace name 'Gms' does not exist"
**Cause:** Google Play Services not restored  
**Fix:**
```powershell
dotnet add package Xamarin.GooglePlayServices.Base --version 117.6.0
```

### Error: "The type or namespace name 'Forms' does not exist in the namespace 'Xamarin'"
**Cause:** Xamarin.Forms package not restored  
**Fix:**
```powershell
dotnet add package Xamarin.Forms --version 5.0.0.2196
```

---

## 📋 Required Packages Checklist

Verify these packages are in `AppBantayBarangay.Android.csproj`:

- [ ] Xamarin.Forms (5.0.0.2196)
- [ ] Xamarin.Essentials (1.7.0)
- [ ] Xamarin.Forms.Maps (5.0.0.2196)
- [ ] Xamarin.Firebase.Auth (120.0.4)
- [ ] Xamarin.Firebase.Database (120.0.1)
- [ ] Xamarin.Firebase.Storage (120.0.1)
- [ ] Xamarin.GooglePlayServices.Base (117.6.0)
- [ ] Newtonsoft.Json (13.0.3)
- [ ] Microsoft.CSharp (4.7.0)
- [ ] Xamarin.AndroidX.Core (1.9.0)
- [ ] Xamarin.AndroidX.AppCompat (1.4.1)
- [ ] Xamarin.AndroidX.Browser (1.4.0)
- [ ] Xamarin.AndroidX.MediaRouter (1.2.6)
- [ ] Xamarin.AndroidX.RecyclerView (1.2.1.1)

---

## 🔧 Nuclear Option (Last Resort)

If nothing else works:

### Step 1: Delete Everything
```powershell
# Close Visual Studio
# Delete these folders:
Remove-Item "C:\Users\ASUS_VX16\source\repos\AppBantayBarangay\AppBantayBarangay\AppBantayBarangay.Android\bin" -Recurse -Force
Remove-Item "C:\Users\ASUS_VX16\source\repos\AppBantayBarangay\AppBantayBarangay\AppBantayBarangay.Android\obj" -Recurse -Force
Remove-Item "C:\Users\ASUS_VX16\source\repos\AppBantayBarangay\AppBantayBarangay\AppBantayBarangay\bin" -Recurse -Force
Remove-Item "C:\Users\ASUS_VX16\source\repos\AppBantayBarangay\AppBantayBarangay\AppBantayBarangay\obj" -Recurse -Force
Remove-Item "C:\Users\ASUS_VX16\source\repos\AppBantayBarangay\AppBantayBarangay\.vs" -Recurse -Force
```

### Step 2: Clear All Caches
```powershell
dotnet nuget locals all --clear
```

### Step 3: Delete packages folder (EXTREME)
```powershell
# WARNING: This will delete ALL NuGet packages for ALL projects
Remove-Item "C:\Users\ASUS_VX16\.nuget\packages" -Recurse -Force
```

### Step 4: Restore Everything
```powershell
cd C:\Users\ASUS_VX16\source\repos\AppBantayBarangay\AppBantayBarangay
dotnet restore
```

### Step 5: Reopen and Rebuild
```
1. Open Visual Studio
2. Open solution
3. Right-click Solution → Restore NuGet Packages
4. Build → Rebuild Solution
```

---

## 📊 Troubleshooting Checklist

Work through this checklist:

- [ ] Visual Studio is closed
- [ ] bin and obj folders deleted
- [ ] NuGet cache cleared (`dotnet nuget locals all --clear`)
- [ ] Packages restored (`dotnet restore`)
- [ ] Visual Studio reopened
- [ ] Packages restored in Visual Studio (Right-click Solution → Restore NuGet Packages)
- [ ] Solution rebuilt (Build → Rebuild Solution)
- [ ] No yellow warning icons in Solution Explorer
- [ ] Error List shows 0 errors

---

## 🎯 Expected Result

After fixing, you should see:
- ✅ Build Output: "Build succeeded"
- ✅ Error List: 0 Errors
- ✅ No yellow warning icons in Solution Explorer
- ✅ IntelliSense recognizes Firebase, JsonConvert, etc.

---

## 📞 Still Having Issues?

### Check Visual Studio Version
```
Help → About Microsoft Visual Studio
```
Ensure you have:
- Visual Studio 2019 (16.11+) or Visual Studio 2022
- .NET workload installed
- Mobile development with .NET workload installed

### Check .NET SDK
```powershell
dotnet --version
```
Should show a version number (e.g., 6.0.x or 7.0.x)

### Check Internet Connection
NuGet package restore requires internet access to download packages from nuget.org

---

## ✨ Summary

**The Problem:** NuGet packages not restored, assemblies not referenced  
**The Solution:** Clean, clear cache, restore packages, rebuild  
**The Result:** All assemblies referenced, code compiles successfully  

**Quick Fix:**
1. Close Visual Studio
2. Run `.\Fix-BuildErrors.ps1`
3. Reopen Visual Studio
4. Rebuild Solution

---

*Assembly reference errors fixed*  
*All packages restored*  
*Ready to build!* 🚀
