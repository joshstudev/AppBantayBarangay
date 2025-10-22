using AppBantayBarangay.Models;
using AppBantayBarangay.Services;
using System;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppBantayBarangay.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ResidentPage : ContentPage
    {
        private double currentLatitude;
        private double currentLongitude;
        private string currentAddress;
        private User currentUser;
        private string currentImageBase64 = "";

        public ResidentPage()
        {
            InitializeComponent();
        }

        public ResidentPage(User user)
        {
            InitializeComponent();
            currentUser = user;
            if (currentUser != null)
            {
                WelcomeLabel.Text = $"Welcome, {currentUser.FirstName}!";
            }
        }

        private async void UploadPhotoButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Please pick a photo"
                });

                if (result != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Image selected: {result.FullPath}");
                    
                    // Convert image to Base64
                    using (var stream = await result.OpenReadAsync())
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await stream.CopyToAsync(memoryStream);
                            byte[] imageBytes = memoryStream.ToArray();
                            currentImageBase64 = Convert.ToBase64String(imageBytes);
                            System.Diagnostics.Debug.WriteLine($"Image converted to Base64, size: {currentImageBase64.Length} characters");
                        }
                    }
                    
                    // Display image
                    var displayStream = await result.OpenReadAsync();
                    UploadedImage.Source = ImageSource.FromStream(() => displayStream);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Upload photo error: {ex.Message}");
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }

        private async void TakePhotoButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                var result = await MediaPicker.CapturePhotoAsync();

                if (result != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Photo captured: {result.FullPath}");
                    
                    // Convert image to Base64
                    using (var stream = await result.OpenReadAsync())
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await stream.CopyToAsync(memoryStream);
                            byte[] imageBytes = memoryStream.ToArray();
                            currentImageBase64 = Convert.ToBase64String(imageBytes);
                            System.Diagnostics.Debug.WriteLine($"Image converted to Base64, size: {currentImageBase64.Length} characters");
                        }
                    }
                    
                    // Display image
                    var displayStream = await result.OpenReadAsync();
                    UploadedImage.Source = ImageSource.FromStream(() => displayStream);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Take photo error: {ex.Message}");
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }

        private async void PinLocationButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                // Show loading indicator
                PinLocationButton.Text = "📍 Getting Location...";
                PinLocationButton.IsEnabled = false;

                var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
                var location = await Geolocation.GetLocationAsync(request);

                if (location != null)
                {
                    currentLatitude = location.Latitude;
                    currentLongitude = location.Longitude;

                    // Update the UI with coordinates
                    LatitudeLabel.Text = currentLatitude.ToString("F6");
                    LongitudeLabel.Text = currentLongitude.ToString("F6");

                    // Try to get address from coordinates
                    try
                    {
                        var placemarks = await Geocoding.GetPlacemarksAsync(location.Latitude, location.Longitude);
                        if (placemarks != null)
                        {
                            var placemark = System.Linq.Enumerable.FirstOrDefault(placemarks);
                            if (placemark != null)
                            {
                                currentAddress = $"{placemark.Thoroughfare}, {placemark.Locality}, {placemark.AdminArea}";
                                AddressLabel.Text = currentAddress;
                            }
                            else
                            {
                                currentAddress = $"Lat: {currentLatitude:F6}, Long: {currentLongitude:F6}";
                                AddressLabel.Text = "Address not available";
                            }
                        }
                    }
                    catch
                    {
                        currentAddress = $"Lat: {currentLatitude:F6}, Long: {currentLongitude:F6}";
                        AddressLabel.Text = "Address not available";
                    }

                    // Show the location frame
                    LocationFrame.IsVisible = true;

                    await DisplayAlert("Success", "Location captured successfully!", "OK");
                }
                else
                {
                    await DisplayAlert("Error", "Unable to get location. Please try again.", "OK");
                }
            }
            catch (FeatureNotSupportedException)
            {
                await DisplayAlert("Error", "Geolocation is not supported on this device.", "OK");
            }
            catch (FeatureNotEnabledException)
            {
                await DisplayAlert("Error", "Please enable location services in your device settings.", "OK");
            }
            catch (PermissionException)
            {
                await DisplayAlert("Error", "Location permission is required. Please grant permission in settings.", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred while getting location: {ex.Message}", "OK");
            }
            finally
            {
                // Reset button
                PinLocationButton.Text = "📍 Get Current Location";
                PinLocationButton.IsEnabled = true;
            }
        }

        private async void SubmitButton_Clicked(object sender, EventArgs e)
        {
            // Validate all fields
            var description = DescriptionEditor.Text;
            var image = UploadedImage.Source;
            var hasLocation = LocationFrame.IsVisible;

            if (string.IsNullOrWhiteSpace(description))
            {
                await DisplayAlert("Missing Information", "Please provide a description of the issue.", "OK");
                return;
            }

            if (image == null)
            {
                await DisplayAlert("Missing Information", "Please upload or take a photo of the issue.", "OK");
                return;
            }

            if (!hasLocation)
            {
                await DisplayAlert("Missing Information", "Please capture your current location.", "OK");
                return;
            }

            var confirm = await DisplayAlert("Confirm Submission",
                "Are you sure you want to submit this report?",
                "Yes", "No");

            if (!confirm)
                return;

            try
            {
                SubmitButton.Text = "⏳ Submitting...";
                SubmitButton.IsEnabled = false;

                System.Diagnostics.Debug.WriteLine("=== Starting Report Submission ===");
                
                // CRITICAL: Validate user is logged in
                if (currentUser == null)
                {
                    System.Diagnostics.Debug.WriteLine("❌ CRITICAL ERROR: currentUser is NULL!");
                    await DisplayAlert("Error", "User session expired. Please log in again.", "OK");
                    await Navigation.PopToRootAsync();
                    return;
                }
                
                System.Diagnostics.Debug.WriteLine($"✅ Current User Validated:");
                System.Diagnostics.Debug.WriteLine($"   UserId: {currentUser.UserId}");
                System.Diagnostics.Debug.WriteLine($"   Name: {currentUser.FullName}");
                System.Diagnostics.Debug.WriteLine($"   Email: {currentUser.Email}");

                // Get Firebase service
                var firebaseService = DependencyService.Get<IFirebaseService>();
                
                if (firebaseService == null)
                {
                    System.Diagnostics.Debug.WriteLine("ERROR: Firebase service is null");
                    await DisplayAlert("Error", "Firebase service not available. Please restart the app.", "OK");
                    return;
                }

                // Generate report ID
                string reportId = Guid.NewGuid().ToString();
                System.Diagnostics.Debug.WriteLine($"Report ID: {reportId}");

                // Use Base64 image data instead of uploading to Storage
                string imageData = currentImageBase64;
                Console.WriteLine("Image data prepared for report." + imageData);
                if (string.IsNullOrEmpty(imageData))
                {
                    System.Diagnostics.Debug.WriteLine("WARNING: No image data available");
                    imageData = "";
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Image data size: {imageData.Length} characters");
                }

                // Log values BEFORE creating report object
                System.Diagnostics.Debug.WriteLine($"📋 Values to be saved:");
                System.Diagnostics.Debug.WriteLine($"   description variable: '{description ?? "NULL"}'");
                System.Diagnostics.Debug.WriteLine($"   currentAddress variable: '{currentAddress ?? "NULL"}'");
                System.Diagnostics.Debug.WriteLine($"   currentLatitude: {currentLatitude}");
                System.Diagnostics.Debug.WriteLine($"   currentLongitude: {currentLongitude}");
                System.Diagnostics.Debug.WriteLine($"   imageData length: {imageData?.Length ?? 0}");
                
                // Create report object (currentUser is now guaranteed to be non-null)
                var report = new Report
                {
                    ReportId = reportId,
                    Description = description,
                    ImageUrl = imageData, // Store Base64 string instead of URL
                    Latitude = currentLatitude,
                    Longitude = currentLongitude,
                    LocationAddress = currentAddress,
                    DateReported = DateTime.UtcNow.ToString("o"),
                    Status = ReportStatus.Pending,
                    ReportedBy = currentUser.UserId,  // No null-coalescing needed
                    ReporterName = currentUser.FullName,
                    ReporterEmail = currentUser.Email
                };
                
                System.Diagnostics.Debug.WriteLine($"📝 Report Object Created:");
                System.Diagnostics.Debug.WriteLine($"   ReportId: {report.ReportId}");
                System.Diagnostics.Debug.WriteLine($"   Description: '{report.Description ?? "NULL"}' (length: {report.Description?.Length ?? 0})");
                System.Diagnostics.Debug.WriteLine($"   LocationAddress: '{report.LocationAddress ?? "NULL"}'");
                System.Diagnostics.Debug.WriteLine($"   Coordinates: ({report.Latitude}, {report.Longitude})");
                System.Diagnostics.Debug.WriteLine($"   ReportedBy (UserId): {report.ReportedBy}");
                System.Diagnostics.Debug.WriteLine($"   ReporterName: {report.ReporterName}");
                System.Diagnostics.Debug.WriteLine($"   ReporterEmail: {report.ReporterEmail}");
                System.Diagnostics.Debug.WriteLine($"   Status: {report.Status}");
                System.Diagnostics.Debug.WriteLine($"   ImageUrl length: {report.ImageUrl?.Length ?? 0}");

                System.Diagnostics.Debug.WriteLine($"Saving report to Firebase...");

                // Save to Firebase Database
                string reportPath = $"reports/{reportId}";
                bool success = await firebaseService.SaveDataAsync(reportPath, report);

                System.Diagnostics.Debug.WriteLine($"💾 Firebase Save Result: {success}");

                if (success)
                {
                    System.Diagnostics.Debug.WriteLine($"✅ SUCCESS: Report saved to Firebase at path: {reportPath}");
                    System.Diagnostics.Debug.WriteLine($"   This report should now be visible to:");
                    System.Diagnostics.Debug.WriteLine($"   - Officials (all reports)");
                    System.Diagnostics.Debug.WriteLine($"   - Resident {currentUser.UserId} (their history)");
                    
                    await DisplayAlert("Success", 
                        "Your report has been submitted successfully! Barangay officials will review it soon.", 
                        "OK");
                    ClearForm();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"❌ FAILED: Could not save report to Firebase");
                    await DisplayAlert("Error", 
                        "Failed to submit report to database. Please check your internet connection and try again.", 
                        "OK");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"=== Submit Error ===");
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                
                await DisplayAlert("Error", 
                    $"Failed to submit report:\n{ex.Message}\n\nPlease try again or contact support.", 
                    "OK");
            }
            finally
            {
                SubmitButton.Text = "✓ Submit Report";
                SubmitButton.IsEnabled = true;
                System.Diagnostics.Debug.WriteLine("=== Submission Complete ===");
            }
        }

        private void ClearForm()
        {
            DescriptionEditor.Text = string.Empty;
            UploadedImage.Source = null;
            LocationFrame.IsVisible = false;
            LatitudeLabel.Text = "--";
            LongitudeLabel.Text = "--";
            AddressLabel.Text = "--";
            currentLatitude = 0;
            currentLongitude = 0;
            currentAddress = string.Empty;
            currentImageBase64 = "";
        }

        private async void MyReportsButton_Clicked(object sender, EventArgs e)
        {
            // Navigate to Report History page
            await Navigation.PushAsync(new ReportHistoryPage(currentUser));
        }

        private async void LogoutButton_Clicked(object sender, EventArgs e)
        {
            var confirm = await DisplayAlert("Logout", 
                "Are you sure you want to logout?", 
                "Yes", "No");

            if (confirm)
            {
                // Navigate back to login page
                await Navigation.PopToRootAsync();
            }
        }
    }
}
