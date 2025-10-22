# Login Issue Diagnostic Guide

## 🐛 Problem: Cannot Login with Correct Credentials

---

## 🔍 STEP-BY-STEP DIAGNOSIS

### Step 1: Check Output Window

1. **Run the app in Debug mode**
2. **View → Output** (Ctrl+Alt+O)
3. **Select "Debug" from dropdown**
4. **Try to login**
5. **Look for these messages:**

**Expected Flow:**
```
Attempting login for: user@example.com
User authenticated with ID: abc123...
[GetData] Path: users/abc123...
[GetData] Snapshot exists: True
[UserType] Converting number 1 to enum
User profile loaded: Juan Dela Cruz
Navigating to Resident page
```

**Common Errors:**
```
Sign in error: [error message]  ← Authentication failed
[GetData] Snapshot exists: False  ← User profile not found
[GetData] Error: [error]  ← Database read failed
User profile not found  ← Profile missing in database
```

---

### Step 2: Verify Firebase Authentication

1. **Firebase Console → Authentication → Users**
2. **Find your email**
3. **Check:**
   - [ ] User exists
   - [ ] Email is verified (or verification not required)
   - [ ] User is enabled (not disabled)

---

### Step 3: Verify User Profile in Database

1. **Firebase Console → Realtime Database → Data**
2. **Navigate to: users/{your-user-id}**
3. **Check:**
   - [ ] User node exists
   - [ ] Has all required fields
   - [ ] UserType is set (0, 1, "Official", or "Resident")

---

## ✅ SOLUTION 1: Enhanced Login with Better Error Messages

Update LoginPage.xaml.cs with detailed error handling:

```csharp
private async void OnLoginClicked(object sender, EventArgs e)
{
    // Validate inputs
    if (UserTypePicker.SelectedIndex == -1)
    {
        await DisplayAlert("Required", "Please select your user type.", "OK");
        return;
    }

    if (string.IsNullOrWhiteSpace(EmailEntry.Text))
    {
        await DisplayAlert("Required", "Please enter your email address.", "OK");
        return;
    }

    if (string.IsNullOrWhiteSpace(PasswordEntry.Text))
    {
        await DisplayAlert("Required", "Please enter your password.", "OK");
        return;
    }

    try
    {
        var loginButton = sender as Button;
        if (loginButton != null)
        {
            loginButton.Text = "🔄 Logging in...";
            loginButton.IsEnabled = false;
        }

        var email = EmailEntry.Text.Trim();
        var password = PasswordEntry.Text;
        var selectedUserType = UserTypePicker.SelectedItem.ToString() == "Official" ? 
                              UserType.Official : UserType.Resident;

        System.Diagnostics.Debug.WriteLine($"=== LOGIN ATTEMPT ===");
        System.Diagnostics.Debug.WriteLine($"Email: {email}");
        System.Diagnostics.Debug.WriteLine($"Selected Type: {selectedUserType}");

        // Step 1: Check Firebase service
        var firebaseService = DependencyService.Get<IFirebaseService>();
        if (firebaseService == null)
        {
            await DisplayAlert("Error", "Firebase service not available. Please restart the app.", "OK");
            return;
        }

        // Step 2: Authenticate
        System.Diagnostics.Debug.WriteLine("Step 1: Authenticating...");
        bool authSuccess = await firebaseService.SignInAsync(email, password);

        if (!authSuccess)
        {
            System.Diagnostics.Debug.WriteLine("❌ Authentication failed");
            await DisplayAlert("Login Failed", 
                "Invalid email or password.\n\nPlease check:\n• Email is correct\n• Password is correct\n• Account exists", 
                "OK");
            return;
        }

        System.Diagnostics.Debug.WriteLine("✅ Authentication successful");

        // Step 3: Get user ID
        string userId = firebaseService.GetCurrentUserId();
        System.Diagnostics.Debug.WriteLine($"Step 2: User ID: {userId}");

        if (string.IsNullOrEmpty(userId))
        {
            await DisplayAlert("Error", "Could not get user ID. Please try again.", "OK");
            await firebaseService.SignOutAsync();
            return;
        }

        // Step 4: Get user profile
        System.Diagnostics.Debug.WriteLine($"Step 3: Loading profile from users/{userId}");
        string userPath = $"users/{userId}";
        var user = await firebaseService.GetDataAsync<User>(userPath);

        if (user == null)
        {
            System.Diagnostics.Debug.WriteLine("❌ User profile not found");
            await DisplayAlert("Profile Not Found", 
                $"Your account exists but profile data is missing.\n\nUser ID: {userId}\n\nPlease contact support.", 
                "OK");
            await firebaseService.SignOutAsync();
            return;
        }

        System.Diagnostics.Debug.WriteLine($"✅ Profile loaded: {user.FullName}");
        System.Diagnostics.Debug.WriteLine($"Profile UserType: {user.UserType}");

        // Step 5: Verify user type
        if (user.UserType != selectedUserType)
        {
            System.Diagnostics.Debug.WriteLine($"❌ UserType mismatch: Expected {selectedUserType}, Got {user.UserType}");
            await DisplayAlert("Wrong User Type", 
                $"This account is registered as {user.UserType}.\n\nPlease select '{user.UserType}' and try again.", 
                "OK");
            await firebaseService.SignOutAsync();
            return;
        }

        System.Diagnostics.Debug.WriteLine($"✅ UserType verified: {user.UserType}");

        // Step 6: Navigate
        System.Diagnostics.Debug.WriteLine($"Step 4: Navigating to {user.UserType} page");
        
        if (user.UserType == UserType.Official)
        {
            await Navigation.PushAsync(new OfficialPage(user));
        }
        else
        {
            await Navigation.PushAsync(new ResidentPage(user));
        }

        // Clear form
        EmailEntry.Text = string.Empty;
        PasswordEntry.Text = string.Empty;
        UserTypePicker.SelectedIndex = -1;

        System.Diagnostics.Debug.WriteLine("=== LOGIN SUCCESS ===");
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($"=== LOGIN ERROR ===");
        System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
        System.Diagnostics.Debug.WriteLine($"Stack: {ex.StackTrace}");
        
        await DisplayAlert("Login Error", 
            $"An error occurred:\n\n{ex.Message}\n\nPlease check Output window for details.", 
            "OK");
        
        var firebaseService = DependencyService.Get<IFirebaseService>();
        if (firebaseService?.IsAuthenticated() == true)
        {
            await firebaseService.SignOutAsync();
        }
    }
    finally
    {
        var loginButton = sender as Button;
        if (loginButton != null)
        {
            loginButton.Text = "🔐 LOGIN";
            loginButton.IsEnabled = true;
        }
    }
}
```

---

## ✅ SOLUTION 2: Check Database Rules

Ensure users can read their own data:

```json
{
  "rules": {
    "users": {
      "$uid": {
        ".read": "$uid === auth.uid || root.child('users').child(auth.uid).child('UserType').val() === 'Official'",
        ".write": "$uid === auth.uid"
      }
    },
    "reports": {
      ".read": "auth != null",
      ".write": "auth != null"
    }
  }
}
```

---

## ✅ SOLUTION 3: Verify User Data Structure

Check Firebase Console - User should look like:

```json
{
  "users": {
    "abc123xyz456": {
      "UserId": "abc123xyz456",
      "Email": "user@example.com",
      "FirstName": "Juan",
      "MiddleName": "A",
      "LastName": "Dela Cruz",
      "PhoneNumber": "09123456789",
      "Address": "123 Main St",
      "UserType": "Resident",  // or 1, or "Official", or 0
      "CreatedAt": "2025-01-15T10:00:00Z"
    }
  }
}
```

**Required fields:**
- UserId
- Email
- FirstName
- LastName
- UserType

---

## 🔍 Common Issues & Fixes

### Issue 1: "Invalid email or password"

**Cause:** Authentication failed

**Check:**
- Email is correct (no typos)
- Password is correct
- Account exists in Firebase Auth
- Account is not disabled

**Fix:**
- Try password reset
- Check Firebase Console → Authentication

---

### Issue 2: "User profile not found"

**Cause:** User authenticated but no profile in database

**Check:**
- Firebase Console → Database → users/{userId}
- Profile exists?

**Fix:**
- Manually create profile in Firebase Console
- Or re-register the account

---

### Issue 3: "Wrong user type"

**Cause:** Selected type doesn't match database

**Check:**
- What's in database: UserType field
- What you selected: Official or Resident

**Fix:**
- Select correct user type
- Or update database UserType

---

### Issue 4: Deserialization Error

**Cause:** User data format issue

**Check Output:**
```
[GetData] Error: Cannot deserialize...
```

**Fix:**
- Check all fields are correct type
- UserType should be 0, 1, "Official", or "Resident"
- Dates should be valid ISO format

---

## 🧪 Quick Test

Add this test button to LoginPage.xaml:

```xml
<Button Text="🧪 Test Firebase"
        Clicked="TestFirebase_Clicked"
        BackgroundColor="Orange"
        TextColor="White"/>
```

Add this method to LoginPage.xaml.cs:

```csharp
private async void TestFirebase_Clicked(object sender, EventArgs e)
{
    try
    {
        var firebaseService = DependencyService.Get<IFirebaseService>();
        
        // Test 1: Service available
        if (firebaseService == null)
        {
            await DisplayAlert("Test Failed", "Firebase service is null", "OK");
            return;
        }
        
        // Test 2: Try to authenticate
        var email = EmailEntry.Text?.Trim();
        var password = PasswordEntry.Text;
        
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            await DisplayAlert("Test", "Enter email and password first", "OK");
            return;
        }
        
        bool authSuccess = await firebaseService.SignInAsync(email, password);
        
        if (!authSuccess)
        {
            await DisplayAlert("Test Result", "❌ Authentication failed", "OK");
            return;
        }
        
        // Test 3: Get user ID
        string userId = firebaseService.GetCurrentUserId();
        
        // Test 4: Try to get profile
        var user = await firebaseService.GetDataAsync<User>($"users/{userId}");
        
        if (user == null)
        {
            await DisplayAlert("Test Result", 
                $"✅ Auth OK\n❌ Profile not found\n\nUser ID: {userId}", 
                "OK");
        }
        else
        {
            await DisplayAlert("Test Result", 
                $"✅ Auth OK\n✅ Profile OK\n\nName: {user.FullName}\nType: {user.UserType}", 
                "OK");
        }
        
        await firebaseService.SignOutAsync();
    }
    catch (Exception ex)
    {
        await DisplayAlert("Test Error", ex.Message, "OK");
    }
}
```

---

## 📊 Checklist

Before asking for help, verify:

- [ ] Email and password are correct
- [ ] User exists in Firebase Authentication
- [ ] User profile exists in Database under users/{userId}
- [ ] UserType field is set in database
- [ ] Database rules allow reading user data
- [ ] Output window shows detailed error messages
- [ ] Firebase service is initialized
- [ ] Internet connection is working

---

*Login diagnostic guide*  
*Check Output window first!* 🔍
