using System.Threading.Tasks;

namespace AppBantayBarangay.Services
{
    /// <summary>
    /// Interface for Firebase services
    /// </summary>
    public interface IFirebaseService
    {
        /// <summary>
        /// Sign in with email and password
        /// </summary>
        Task<bool> SignInAsync(string email, string password);

        /// <summary>
        /// Sign up with email and password
        /// </summary>
        Task<bool> SignUpAsync(string email, string password);

        /// <summary>
        /// Sign out current user
        /// </summary>
        Task SignOutAsync();

        /// <summary>
        /// Get current user ID
        /// </summary>
        string GetCurrentUserId();

        /// <summary>
        /// Check if user is authenticated
        /// </summary>
        bool IsAuthenticated();

        /// <summary>
        /// Save data to Firebase Realtime Database
        /// </summary>
        Task<bool> SaveDataAsync(string path, object data);

        /// <summary>
        /// Get data from Firebase Realtime Database
        /// </summary>
        Task<T> GetDataAsync<T>(string path);

        /// <summary>
        /// Upload file to Firebase Storage
        /// </summary>
        Task<string> UploadFileAsync(string localPath, string storagePath);

        /// <summary>
        /// Download file from Firebase Storage
        /// </summary>
        Task<string> DownloadFileAsync(string storagePath, string localPath);
    }
}
