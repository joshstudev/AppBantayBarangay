using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Gms.Extensions;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Storage;
using AppBantayBarangay.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Java.Util;
using Android.Runtime;

[assembly: Xamarin.Forms.Dependency(typeof(AppBantayBarangay.Droid.Services.FirebaseService))]
namespace AppBantayBarangay.Droid.Services
{
    /// <summary>
    /// Android implementation of Firebase services using Xamarin-compatible API
    /// </summary>
    public class FirebaseService : IFirebaseService
    {
        private FirebaseAuth _auth;
        private FirebaseDatabase _database;
        private FirebaseStorage _storage;

        public FirebaseService()
        {
            InitializeFirebase();
        }

        private void InitializeFirebase()
        {
            try
            {
                // Firebase is automatically initialized from google-services.json
                _auth = FirebaseAuth.Instance;
                _database = FirebaseDatabase.Instance;
                _storage = FirebaseStorage.Instance;

                // Set database URL from config
                _database.SetPersistenceEnabled(true);
                
                System.Diagnostics.Debug.WriteLine("[Firebase] Initialized successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Firebase] Initialization error: {ex.Message}");
            }
        }

        public async Task<bool> SignInAsync(string email, string password)
        {
            try
            {
                var authResult = await _auth.SignInWithEmailAndPasswordAsync(email, password);
                return authResult?.User != null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Sign in error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> SignUpAsync(string email, string password)
        {
            try
            {
                var authResult = await _auth.CreateUserWithEmailAndPasswordAsync(email, password);
                return authResult?.User != null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Sign up error: {ex.Message}");
                return false;
            }
        }

        public Task SignOutAsync()
        {
            try
            {
                _auth.SignOut();
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Sign out error: {ex.Message}");
                return Task.CompletedTask;
            }
        }

        public string GetCurrentUserId()
        {
            return _auth.CurrentUser?.Uid;
        }

        public bool IsAuthenticated()
        {
            return _auth.CurrentUser != null;
        }

        public async Task<bool> SaveDataAsync(string path, object data)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"[SaveData] Starting save to path: {path}");
                
                var reference = _database.GetReference(path);
                
                // Convert object to JSON then to Java HashMap
                var jsonString = JsonConvert.SerializeObject(data);
                System.Diagnostics.Debug.WriteLine($"[SaveData] JSON length: {jsonString.Length} characters");
                
                // Parse JSON to JObject
                var jObject = JObject.Parse(jsonString);
                
                // Convert to Java HashMap
                var javaMap = ConvertJObjectToHashMap(jObject);
                
                System.Diagnostics.Debug.WriteLine("[SaveData] Converted to Java HashMap");
                
                // Use TaskCompletionSource for async operation
                var tcs = new TaskCompletionSource<bool>();
                
                await reference.SetValue(javaMap)
                    .AddOnSuccessListener(new OnSuccessListener(_ => 
                    {
                        System.Diagnostics.Debug.WriteLine("[SaveData] ✅ Success!");
                        tcs.TrySetResult(true);
                    }))
                    .AddOnFailureListener(new OnFailureListener(ex => 
                    {
                        System.Diagnostics.Debug.WriteLine($"[SaveData] ❌ Failure: {ex.Message}");
                        tcs.TrySetResult(false);
                    }));
                
                var result = await tcs.Task;
                System.Diagnostics.Debug.WriteLine($"[SaveData] Final result: {result}");
                return result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SaveData] ❌ Exception: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[SaveData] Stack: {ex.StackTrace}");
                return false;
            }
        }

        private HashMap ConvertJObjectToHashMap(JObject jObject)
        {
            var map = new HashMap();
            
            foreach (var property in jObject.Properties())
            {
                var value = ConvertJTokenToJavaObject(property.Value);
                map.Put(property.Name, value);
            }
            
            return map;
        }

        private Java.Lang.Object ConvertJTokenToJavaObject(JToken token)
        {
            switch (token.Type)
            {
                case JTokenType.Object:
                    return ConvertJObjectToHashMap((JObject)token);
                    
                case JTokenType.Array:
                    var list = new ArrayList();
                    foreach (var item in (JArray)token)
                    {
                        list.Add(ConvertJTokenToJavaObject(item));
                    }
                    return list;
                    
                case JTokenType.Integer:
                    return Java.Lang.Long.ValueOf(token.Value<long>());
                    
                case JTokenType.Float:
                    return Java.Lang.Double.ValueOf(token.Value<double>());
                    
                case JTokenType.String:
                    return new Java.Lang.String(token.Value<string>());
                    
                case JTokenType.Boolean:
                    return Java.Lang.Boolean.ValueOf(token.Value<bool>());
                    
                case JTokenType.Null:
                    return null;
                    
                default:
                    return new Java.Lang.String(token.ToString());
            }
        }

        public async Task<T> GetDataAsync<T>(string path)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"[GetData] Path: {path}");
                
                var reference = _database.GetReference(path);
                
                var tcs = new TaskCompletionSource<DataSnapshot>();
                
                reference.AddListenerForSingleValueEvent(new ValueEventListener(
                    snapshot => tcs.TrySetResult(snapshot),
                    error => tcs.TrySetException(new Exception(error.Message))
                ));
                
                var snapshot = await tcs.Task;
                
                System.Diagnostics.Debug.WriteLine($"[GetData] Snapshot exists: {snapshot?.Exists()}");
                
                if (snapshot != null && snapshot.Exists())
                {
                    // Build JSON from snapshot children using Iterator
                    var dict = new Dictionary<string, object>();
                    
                    var children = snapshot.Children;
                    var iterator = children.Iterator();
                    
                    while (iterator.HasNext)
                    {
                        var child = iterator.Next() as DataSnapshot;
                        if (child != null)
                        {
                            var key = child.Key;
                            var value = child.Value;
                            
                            System.Diagnostics.Debug.WriteLine($"[GetData] Field: {key} = {value} (type: {value?.GetType().Name})");
                            
                            if (value != null)
                            {
                                // Recursively convert the value (handles nested HashMaps)
                                dict[key] = ConvertJavaValueToNet(value);
                            }
                        }
                    }
                    
                    // Serialize to JSON
                    var jsonString = JsonConvert.SerializeObject(dict);
                    System.Diagnostics.Debug.WriteLine($"[GetData] JSON length: {jsonString.Length}");
                    
                    // Deserialize to target type
                    var result = JsonConvert.DeserializeObject<T>(jsonString);
                    System.Diagnostics.Debug.WriteLine($"[GetData] ✅ Successfully deserialized to {typeof(T).Name}");
                    
                    return result;
                }
                
                System.Diagnostics.Debug.WriteLine("[GetData] Returning default (no data)");
                return default(T);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[GetData] ❌ Error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[GetData] Stack: {ex.StackTrace}");
                return default(T);
            }
        }

        private object ConvertJavaValueToNet(Java.Lang.Object value)
        {
            if (value == null)
                return null;

            System.Diagnostics.Debug.WriteLine($"[Convert] Converting type: {value.GetType().FullName}");

            // Check for HashMap/Map types (including JavaDictionary which wraps HashMap)
            IMap javaMap = null;
            
            if (value is JavaDictionary javaDictionary)
            {
                System.Diagnostics.Debug.WriteLine($"[Convert] Detected JavaDictionary");
                
                // JavaDictionary is a wrapper - we need to get the underlying HashMap
                // Try multiple approaches to get the actual map
                try
                {
                    // Approach 1: Try to enumerate the JavaDictionary directly
                    var dict = new Dictionary<string, object>();
                    int count = 0;
                    
                    foreach (var kvp in javaDictionary)
                    {
                        if (kvp is System.Collections.DictionaryEntry entry)
                        {
                            var key = entry.Key?.ToString();
                            var val = entry.Value;
                            
                            if (key != null && val != null)
                            {
                                // The val is already a Java object, just need to convert it
                                object convertedValue;
                                
                                if (val is Java.Lang.Object javaObj)
                                {
                                    // It's a Java object, use our converter
                                    convertedValue = ConvertJavaValueToNet(javaObj);
                                }
                                else
                                {
                                    // It's already a .NET object, use it directly
                                    convertedValue = val;
                                }
                                
                                dict[key] = convertedValue;
                                count++;
                                System.Diagnostics.Debug.WriteLine($"[Convert]   [{count}] {key} = {convertedValue?.GetType().Name ?? "null"} (raw type: {val.GetType().ToString()})");
                            }
                            else if (key != null)
                            {
                                System.Diagnostics.Debug.WriteLine($"[Convert]   [{count}] {key} = NULL VALUE");
                            }
                        }
                    }
                    
                    System.Diagnostics.Debug.WriteLine($"[Convert] ✅ Converted JavaDictionary to Dictionary with {dict.Count} entries via enumeration");
                    return dict;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[Convert] ⚠️ JavaDictionary enumeration failed: {ex.Message}");
                    // Fall through to try IMap approach
                    javaMap = value as IMap;
                }
            }
            else if (value is HashMap)
            {
                System.Diagnostics.Debug.WriteLine($"[Convert] Detected HashMap");
                javaMap = value as IMap;
            }
            else if (value is IMap)
            {
                System.Diagnostics.Debug.WriteLine($"[Convert] Detected IMap");
                javaMap = value as IMap;
            }
            
            if (javaMap != null)
            {
                System.Diagnostics.Debug.WriteLine($"[Convert] Processing Map with {javaMap.Size()} entries");
                var dict = new Dictionary<string, object>();
                
                try
                {
                    var entrySet = javaMap.EntrySet();
                    if (entrySet != null)
                    {
                        // Cast to ISet to get Iterator
                        var entrySetAsSet = entrySet as ISet;
                        if (entrySetAsSet != null)
                        {
                            var iterator = entrySetAsSet.Iterator();
                            int processedCount = 0;
                            
                            while (iterator.HasNext)
                            {
                                var entry = iterator.Next();
                                if (entry is IMapEntry mapEntry)
                                {
                                    var key = mapEntry.Key;
                                    var mapValue = mapEntry.Value as Java.Lang.Object;
                                    
                                    if (key != null)
                                    {
                                        var keyStr = key.ToString();
                                        var convertedValue = ConvertJavaValueToNet(mapValue);
                                        dict[keyStr] = convertedValue;
                                        processedCount++;
                                        System.Diagnostics.Debug.WriteLine($"[Convert]   [{processedCount}] {keyStr} = {convertedValue?.GetType().Name ?? "null"}");
                                    }
                                }
                            }
                            
                            System.Diagnostics.Debug.WriteLine($"[Convert] ✅ Converted Map to Dictionary with {dict.Count} entries (processed {processedCount})");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"[Convert] ⚠️ EntrySet could not be cast to ISet, trying foreach");
                            
                            // Fallback: try foreach enumeration
                            int processedCount = 0;
                            foreach (var entry in entrySet)
                            {
                                if (entry is IMapEntry mapEntry)
                                {
                                    var key = mapEntry.Key;
                                    var mapValue = mapEntry.Value as Java.Lang.Object;
                                    
                                    if (key != null)
                                    {
                                        var keyStr = key.ToString();
                                        var convertedValue = ConvertJavaValueToNet(mapValue);
                                        dict[keyStr] = convertedValue;
                                        processedCount++;
                                        System.Diagnostics.Debug.WriteLine($"[Convert]   [{processedCount}] {keyStr} = {convertedValue?.GetType().Name ?? "null"}");
                                    }
                                }
                            }
                            
                            System.Diagnostics.Debug.WriteLine($"[Convert] ✅ Converted Map to Dictionary with {dict.Count} entries (processed {processedCount}) via foreach");
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"[Convert] ⚠️ EntrySet is null");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[Convert] ❌ Error iterating map: {ex.Message}");
                    System.Diagnostics.Debug.WriteLine($"[Convert] Stack trace: {ex.StackTrace}");
                }
                
                return dict;
            }

            // ArrayList - recursively convert
            if (value is IList javaList)
            {
                var array = new List<object>();
                for (int idx = 0; idx < javaList.Size(); idx++)
                {
                    var item = javaList.Get(idx) as Java.Lang.Object;
                    array.Add(ConvertJavaValueToNet(item));
                }
                return array;
            }

            // String
            if (value is Java.Lang.String)
                return value.ToString();

            // Numbers
            if (value is Java.Lang.Long lng)
                return lng.LongValue();
            if (value is Java.Lang.Integer intVal)
                return intVal.IntValue();
            if (value is Java.Lang.Double dbl)
                return dbl.DoubleValue();
            if (value is Java.Lang.Float flt)
                return flt.FloatValue();

            // Boolean
            if (value is Java.Lang.Boolean boolVal)
                return boolVal.BooleanValue();

            // Default
            System.Diagnostics.Debug.WriteLine($"[Convert] Unknown type, using ToString()");
            return value.ToString();
        }

        public async Task<string> UploadFileAsync(string localPath, string storagePath)
        {
            try
            {
                var storageRef = _storage.GetReference(storagePath);
                var fileUri = Android.Net.Uri.FromFile(new Java.IO.File(localPath));
                
                // Upload file - UploadTask doesn't support ConfigureAwait
                var uploadTask = storageRef.PutFile(fileUri);
                await uploadTask;
                
                // Get download URL using TaskCompletionSource
                var tcs = new TaskCompletionSource<Android.Net.Uri>();
                
                storageRef.GetDownloadUrl().AddOnSuccessListener(new OnSuccessListener(
                    uri => tcs.TrySetResult(uri as Android.Net.Uri)
                )).AddOnFailureListener(new OnFailureListener(
                    ex => tcs.TrySetException(ex)
                ));
                
                var downloadUri = await tcs.Task;
                return downloadUri?.ToString();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Upload file error: {ex.Message}");
                return null;
            }
        }

        public async Task<string> DownloadFileAsync(string storagePath, string localPath)
        {
            try
            {
                var storageRef = _storage.GetReference(storagePath);
                var localFile = new Java.IO.File(localPath);
                
                // Download file using TaskCompletionSource
                var tcs = new TaskCompletionSource<bool>();
                
                storageRef.GetFile(localFile).AddOnSuccessListener(new OnSuccessListener(
                    result => tcs.TrySetResult(true)
                )).AddOnFailureListener(new OnFailureListener(
                    ex => tcs.TrySetException(ex)
                ));
                
                await tcs.Task;
                return localPath;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Download file error: {ex.Message}");
                return null;
            }
        }
    }

    // Helper class for ValueEventListener
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

    // Helper class for OnSuccessListener
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

    // Helper class for OnFailureListener
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
}
