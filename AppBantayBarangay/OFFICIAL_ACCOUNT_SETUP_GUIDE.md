# Official Account Setup Guide

## 🎯 Overview

This guide explains how to:
1. **Restrict app registration to Residents only**
2. **Create Official accounts manually in Firebase**
3. **Prevent unauthorized Official account creation**

---

## 🔒 Security Strategy

### Current Setup (Insecure)
- ❌ Anyone can register as Official or Resident
- ❌ No verification for Official accounts
- ❌ Security risk

### Recommended Setup (Secure)
- ✅ Only Residents can register through the app
- ✅ Officials are created manually by administrators
- ✅ Official accounts require verification
- ✅ Secure and controlled

---

## 📋 Implementation Steps

### Step 1: Update Registration Page (App-Side)

The registration page should **only allow Resident registration**.

#### Option A: Remove User Type Selection (Recommended)

Update `RegistrationPage.xaml.cs`:

```csharp
// In OnRegisterClicked method, change this line:
UserType = UserType.Resident, // Always Resident for app registration

// Remove or hide any user type picker in the UI
```

#### Option B: Lock User Type to Resident

If you want to keep the UI but lock it:

```csharp
// In RegistrationPage constructor
UserTypePicker.SelectedIndex = 0; // Resident
UserTypePicker.IsEnabled = false; // Disable selection
```

---

### Step 2: Create Official Accounts in Firebase Console

#### Method 1: Using Firebase Console (Easiest)

**Step 2.1: Create Authentication Account**

1. **Open Firebase Console**
   - Go to https://console.firebase.google.com
   - Select your project: `bantaybarangay`

2. **Navigate to Authentication**
   - Click "Authentication" in left menu
   - Click "Users" tab
   - Click "Add User" button

3. **Create Official Account**
   ```
   Email: official@bantaybarangay.ph
   Password: [Create strong password]
   ```
   - Click "Add User"
   - **Copy the User UID** (you'll need this)

**Step 2.2: Create User Profile in Database**

1. **Navigate to Realtime Database**
   - Click "Realtime Database" in left menu
   - Click on "Data" tab

2. **Add Official Profile**
   - Click on `users` node
   - Click "+" to add child
   - **Key**: Paste the User UID from Step 2.1
   - **Value**: Click "+" to add fields

3. **Add Profile Fields**
   ```json
   {
     "UserId": "paste-user-uid-here",
     "Email": "official@bantaybarangay.ph",
     "FirstName": "Juan",
     "MiddleName": "Santos",
     "LastName": "Dela Cruz",
     "PhoneNumber": "09123456789",
     "Address": "Barangay Hall, Main Street",
     "UserType": "Official",
     "CreatedAt": "2025-01-15T10:00:00Z"
   }
   ```

4. **Click "Add"**

---

#### Method 2: Using Firebase Admin SDK (Advanced)

If you want to create a script to add officials:

**Create a Node.js script:**

```javascript
// create-official.js
const admin = require('firebase-admin');
const serviceAccount = require('./serviceAccountKey.json');

admin.initializeApp({
  credential: admin.credential.cert(serviceAccount),
  databaseURL: 'https://bantaybarangay-default-rtdb.asia-southeast1.firebasedatabase.app'
});

async function createOfficial(email, password, firstName, lastName, phone) {
  try {
    // Create authentication account
    const userRecord = await admin.auth().createUser({
      email: email,
      password: password,
      emailVerified: false
    });

    console.log('User created:', userRecord.uid);

    // Create user profile in database
    const userProfile = {
      UserId: userRecord.uid,
      Email: email,
      FirstName: firstName,
      MiddleName: '',
      LastName: lastName,
      PhoneNumber: phone,
      Address: 'Barangay Hall',
      UserType: 'Official',
      CreatedAt: new Date().toISOString()
    };

    await admin.database().ref(`users/${userRecord.uid}`).set(userProfile);
    console.log('Profile created successfully');

  } catch (error) {
    console.error('Error:', error);
  }
}

// Usage
createOfficial(
  'official@bantaybarangay.ph',
  'SecurePassword123!',
  'Juan',
  'Dela Cruz',
  '09123456789'
);
```

**Run the script:**
```bash
npm install firebase-admin
node create-official.js
```

---

### Step 3: Update Firebase Security Rules

Protect the database so only authenticated users can modify their own data, and officials can read all data.

**Realtime Database Rules:**

```json
{
  "rules": {
    "users": {
      "$uid": {
        ".read": "$uid === auth.uid || root.child('users').child(auth.uid).child('UserType').val() === 'Official'",
        ".write": "$uid === auth.uid && (!data.exists() || data.child('UserType').val() !== 'Official')"
      }
    }
  }
}
```

**Explanation:**
- **Read**: Users can read their own data, Officials can read all user data
- **Write**: Users can only write their own data, and cannot change UserType to Official

**To apply these rules:**
1. Firebase Console → Realtime Database
2. Click "Rules" tab
3. Paste the rules above
4. Click "Publish"

---

### Step 4: Update Registration Page Code

Update the registration to enforce Resident-only registration:

```csharp
// RegistrationPage.xaml.cs

private async void OnRegisterClicked(object sender, EventArgs e)
{
    // Validate all required fields
    if (!ValidateInputs())
        return;

    try
    {
        // Show loading state
        var registerButton = sender as Button;
        if (registerButton != null)
        {
            registerButton.Text = "⏳ Creating Account...";
            registerButton.IsEnabled = false;
        }

        var email = EmailEntry.Text.Trim();
        var password = PasswordEntry.Text;

        // Step 1: Create Firebase Authentication account
        System.Diagnostics.Debug.WriteLine($"Attempting to register user: {email}");
        bool authSuccess = await _firebaseService.SignUpAsync(email, password);

        if (!authSuccess)
        {
            await DisplayAlert("Registration Failed", 
                "Could not create account. Email may already be in use.", 
                "OK");
            return;
        }

        // Step 2: Get the user ID from Firebase Auth
        string userId = _firebaseService.GetCurrentUserId();
        System.Diagnostics.Debug.WriteLine($"User created with ID: {userId}");

        // Step 3: Create user profile data (ALWAYS Resident)
        var user = new User
        {
            UserId = userId,
            FirstName = FirstNameEntry.Text.Trim(),
            MiddleName = MiddleNameEntry?.Text?.Trim() ?? string.Empty,
            LastName = LastNameEntry.Text.Trim(),
            Email = email,
            Address = AddressEntry.Text.Trim(),
            PhoneNumber = PhoneNumberEntry.Text.Trim(),
            UserType = UserType.Resident, // ALWAYS Resident - Officials created manually
            CreatedAt = DateTime.UtcNow.ToString("o")
        };

        // Step 4: Save user profile to Firebase Database
        string userPath = $"users/{userId}";
        bool saveSuccess = await _firebaseService.SaveDataAsync(userPath, user);

        if (!saveSuccess)
        {
            await DisplayAlert("Warning", 
                "Account created but profile data could not be saved. Please contact support.", 
                "OK");
            return;
        }

        System.Diagnostics.Debug.WriteLine($"User profile saved successfully");

        // Step 5: Sign out the user (they need to login)
        await _firebaseService.SignOutAsync();

        // Step 6: Show success message
        await DisplayAlert("Success", 
            $"Welcome {user.FirstName}! Your Resident account has been created successfully.\n\nYou can now login with:\nEmail: {email}", 
            "OK");

        // Step 7: Navigate back to login page
        await Navigation.PopAsync();
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($"Registration error: {ex.Message}");
        await DisplayAlert("Error", 
            $"Registration failed: {ex.Message}\n\nPlease try again or contact support.", 
            "OK");
    }
    finally
    {
        // Reset button state
        var registerButton = sender as Button;
        if (registerButton != null)
        {
            registerButton.Text = "✓ CREATE ACCOUNT";
            registerButton.IsEnabled = true;
        }
    }
}
```

---

### Step 5: Update Registration UI (Optional)

Remove user type selection from the UI:

**RegistrationPage.xaml** - Remove or comment out:

```xml
<!-- Remove this section -->
<!--
<Label Text="User Type" ... />
<Picker x:Name="UserTypePicker" ... />
-->
```

Or add a note:

```xml
<Frame BackgroundColor="#FFF3CD" BorderColor="#FFC107" ...>
    <Label Text="ℹ️ Note: All app registrations are for Residents only. Official accounts are created by administrators." 
           FontSize="12" 
           TextColor="#856404"/>
</Frame>
```

---

## 🧪 Testing

### Test 1: Register as Resident

1. **Open the app**
2. **Click "Register"**
3. **Fill in details**
4. **Click "Create Account"**
5. **Verify**:
   - Account created successfully
   - Firebase Auth shows new user
   - Database shows UserType: "Resident"

### Test 2: Login as Resident

1. **Login with registered account**
2. **Select "Resident" user type**
3. **Verify**:
   - Login successful
   - Navigates to ResidentPage

### Test 3: Login as Official

1. **Create Official account in Firebase (Step 2)**
2. **Login with Official credentials**
3. **Select "Official" user type**
4. **Verify**:
   - Login successful
   - Navigates to OfficialPage

### Test 4: Try to Change UserType (Security Test)

1. **Login as Resident**
2. **Try to modify UserType in database**
3. **Verify**:
   - Firebase rules prevent modification
   - UserType remains "Resident"

---

## 📊 Official Account Template

Use this template when creating official accounts:

```json
{
  "UserId": "[Firebase Auth UID]",
  "Email": "official.name@bantaybarangay.ph",
  "FirstName": "Juan",
  "MiddleName": "Santos",
  "LastName": "Dela Cruz",
  "PhoneNumber": "09123456789",
  "Address": "Barangay Hall, Main Street, Barangay Center",
  "UserType": "Official",
  "CreatedAt": "2025-01-15T10:00:00Z",
  "Position": "Barangay Captain",
  "Department": "Administration"
}
```

**Optional Fields:**
- `Position`: Barangay Captain, Kagawad, Secretary, etc.
- `Department`: Administration, Health, Security, etc.
- `EmployeeId`: Official employee ID
- `DateHired`: When they started

---

## 🔐 Security Best Practices

### 1. Strong Passwords for Officials
```
Minimum requirements:
- At least 12 characters
- Mix of uppercase and lowercase
- Numbers and special characters
- Not easily guessable
```

### 2. Email Verification
Enable email verification for officials:
```
Firebase Console → Authentication → Templates
Customize verification email
```

### 3. Two-Factor Authentication (Future)
Consider adding 2FA for official accounts

### 4. Regular Audits
- Review official accounts monthly
- Remove inactive accounts
- Update passwords regularly

### 5. Separate Admin Account
Create a super-admin account for managing officials

---

## 📋 Official Account Management Checklist

### Creating New Official

- [ ] Verify official's identity
- [ ] Create Firebase Auth account
- [ ] Copy User UID
- [ ] Create database profile
- [ ] Set UserType to "Official"
- [ ] Add position and department
- [ ] Send credentials securely
- [ ] Verify login works
- [ ] Document in admin log

### Removing Official

- [ ] Verify authorization
- [ ] Disable Firebase Auth account
- [ ] Archive database profile (don't delete)
- [ ] Update UserType to "Former Official"
- [ ] Document removal reason
- [ ] Notify relevant parties

---

## 🎯 Quick Reference

### Create Official Account (Quick Steps)

1. **Firebase Console → Authentication → Add User**
   - Email: official@example.com
   - Password: [Strong password]
   - Copy UID

2. **Firebase Console → Database → users → Add Child**
   - Key: [Paste UID]
   - Add fields: Email, FirstName, LastName, Phone, Address
   - **UserType: "Official"**

3. **Test Login**
   - Open app
   - Select "Official"
   - Login with credentials

---

## 📞 Support

### Common Issues

**Issue**: Official can't login
- **Check**: Email and password correct
- **Check**: UserType is "Official" in database
- **Check**: Account exists in Firebase Auth

**Issue**: Resident registered as Official
- **Fix**: Update UserType in database to "Resident"
- **Prevent**: Update app code to force Resident

**Issue**: Can't create official in Firebase
- **Check**: You have admin access
- **Check**: Email not already in use
- **Check**: Firebase project selected

---

## 📚 Additional Resources

- [Firebase Authentication Docs](https://firebase.google.com/docs/auth)
- [Firebase Database Rules](https://firebase.google.com/docs/database/security)
- [Firebase Admin SDK](https://firebase.google.com/docs/admin/setup)

---

*Official Account Setup Guide*  
*Secure and controlled official account management*  
*Only residents can register through the app!* 🔒
