using System;
using AppBantayBarangay.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppBantayBarangay.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegistrationPage : ContentPage
    {
        public RegistrationPage()
        {
            InitializeComponent();
        }

        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            // Create new user with available data
            var user = new User
            {
                FirstName = FirstNameEntry?.Text ?? string.Empty,
                MiddleName = MiddleNameEntry?.Text ?? string.Empty,
                LastName = LastNameEntry?.Text ?? string.Empty,
                Email = EmailEntry?.Text ?? string.Empty,
                Password = PasswordEntry?.Text ?? string.Empty,
                Address = AddressEntry?.Text ?? string.Empty,
                PhoneNumber = PhoneNumberEntry?.Text ?? string.Empty,
                UserType = UserType.Resident // Default to Resident for new registrations
            };

            // TODO: Implement actual registration logic

            await DisplayAlert("Success", "Registration successful! Please login.", "OK");
            await Navigation.PopAsync(); // Go back to login page
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync(); // Go back to login page
        }
    }
}