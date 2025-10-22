using System;
using System.Text.RegularExpressions;
using AppBantayBarangay.Models;
using AppBantayBarangay.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppBantayBarangay.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegistrationPage : ContentPage
    {
        private readonly IFirebaseService _firebaseService;

        public RegistrationPage()
        {
            InitializeComponent();
            _firebaseService = DependencyService.Get<IFirebaseService>();
        }

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

                // Step 3: Create user profile data (ALWAYS Resident - Officials are created manually by administrators)
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

                System.Diagnostics.Debug.WriteLine($"Resident account created successfully for: {user.FullName}");

                // Step 5: Sign out the user (they need to login)
                await _firebaseService.SignOutAsync();

                // Step 6: Show success message
                await DisplayAlert("Success", 
                    $"Welcome {user.FirstName}! Your Resident account has been created successfully.\n\nYou can now login with:\nEmail: {email}\n\nNote: If you are a Barangay Official, please contact the administrator to upgrade your account.", 
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

        private bool ValidateInputs()
        {
            // First Name validation
            if (string.IsNullOrWhiteSpace(FirstNameEntry.Text))
            {
                DisplayAlert("Required Field", "Please enter your first name.", "OK");
                return false;
            }

            // Last Name validation
            if (string.IsNullOrWhiteSpace(LastNameEntry.Text))
            {
                DisplayAlert("Required Field", "Please enter your last name.", "OK");
                return false;
            }

            // Email validation
            if (string.IsNullOrWhiteSpace(EmailEntry.Text))
            {
                DisplayAlert("Required Field", "Please enter your email address.", "OK");
                return false;
            }

            if (!IsValidEmail(EmailEntry.Text))
            {
                DisplayAlert("Invalid Email", "Please enter a valid email address.", "OK");
                return false;
            }

            // Phone Number validation
            if (string.IsNullOrWhiteSpace(PhoneNumberEntry.Text))
            {
                DisplayAlert("Required Field", "Please enter your phone number.", "OK");
                return false;
            }

            if (!IsValidPhoneNumber(PhoneNumberEntry.Text))
            {
                DisplayAlert("Invalid Phone Number", 
                    "Please enter a valid Philippine phone number (e.g., 09XX XXX XXXX).", 
                    "OK");
                return false;
            }

            // Address validation
            if (string.IsNullOrWhiteSpace(AddressEntry.Text))
            {
                DisplayAlert("Required Field", "Please enter your address.", "OK");
                return false;
            }

            // Password validation
            if (string.IsNullOrWhiteSpace(PasswordEntry.Text))
            {
                DisplayAlert("Required Field", "Please enter a password.", "OK");
                return false;
            }

            if (PasswordEntry.Text.Length < 6)
            {
                DisplayAlert("Weak Password", 
                    "Password must be at least 6 characters long.", 
                    "OK");
                return false;
            }

            // Confirm Password validation
            if (string.IsNullOrWhiteSpace(ConfirmPasswordEntry.Text))
            {
                DisplayAlert("Required Field", "Please confirm your password.", "OK");
                return false;
            }

            if (PasswordEntry.Text != ConfirmPasswordEntry.Text)
            {
                DisplayAlert("Password Mismatch", 
                    "Passwords do not match. Please try again.", 
                    "OK");
                return false;
            }

            return true;
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Trim the email
                email = email.Trim();
                
                // Simple but effective email validation pattern
                var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                return Regex.IsMatch(email, emailPattern, RegexOptions.IgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return false;

            try
            {
                // Remove spaces and dashes
                var cleanNumber = phoneNumber.Replace(" ", "").Replace("-", "");
                
                // Check if it's a valid Philippine mobile number (09XX XXX XXXX or +639XX XXX XXXX)
                var phonePattern = @"^(09|\+639)\d{9}$";
                return Regex.IsMatch(cleanNumber, phonePattern);
            }
            catch
            {
                return false;
            }
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync(); // Go back to login page
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync(); // Go back to login page
        }
    }
}
