# Firebase Quick Reference Card

## 🔥 Firebase Configuration
- **Project**: bantaybarangay
- **Database**: https://bantaybarangay-default-rtdb.asia-southeast1.firebasedatabase.app
- **Storage**: bantaybarangay.firebasestorage.app
- **Package**: com.companyname.appbantaybarangay

## 📦 Get Firebase Service
```csharp
var firebase = DependencyService.Get<IFirebaseService>();
```

## 🔐 Authentication

### Sign Up
```csharp
await firebase.SignUpAsync("email@example.com", "password");
```

### Sign In
```csharp
await firebase.SignInAsync("email@example.com", "password");
```

### Sign Out
```csharp
await firebase.SignOutAsync();
```

### Get User ID
```csharp
string userId = firebase.GetCurrentUserId();
```

### Check Auth Status
```csharp
bool isLoggedIn = firebase.IsAuthenticated();
```

## 💾 Database Operations

### Save Data
```csharp
var data = new { Name = "John", Age = 30 };
await firebase.SaveDataAsync("users/123", data);
```

### Get Data
```csharp
var user = await firebase.GetDataAsync<User>("users/123");
```

## 📁 Storage Operations

### Upload File
```csharp
string url = await firebase.UploadFileAsync(
    "/local/path/image.jpg", 
    "uploads/image.jpg"
);
```

### Download File
```csharp
await firebase.DownloadFileAsync(
    "uploads/image.jpg", 
    "/local/path/image.jpg"
);
```

## 🗂️ Recommended Database Paths

| Data Type | Path Pattern |
|-----------|-------------|
| User Profile | `users/{userId}/profile` |
| User Settings | `users/{userId}/settings` |
| Reports | `reports/{reportId}` |
| User Reports | `reports/{userId}/{reportId}` |
| Barangay Data | `barangays/{barangayId}` |

## 📸 Recommended Storage Paths

| File Type | Path Pattern |
|-----------|-------------|
| Report Images | `reports/{userId}/{reportId}/image.jpg` |
| Profile Pictures | `profiles/{userId}/avatar.jpg` |
| Documents | `documents/{barangayId}/file.pdf` |

## ⚠️ Common Issues

### Build Error: "google-services.json not found"
✅ Ensure file is in `Assets/` folder with Build Action: `GoogleServicesJson`

### Runtime Error: "Firebase not initialized"
✅ Check MainActivity.cs has `InitializeFirebase()` call

### Auth Error: "Sign in failed"
✅ Enable Email/Password auth in Firebase Console

### Network Error
✅ Add `INTERNET` permission to AndroidManifest.xml

## 🔧 Build & Run

1. **Restore NuGet Packages**
   - Right-click solution → Restore NuGet Packages

2. **Clean & Rebuild**
   - Build → Clean Solution
   - Build → Rebuild Solution

3. **Deploy to Device/Emulator**
   - Set Android project as startup
   - Press F5 to run

## 📚 Files to Know

| File | Purpose |
|------|---------|
| `google-services.json` | Firebase configuration |
| `IFirebaseService.cs` | Service interface |
| `FirebaseService.cs` | Android implementation |
| `FirebaseConfig.cs` | Configuration constants |
| `MainActivity.cs` | Firebase initialization |

## 🎯 Example: Complete Report Submission

```csharp
// 1. Get service
var firebase = DependencyService.Get<IFirebaseService>();

// 2. Ensure user is authenticated
if (!firebase.IsAuthenticated())
{
    await firebase.SignInAsync(email, password);
}

// 3. Upload image
string imageUrl = await firebase.UploadFileAsync(
    localImagePath, 
    $"reports/{firebase.GetCurrentUserId()}/image.jpg"
);

// 4. Save report data
var report = new
{
    Title = "Broken Light",
    Description = "Street light not working",
    ImageUrl = imageUrl,
    Timestamp = DateTime.UtcNow.ToString("o"),
    Status = "Pending",
    ReportedBy = firebase.GetCurrentUserId()
};

string reportId = Guid.NewGuid().ToString();
await firebase.SaveDataAsync($"reports/{reportId}", report);
```

## 🔗 Quick Links
- [Full Integration Guide](FIREBASE_INTEGRATION_GUIDE.md)
- [Usage Examples](AppBantayBarangay/Services/FirebaseUsageExample.cs)
- [Firebase Console](https://console.firebase.google.com/project/bantaybarangay)
