# Splash Screen Implementation

## ✅ What Was Added

A professional splash screen that displays when the app launches, showing your logo on a blue background for 2 seconds before transitioning to the main app.

## 📁 Files Created/Modified

### New Files:
1. **`SplashActivity.cs`** - The splash screen activity
2. **`Resources/drawable/splash_screen.xml`** - Splash screen layout

### Modified Files:
1. **`MainActivity.cs`** - Changed `MainLauncher = false`
2. **`Resources/values/styles.xml`** - Added SplashTheme
3. **`AppBantayBarangay.Android.csproj`** - Added new files

## 🎨 Splash Screen Design

### Current Design:
- **Background**: Blue (#007AFF - matches app theme)
- **Logo**: Centered on screen
- **Duration**: 2 seconds
- **Transition**: Smooth to LoginPage

### Visual:
```
┌─────────────────────┐
│                     │
│                     │
│                     │
│       [LOGO]        │
│                     │
│                     │
│                     │
└─────────────────────┘
     Blue Background
```

## 🔧 How It Works

### 1. App Launch Sequence:
```
User taps app icon
    ↓
SplashActivity launches (MainLauncher = true)
    ↓
Shows splash screen with logo
    ↓
Waits 2 seconds
    ↓
Launches MainActivity
    ↓
Shows LoginPage
```

### 2. Technical Flow:
- **SplashActivity** is set as `MainLauncher = true`
- Uses `SplashTheme` which displays `splash_screen.xml`
- After 2 seconds, starts `MainActivity`
- `NoHistory = true` prevents back button from returning to splash

## 📝 Code Breakdown

### SplashActivity.cs
```csharp
[Activity(
    Label = "Bantay Barangay",
    Theme = "@style/SplashTheme",      // Uses splash theme
    MainLauncher = true,                // First activity to launch
    NoHistory = true,                   // Don't keep in back stack
    Icon = "@mipmap/icon")]
public class SplashActivity : AppCompatActivity
{
    protected override void OnResume()
    {
        base.OnResume();
        
        Task.Run(async () =>
        {
            await Task.Delay(2000);     // Wait 2 seconds
            StartActivity(new Intent(Application.Context, typeof(MainActivity)));
            Finish();                    // Close splash
        });
    }
}
```

### splash_screen.xml
```xml
<layer-list>
    <!-- Blue background -->
    <item>
        <color android:color="#007AFF"/>
    </item>
    
    <!-- Centered logo -->
    <item>
        <bitmap android:src="@drawable/logo" android:gravity="center"/>
    </item>
</layer-list>
```

### SplashTheme (styles.xml)
```xml
<style name="SplashTheme" parent="Theme.AppCompat.Light.NoActionBar">
    <item name="android:windowBackground">@drawable/splash_screen</item>
    <item name="android:windowNoTitle">true</item>
    <item name="android:windowFullscreen">true</item>
</style>
```

## 🎯 Customization Options

### Change Splash Duration
In `SplashActivity.cs`, line ~30:
```csharp
await Task.Delay(2000);  // Change 2000 to desired milliseconds
```

Examples:
- `1000` = 1 second
- `3000` = 3 seconds
- `1500` = 1.5 seconds

### Change Background Color
In `splash_screen.xml`, line ~4:
```xml
<color android:color="#007AFF"/>  <!-- Change color here -->
```

Color options:
- `#007AFF` - Blue (current)
- `#FFFFFF` - White
- `#000000` - Black
- `#34C759` - Green
- `#FF3B30` - Red

### Change Logo Size
In `splash_screen.xml`, modify the bitmap item:
```xml
<item android:width="200dp" android:height="200dp">
    <bitmap 
        android:src="@drawable/logo" 
        android:gravity="center"/>
</item>
```

### Add Text Below Logo
Modify `splash_screen.xml`:
```xml
<layer-list>
    <!-- Background -->
    <item>
        <color android:color="#007AFF"/>
    </item>
    
    <!-- Logo -->
    <item android:top="200dp" android:bottom="300dp">
        <bitmap android:src="@drawable/logo" android:gravity="center"/>
    </item>
    
    <!-- Text (requires creating a text drawable) -->
    <!-- Or use a custom layout instead -->
</layer-list>
```

## 🚀 Advanced: Custom Splash Layout

For more control (text, animations, etc.), create a custom layout:

### 1. Create `Resources/layout/splash_screen.xml`:
```xml
<?xml version="1.0" encoding="utf-8"?>
<RelativeLayout xmlns:android="http://schemas.android.com/apk/res/android"
    android:layout_width="match_parent"
    android:layout_height="match_parent"
    android:background="#007AFF">
    
    <ImageView
        android:layout_width="200dp"
        android:layout_height="200dp"
        android:layout_centerInParent="true"
        android:src="@drawable/logo"/>
    
    <TextView
        android:layout_width="wrap_content"
        android:layout_height="wrap_content"
        android:layout_alignParentBottom="true"
        android:layout_centerHorizontal="true"
        android:layout_marginBottom="50dp"
        android:text="Bantay Barangay"
        android:textColor="#FFFFFF"
        android:textSize="24sp"
        android:textStyle="bold"/>
</RelativeLayout>
```

### 2. Update `SplashActivity.cs`:
```csharp
protected override void OnCreate(Bundle savedInstanceState)
{
    base.OnCreate(savedInstanceState);
    SetContentView(Resource.Layout.splash_screen);  // Use custom layout
}
```

## 📱 Testing

### What to Test:
1. ✅ Splash screen appears when app launches
2. ✅ Logo is centered and visible
3. ✅ Background color is correct
4. ✅ Transitions to LoginPage after 2 seconds
5. ✅ Back button doesn't return to splash
6. ✅ App icon is correct

### Testing Steps:
1. **Clean and Rebuild** the solution
2. **Uninstall** old app from device/emulator
3. **Run** the app (F5)
4. **Observe** splash screen on launch
5. **Wait** for transition to LoginPage

## 🎨 Design Recommendations

### Professional Splash Screens:
- ✅ Keep it simple (logo + background)
- ✅ Use brand colors
- ✅ 1-3 seconds duration
- ✅ Smooth transition
- ❌ Avoid too much text
- ❌ Avoid animations (can be slow)
- ❌ Don't make it too long

### Accessibility:
- Ensure logo has good contrast with background
- Don't rely on color alone for information
- Keep duration reasonable (not too long)

## 🔍 Troubleshooting

### Splash Screen Doesn't Appear
1. Check `SplashActivity` has `MainLauncher = true`
2. Check `MainActivity` has `MainLauncher = false`
3. Clean and rebuild solution
4. Uninstall old app completely

### Logo Not Showing
1. Verify `logo.png` exists in `drawable` folder
2. Check it's included in .csproj as AndroidResource
3. Rebuild solution

### App Crashes on Launch
1. Check debug output for errors
2. Verify all files are included in .csproj
3. Check `splash_screen.xml` syntax

### Splash Shows Too Long/Short
1. Adjust `Task.Delay(2000)` in SplashActivity
2. Rebuild and test

## 📊 Performance Notes

### Why This Approach:
- **Fast**: Uses drawable (no layout inflation)
- **Smooth**: No loading delays
- **Native**: Feels like a native Android app
- **Simple**: Easy to maintain

### Alternative Approaches:
1. **Xamarin.Forms Splash**: Slower, less native feel
2. **Custom Layout**: More flexible but slower
3. **Animated Splash**: Cool but can be slow on older devices

## ✅ Verification Checklist

After implementation:
- [ ] Splash screen appears on app launch
- [ ] Logo is centered and visible
- [ ] Background color is correct (#007AFF blue)
- [ ] Duration is 2 seconds
- [ ] Transitions smoothly to LoginPage
- [ ] Back button doesn't return to splash
- [ ] No crashes or errors
- [ ] Works on different screen sizes

## 🎯 Next Steps

### Optional Enhancements:
1. **Add fade animation** when transitioning
2. **Add app version** text at bottom
3. **Add loading indicator** for slow connections
4. **Add tagline** below logo
5. **Create adaptive splash** for different screen sizes

---

**Status**: ✅ Implemented  
**Duration**: 2 seconds  
**Background**: Blue (#007AFF)  
**Logo**: Centered
