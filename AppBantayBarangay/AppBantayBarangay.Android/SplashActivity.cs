using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.AppCompat.App;
using System.Threading.Tasks;

namespace AppBantayBarangay.Droid
{
    [Activity(
        Label = "Bantay Barangay",
        Theme = "@style/Theme.AppCompat.Light.NoActionBar",
        MainLauncher = true,
        NoHistory = true,
        Icon = "@mipmap/icon")]
    public class SplashActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            // Set the splash screen layout
            SetContentView(Resource.Layout.splash_screen);
            
            System.Diagnostics.Debug.WriteLine("🚀 SplashActivity: Starting...");
        }

        protected override void OnResume()
        {
            base.OnResume();
            
            // Start the main activity after a short delay
            Task.Run(async () =>
            {
                // Show splash for 2 seconds
                await Task.Delay(2000);
                
                System.Diagnostics.Debug.WriteLine("🚀 SplashActivity: Launching MainActivity...");
                
                // Start MainActivity
                StartActivity(new Intent(Application.Context, typeof(MainActivity)));
                
                // Close splash activity
                Finish();
            });
        }

        public override void OnBackPressed()
        {
            // Disable back button on splash screen
        }
    }
}
