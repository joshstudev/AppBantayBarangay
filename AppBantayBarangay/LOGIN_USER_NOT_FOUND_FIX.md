# Login "User Not Found" Error - Fix Guide

## 🐛 Problem

After updates, existing user accounts cannot login - shows "User not found" error.

---

## 🔍 Root Cause

The `UserType` field format changed:
- **Old format**: Stored as number (0 = Official, 1 = Resident)
- **New format**: Stored as string ("Official", "Resident")

Existing users have numbers, but code expects strings, causing deserialization to fail.

---

## ✅ SOLUTION 1: Update Existing Users in Firebase (Recommended)

### Step 1: Open Firebase Console

```
1. Go to https://console.firebase.google.com
2. Select "bantaybarangay" project
3. Realtime Database → Data
4. Navigate to "users"
```

### Step 2: Update Each User's UserType

For each user:

```
1. Click on the user node
2. Find "UserType" field
3. Click on the value
4. Change:
   - From: 0  →  To: "Official"
   - From: 1  →  To: "Resident"
5. Press Enter to save
```

### Step 3: Test Login

```
1. Restart the app
2. Try logging in
3. Should work now
```

---

## ✅ SOLUTION 2: Make Code Handle Both Formats

Update the User model to handle both number and string formats:

### Update User.cs

```csharp
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AppBantayBarangay.Models
{
    public class User
    {
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        
        // Handle both number (0,1) and string ("Official","Resident") formats
        [JsonConverter(typeof(FlexibleEnumConverter))]
        public UserType UserType { get; set; }
        
        public string CreatedAt { get; set; }
        
        public string FullName => $"{FirstName} {MiddleName} {LastName}".Replace("  ", " ").Trim();
    }
    
    // Custom converter to handle both formats
    public class FlexibleEnumConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(UserType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Integer)
            {
                // Handle number format (0 or 1)
                var value = Convert.ToInt32(reader.Value);
                return (UserType)value;
            }
            else if (reader.TokenType == JsonToken.String)
            {
                // Handle string format ("Official" or "Resident")
                var value = reader.Value.ToString();
                if (Enum.TryParse<UserType>(value, true, out var result))
                {
                    return result;
                }
            }
            
            return UserType.Resident; // Default
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            // Always write as string
            writer.WriteValue(value.ToString());
        }
    }
}
```

---

## ✅ SOLUTION 3: Quick Firebase Console Fix

### For Each User:

**Official User:**
```json
{
  "UserType": "Official"  ← Change from 0 to "Official"
}
```

**Resident User:**
```json
{
  "UserType": "Resident"  ← Change from 1 to "Resident"
}
```

---

## 🔍 How to Check Current Format

### In Firebase Console:

```
1. Database → Data → users → {userId}
2. Look at UserType field:
   - If shows: 0 or 1 → Old format (needs update)
   - If shows: "Official" or "Resident" → New format (OK)
```

---

## 🧪 Testing

### Test 1: Check Firebase Data

```
1. Open Firebase Console
2. Check a user's UserType
3. Should be string, not number
```

### Test 2: Try Login

```
1. Open app
2. Select user type
3. Enter email and password
4. Click LOGIN
5. Should work
```

### Test 3: Check Output Window

```
Look for:
[GetData] ✅ Successfully deserialized to User
User profile loaded: [Name]
```

---

## 📊 Quick Reference

| Old Value | New Value | User Type |
|-----------|-----------|-----------|
| 0 | "Official" | Official |
| 1 | "Resident" | Resident |

---

## 🎯 Fastest Fix

**If you only have a few users:**

1. **Firebase Console → Database → users**
2. **For each user, change UserType:**
   - 0 → "Official"
   - 1 → "Resident"
3. **Done!**

---

## 💡 Prevention

For new users, the code now automatically saves UserType as string, so this won't happen again.

---

*Login issue fix guide*  
*Update UserType format in Firebase!* 🔧
