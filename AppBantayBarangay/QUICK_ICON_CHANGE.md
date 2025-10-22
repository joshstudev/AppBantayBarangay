# Quick Guide: Change App Icon to Logo

## 🚀 Fastest Method (2 Minutes)

### Step 1: Run the Batch File
1. Double-click `copy-logo-to-icons.bat` in the project root
2. Wait for it to complete
3. Press any key to close

### Step 2: Clean and Rebuild
1. Open Visual Studio
2. **Build** → **Clean Solution**
3. **Build** → **Rebuild Solution**

### Step 3: Test
1. **Uninstall** the old app from your device/emulator
2. **Run** the app (F5)
3. Check the home screen for your new icon!

## ✅ What the Script Does

The `copy-logo-to-icons.bat` script:
- Copies your logo.png to all 5 icon locations:
  - mipmap-mdpi/icon.png (48x48)
  - mipmap-hdpi/icon.png (72x72)
  - mipmap-xhdpi/icon.png (96x96)
  - mipmap-xxhdpi/icon.png (144x144)
  - mipmap-xxxhdpi/icon.png (192x192)

## ⚠️ Important Notes

### This is a Quick Solution
- ✅ **Pros**: Fast, easy, works immediately
- ⚠️ **Cons**: Not optimized for different screen sizes

### For Production Apps
Use properly sized icons:
1. Go to https://icon.kitchen/
2. Upload your logo
3. Download Android icons
4. Copy to project
5. See `CHANGE_APP_ICON_GUIDE.md` for details

## 🔍 Troubleshooting

### Icon Doesn't Change?
1. **Uninstall** the old app completely
2. **Restart** the emulator/device
3. **Clean** and **Rebuild** again
4. Run the app

### Script Fails?
1. Make sure you're in the project root directory
2. Check that logo.png exists in drawable folder
3. Run as Administrator if needed

## 📱 Expected Result

After following the steps:
- ✅ App icon on home screen = Your logo
- ✅ App icon in app drawer = Your logo
- ✅ App icon in recent apps = Your logo

## 🎯 Files Modified

The script updates these files:
```
AppBantayBarangay.Android/Resources/
├── mipmap-mdpi/icon.png ← Updated
├── mipmap-hdpi/icon.png ← Updated
├── mipmap-xhdpi/icon.png ← Updated
├── mipmap-xxhdpi/icon.png ← Updated
└── mipmap-xxxhdpi/icon.png ← Updated
```

## 💡 Pro Tip

For the best-looking icon:
1. Use the online tool (icon.kitchen)
2. It creates properly sized icons for each density
3. Your icon will look sharp on all devices

---

**Ready?** Just double-click `copy-logo-to-icons.bat` and follow the steps above!
