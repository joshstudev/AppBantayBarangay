# Firebase Integration Guide for AppBantayBarangay

## Overview
This guide explains how Firebase has been integrated into the AppBantayBarangay Xamarin.Forms application using the `google-services.json` configuration file.

## Firebase Configuration

### Project Information
- **Project ID**: bantaybarangay
- **Project Number**: 159926877525
- **Firebase Database URL**: https://bantaybarangay-default-rtdb.asia-southeast1.firebasedatabase.app
- **Storage Bucket**: bantaybarangay.firebasestorage.app
- **API Key**: AIzaSyD7LVRy5gDLvA_7-QbYK5Ar8qJvfasq7f0

### Package Name
The app is configured for package name: `com.companyname.appbantaybarangay`

## Files Added/Modified

### 1. Android Project Configuration
**File**: `AppBantayBarangay.Android/AppBantayBarangay.Android.csproj`
- Added `<GoogleServicesJson Include="Assets\google-services.json" />` to reference the Firebase configuration
- Added Firebase NuGet packages:
  - Xamarin.Firebase.Auth (121.0.8)
  - Xamarin.Firebase.Database (120.3.1)
  - Xamarin.Firebase.Storage (120.3.0)
  - Xamarin.GooglePlayServices.Base (118.5.0)
  - Xamarin.Google.Dagger (2.51.0.1)
  - Newtonsoft.Json (13.0.3)

### 2. Android Manifest
**File**: `AppBantayBarangay.Android/Properties/AndroidManifest.xml`
- Added Internet permissions required for Firebase:
  - `android.permission.INTERNET`
  - `android.permission.ACCESS_NETWORK_STATE`

### 3. MainActivity Initialization
**File**: `AppBantayBarangay.Android/MainActivity.cs`
- Added Firebase initialization in `OnCreate` method
- Firebase is automatically configured from `google-services.json`

### 4. Shared Project Services

#### IFirebaseService Interface
**File**: `AppBantayBarangay/Services/IFirebaseService.cs`
- Defines the contract for Firebase operations
- Methods for authentication, database, and storage operations

#### FirebaseConfig Class
**File**: `AppBantayBarangay/Services/FirebaseConfig.cs`
- Contains Firebase configuration constants extracted from `google-services.json`
- Easy access to project settings throughout the app

#### FirebaseService Implementation
**File**: `AppBantayBarangay.Android/Services/FirebaseService.cs`
- Android-specific implementation of `IFirebaseService`
- Registered with Xamarin.Forms DependencyService
- Handles all Firebase operations

#### Usage Examples
**File**: `AppBantayBarangay/Services/FirebaseUsageExample.cs`
- Demonstrates how to use Firebase services in your app
- Examples for authentication, database, and storage operations

## How to Use Firebase in Your App

### 1. Get the Firebase Service
```csharp
using AppBantayBarangay.Services;
using Xamarin.Forms;

var firebaseService = DependencyService.Get<IFirebaseService>();
```

### 2. User Authentication

#### Sign Up
```csharp
bool success = await firebaseService.SignUpAsync("user@example.com", "password123");
if (success)
{
    // User created successfully
}
```

#### Sign In
```csharp
bool success = await firebaseService.SignInAsync("user@example.com", "password123");
if (success)
{
    string userId = firebaseService.GetCurrentUserId();
    // User signed in successfully
}
```

#### Sign Out
```csharp
await firebaseService.SignOutAsync();
```

#### Check Authentication
```csharp
if (firebaseService.IsAuthenticated())
{
    string userId = firebaseService.GetCurrentUserId();
    // User is logged in
}
```

### 3. Firebase Realtime Database

#### Save Data
```csharp
var report = new
{
    Title = "Broken Street Light",
    Description = "Street light not working",
    Timestamp = DateTime.UtcNow.ToString("o"),
    Status = "Pending"
};

string path = $"reports/{userId}/{reportId}";
bool success = await firebaseService.SaveDataAsync(path, report);
```

#### Get Data
```csharp
string path = $"reports/{userId}/{reportId}";
var report = await firebaseService.GetDataAsync<YourReportModel>(path);
```

### 4. Firebase Storage

#### Upload File
```csharp
string localPath = "/path/to/image.jpg";
string storagePath = $"reports/{userId}/image.jpg";
string downloadUrl = await firebaseService.UploadFileAsync(localPath, storagePath);
```

#### Download File
```csharp
string storagePath = $"reports/{userId}/image.jpg";
string localPath = "/path/to/save/image.jpg";
string savedPath = await firebaseService.DownloadFileAsync(storagePath, localPath);
```

## Firebase Database Structure Recommendation

```
bantaybarangay/
├── users/
│   └── {userId}/
│       ├── profile/
│       │   ├── name
│       │   ├── email
│       │   ├── barangay
│       │   └── role
│       └── settings/
├── reports/
│   └── {reportId}/
│       ├── title
│       ├── description
│       ├── location
│       ├── timestamp
│       ├── status
│       ├── reportedBy
│       ├── assignedTo
│       └── images/
└── barangays/
    └── {barangayId}/
        ├── name
        ├── officials/
        └── statistics/
```

## Firebase Storage Structure Recommendation

```
bantaybarangay.firebasestorage.app/
├── reports/
│   └── {userId}/
│       └── {reportId}/
│           ├── image1.jpg
│           ├── image2.jpg
│           └── ...
├── profiles/
│   └── {userId}/
│       └── avatar.jpg
└── documents/
    └── {barangayId}/
        └── ...
```

## Security Rules

### Realtime Database Rules (to be set in Firebase Console)
```json
{
  "rules": {
    "users": {
      "$uid": {
        ".read": "$uid === auth.uid",
        ".write": "$uid === auth.uid"
      }
    },
    "reports": {
      ".read": "auth != null",
      "$reportId": {
        ".write": "auth != null"
      }
    }
  }
}
```

### Storage Rules (to be set in Firebase Console)
```
rules_version = '2';
service firebase.storage {
  match /b/{bucket}/o {
    match /reports/{userId}/{allPaths=**} {
      allow read: if request.auth != null;
      allow write: if request.auth != null && request.auth.uid == userId;
    }
    match /profiles/{userId}/{allPaths=**} {
      allow read: if request.auth != null;
      allow write: if request.auth != null && request.auth.uid == userId;
    }
  }
}
```

## Next Steps

1. **Restore NuGet Packages**: Open the solution in Visual Studio and restore all NuGet packages
2. **Build the Project**: Build the Android project to ensure all Firebase packages are properly installed
3. **Set Firebase Rules**: Configure security rules in the Firebase Console
4. **Test Authentication**: Test user sign-up and sign-in functionality
5. **Test Database**: Test saving and retrieving data from Firebase Realtime Database
6. **Test Storage**: Test uploading and downloading files from Firebase Storage

## Troubleshooting

### Build Errors
- Ensure all NuGet packages are restored
- Clean and rebuild the solution
- Check that `google-services.json` is in the Assets folder with Build Action set to `GoogleServicesJson`

### Runtime Errors
- Verify Firebase is initialized in MainActivity
- Check that Internet permissions are granted
- Ensure the package name in AndroidManifest.xml matches the one in google-services.json
- Check Firebase Console for any configuration issues

### Authentication Issues
- Verify Email/Password authentication is enabled in Firebase Console
- Check that the API key is correct
- Ensure the app is connected to the internet

## Additional Resources

- [Firebase Documentation](https://firebase.google.com/docs)
- [Xamarin Firebase Documentation](https://docs.microsoft.com/xamarin/android/data-cloud/google-messaging/firebase-cloud-messaging)
- [Firebase Console](https://console.firebase.google.com/)

## Support

For issues specific to this integration, check:
1. Firebase Console for project configuration
2. Visual Studio Output window for error messages
3. Android Logcat for runtime errors
