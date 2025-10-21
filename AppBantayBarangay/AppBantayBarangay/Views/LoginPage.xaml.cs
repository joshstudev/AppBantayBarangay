using System;
using AppBantayBarangay.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppBantayBarangay.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            // Get the selected user type, default to Resident if none selected
            var userType = UserTypePicker.SelectedIndex != -1 && 
                          UserTypePicker.SelectedItem.ToString() == "Official" ? 
                          UserType.Official : UserType.Resident;

            // Navigate to appropriate page based on user type
            if (userType == UserType.Official)
            {
                await Navigation.PushAsync(new OfficialPage());
            }
            else
            {
                await Navigation.PushAsync(new ResidentPage());
            }
        }

        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RegistrationPage());
        }
    }
}