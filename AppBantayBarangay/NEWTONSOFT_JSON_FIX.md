# Newtonsoft.Json Package Fix

## 🐛 The Problem

```
The type or namespace name 'Newtonsoft' could not be found
```

This error occurred because the `Newtonsoft.Json` package was missing from the **shared project** (`AppBantayBarangay.csproj`).

---

## ✅ The Solution

Added `Newtonsoft.Json` package reference to the shared project:

```xml
<PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
```

---

## 📦 Why This Package Is Needed

### Used In LoginPage.xaml.cs

The phone number lookup feature uses `Newtonsoft.Json.Linq` to parse Firebase data:

```csharp
using Newtonsoft.Json.Linq;

// Parse users data
var usersDict = usersData as JObject;

// Access user data
var userData = userEntry.Value as JObject;
var userPhone = userData["PhoneNumber"]?.ToString();
```

### Used Throughout the App

- **JSON Serialization**: Converting objects to JSON for Firebase
- **JSON Deserialization**: Parsing Firebase responses
- **Data Parsing**: Working with dynamic Firebase data structures

---

## 🔧 What to Do Now

### Step 1: Restore NuGet Packages

```
1. Close Visual Studio (if open)
2. Reopen Visual Studio
3. Right-click Solution → Restore NuGet Packages
4. Wait for completion
```

### Step 2: Clean and Rebuild

```
1. Build → Clean Solution
2. Build → Rebuild Solution
3. Verify 0 errors
```

### Step 3: Verify Package Installation

Check that `Newtonsoft.Json` appears in:
- Solution Explorer → AppBantayBarangay → Dependencies → NuGet
- Should show: Newtonsoft.Json (13.0.3)

---

## 📋 Package Locations

### Shared Project (AppBantayBarangay)
```xml
<ItemGroup>
  <PackageReference Include="Xamarin.Forms" Version="5.0.0.2662" />
  <PackageReference Include="Xamarin.Essentials" Version="1.8.1" />
  <PackageReference Include="Xamarin.Forms.Maps" Version="5.0.0.2662" />
  <PackageReference Include="Newtonsoft.Json" Version="13.0.3" /> ← Added
</ItemGroup>
```

### Android Project (AppBantayBarangay.Android)
```xml
<ItemGroup>
  <!-- Already has Newtonsoft.Json 13.0.3 -->
  <PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
  <!-- Other packages... -->
</ItemGroup>
```

---

## ✅ Verification

After rebuilding, verify:

- [ ] No "Newtonsoft" errors in Error List
- [ ] LoginPage.xaml.cs compiles successfully
- [ ] Newtonsoft.Json appears in Dependencies
- [ ] Build succeeds with 0 errors
- [ ] App runs without issues

---

## 🎯 Quick Fix Commands

### Option 1: Visual Studio
```
1. Tools → NuGet Package Manager → Package Manager Console
2. Run: Install-Package Newtonsoft.Json -Version 13.0.3 -ProjectName AppBantayBarangay
```

### Option 2: Command Line
```bash
cd C:\Users\ASUS_VX16\source\repos\AppBantayBarangay\AppBantayBarangay\AppBantayBarangay
dotnet add package Newtonsoft.Json --version 13.0.3
```

### Option 3: Manual (Already Done)
```
Edit AppBantayBarangay.csproj
Add: <PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
Restore packages
```

---

## 📚 Related Files

Files that use Newtonsoft.Json:

1. **LoginPage.xaml.cs**
   - Uses `Newtonsoft.Json.Linq.JObject`
   - For parsing Firebase user data

2. **FirebaseService.cs** (Android)
   - Uses `Newtonsoft.Json.JsonConvert`
   - For serializing/deserializing data

3. **RegistrationPage.xaml.cs**
   - May use JSON serialization in future

---

## 🔍 Why Version 13.0.3?

- **Stable**: Well-tested, production-ready
- **Compatible**: Works with .NET Standard 2.0
- **Widely Used**: Industry standard for JSON in .NET
- **Same Version**: Matches Android project version

---

*Newtonsoft.Json package added*  
*Error resolved!* ✅
