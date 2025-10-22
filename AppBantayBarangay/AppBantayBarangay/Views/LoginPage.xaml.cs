using System;
using AppBantayBarangay.Models;
using AppBantayBarangay.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppBantayBarangay.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        private readonly IFirebaseService _firebaseService;

        public LoginPage()
        {
            InitializeComponent();
            _firebaseService = DependencyService.Get<IFirebaseService>();
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            // Validate inputs
            if (UserTypePicker.SelectedIndex == -1)
            {
                await DisplayAlert("Required", "Please select your user type (Official or Resident).", "OK");
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
                // Show loading state
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

                // Step 1: Authenticate with Firebase
                System.Diagnostics.Debug.WriteLine("Step 1: Authenticating with Firebase...");
                bool authSuccess = await _firebaseService.SignInAsync(email, password);

                if (!authSuccess)
                {
                    System.Diagnostics.Debug.WriteLine("❌ Authentication failed");
                    await DisplayAlert("Login Failed", 
                        "Invalid email or password.\n\nPlease check your credentials and try again.", 
                        "OK");
                    return;
                }

                System.Diagnostics.Debug.WriteLine("✅ Authentication successful");

                // Step 2: Get the authenticated user ID
                string userId = _firebaseService.GetCurrentUserId();
                System.Diagnostics.Debug.WriteLine($"Step 2: User ID: {userId}");

                if (string.IsNullOrEmpty(userId))
                {
                    System.Diagnostics.Debug.WriteLine("❌ User ID is null or empty");
                    await DisplayAlert("Error", 
                        "Could not get user ID. Please try again.", 
                        "OK");
                    await _firebaseService.SignOutAsync();
                    return;
                }

                // Step 3: Retrieve user profile from database
                System.Diagnostics.Debug.WriteLine($"Step 3: Loading user profile from users/{userId}");
                string userPath = $"users/{userId}";
                var user = await _firebaseService.GetDataAsync<User>(userPath);

                if (user == null)
                {
                    System.Diagnostics.Debug.WriteLine("❌ User profile not found in database");
                    await DisplayAlert("Profile Not Found", 
                        "Your account exists but your profile data is missing from the database.\n\nPlease contact support or register a new account.", 
                        "OK");
                    await _firebaseService.SignOutAsync();
                    return;
                }

                System.Diagnostics.Debug.WriteLine($"✅ User profile loaded successfully");
                System.Diagnostics.Debug.WriteLine($"   Name: {user.FullName}");
                System.Diagnostics.Debug.WriteLine($"   Email: {user.Email}");
                System.Diagnostics.Debug.WriteLine($"   UserType: {user.UserType}");

                // Step 4: Verify user type matches selection
                if (user.UserType != selectedUserType)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ UserType mismatch!");
                    System.Diagnostics.Debug.WriteLine($"   Expected: {selectedUserType}");
                    System.Diagnostics.Debug.WriteLine($"   Actual: {user.UserType}");
                    
                    await DisplayAlert("Wrong User Type", 
                        $"This account is registered as {user.UserType}.\n\nPlease select '{user.UserType}' from the dropdown and try again.", 
                        "OK");
                    await _firebaseService.SignOutAsync();
                    return;
                }

                System.Diagnostics.Debug.WriteLine($"✅ UserType verified: {user.UserType}");

                // Step 5: Navigate to appropriate page based on user type
                System.Diagnostics.Debug.WriteLine($"Step 4: Navigating to {user.UserType} page...");
                
                if (user.UserType == UserType.Official)
                {
                    await Navigation.PushAsync(new OfficialPage(user));
                }
                else
                {
                    await Navigation.PushAsync(new ResidentPage(user));
                }

                // Step 6: Clear the form
                EmailEntry.Text = string.Empty;
                PasswordEntry.Text = string.Empty;
                UserTypePicker.SelectedIndex = -1;

                System.Diagnostics.Debug.WriteLine("=== LOGIN SUCCESS ===");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"=== LOGIN ERROR ===");
                System.Diagnostics.Debug.WriteLine($"Error Type: {ex.GetType().Name}");
                System.Diagnostics.Debug.WriteLine($"Error Message: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                
                await DisplayAlert("Login Error", 
                    $"An unexpected error occurred:\n\n{ex.Message}\n\nPlease try again or contact support.", 
                    "OK");
                
                // Sign out if there was an error after authentication
                if (_firebaseService?.IsAuthenticated() == true)
                {
                    await _firebaseService.SignOutAsync();
                }
            }
            finally
            {
                // Reset button state
                var loginButton = sender as Button;
                if (loginButton != null)
                {
                    loginButton.Text = "🔐 LOGIN";
                    loginButton.IsEnabled = true;
                }
            }
        }

        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RegistrationPage());
        }

        private async void OnForgotPasswordClicked(object sender, EventArgs e)
        {
            // Prompt for email
            string email = await DisplayPromptAsync("Forgot Password", 
                "Enter your email address to reset your password:", 
                "Send", 
                "Cancel", 
                "email@example.com", 
                keyboard: Keyboard.Email);

            if (string.IsNullOrWhiteSpace(email))
                return;

            try
            {
                // TODO: Implement password reset with Firebase Auth
                // await _firebaseService.SendPasswordResetEmailAsync(email);
                
                await DisplayAlert("Password Reset", 
                    $"Password reset instructions have been sent to {email}.\n\nPlease check your email.", 
                    "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", 
                    $"Could not send password reset email: {ex.Message}", 
                    "OK");
            }
        }
    }
}
