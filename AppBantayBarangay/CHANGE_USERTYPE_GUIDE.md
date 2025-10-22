# How to Change UserType in Firebase Realtime Database

## 🎯 Overview

This guide explains how to change a user's `UserType` from **Resident** to **Official** (or vice versa) in Firebase Realtime Database.

---

## 📋 Quick Answer

### To Change Resident → Official

**In Firebase Console:**
1. Go to Realtime Database → Data
2. Navigate to `users/{userId}`
3. Click on `UserType` field
4. Change value from `1` or `"Resident"` to `0` or `"Official"`
5. Press Enter to save

---

## 🔧 Method 1: Using Firebase Console (Easiest)

### Step-by-Step Instructions

#### **Step 1: Open Firebase Console**
```
1. Go to https://console.firebase.google.com
2. Select your project: bantaybarangay
3. Click "Realtime Database" in the left menu
4. Click "Data" tab
```

#### **Step 2: Find the User**
```
1. Expand the "users" node
2. Find the user you want to change
   - Look for their email or name in the data
   - Or search by User ID if you know it
```

#### **Step 3: Change UserType**

**Option A: If UserType is stored as Number**
```
1. Click on the UserType field
2. Current value: 1 (Resident)
3. Change to: 0 (Official)
4. Press Enter to save
```

**Option B: If UserType is stored as String**
```
1. Click on the UserType field
2. Current value: "Resident"
3. Change to: "Official"
4. Press Enter to save
```

#### **Step 4: Verify the Change**
```
1. Refresh the database view
2. Verify UserType shows new value
3. Test login with the account
4. Should now access Official pages
```

---

## 🖼️ Visual Guide

### Firebase Console Navigation

```
Firebase Console
└── Realtime Database
    └── Data
        └── users
            └── {userId}
                ├── UserId: "abc123..."
                ├── Email: "user@example.com"
                ├── FirstName: "Juan"
                ├── LastName: "Dela Cruz"
                ├── PhoneNumber: "09123456789"
                ├── Address: "123 Main St"
                ├── UserType: 1  ← Click here to edit
                └── CreatedAt: "2025-01-15..."
```

### Editing UserType

```
Before:
UserType: 1  (Resident)
         ↓
Click on the value
         ↓
Edit field appears
         ↓
Change to: 0  (Official)
         ↓
Press Enter
         ↓
After:
UserType: 0  (Official) ✅
```

---

## 📊 UserType Values Reference

### Number Format

| Value | User Type | Access Level |
|-------|-----------|--------------|
| **0** | Official | OfficialPage, Admin features |
| **1** | Resident | ResidentPage, Basic features |

### String Format (Recommended)

| Value | User Type | Access Level |
|-------|-----------|--------------|
| **"Official"** | Official | OfficialPage, Admin features |
| **"Resident"** | Resident | ResidentPage, Basic features |

---

## 🔧 Method 2: Using Firebase Admin SDK (Advanced)

### Node.js Script

Create a script to change UserType programmatically:

```javascript
// change-usertype.js
const admin = require('firebase-admin');
const serviceAccount = require('./serviceAccountKey.json');

admin.initializeApp({
  credential: admin.credential.cert(serviceAccount),
  databaseURL: 'https://bantaybarangay-default-rtdb.asia-southeast1.firebasedatabase.app'
});

async function changeUserType(userId, newUserType) {
  try {
    const userRef = admin.database().ref(`users/${userId}`);
    
    // Update UserType
    await userRef.update({
      UserType: newUserType  // "Official" or "Resident"
    });
    
    console.log(`✅ UserType changed to: ${newUserType}`);
  } catch (error) {
    console.error('❌ Error:', error);
  }
}

// Usage Examples:

// Change to Official
changeUserType('user-id-here', 'Official');

// Change to Resident
changeUserType('user-id-here', 'Resident');

// Or using numbers
// changeUserType('user-id-here', 0);  // Official
// changeUserType('user-id-here', 1);  // Resident
```

**Run the script:**
```bash
npm install firebase-admin
node change-usertype.js
```

---

## 🔧 Method 3: Using REST API

### cURL Command

```bash
# Change to Official (string format)
curl -X PATCH \
  'https://bantaybarangay-default-rtdb.asia-southeast1.firebasedatabase.app/users/USER_ID_HERE.json?auth=YOUR_AUTH_TOKEN' \
  -H 'Content-Type: application/json' \
  -d '{"UserType": "Official"}'

# Change to Resident (string format)
curl -X PATCH \
  'https://bantaybarangay-default-rtdb.asia-southeast1.firebasedatabase.app/users/USER_ID_HERE.json?auth=YOUR_AUTH_TOKEN' \
  -H 'Content-Type: application/json' \
  -d '{"UserType": "Resident"}'

# Change to Official (number format)
curl -X PATCH \
  'https://bantaybarangay-default-rtdb.asia-southeast1.firebasedatabase.app/users/USER_ID_HERE.json?auth=YOUR_AUTH_TOKEN' \
  -H 'Content-Type: application/json' \
  -d '{"UserType": 0}'
```

---

## 🔍 Finding User ID

### Method 1: From Firebase Console

```
1. Firebase Console → Realtime Database → Data
2. Expand "users" node
3. Each child node name is the User ID
   Example: users/abc123xyz456... ← This is the User ID
```

### Method 2: From Authentication

```
1. Firebase Console → Authentication → Users
2. Find the user by email
3. Copy the "User UID" column
4. This is the User ID
```

### Method 3: From App Login

```
1. Login with the user account
2. Check debug output
3. Look for: "User authenticated with ID: abc123..."
4. This is the User ID
```

---

## ✅ Verification Steps

### After Changing UserType

1. **Check Database**
   ```
   Firebase Console → Database → users/{userId}
   Verify: UserType = "Official" or 0
   ```

2. **Test Login**
   ```
   1. Open app
   2. Select "Official" user type
   3. Login with user credentials
   4. Should navigate to OfficialPage ✅
   ```

3. **Check Access**
   ```
   - User should see Official features
   - User should have admin permissions
   - User should access Official dashboard
   ```

---

## 🎯 Common Scenarios

### Scenario 1: Promote Resident to Official

**Use Case:** A resident becomes a barangay official

**Steps:**
1. Find user in database
2. Change `UserType` from `1` or `"Resident"` to `0` or `"Official"`
3. Optionally add fields:
   ```json
   {
     "Position": "Barangay Kagawad",
     "Department": "Health",
     "DatePromoted": "2025-01-15T10:00:00Z"
   }
   ```
4. Notify user of promotion
5. User can now login as Official

### Scenario 2: Demote Official to Resident

**Use Case:** An official's term ends

**Steps:**
1. Find user in database
2. Change `UserType` from `0` or `"Official"` to `1` or `"Resident"`
3. Optionally update fields:
   ```json
   {
     "FormerPosition": "Barangay Kagawad",
     "DateDemoted": "2025-01-15T10:00:00Z"
   }
   ```
4. Notify user
5. User now accesses Resident features only

### Scenario 3: Fix Incorrect UserType

**Use Case:** User registered with wrong type

**Steps:**
1. Find user in database
2. Change `UserType` to correct value
3. Verify user can login with correct type
4. Test access to appropriate features

---

## 🔒 Security Considerations

### Database Rules

Ensure your Firebase rules prevent users from changing their own UserType:

```json
{
  "rules": {
    "users": {
      "$uid": {
        ".read": "$uid === auth.uid || root.child('users').child(auth.uid).child('UserType').val() === 'Official'",
        ".write": "$uid === auth.uid && (!data.exists() || data.child('UserType').val() === newData.child('UserType').val())"
      }
    }
  }
}
```

**This rule:**
- ✅ Allows users to update their own data
- ❌ Prevents users from changing their UserType
- ✅ Only admins can change UserType via Firebase Console

---

## 📋 Batch Changes

### Change Multiple Users

If you need to change UserType for multiple users:

```javascript
// batch-change-usertype.js
const admin = require('firebase-admin');
const serviceAccount = require('./serviceAccountKey.json');

admin.initializeApp({
  credential: admin.credential.cert(serviceAccount),
  databaseURL: 'https://bantaybarangay-default-rtdb.asia-southeast1.firebasedatabase.app'
});

async function batchChangeUserType(userIds, newUserType) {
  const updates = {};
  
  userIds.forEach(userId => {
    updates[`users/${userId}/UserType`] = newUserType;
  });
  
  try {
    await admin.database().ref().update(updates);
    console.log(`✅ Changed ${userIds.length} users to ${newUserType}`);
  } catch (error) {
    console.error('❌ Error:', error);
  }
}

// Usage
const officialIds = [
  'user-id-1',
  'user-id-2',
  'user-id-3'
];

batchChangeUserType(officialIds, 'Official');
```

---

## 🧪 Testing

### Test Checklist

After changing UserType:

- [ ] Database shows correct UserType value
- [ ] User can login with new user type
- [ ] User navigates to correct page (OfficialPage/ResidentPage)
- [ ] User has appropriate permissions
- [ ] User sees correct features
- [ ] Old user type login fails with error message
- [ ] Database rules prevent self-modification

---

## 📞 Troubleshooting

### Issue 1: Change Doesn't Save

**Problem:** Value reverts after editing

**Solutions:**
- Check Firebase rules allow the change
- Ensure you pressed Enter after editing
- Verify you have admin access
- Try refreshing the page

### Issue 2: User Can't Login After Change

**Problem:** Login fails with new UserType

**Solutions:**
- Verify UserType value is correct (0 or "Official")
- Check user selects correct type during login
- Ensure user profile exists in database
- Verify Firebase Auth account is active

### Issue 3: Wrong Page After Login

**Problem:** User goes to wrong page

**Solutions:**
- Clear app cache
- Logout and login again
- Verify UserType in database
- Check app code for navigation logic

---

## 📊 Audit Log (Optional)

### Track UserType Changes

Add an audit log when changing UserType:

```json
{
  "users": {
    "user-id": {
      "UserType": "Official",
      "UserTypeHistory": {
        "2025-01-15T10:00:00Z": {
          "OldType": "Resident",
          "NewType": "Official",
          "ChangedBy": "admin@bantaybarangay.ph",
          "Reason": "Elected as Barangay Kagawad"
        }
      }
    }
  }
}
```

---

## ✅ Quick Reference

### Change Resident → Official

**Firebase Console:**
```
1. Database → users/{userId}
2. UserType: 1 → 0
   OR
   UserType: "Resident" → "Official"
3. Press Enter
```

### Change Official → Resident

**Firebase Console:**
```
1. Database → users/{userId}
2. UserType: 0 → 1
   OR
   UserType: "Official" → "Resident"
3. Press Enter
```

---

## 📚 Related Documentation

- [OFFICIAL_ACCOUNT_SETUP_GUIDE.md](OFFICIAL_ACCOUNT_SETUP_GUIDE.md) - Creating official accounts
- [USERTYPE_ENUM_GUIDE.md](USERTYPE_ENUM_GUIDE.md) - UserType values explained
- [Firebase Database Rules](https://firebase.google.com/docs/database/security)

---

*UserType Change Guide*  
*Simple steps to promote or demote users*  
*Manage user access levels easily!* 🔧
