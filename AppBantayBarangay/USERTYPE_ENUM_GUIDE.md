# UserType Enum Values Guide

## 🔢 Current Issue

Firebase is storing `UserType` as a **number** instead of a **string**.

### Enum Definition

```csharp
public enum UserType
{
    Official,   // = 0
    Resident    // = 1
}
```

### What This Means

| UserType | Enum Value | Stored in Firebase |
|----------|-----------|-------------------|
| Official | 0 | `"UserType": 0` |
| Resident | 1 | `"UserType": 1` |

---

## ✅ Quick Answer

**For Official accounts in Firebase:**
- Set `UserType` to **0** (number)
- Or better: Set to **"Official"** (string)

**For Resident accounts in Firebase:**
- Set `UserType` to **1** (number)
- Or better: Set to **"Resident"** (string)

---

## 🔧 Solution: Store as String (Recommended)

### Option 1: Update User Model (Best Solution)

Add a converter to store UserType as string:

```csharp
// User.cs
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
        
        [JsonConverter(typeof(StringEnumConverter))]
        public UserType UserType { get; set; }  // Now stores as "Official" or "Resident"
        
        public string CreatedAt { get; set; }
        
        public string FullName => $"{FirstName} {MiddleName} {LastName}".Replace("  ", " ").Trim();
    }
}
```

This will automatically convert:
- `UserType.Official` → `"Official"` (string)
- `UserType.Resident` → `"Resident"` (string)

---

## 📋 Creating Official Accounts

### Method 1: Using Number (Current System)

```json
{
  "UserId": "abc123...",
  "Email": "official@bantaybarangay.ph",
  "FirstName": "Juan",
  "LastName": "Dela Cruz",
  "PhoneNumber": "09123456789",
  "Address": "Barangay Hall",
  "UserType": 0,  ← 0 for Official
  "CreatedAt": "2025-01-15T10:00:00Z"
}
```

### Method 2: Using String (Recommended)

```json
{
  "UserId": "abc123...",
  "Email": "official@bantaybarangay.ph",
  "FirstName": "Juan",
  "LastName": "Dela Cruz",
  "PhoneNumber": "09123456789",
  "Address": "Barangay Hall",
  "UserType": "Official",  ← String is clearer
  "CreatedAt": "2025-01-15T10:00:00Z"
}
```

---

## 🔍 How to Check Current Storage

### In Firebase Console

1. **Open Firebase Console**
2. **Realtime Database → Data**
3. **Navigate to `users/{userId}`**
4. **Check `UserType` field**

**If you see:**
- `"UserType": 0` → Official (number format)
- `"UserType": 1` → Resident (number format)
- `"UserType": "Official"` → Official (string format) ✅
- `"UserType": "Resident"` → Resident (string format) ✅

---

## 🔄 Converting Existing Data

### If You Have Existing Users with Numbers

You can convert them manually or with a script:

#### Manual Conversion (Firebase Console)

1. **Navigate to user in database**
2. **Click on `UserType` field**
3. **Change value:**
   - From `0` to `"Official"`
   - From `1` to `"Resident"`
4. **Save**

#### Script Conversion (Firebase Admin SDK)

```javascript
// convert-usertype.js
const admin = require('firebase-admin');
const serviceAccount = require('./serviceAccountKey.json');

admin.initializeApp({
  credential: admin.credential.cert(serviceAccount),
  databaseURL: 'https://bantaybarangay-default-rtdb.asia-southeast1.firebasedatabase.app'
});

async function convertUserTypes() {
  const usersRef = admin.database().ref('users');
  const snapshot = await usersRef.once('value');
  const users = snapshot.val();

  for (const userId in users) {
    const user = users[userId];
    
    // Convert number to string
    if (typeof user.UserType === 'number') {
      const userTypeString = user.UserType === 0 ? 'Official' : 'Resident';
      await usersRef.child(userId).update({
        UserType: userTypeString
      });
      console.log(`Converted ${userId}: ${user.UserType} → ${userTypeString}`);
    }
  }
  
  console.log('Conversion complete!');
}

convertUserTypes();
```

---

## 📊 Comparison Table

| Format | Official | Resident | Pros | Cons |
|--------|----------|----------|------|------|
| **Number** | 0 | 1 | Compact | Not readable, confusing |
| **String** | "Official" | "Resident" | Clear, readable | Slightly larger |

**Recommendation:** Use **String** format for clarity and maintainability.

---

## 🎯 Implementation Steps

### Step 1: Update User Model

Add the `JsonConverter` attribute:

```csharp
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public class User
{
    // ... other properties ...
    
    [JsonConverter(typeof(StringEnumConverter))]
    public UserType UserType { get; set; }
    
    // ... other properties ...
}
```

### Step 2: Rebuild the App

```
Build → Rebuild Solution
```

### Step 3: Test Registration

1. Register a new resident
2. Check Firebase Database
3. Verify `UserType` is stored as `"Resident"` (string)

### Step 4: Create Official Account

In Firebase Console:
```json
{
  "UserType": "Official"  ← Use string, not 0
}
```

### Step 5: Test Login

1. Login as Official
2. Login as Resident
3. Verify both work correctly

---

## 🧪 Testing

### Test 1: Check Existing Data

```
Firebase Console → Database → users
Look at UserType field:
- If number (0 or 1): Old format
- If string ("Official" or "Resident"): New format ✅
```

### Test 2: Register New User

```
1. Register new resident
2. Check Firebase
3. Should see: "UserType": "Resident"
```

### Test 3: Create Official

```
1. Create official in Firebase
2. Use: "UserType": "Official"
3. Test login
4. Should work correctly
```

---

## 🔍 Debugging

### If Login Fails

Check the comparison logic in LoginPage.xaml.cs:

```csharp
// This should work with both number and string
if (user.UserType != selectedUserType)
{
    // Error
}
```

The enum comparison works because:
- `UserType.Official` (enum) == `0` (number) ✅
- `UserType.Official` (enum) == `"Official"` (string after deserialization) ✅

---

## 📝 Quick Reference

### Creating Official Account

**Option 1: Number Format (Works but not recommended)**
```json
{
  "UserType": 0
}
```

**Option 2: String Format (Recommended)**
```json
{
  "UserType": "Official"
}
```

### Creating Resident Account

**Option 1: Number Format (Works but not recommended)**
```json
{
  "UserType": 1
}
```

**Option 2: String Format (Recommended)**
```json
{
  "UserType": "Resident"
}
```

---

## ✅ Best Practices

### 1. Use String Format
```json
"UserType": "Official"  ✅
"UserType": "Resident"  ✅
```

### 2. Add JsonConverter
```csharp
[JsonConverter(typeof(StringEnumConverter))]
public UserType UserType { get; set; }
```

### 3. Consistent Storage
- All new users: String format
- Convert old users: Number → String

### 4. Clear Documentation
- Document that 0 = Official, 1 = Resident
- Or better: Use strings to avoid confusion

---

## 🎯 Summary

**Current System:**
- Official = 0
- Resident = 1

**Recommended System:**
- Official = "Official"
- Resident = "Resident"

**To Create Official Account:**
- Use `"UserType": 0` (works now)
- Or better: Use `"UserType": "Official"` (clearer)

**To Fix:**
1. Add `[JsonConverter(typeof(StringEnumConverter))]` to User model
2. Rebuild app
3. New users will use string format
4. Optionally convert existing users

---

*UserType Enum Guide*  
*Official = 0, Resident = 1*  
*Recommended: Use string format for clarity!* 📊
