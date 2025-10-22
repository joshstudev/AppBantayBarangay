# Firebase Xamarin API - Correct Usage Guide

## 🚨 The Problem

Xamarin Firebase bindings (version 120.x) use the **native Android Firebase SDK**, which has a **callback-based API**, not async/await methods like:
- ❌ `GetAsync()` - Doesn't exist
- ❌ `GetValueAsync()` - Doesn't exist  
- ❌ `GetFileAsync()` - Doesn't exist
- ❌ `DownloadUrl` property - Doesn't exist

---

## ✅ The Solution

Use **TaskCompletionSource** with **listeners** to convert callback-based APIs to async/await.

---

## 📋 Correct API Patterns

### 1. Database Read (Get Data)

#### ❌ WRONG - These methods don't exist:
```csharp
await reference.GetAsync()
await reference.GetValueAsync()
```

#### ✅ CORRECT - Use ValueEventListener:
```csharp
public async Task<T> GetDataAsync<T>(string path)
{
    var reference = _database.GetReference(path);
    
    // Create TaskCompletionSource to convert callback to Task
    var tcs = new TaskCompletionSource<DataSnapshot>();
    
    // Use AddListenerForSingleValueEvent with custom listener
    reference.AddListenerForSingleValueEvent(new ValueEventListener(
        snapshot => tcs.TrySetResult(snapshot),
        error => tcs.TrySetException(new Exception(error.Message))
    ));
    
    var snapshot = await tcs.Task;
    
    if (snapshot != null && snapshot.Exists())
    {
        var value = snapshot.Value;
        if (value != null)
        {
            var jsonData = value.ToString();
            return JsonConvert.DeserializeObject<T>(jsonData);
        }
    }
    
    return default(T);
}
```

**Helper Class:**
```csharp
internal class ValueEventListener : Java.Lang.Object, IValueEventListener
{
    private readonly Action<DataSnapshot> _onDataChange;
    private readonly Action<DatabaseError> _onCancelled;

    public ValueEventListener(Action<DataSnapshot> onDataChange, Action<DatabaseError> onCancelled)
    {
        _onDataChange = onDataChange;
        _onCancelled = onCancelled;
    }

    public void OnDataChange(DataSnapshot snapshot)
    {
        _onDataChange?.Invoke(snapshot);
    }

    public void OnCancelled(DatabaseError error)
    {
        _onCancelled?.Invoke(error);
    }
}
```

---

### 2. Database Write (Set Data)

#### ✅ CORRECT - SetValueAsync works:
```csharp
public async Task<bool> SaveDataAsync(string path, object data)
{
    var reference = _database.GetReference(path);
    var jsonData = JsonConvert.SerializeObject(data);
    
    await reference.SetValueAsync(jsonData).ConfigureAwait(false);
    
    return true;
}
```

**Note:** `SetValueAsync()` is one of the few async methods that actually exists!

---

### 3. Storage Upload

#### ✅ CORRECT - PutFile works:
```csharp
var uploadTask = storageRef.PutFile(fileUri);
await uploadTask.ConfigureAwait(false);
```

---

### 4. Get Download URL

#### ❌ WRONG - These don't exist:
```csharp
await storageRef.GetDownloadUrlAsync()
var uri = await storageRef.DownloadUrl
```

#### ✅ CORRECT - Use GetDownloadUrl() with listeners:
```csharp
public async Task<string> UploadFileAsync(string localPath, string storagePath)
{
    var storageRef = _storage.GetReference(storagePath);
    var fileUri = Android.Net.Uri.FromFile(new Java.IO.File(localPath));
    
    // Upload file
    var uploadTask = storageRef.PutFile(fileUri);
    await uploadTask.ConfigureAwait(false);
    
    // Get download URL using TaskCompletionSource
    var tcs = new TaskCompletionSource<Android.Net.Uri>();
    
    storageRef.GetDownloadUrl()
        .AddOnSuccessListener(new OnSuccessListener(
            uri => tcs.TrySetResult(uri as Android.Net.Uri)
        ))
        .AddOnFailureListener(new OnFailureListener(
            ex => tcs.TrySetException(ex)
        ));
    
    var downloadUri = await tcs.Task;
    return downloadUri?.ToString();
}
```

**Helper Classes:**
```csharp
internal class OnSuccessListener : Java.Lang.Object, Android.Gms.Tasks.IOnSuccessListener
{
    private readonly Action<Java.Lang.Object> _onSuccess;

    public OnSuccessListener(Action<Java.Lang.Object> onSuccess)
    {
        _onSuccess = onSuccess;
    }

    public void OnSuccess(Java.Lang.Object result)
    {
        _onSuccess?.Invoke(result);
    }
}

internal class OnFailureListener : Java.Lang.Object, Android.Gms.Tasks.IOnFailureListener
{
    private readonly Action<Java.Lang.Exception> _onFailure;

    public OnFailureListener(Action<Java.Lang.Exception> onFailure)
    {
        _onFailure = onFailure;
    }

    public void OnFailure(Java.Lang.Exception exception)
    {
        _onFailure?.Invoke(exception);
    }
}
```

---

### 5. Storage Download

#### ❌ WRONG - GetFileAsync doesn't exist:
```csharp
await storageRef.GetFileAsync(localFile)
```

#### ✅ CORRECT - Use GetFile() with listeners:
```csharp
public async Task<string> DownloadFileAsync(string storagePath, string localPath)
{
    var storageRef = _storage.GetReference(storagePath);
    var localFile = new Java.IO.File(localPath);
    
    // Download file using TaskCompletionSource
    var tcs = new TaskCompletionSource<bool>();
    
    storageRef.GetFile(localFile)
        .AddOnSuccessListener(new OnSuccessListener(
            result => tcs.TrySetResult(true)
        ))
        .AddOnFailureListener(new OnFailureListener(
            ex => tcs.TrySetException(ex)
        ));
    
    await tcs.Task;
    return localPath;
}
```

---

## 🔑 Key Concepts

### 1. TaskCompletionSource Pattern

This is the standard pattern for converting callback-based APIs to async/await:

```csharp
// Create a TaskCompletionSource
var tcs = new TaskCompletionSource<TResult>();

// Attach listeners
someObject.AddOnSuccessListener(new OnSuccessListener(
    result => tcs.TrySetResult(result)
));

someObject.AddOnFailureListener(new OnFailureListener(
    ex => tcs.TrySetException(ex)
));

// Await the task
var result = await tcs.Task;
```

### 2. Listener Pattern

Firebase Android SDK uses listeners for async operations:

- **IValueEventListener** - For database reads
- **IOnSuccessListener** - For successful operations
- **IOnFailureListener** - For failed operations

### 3. Java Interop

Since Xamarin wraps Java/Android APIs:
- Inherit from `Java.Lang.Object`
- Implement Java interfaces (e.g., `IValueEventListener`)
- Use `Java.IO.File` instead of `System.IO.File` for Firebase

---

## 📋 Complete Method Reference

### Firebase Database

| Operation | Method | Pattern |
|-----------|--------|---------|
| Read data | `AddListenerForSingleValueEvent()` | ValueEventListener + TaskCompletionSource |
| Write data | `SetValueAsync()` | ✅ Has async method |
| Update data | `UpdateChildrenAsync()` | ✅ Has async method |
| Delete data | `RemoveValueAsync()` | ✅ Has async method |

### Firebase Storage

| Operation | Method | Pattern |
|-----------|--------|---------|
| Upload | `PutFile()` | Returns awaitable Task |
| Download | `GetFile()` | Listeners + TaskCompletionSource |
| Get URL | `GetDownloadUrl()` | Listeners + TaskCompletionSource |
| Delete | `Delete()` | Listeners + TaskCompletionSource |

### Firebase Auth

| Operation | Method | Pattern |
|-----------|--------|---------|
| Sign in | `SignInWithEmailAndPasswordAsync()` | ✅ Has async method |
| Sign up | `CreateUserWithEmailAndPasswordAsync()` | ✅ Has async method |
| Sign out | `SignOut()` | Synchronous method |

---

## 🎯 Why This Is Necessary

### The Root Cause

1. **Firebase Android SDK** (Java) uses callbacks
2. **Xamarin bindings** wrap the Java SDK
3. **Some methods** have async wrappers (like `SetValueAsync`)
4. **Most methods** don't have async wrappers (like `GetDownloadUrl`)
5. **Solution:** Use `TaskCompletionSource` to create async wrappers

### The Pattern

```
Java Callback API
      ↓
TaskCompletionSource
      ↓
C# async/await
```

---

## ✅ Required Using Statements

```csharp
using System;
using System.Threading.Tasks;
using Android.Gms.Extensions;      // For some extensions
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Storage;
using Newtonsoft.Json;
using Java.IO;                     // For Java.IO.File
```

---

## 🔍 How to Identify Correct Methods

### Step 1: Check IntelliSense
```csharp
storageRef.  // Press Ctrl+Space
```
Look for methods that return:
- `Android.Gms.Tasks.Task`
- Methods with `AddOnSuccessListener`
- Methods with `AddOnFailureListener`

### Step 2: Check Object Browser
```
View → Object Browser
Search: Firebase.Storage.StorageReference
```

### Step 3: Check Android Documentation
Since Xamarin wraps Android SDK:
- [Firebase Android Reference](https://firebase.google.com/docs/reference/android)
- Look for Java method signatures
- Convert to C# equivalents

---

## 🐛 Common Errors and Fixes

### Error: "Does not contain a definition for 'GetAsync'"
**Fix:** Use `AddListenerForSingleValueEvent` with `ValueEventListener`

### Error: "Does not contain a definition for 'DownloadUrl'"
**Fix:** Use `GetDownloadUrl()` with `OnSuccessListener`

### Error: "Cannot convert from 'System.IO.File' to 'Java.IO.File'"
**Fix:** Use `new Java.IO.File(path)` instead of `System.IO.File`

### Error: "TaskCompletionSource never completes"
**Fix:** Ensure you call `TrySetResult()` or `TrySetException()` in listeners

---

## 📚 Additional Resources

### Official Documentation
- [Xamarin Firebase GitHub](https://github.com/xamarin/GooglePlayServicesComponents)
- [Firebase Android SDK](https://firebase.google.com/docs/reference/android)
- [Android Tasks API](https://developers.google.com/android/reference/com/google/android/gms/tasks/Task)

### Xamarin Bindings
- [Xamarin.Firebase.Auth](https://www.nuget.org/packages/Xamarin.Firebase.Auth/)
- [Xamarin.Firebase.Database](https://www.nuget.org/packages/Xamarin.Firebase.Database/)
- [Xamarin.Firebase.Storage](https://www.nuget.org/packages/Xamarin.Firebase.Storage/)

---

## ✨ Summary

**The Problem:**
- Xamarin Firebase uses native Android SDK (callback-based)
- Most methods don't have async/await versions
- Methods like `GetAsync()`, `DownloadUrl` don't exist

**The Solution:**
- Use `TaskCompletionSource` to convert callbacks to Tasks
- Implement listener classes (`ValueEventListener`, `OnSuccessListener`, etc.)
- Use `AddOnSuccessListener` and `AddOnFailureListener` patterns

**The Result:**
- Clean async/await code in your app
- Proper error handling
- Works with Xamarin Firebase 120.x packages

---

*Xamarin Firebase API Guide*  
*For Firebase packages 120.x*  
*Using callback-to-async conversion patterns*
