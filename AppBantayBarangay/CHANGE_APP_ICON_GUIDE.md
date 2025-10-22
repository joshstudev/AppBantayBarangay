# Change App Icon to Logo - Complete Guide

## 📱 Overview

To change the app icon to your logo, you need to create different sizes of your logo for different Android screen densities.

## 🎯 Required Icon Sizes

Android requires these icon sizes:

| Density | Folder | Size (pixels) |
|---------|--------|---------------|
| mdpi | mipmap-mdpi | 48 x 48 |
| hdpi | mipmap-hdpi | 72 x 72 |
| xhdpi | mipmap-xhdpi | 96 x 96 |
| xxhdpi | mipmap-xxhdpi | 144 x 144 |
| xxxhdpi | mipmap-xxxhdpi | 192 x 192 |

## 🚀 Quick Method: Use Online Tool

### Option 1: Android Asset Studio (Recommended)
1. Go to: https://romannurik.github.io/AndroidAssetStudio/icons-launcher.html
2. Upload your logo.png
3. Adjust padding/scaling as needed
4. Download the generated icon pack
5. Extract and copy the mipmap folders to your project

### Option 2: AppIcon.co
1. Go to: https://www.appicon.co/
2. Upload your logo.png (should be at least 1024x1024)
3. Select "Android" platform
4. Download the generated icons
5. Copy to your project

## 📂 Manual Method: Resize Logo

If you want to do it manually:

### Using Image Editing Software (Photoshop, GIMP, etc.):

1. Open your logo.png
2. Create 5 versions with these sizes:
   - 48x48 → Save to `mipmap-mdpi/icon.png`
   - 72x72 → Save to `mipmap-hdpi/icon.png`
   - 96x96 → Save to `mipmap-xhdpi/icon.png`
   - 144x144 → Save to `mipmap-xxhdpi/icon.png`
   - 192x192 → Save to `mipmap-xxxhdpi/icon.png`

### Using PowerShell Script (Windows):

Save this as `resize-icon.ps1` and run it:

```powershell
# Requires ImageMagick installed
# Install: choco install imagemagick

$sourceLogo = "C:\Users\ASUS_VX16\source\repos\AppBantayBarangay\AppBantayBarangay\AppBantayBarangay.Android\Resources\drawable\logo.png"
$projectPath = "C:\Users\ASUS_VX16\source\repos\AppBantayBarangay\AppBantayBarangay\AppBantayBarangay.Android\Resources"

$sizes = @{
    "mipmap-mdpi" = 48
    "mipmap-hdpi" = 72
    "mipmap-xhdpi" = 96
    "mipmap-xxhdpi" = 144
    "mipmap-xxxhdpi" = 192
}

foreach ($folder in $sizes.Keys) {
    $size = $sizes[$folder]
    $outputPath = Join-Path $projectPath "$folder\icon.png"
    
    Write-Host "Creating $size x $size icon for $folder..."
    magick convert $sourceLogo -resize "${size}x${size}" $outputPath
}

Write-Host "Done! All icons created."
```

## 🔧 Step-by-Step Instructions

### Step 1: Prepare Your Logo

Your logo should be:
- ✅ Square (1:1 aspect ratio)
- ✅ At least 512x512 pixels (1024x1024 recommended)
- ✅ PNG format with transparency
- ✅ Clear and recognizable when small

### Step 2: Generate Icon Sizes

**Easiest Method - Use Online Tool:**

1. Go to https://icon.kitchen/
2. Click "Upload Image"
3. Select your logo.png
4. Choose "Android" platform
5. Adjust padding if needed
6. Click "Download"
7. Extract the ZIP file

### Step 3: Copy Icons to Project

1. Navigate to the extracted folder
2. Find the `android` folder inside
3. Copy all `mipmap-*` folders
4. Paste into: `AppBantayBarangay.Android\Resources\`
5. **Overwrite** the existing icon.png files

### Step 4: Update Project File (Already Done)

The icons are already referenced in the .csproj file:
```xml
<AndroidResource Include="Resources\mipmap-mdpi\icon.png" />
<AndroidResource Include="Resources\mipmap-hdpi\icon.png" />
<AndroidResource Include="Resources\mipmap-xhdpi\icon.png" />
<AndroidResource Include="Resources\mipmap-xxhdpi\icon.png" />
<AndroidResource Include="Resources\mipmap-xxxhdpi\icon.png" />
```

### Step 5: Clean and Rebuild

1. **Build** → **Clean Solution**
2. **Build** → **Rebuild Solution**
3. **Uninstall** the old app from emulator/device
4. **Run** the app

## 🎨 Icon Design Tips

### Best Practices:
- ✅ Use simple, recognizable design
- ✅ Ensure logo is visible at small sizes
- ✅ Use transparent background
- ✅ Center the logo with some padding
- ✅ Test on different backgrounds (light/dark)

### Avoid:
- ❌ Too much detail (won't be visible when small)
- ❌ Thin lines (may disappear at small sizes)
- ❌ Text (usually unreadable in icons)
- ❌ White background (use transparency)

## 📱 Adaptive Icons (Android 8.0+)

For modern Android, you can also create adaptive icons:

### Files to Update:
- `mipmap-anydpi-v26/icon.xml`
- `mipmap-*/launcher_foreground.png`

### icon.xml structure:
```xml
<?xml version="1.0" encoding="utf-8"?>
<adaptive-icon xmlns:android="http://schemas.android.com/apk/res/android">
    <background android:drawable="@color/ic_launcher_background"/>
    <foreground android:drawable="@mipmap/launcher_foreground"/>
</adaptive-icon>
```

## 🔍 Verification

After rebuilding and running:

1. **Home Screen**: Check the app icon
2. **App Drawer**: Verify icon appears correctly
3. **Recent Apps**: Check icon in task switcher
4. **Settings**: Verify in app settings

## 🛠️ Quick Fix: Use Logo Directly (Simple Approach)

If you just want a quick solution without resizing:

### PowerShell Script to Copy Logo:
```powershell
$logo = "C:\Users\ASUS_VX16\source\repos\AppBantayBarangay\AppBantayBarangay\AppBantayBarangay.Android\Resources\drawable\logo.png"
$base = "C:\Users\ASUS_VX16\source\repos\AppBantayBarangay\AppBantayBarangay\AppBantayBarangay.Android\Resources"

Copy-Item $logo "$base\mipmap-mdpi\icon.png" -Force
Copy-Item $logo "$base\mipmap-hdpi\icon.png" -Force
Copy-Item $logo "$base\mipmap-xhdpi\icon.png" -Force
Copy-Item $logo "$base\mipmap-xxhdpi\icon.png" -Force
Copy-Item $logo "$base\mipmap-xxxhdpi\icon.png" -Force

Write-Host "Logo copied to all mipmap folders!"
```

**Note**: This will work but the icon won't be optimized for different screen densities.

## 📊 File Locations

Your current structure:
```
AppBantayBarangay.Android/Resources/
├── drawable/
│   └── logo.png (670 KB - source logo)
├── mipmap-mdpi/
│   └── icon.png (replace this)
├── mipmap-hdpi/
│   └── icon.png (replace this)
├── mipmap-xhdpi/
│   └── icon.png (replace this)
├── mipmap-xxhdpi/
│   └── icon.png (replace this)
└── mipmap-xxxhdpi/
    └── icon.png (replace this)
```

## ⚠️ Important Notes

1. **Uninstall Old App**: Always uninstall the old version before testing new icon
2. **Clean Build**: Always clean and rebuild after changing icons
3. **Cache**: Sometimes emulator caches icons - restart emulator if needed
4. **File Names**: Must be exactly `icon.png` (lowercase)
5. **Format**: Must be PNG format

## 🎯 Recommended Workflow

1. ✅ Use online tool (icon.kitchen or Android Asset Studio)
2. ✅ Upload your logo.png
3. ✅ Download generated icons
4. ✅ Copy to project mipmap folders
5. ✅ Clean and rebuild
6. ✅ Uninstall old app
7. ✅ Run and verify

## 🔗 Useful Tools

- **Icon Kitchen**: https://icon.kitchen/ (Easiest)
- **Android Asset Studio**: https://romannurik.github.io/AndroidAssetStudio/
- **AppIcon.co**: https://www.appicon.co/
- **MakeAppIcon**: https://makeappicon.com/

---

**Next Steps**: Choose a method above and follow the instructions to replace your app icon with your logo!
