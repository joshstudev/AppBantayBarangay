using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AppBantayBarangay.Services
{
    /// <summary>
    /// Example usage of Firebase services
    /// This class demonstrates how to use the IFirebaseService in your app
    /// </summary>
    public class FirebaseUsageExample
    {
        private readonly IFirebaseService _firebaseService;

        public FirebaseUsageExample()
        {
            // Get Firebase service using Xamarin.Forms DependencyService
            _firebaseService = DependencyService.Get<IFirebaseService>();
        }

        /// <summary>
        /// Example: User Sign Up
        /// </summary>
        public async Task<bool> SignUpUserExample(string email, string password)
        {
            try
            {
                bool success = await _firebaseService.SignUpAsync(email, password);
                if (success)
                {
                    System.Diagnostics.Debug.WriteLine("User signed up successfully!");
                    return true;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Sign up failed");
                    return false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Example: User Sign In
        /// </summary>
        public async Task<bool> SignInUserExample(string email, string password)
        {
            try
            {
                bool success = await _firebaseService.SignInAsync(email, password);
                if (success)
                {
                    string userId = _firebaseService.GetCurrentUserId();
                    System.Diagnostics.Debug.WriteLine($"User signed in! User ID: {userId}");
                    return true;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Sign in failed");
                    return false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Example: Save Report to Firebase Database
        /// </summary>
        public async Task<bool> SaveReportExample()
        {
            try
            {
                // Create a sample report object
                var report = new
                {
                    Title = "Broken Street Light",
                    Description = "Street light on Main St is not working",
                    Location = "Main Street, Barangay Center",
                    Timestamp = DateTime.UtcNow.ToString("o"),
                    Status = "Pending",
                    ReportedBy = _firebaseService.GetCurrentUserId()
                };

                // Save to Firebase Database under "reports/{userId}/{reportId}"
                string userId = _firebaseService.GetCurrentUserId();
                string reportId = Guid.NewGuid().ToString();
                string path = $"reports/{userId}/{reportId}";

                bool success = await _firebaseService.SaveDataAsync(path, report);
                if (success)
                {
                    System.Diagnostics.Debug.WriteLine($"Report saved successfully at: {path}");
                    return true;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Failed to save report");
                    return false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Example: Get Report from Firebase Database
        /// </summary>
        public async Task<object> GetReportExample(string userId, string reportId)
        {
            try
            {
                string path = $"reports/{userId}/{reportId}";
                var report = await _firebaseService.GetDataAsync<object>(path);

                if (report != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Report retrieved: {report}");
                    return report;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Report not found");
                    return null;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Example: Upload Image to Firebase Storage
        /// </summary>
        public async Task<string> UploadImageExample(string localImagePath)
        {
            try
            {
                string userId = _firebaseService.GetCurrentUserId();
                string fileName = System.IO.Path.GetFileName(localImagePath);
                string storagePath = $"reports/{userId}/{fileName}";

                string downloadUrl = await _firebaseService.UploadFileAsync(localImagePath, storagePath);
                
                if (!string.IsNullOrEmpty(downloadUrl))
                {
                    System.Diagnostics.Debug.WriteLine($"Image uploaded! Download URL: {downloadUrl}");
                    return downloadUrl;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Failed to upload image");
                    return null;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Example: Check Authentication Status
        /// </summary>
        public void CheckAuthenticationExample()
        {
            if (_firebaseService.IsAuthenticated())
            {
                string userId = _firebaseService.GetCurrentUserId();
                System.Diagnostics.Debug.WriteLine($"User is authenticated. User ID: {userId}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("User is not authenticated");
            }
        }

        /// <summary>
        /// Example: Sign Out
        /// </summary>
        public async Task<bool> SignOutExample()
        {
            try
            {
                await _firebaseService.SignOutAsync();
                System.Diagnostics.Debug.WriteLine("User signed out successfully");
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }
    }
}
