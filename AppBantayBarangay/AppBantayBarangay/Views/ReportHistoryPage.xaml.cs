using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using AppBantayBarangay.Models;
using AppBantayBarangay.Services;
using Newtonsoft.Json;

namespace AppBantayBarangay.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReportHistoryPage : ContentPage
    {
        private List<Report> allReports;
        private List<Report> filteredReports;
        private Report selectedReport;
        private string currentFilter = "All";
        private User currentUser;
        private IFirebaseService _firebaseService;

        public ReportHistoryPage(User user)
        {
            InitializeComponent();
            currentUser = user;
            _firebaseService = DependencyService.Get<IFirebaseService>();
            allReports = new List<Report>();
            filteredReports = new List<Report>();

            // Load user's reports from Firebase
            LoadMyReportsFromFirebase();
        }

        private async void LoadMyReportsFromFirebase()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("=== LOADING REPORT HISTORY ===");
                
                if (currentUser == null)
                {
                    System.Diagnostics.Debug.WriteLine("❌ ERROR: currentUser is NULL in ReportHistoryPage!");
                    await DisplayAlert("Error", "User session expired. Please log in again.", "OK");
                    return;
                }
                
                System.Diagnostics.Debug.WriteLine($"👤 Loading reports for user:");
                System.Diagnostics.Debug.WriteLine($"   UserId: {currentUser.UserId}");
                System.Diagnostics.Debug.WriteLine($"   Name: {currentUser.FullName}");
                
                // Get all reports from Firebase
                var reportsData = await _firebaseService.GetDataAsync<Dictionary<string, object>>("reports");

                if (reportsData != null && reportsData.Count > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"📊 Retrieved {reportsData.Count} total reports from Firebase");
                    
                    allReports = new List<Report>();
                    int myReportsCount = 0;
                    int otherReportsCount = 0;

                    foreach (var reportEntry in reportsData)
                    {
                        try
                        {
                            // Convert each report entry to JSON then to Report object
                            var reportJson = JsonConvert.SerializeObject(reportEntry.Value);
                            
                            var report = JsonConvert.DeserializeObject<Report>(reportJson);
                            
                            if (report != null)
                            {
                                System.Diagnostics.Debug.WriteLine($"📝 Report: {report.ReportId}");
                                System.Diagnostics.Debug.WriteLine($"   ReportedBy: '{report.ReportedBy}'");
                                System.Diagnostics.Debug.WriteLine($"   CurrentUser.UserId: '{currentUser.UserId}'");
                                System.Diagnostics.Debug.WriteLine($"   Match: {report.ReportedBy == currentUser.UserId}");
                                
                                // Only include reports submitted by this user
                                if (report.ReportedBy == currentUser.UserId)
                                {
                                    allReports.Add(report);
                                    myReportsCount++;
                                    System.Diagnostics.Debug.WriteLine($"   ✅ MATCHED - Added to my reports (Status: {report.Status})");
                                }
                                else
                                {
                                    otherReportsCount++;
                                    System.Diagnostics.Debug.WriteLine($"   ⚠️ NOT MATCHED - Skipped (belongs to: {report.ReportedBy})");
                                }
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine($"❌ Report {reportEntry.Key} deserialized to null");
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"❌ Error parsing report {reportEntry.Key}: {ex.Message}");
                        }
                    }
                    
                    System.Diagnostics.Debug.WriteLine($"📊 Summary:");
                    System.Diagnostics.Debug.WriteLine($"   My reports: {myReportsCount}");
                    System.Diagnostics.Debug.WriteLine($"   Other users' reports: {otherReportsCount}");
                    System.Diagnostics.Debug.WriteLine($"   Total in database: {reportsData.Count}");

                    filteredReports = new List<Report>(allReports);
                    
                    System.Diagnostics.Debug.WriteLine($"✅ Loaded {allReports.Count} reports for this user");
                    
                    // Update UI
                    UpdateStatistics();
                    DisplayReports();
                    
                    System.Diagnostics.Debug.WriteLine("=== REPORT HISTORY LOAD COMPLETE ===");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("⚠️ No reports found in Firebase database");
                    allReports = new List<Report>();
                    filteredReports = new List<Report>();
                    UpdateStatistics();
                    DisplayReports();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ReportHistory] Load reports error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[ReportHistory] Stack trace: {ex.StackTrace}");
                
                await DisplayAlert("Error", 
                    "Failed to load your reports. Please check your internet connection and try again.", 
                    "OK");
                
                allReports = new List<Report>();
                filteredReports = new List<Report>();
                UpdateStatistics();
                DisplayReports();
            }
        }

        private void UpdateStatistics()
        {
            if (allReports == null || allReports.Count == 0)
            {
                TotalCountLabel.Text = "0";
                PendingCountLabel.Text = "0";
                InProgressCountLabel.Text = "0";
                ResolvedCountLabel.Text = "0";
                return;
            }

            var pendingCount = allReports.Count(r => r.Status == ReportStatus.Pending);
            var inProgressCount = allReports.Count(r => r.Status == ReportStatus.InProgress);
            var resolvedCount = allReports.Count(r => r.Status == ReportStatus.Resolved);
            var totalCount = allReports.Count;

            TotalCountLabel.Text = totalCount.ToString();
            PendingCountLabel.Text = pendingCount.ToString();
            InProgressCountLabel.Text = inProgressCount.ToString();
            ResolvedCountLabel.Text = resolvedCount.ToString();
        }

        private void DisplayReports()
        {
            ReportsContainer.Children.Clear();

            if (filteredReports == null || filteredReports.Count == 0)
            {
                EmptyStateContainer.IsVisible = true;
                return;
            }

            EmptyStateContainer.IsVisible = false;

            foreach (var report in filteredReports.OrderByDescending(r => r.DateReported))
            {
                var reportCard = CreateReportCard(report);
                ReportsContainer.Children.Add(reportCard);
            }
        }

        private Frame CreateReportCard(Report report)
        {
            var statusColor = GetStatusColor(report.Status);
            var statusText = report.Status.ToString();

            var card = new Frame
            {
                BackgroundColor = Color.White,
                CornerRadius = 15,
                Padding = 15,
                HasShadow = true,
                Margin = new Thickness(0, 5)
            };

            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += (s, e) => ShowReportDetails(report);
            card.GestureRecognizers.Add(tapGesture);

            var mainLayout = new StackLayout { Spacing = 10 };

            // Header with Status
            var headerGrid = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = GridLength.Auto }
                }
            };

            var dateLabel = new Label
            {
                Text = DateTime.TryParse(report.DateReported, out var date) ? date.ToString("MMM dd, yyyy") : "Unknown date",
                FontSize = 14,
                FontAttributes = FontAttributes.Bold,
                TextColor = (Color)this.Resources["DarkGray"]
            };
            headerGrid.Children.Add(dateLabel, 0, 0);

            var statusFrame = new Frame
            {
                BackgroundColor = statusColor,
                CornerRadius = 12,
                Padding = new Thickness(10, 5),
                HasShadow = false
            };
            var statusLabel = new Label
            {
                Text = statusText,
                TextColor = Color.White,
                FontSize = 12,
                FontAttributes = FontAttributes.Bold
            };
            statusFrame.Content = statusLabel;
            headerGrid.Children.Add(statusFrame, 1, 0);

            mainLayout.Children.Add(headerGrid);

            // Description
            var descriptionLabel = new Label
            {
                Text = report.Description,
                FontSize = 14,
                TextColor = (Color)this.Resources["DarkGray"],
                MaxLines = 2,
                LineBreakMode = LineBreakMode.TailTruncation
            };
            mainLayout.Children.Add(descriptionLabel);

            // Location
            var locationStack = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Spacing = 5
            };
            locationStack.Children.Add(new Label
            {
                Text = "📍",
                FontSize = 12
            });
            locationStack.Children.Add(new Label
            {
                Text = report.LocationAddress,
                FontSize = 12,
                TextColor = (Color)this.Resources["DarkGray"],
                LineBreakMode = LineBreakMode.TailTruncation
            });
            mainLayout.Children.Add(locationStack);

            card.Content = mainLayout;
            return card;
        }

        private Color GetStatusColor(ReportStatus status)
        {
            switch (status)
            {
                case ReportStatus.Pending:
                    return (Color)this.Resources["AccentOrange"];
                case ReportStatus.InProgress:
                    return (Color)this.Resources["PrimaryBlue"];
                case ReportStatus.Resolved:
                    return (Color)this.Resources["AccentGreen"];
                case ReportStatus.Rejected:
                    return (Color)this.Resources["AccentRed"];
                default:
                    return (Color)this.Resources["DarkGray"];
            }
        }

        private void FilterButton_Clicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            if (button == null) return;

            // Reset all button styles
            AllButton.BackgroundColor = (Color)this.Resources["MediumGray"];
            AllButton.TextColor = (Color)this.Resources["DarkGray"];
            PendingButton.BackgroundColor = (Color)this.Resources["MediumGray"];
            PendingButton.TextColor = (Color)this.Resources["DarkGray"];
            InProgressButton.BackgroundColor = (Color)this.Resources["MediumGray"];
            InProgressButton.TextColor = (Color)this.Resources["DarkGray"];
            ResolvedButton.BackgroundColor = (Color)this.Resources["MediumGray"];
            ResolvedButton.TextColor = (Color)this.Resources["DarkGray"];

            // Set active button style
            button.BackgroundColor = (Color)this.Resources["PrimaryBlue"];
            button.TextColor = Color.White;

            // Filter reports
            currentFilter = button.Text;
            switch (currentFilter)
            {
                case "All":
                    filteredReports = new List<Report>(allReports);
                    break;
                case "Pending":
                    filteredReports = allReports.Where(r => r.Status == ReportStatus.Pending).ToList();
                    break;
                case "In Progress":
                    filteredReports = allReports.Where(r => r.Status == ReportStatus.InProgress).ToList();
                    break;
                case "Resolved":
                    filteredReports = allReports.Where(r => r.Status == ReportStatus.Resolved).ToList();
                    break;
            }

            DisplayReports();
        }

        private void ShowReportDetails(Report report)
        {
            selectedReport = report;

            // Set report image
            if (!string.IsNullOrEmpty(report.ImageUrl))
            {
                try
                {
                    // Check if it's a Base64 string or URL
                    if (report.ImageUrl.StartsWith("data:image") || report.ImageUrl.Length > 1000)
                    {
                        // It's a Base64 string
                        var base64Data = report.ImageUrl;
                        
                        // Remove data:image/png;base64, prefix if present
                        if (base64Data.Contains(","))
                        {
                            base64Data = base64Data.Substring(base64Data.IndexOf(",") + 1);
                        }
                        
                        var imageBytes = Convert.FromBase64String(base64Data);
                        ModalReportImage.Source = ImageSource.FromStream(() => new System.IO.MemoryStream(imageBytes));
                    }
                    else
                    {
                        // It's a URL
                        ModalReportImage.Source = ImageSource.FromUri(new Uri(report.ImageUrl));
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error loading image: {ex.Message}");
                    ModalReportImage.Source = null;
                }
            }
            else
            {
                // Set a placeholder or hide the image
                ModalReportImage.Source = null;
            }

            // Set report details
            ModalDescription.Text = report.Description;
            
            // Parse and format date
            DateTime reportDate;
            if (DateTime.TryParse(report.DateReported, out reportDate))
            {
                ModalDateReported.Text = reportDate.ToString("MMMM dd, yyyy 'at' hh:mm tt");
            }
            
            ModalLocation.Text = report.LocationAddress;
            ModalStatus.Text = report.Status.ToString();
            ModalStatusFrame.BackgroundColor = GetStatusColor(report.Status);

            // Show/hide resolution notes
            if (report.Status == ReportStatus.Resolved && !string.IsNullOrEmpty(report.ResolutionNotes))
            {
                ResolutionNotesContainer.IsVisible = true;
                ModalResolutionNotes.Text = report.ResolutionNotes;
                
                DateTime resolvedDate;
                if (DateTime.TryParse(report.DateResolved, out resolvedDate))
                {
                    ModalResolvedBy.Text = $"Resolved by {report.ResolvedBy} on {resolvedDate.ToString("MMM dd, yyyy")}";
                }
                else
                {
                    ModalResolvedBy.Text = $"Resolved by {report.ResolvedBy}";
                }
            }
            else
            {
                ResolutionNotesContainer.IsVisible = false;
            }

            // Show modal
            ReportDetailsModal.IsVisible = true;
        }

        private void CloseModal_Clicked(object sender, EventArgs e)
        {
            ReportDetailsModal.IsVisible = false;
        }

        private async void BackButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
