# Official Page - Implementation Guide

## 🚀 Quick Start

### Prerequisites
- Xamarin.Forms project setup
- Xamarin.Forms.Maps package installed
- .NET Standard 2.0 or higher

### Installation Steps

1. **Add the Report Model**
   ```bash
   # File already created at:
   AppBantayBarangay/Models/Report.cs
   ```

2. **Update OfficialPage Files**
   ```bash
   # Files already created at:
   AppBantayBarangay/Views/OfficialPage.xaml
   AppBantayBarangay/Views/OfficialPage.xaml.cs
   ```

3. **Configure Map Services**
   
   **For Android** (MainActivity.cs):
   ```csharp
   protected override void OnCreate(Bundle savedInstanceState)
   {
       base.OnCreate(savedInstanceState);
       
       Xamarin.Essentials.Platform.Init(this, savedInstanceState);
       Xamarin.FormsMaps.Init(this, savedInstanceState);
       
       LoadApplication(new App());
   }
   ```

   **For iOS** (AppDelegate.cs):
   ```csharp
   public override bool FinishedLaunching(UIApplication app, NSDictionary options)
   {
       Xamarin.Forms.Forms.Init();
       Xamarin.FormsMaps.Init();
       
       LoadApplication(new App());
       return base.FinishedLaunching(app, options);
   }
   ```

4. **Add Required Permissions**

   **Android** (AndroidManifest.xml):
   ```xml
   <uses-permission android:name="android.permission.ACCESS_COARSE_LOCATION" />
   <uses-permission android:name="android.permission.ACCESS_FINE_LOCATION" />
   <uses-permission android:name="android.permission.INTERNET" />
   ```

   **iOS** (Info.plist):
   ```xml
   <key>NSLocationWhenInUseUsageDescription</key>
   <string>This app needs access to location to display report locations.</string>
   ```

---

## 🔧 Integration with Backend

### Database Schema

#### Reports Table
```sql
CREATE TABLE Reports (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    Description TEXT NOT NULL,
    ImagePath VARCHAR(255),
    Latitude DECIMAL(10, 8),
    Longitude DECIMAL(11, 8),
    LocationAddress VARCHAR(255),
    DateReported DATETIME NOT NULL,
    Status ENUM('Pending', 'InProgress', 'Resolved', 'Rejected') DEFAULT 'Pending',
    ReportedBy VARCHAR(100),
    ReportedByUserId INT,
    ResolvedBy VARCHAR(100),
    ResolvedByUserId INT,
    DateResolved DATETIME,
    ResolutionNotes TEXT,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (ReportedByUserId) REFERENCES Users(Id),
    FOREIGN KEY (ResolvedByUserId) REFERENCES Users(Id)
);
```

#### Indexes for Performance
```sql
CREATE INDEX idx_status ON Reports(Status);
CREATE INDEX idx_date_reported ON Reports(DateReported DESC);
CREATE INDEX idx_reported_by ON Reports(ReportedByUserId);
CREATE INDEX idx_resolved_by ON Reports(ResolvedByUserId);
```

### API Endpoints

#### 1. Get All Reports
```http
GET /api/reports
Authorization: Bearer {token}

Response:
{
    "success": true,
    "data": [
        {
            "id": 1,
            "description": "Broken streetlight...",
            "imagePath": "https://cdn.example.com/images/report1.jpg",
            "latitude": 14.5995,
            "longitude": 120.9842,
            "locationAddress": "Main Street, Barangay Centro",
            "dateReported": "2024-05-15T15:30:00Z",
            "status": "Pending",
            "reportedBy": "Juan Dela Cruz",
            "reportedByUserId": 123,
            "resolvedBy": null,
            "dateResolved": null,
            "resolutionNotes": null
        }
    ]
}
```

#### 2. Get Reports by Status
```http
GET /api/reports?status=Pending
Authorization: Bearer {token}

Response: Same as above, filtered by status
```

#### 3. Update Report Status
```http
PUT /api/reports/{id}/status
Authorization: Bearer {token}
Content-Type: application/json

Request Body:
{
    "status": "InProgress",
    "officialId": 456
}

Response:
{
    "success": true,
    "message": "Report status updated successfully",
    "data": {
        "id": 1,
        "status": "InProgress",
        "updatedAt": "2024-05-20T10:15:00Z"
    }
}
```

#### 4. Resolve Report
```http
PUT /api/reports/{id}/resolve
Authorization: Bearer {token}
Content-Type: application/json

Request Body:
{
    "resolutionNotes": "Issue has been fixed...",
    "resolvedByUserId": 456
}

Response:
{
    "success": true,
    "message": "Report marked as resolved",
    "data": {
        "id": 1,
        "status": "Resolved",
        "resolutionNotes": "Issue has been fixed...",
        "resolvedBy": "Official Name",
        "dateResolved": "2024-05-20T14:30:00Z"
    }
}
```

#### 5. Reject Report
```http
PUT /api/reports/{id}/reject
Authorization: Bearer {token}
Content-Type: application/json

Request Body:
{
    "rejectedByUserId": 456,
    "reason": "Duplicate report"
}

Response:
{
    "success": true,
    "message": "Report rejected",
    "data": {
        "id": 1,
        "status": "Rejected"
    }
}
```

### API Service Implementation

Create a new file: `Services/ReportService.cs`

```csharp
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using AppBantayBarangay.Models;

namespace AppBantayBarangay.Services
{
    public class ReportService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://your-api-url.com/api";

        public ReportService()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {GetAuthToken()}");
        }

        public async Task<List<Report>> GetAllReportsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BaseUrl}/reports");
                response.EnsureSuccessStatusCode();
                
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApiResponse<List<Report>>>(json);
                
                return result.Data;
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Error fetching reports: {ex.Message}");
                return new List<Report>();
            }
        }

        public async Task<List<Report>> GetReportsByStatusAsync(ReportStatus status)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BaseUrl}/reports?status={status}");
                response.EnsureSuccessStatusCode();
                
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApiResponse<List<Report>>>(json);
                
                return result.Data;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching reports by status: {ex.Message}");
                return new List<Report>();
            }
        }

        public async Task<bool> UpdateReportStatusAsync(int reportId, ReportStatus status, int officialId)
        {
            try
            {
                var payload = new
                {
                    status = status.ToString(),
                    officialId = officialId
                };

                var json = JsonConvert.SerializeObject(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"{BaseUrl}/reports/{reportId}/status", content);
                response.EnsureSuccessStatusCode();
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating report status: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ResolveReportAsync(int reportId, string resolutionNotes, int resolvedByUserId)
        {
            try
            {
                var payload = new
                {
                    resolutionNotes = resolutionNotes,
                    resolvedByUserId = resolvedByUserId
                };

                var json = JsonConvert.SerializeObject(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"{BaseUrl}/reports/{reportId}/resolve", content);
                response.EnsureSuccessStatusCode();
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error resolving report: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> RejectReportAsync(int reportId, int rejectedByUserId, string reason = "")
        {
            try
            {
                var payload = new
                {
                    rejectedByUserId = rejectedByUserId,
                    reason = reason
                };

                var json = JsonConvert.SerializeObject(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"{BaseUrl}/reports/{reportId}/reject", content);
                response.EnsureSuccessStatusCode();
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error rejecting report: {ex.Message}");
                return false;
            }
        }

        private string GetAuthToken()
        {
            // Retrieve stored auth token
            // This is a placeholder - implement your actual token retrieval
            return Xamarin.Essentials.SecureStorage.GetAsync("auth_token").Result;
        }
    }

    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T Data { get; set; }
        public string Message { get; set; }
    }
}
```

### Update OfficialPage to Use API

Modify `OfficialPage.xaml.cs`:

```csharp
// Add at the top
using AppBantayBarangay.Services;

// Add field
private ReportService _reportService;

// Update constructor
public OfficialPage(User user)
{
    InitializeComponent();
    currentUser = user;
    _reportService = new ReportService();
    
    if (currentUser != null)
    {
        WelcomeLabel.Text = $"Welcome, {currentUser.FirstName}!";
    }
    
    InitializePage();
}

// Update LoadSampleReports to LoadReportsFromApi
private async void LoadReportsFromApi()
{
    try
    {
        // Show loading indicator
        // ActivityIndicator.IsRunning = true;
        
        allReports = await _reportService.GetAllReportsAsync();
        filteredReports = new List<Report>(allReports);
        
        UpdateStatistics();
        DisplayReports();
    }
    catch (Exception ex)
    {
        await DisplayAlert("Error", $"Failed to load reports: {ex.Message}", "OK");
    }
    finally
    {
        // Hide loading indicator
        // ActivityIndicator.IsRunning = false;
    }
}

// Update MarkResolved_Clicked
private async void MarkResolved_Clicked(object sender, EventArgs e)
{
    if (selectedReport == null) return;

    if (!ResolutionInputContainer.IsVisible)
    {
        ResolutionInputContainer.IsVisible = true;
        MarkResolvedButton.Text = "Confirm Resolution";
        return;
    }

    if (string.IsNullOrWhiteSpace(ResolutionNotesEditor.Text))
    {
        await DisplayAlert("Required", "Please enter resolution notes.", "OK");
        return;
    }

    var confirm = await DisplayAlert("Confirm", 
        "Mark this report as Resolved?", 
        "Yes", "No");

    if (confirm)
    {
        var success = await _reportService.ResolveReportAsync(
            selectedReport.Id, 
            ResolutionNotesEditor.Text, 
            currentUser.Id
        );

        if (success)
        {
            selectedReport.Status = ReportStatus.Resolved;
            selectedReport.ResolutionNotes = ResolutionNotesEditor.Text;
            selectedReport.ResolvedBy = $"{currentUser.FirstName} {currentUser.LastName}";
            selectedReport.DateResolved = DateTime.Now;

            UpdateStatistics();
            DisplayReports();
            CloseModal_Clicked(null, null);
            await DisplayAlert("Success", "Report marked as Resolved", "OK");
        }
        else
        {
            await DisplayAlert("Error", "Failed to update report. Please try again.", "OK");
        }
    }
}
```

---

## 📊 Adding Real-Time Updates

### Using SignalR for Live Updates

1. **Install NuGet Package**
   ```bash
   Install-Package Microsoft.AspNetCore.SignalR.Client
   ```

2. **Create SignalR Service**

Create `Services/SignalRService.cs`:

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace AppBantayBarangay.Services
{
    public class SignalRService
    {
        private HubConnection _hubConnection;
        public event EventHandler<Report> OnNewReport;
        public event EventHandler<Report> OnReportUpdated;

        public async Task ConnectAsync()
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl("https://your-api-url.com/reportHub")
                .Build();

            _hubConnection.On<Report>("NewReport", (report) =>
            {
                OnNewReport?.Invoke(this, report);
            });

            _hubConnection.On<Report>("ReportUpdated", (report) =>
            {
                OnReportUpdated?.Invoke(this, report);
            });

            await _hubConnection.StartAsync();
        }

        public async Task DisconnectAsync()
        {
            if (_hubConnection != null)
            {
                await _hubConnection.StopAsync();
                await _hubConnection.DisposeAsync();
            }
        }
    }
}
```

3. **Integrate in OfficialPage**

```csharp
private SignalRService _signalRService;

protected override async void OnAppearing()
{
    base.OnAppearing();
    
    _signalRService = new SignalRService();
    _signalRService.OnNewReport += OnNewReportReceived;
    _signalRService.OnReportUpdated += OnReportUpdatedReceived;
    
    await _signalRService.ConnectAsync();
}

protected override async void OnDisappearing()
{
    base.OnDisappearing();
    
    if (_signalRService != null)
    {
        await _signalRService.DisconnectAsync();
    }
}

private void OnNewReportReceived(object sender, Report report)
{
    Device.BeginInvokeOnMainThread(() =>
    {
        allReports.Insert(0, report);
        UpdateStatistics();
        DisplayReports();
        
        DisplayAlert("New Report", 
            $"A new report has been submitted: {report.Description}", 
            "OK");
    });
}

private void OnReportUpdatedReceived(object sender, Report report)
{
    Device.BeginInvokeOnMainThread(() =>
    {
        var existingReport = allReports.FirstOrDefault(r => r.Id == report.Id);
        if (existingReport != null)
        {
            var index = allReports.IndexOf(existingReport);
            allReports[index] = report;
            UpdateStatistics();
            DisplayReports();
        }
    });
}
```

---

## 🎨 Customization Guide

### Changing Colors

Edit the ResourceDictionary in `OfficialPage.xaml`:

```xml
<ContentPage.Resources>
    <ResourceDictionary>
        <!-- Change these values to customize colors -->
        <Color x:Key="PrimaryBlue">#007AFF</Color>      <!-- Main brand color -->
        <Color x:Key="AccentYellow">#FFD700</Color>     <!-- Highlights -->
        <Color x:Key="AccentRed">#FF3B30</Color>        <!-- Danger/Reject -->
        <Color x:Key="AccentGreen">#34C759</Color>      <!-- Success/Resolved -->
        <Color x:Key="AccentOrange">#FF9500</Color>     <!-- Warning/Pending -->
        <Color x:Key="LightGray">#F5F5F5</Color>        <!-- Background -->
        <Color x:Key="MediumGray">#E0E0E0</Color>       <!-- Borders -->
        <Color x:Key="DarkGray">#666666</Color>         <!-- Text -->
    </ResourceDictionary>
</ContentPage.Resources>
```

### Adding Custom Fonts

1. Add font files to your project
2. Update `AssemblyInfo.cs`:
   ```csharp
   [assembly: ExportFont("YourFont-Regular.ttf", Alias = "RegularFont")]
   [assembly: ExportFont("YourFont-Bold.ttf", Alias = "BoldFont")]
   ```

3. Use in XAML:
   ```xml
   <Label Text="Custom Font" FontFamily="RegularFont" />
   ```

### Adding Icons

Replace emoji with icon fonts (e.g., FontAwesome):

```xml
<Label Text="&#xf017;" FontFamily="FontAwesome" FontSize="30" />
```

---

## 🔒 Security Best Practices

### 1. Authentication
```csharp
// Store token securely
await SecureStorage.SetAsync("auth_token", token);

// Retrieve token
var token = await SecureStorage.GetAsync("auth_token");

// Clear on logout
SecureStorage.Remove("auth_token");
```

### 2. Input Validation
```csharp
private bool ValidateResolutionNotes(string notes)
{
    if (string.IsNullOrWhiteSpace(notes))
        return false;
    
    if (notes.Length < 10)
    {
        DisplayAlert("Validation", "Resolution notes must be at least 10 characters.", "OK");
        return false;
    }
    
    if (notes.Length > 1000)
    {
        DisplayAlert("Validation", "Resolution notes cannot exceed 1000 characters.", "OK");
        return false;
    }
    
    return true;
}
```

### 3. Role-Based Access
```csharp
public OfficialPage(User user)
{
    InitializeComponent();
    
    if (user.UserType != UserType.Official)
    {
        DisplayAlert("Access Denied", "You don't have permission to access this page.", "OK");
        Navigation.PopAsync();
        return;
    }
    
    currentUser = user;
    InitializePage();
}
```

---

## 📈 Performance Optimization

### 1. Lazy Loading
```csharp
private const int PageSize = 20;
private int currentPage = 0;

private async Task LoadMoreReports()
{
    var newReports = await _reportService.GetReportsPagedAsync(currentPage, PageSize);
    allReports.AddRange(newReports);
    currentPage++;
    DisplayReports();
}
```

### 2. Image Caching
```csharp
// Use FFImageLoading for better image performance
// Install-Package Xamarin.FFImageLoading.Forms

<ffimageloading:CachedImage 
    Source="{Binding ImagePath}"
    CacheDuration="7"
    DownsampleToViewSize="True"
    HeightRequest="200"/>
```

### 3. Virtual Scrolling
```csharp
// Replace StackLayout with CollectionView for better performance
<CollectionView ItemsSource="{Binding FilteredReports}">
    <CollectionView.ItemTemplate>
        <DataTemplate>
            <!-- Report card template -->
        </DataTemplate>
    </CollectionView.ItemTemplate>
</CollectionView>
```

---

## 🧪 Testing

### Unit Tests Example

Create `Tests/OfficialPageTests.cs`:

```csharp
using Xunit;
using AppBantayBarangay.Models;
using AppBantayBarangay.Views;

namespace AppBantayBarangay.Tests
{
    public class OfficialPageTests
    {
        [Fact]
        public void GetStatusColor_Pending_ReturnsOrange()
        {
            // Arrange
            var page = new OfficialPage();
            
            // Act
            var color = page.GetStatusColor(ReportStatus.Pending);
            
            // Assert
            Assert.Equal(Color.FromHex("#FF9500"), color);
        }

        [Fact]
        public void FilterReports_ByPending_ReturnsOnlyPendingReports()
        {
            // Arrange
            var reports = new List<Report>
            {
                new Report { Status = ReportStatus.Pending },
                new Report { Status = ReportStatus.Resolved },
                new Report { Status = ReportStatus.Pending }
            };
            
            // Act
            var filtered = reports.Where(r => r.Status == ReportStatus.Pending).ToList();
            
            // Assert
            Assert.Equal(2, filtered.Count);
        }
    }
}
```

---

## 📱 Platform-Specific Considerations

### Android
- **Back Button**: Handle hardware back button
  ```csharp
  protected override bool OnBackButtonPressed()
  {
      if (ReportDetailsModal.IsVisible)
      {
          CloseModal_Clicked(null, null);
          return true;
      }
      return base.OnBackButtonPressed();
  }
  ```

### iOS
- **Safe Area**: Respect notch and home indicator
  ```xml
  <ContentPage xmlns:ios="clr-namespace:Xamarin.Forms.PlatformConfiguration.iOSSpecific;assembly=Xamarin.Forms.Core"
               ios:Page.UseSafeArea="True">
  ```

---

## 🐛 Troubleshooting

### Common Issues

1. **Maps not displaying**
   - Ensure API keys are configured
   - Check permissions are granted
   - Verify internet connection

2. **Images not loading**
   - Check image paths are correct
   - Verify server CORS settings
   - Use HTTPS for image URLs

3. **Statistics not updating**
   - Ensure `UpdateStatistics()` is called after data changes
   - Check data binding is correct

---

## 📚 Additional Resources

- [Xamarin.Forms Documentation](https://docs.microsoft.com/en-us/xamarin/xamarin-forms/)
- [Xamarin.Forms.Maps Guide](https://docs.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/map/)
- [Xamarin.Essentials](https://docs.microsoft.com/en-us/xamarin/essentials/)
- [Material Design Guidelines](https://material.io/design)

---

This implementation guide provides everything needed to integrate the Official Page with a real backend, optimize performance, and customize the design to match your specific requirements.
