using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using AppBantayBarangay.Services;

namespace AppBantayBarangay.Views
{
    /// <summary>
    /// Example login page demonstrating Firebase authentication
    /// This is a reference implementation - adapt it to your needs
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FirebaseLoginExample : ContentPage
    {
        private readonly IFirebaseService _firebaseService;

        public FirebaseLoginExample()
        {
            InitializeComponent();
            
            // Get Firebase service using DependencyService
            _firebaseService = DependencyService.Get<IFirebaseService>();
        }

        private async void OnSignInClicked(object sender, EventArgs e)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(EmailEntry.Text) || 
                    string.IsNullOrWhiteSpace(PasswordEntry.Text))
                {
                    await DisplayAlert("Error", "Please enter email and password", "OK");
                    return;
                }

                // Show loading indicator
                ActivityIndicator.IsRunning = true;
                SignInButton.IsEnabled = false;

                // Attempt sign in
                bool success = await _firebaseService.SignInAsync(
                    EmailEntry.Text.Trim(), 
                    PasswordEntry.Text
                );

                if (success)
                {
                    string userId = _firebaseService.GetCurrentUserId();
                    await DisplayAlert("Success", $"Signed in successfully!\nUser ID: {userId}", "OK");
                    
                    // Navigate to main page or dashboard
                    // await Navigation.PushAsync(new MainPage());
                }
                else
                {
                    await DisplayAlert("Error", "Invalid email or password", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Sign in failed: {ex.Message}", "OK");
            }
            finally
            {
                // Hide loading indicator
                ActivityIndicator.IsRunning = false;
                SignInButton.IsEnabled = true;
            }
        }

        private async void OnSignUpClicked(object sender, EventArgs e)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(EmailEntry.Text) || 
                    string.IsNullOrWhiteSpace(PasswordEntry.Text))
                {
                    await DisplayAlert("Error", "Please enter email and password", "OK");
                    return;
                }

                if (PasswordEntry.Text.Length < 6)
                {
                    await DisplayAlert("Error", "Password must be at least 6 characters", "OK");
                    return;
                }

                // Show loading indicator
                ActivityIndicator.IsRunning = true;
                SignUpButton.IsEnabled = false;

                // Attempt sign up
                bool success = await _firebaseService.SignUpAsync(
                    EmailEntry.Text.Trim(), 
                    PasswordEntry.Text
                );

                if (success)
                {
                    await DisplayAlert("Success", "Account created successfully!", "OK");
                    
                    // Optionally save user profile to database
                    string userId = _firebaseService.GetCurrentUserId();
                    var userProfile = new
                    {
                        Email = EmailEntry.Text.Trim(),
                        CreatedAt = DateTime.UtcNow.ToString("o"),
                        Role = "Resident"
                    };
                    
                    await _firebaseService.SaveDataAsync($"users/{userId}/profile", userProfile);
                    
                    // Navigate to main page
                    // await Navigation.PushAsync(new MainPage());
                }
                else
                {
                    await DisplayAlert("Error", "Failed to create account. Email may already be in use.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Sign up failed: {ex.Message}", "OK");
            }
            finally
            {
                // Hide loading indicator
                ActivityIndicator.IsRunning = false;
                SignUpButton.IsEnabled = true;
            }
        }

        private async void OnForgotPasswordClicked(object sender, EventArgs e)
        {
            // Implement password reset functionality
            await DisplayAlert("Info", "Password reset functionality can be implemented using Firebase Auth", "OK");
        }
    }
}
