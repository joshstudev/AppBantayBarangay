# Phone Number Login Troubleshooting Guide

## 🐛 Problem: "No account found with this phone number"

Even though the phone number and password are correct, the system says no account found.

---

## ✅ Solutions Applied

### 1. **Phone Number Normalization**

Added automatic normalization to handle different phone number formats:

```csharp
private string NormalizePhoneNumber(string phoneNumber)
{
    // Remove all non-digit characters
    string digitsOnly = new string(phoneNumber.Where(char.IsDigit).ToArray());
    
    // Handle different formats:
    // +639123456789 -> 09123456789
    // 639123456789 -> 09123456789
    // 09123456789 -> 09123456789
    
    if (digitsOnly.StartsWith("63") && digitsOnly.Length == 12)
    {
        digitsOnly = "0" + digitsOnly.Substring(2);
    }
    
    return digitsOnly;
}
```

### 2. **Enhanced Debug Logging**

Added detailed logging to track the lookup process:

```csharp
System.Diagnostics.Debug.WriteLine($"[Phone Lookup] Searching for phone: {phoneNumber}");
System.Diagnostics.Debug.WriteLine($"[Phone Lookup] Normalized input: {normalizedInput}");
System.Diagnostics.Debug.WriteLine($"[Phone Lookup] Found {usersDict.Count} users in database");
System.Diagnostics.Debug.WriteLine($"[Phone Lookup] User {userIndex}: Phone={userPhone}, Email={userEmail}");
```

### 3. **Improved Error Messages**

More helpful error message with the phone number shown:

```csharp
await DisplayAlert("Login Failed", 
    $"No account found with phone number: {loginIdentifier}\n\nPlease check the number and try again, or register a new account.", 
    "OK");
```

---

## 🔍 How to Debug

### Step 1: Check Output Window

1. **Run the app in Debug mode**
2. **View → Output** (or Ctrl+Alt+O)
3. **Select "Debug" from dropdown**
4. **Try to login with phone number**
5. **Look for messages starting with `[Phone Lookup]`**

### Step 2: Verify Phone Number Format

Check what's stored in Firebase:

1. **Open Firebase Console**
2. **Database → Data**
3. **Navigate to `users/{userId}`**
4. **Check `PhoneNumber` field**

Example:
```json
{
  "users": {
    "abc123": {
      "PhoneNumber": "09123456789",  ← Check this format
      "Email": "user@example.com"
    }
  }
}
```

### Step 3: Check Debug Output

Look for these messages:

```
[Phone Lookup] Searching for phone: 09123456789
[Phone Lookup] Normalized input: 09123456789
[Phone Lookup] Found 5 users in database
[Phone Lookup] User 1: Phone=09111111111, Email=user1@example.com
[Phone Lookup] User 2: Phone=09123456789, Email=user2@example.com
[Phone Lookup] MATCH FOUND! Email: user2@example.com
```

---

## 🎯 Common Issues & Solutions

### Issue 1: Phone Number Format Mismatch

**Problem:**
- Registered with: `09123456789`
- Trying to login with: `+639123456789`
- Result: No match

**Solution:**
✅ Normalization now handles this automatically

**Supported Formats:**
- `09123456789` ✅
- `+639123456789` ✅
- `639123456789` ✅
- `0912 345 6789` ✅
- `0912-345-6789` ✅

All convert to: `09123456789`

---

### Issue 2: No Users in Database

**Problem:**
Debug shows: `[Phone Lookup] No users data found`

**Solution:**
1. Check Firebase Database rules
2. Ensure user is registered
3. Verify database path is `users/{userId}`

---

### Issue 3: Data Not Loading

**Problem:**
Debug shows: `[Phone Lookup] Failed to parse users data as JObject`

**Solution:**
1. Check Firebase Database structure
2. Ensure data is in correct format
3. Verify Firebase service is working

---

### Issue 4: Phone Number Not Stored

**Problem:**
Debug shows: `Phone=null` or `Phone=`

**Solution:**
1. Re-register the account
2. Ensure RegistrationPage saves phone number
3. Check Firebase Database for the user

---

## 🧪 Testing Steps

### Test 1: Register New Account

1. **Register with:**
   - Email: test@example.com
   - Phone: 09123456789
   - Password: test123

2. **Check Firebase Database:**
   - Navigate to `users/{userId}`
   - Verify `PhoneNumber: "09123456789"`
   - Verify `Email: "test@example.com"`

### Test 2: Login with Phone (Exact Format)

1. **Login with:**
   - Method: Phone Number
   - Phone: 09123456789
   - Password: test123

2. **Expected:**
   - Debug: `[Phone Lookup] MATCH FOUND!`
   - Result: Login successful

### Test 3: Login with Phone (Different Format)

1. **Login with:**
   - Method: Phone Number
   - Phone: +639123456789
   - Password: test123

2. **Expected:**
   - Debug: `[Phone Lookup] Normalized input: 09123456789`
   - Debug: `[Phone Lookup] MATCH FOUND!`
   - Result: Login successful

### Test 4: Login with Wrong Phone

1. **Login with:**
   - Method: Phone Number
   - Phone: 09999999999
   - Password: test123

2. **Expected:**
   - Debug: `[Phone Lookup] No matching phone number found`
   - Result: "No account found with phone number: 09999999999"

---

## 📊 Debug Output Examples

### Successful Lookup

```
[Phone Lookup] Searching for phone: +639123456789
[Phone Lookup] Normalized input: 09123456789
[Phone Lookup] Users data type: JObject
[Phone Lookup] Found 3 users in database
[Phone Lookup] User 1: Phone=09111111111, Email=user1@example.com
[Phone Lookup] User 2: Phone=09123456789, Email=test@example.com
[Phone Lookup] MATCH FOUND! Email: test@example.com
Found email for phone number: test@example.com
User authenticated with ID: abc123
User profile loaded: Juan Dela Cruz
Navigating to Resident page
```

### Failed Lookup

```
[Phone Lookup] Searching for phone: 09999999999
[Phone Lookup] Normalized input: 09999999999
[Phone Lookup] Users data type: JObject
[Phone Lookup] Found 3 users in database
[Phone Lookup] User 1: Phone=09111111111, Email=user1@example.com
[Phone Lookup] User 2: Phone=09123456789, Email=test@example.com
[Phone Lookup] User 3: Phone=09222222222, Email=user3@example.com
[Phone Lookup] No matching phone number found
```

---

## 🔧 Manual Verification

### Check Firebase Database

1. **Open Firebase Console**
2. **Select your project**
3. **Database → Realtime Database**
4. **Navigate to `users`**
5. **Check each user's `PhoneNumber` field**

Example structure:
```json
{
  "users": {
    "user_id_1": {
      "Email": "user1@example.com",
      "PhoneNumber": "09123456789",
      "FirstName": "Juan"
    },
    "user_id_2": {
      "Email": "user2@example.com",
      "PhoneNumber": "09987654321",
      "FirstName": "Maria"
    }
  }
}
```

---

## ✅ Verification Checklist

After implementing the fix:

- [ ] Phone number normalization works
- [ ] Debug logging shows in Output window
- [ ] Can login with exact phone format (09XXXXXXXXX)
- [ ] Can login with +63 format (+639XXXXXXXXX)
- [ ] Can login with 63 format (639XXXXXXXXX)
- [ ] Can login with spaces (0912 345 6789)
- [ ] Can login with dashes (0912-345-6789)
- [ ] Error message shows phone number when not found
- [ ] Debug output shows all users being checked
- [ ] Debug output shows when match is found

---

## 🚀 Next Steps

### If Still Not Working

1. **Check Output Window**
   - Look for `[Phone Lookup]` messages
   - Identify where the process fails

2. **Verify Database**
   - Ensure user exists
   - Check phone number format in database
   - Verify email is stored

3. **Test with Email Login**
   - Try logging in with email
   - If email works, issue is phone lookup
   - If email fails, issue is authentication

4. **Re-register**
   - Delete old account
   - Register new account
   - Verify phone number is saved
   - Try login again

---

## 📞 Support

If issue persists:

1. **Collect Debug Output**
   - Copy all `[Phone Lookup]` messages
   - Note the phone number used
   - Note the expected email

2. **Check Firebase**
   - Screenshot of user data
   - Verify phone number field

3. **Provide Details**
   - Phone number format used
   - Error message received
   - Debug output

---

*Phone number login troubleshooting guide*  
*Enhanced with normalization and debugging!* 🔍
