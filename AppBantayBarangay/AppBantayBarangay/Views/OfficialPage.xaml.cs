using AppBantayBarangay.Models;
using AppBantayBarangay.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace AppBantayBarangay.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OfficialPage : ContentPage
    {
        private List<Report> allReports;
        private List<Report> filteredReports;
        private Report selectedReport;
        private string currentFilter = "All";
        private User currentUser;
        private IFirebaseService _firebaseService;

        public OfficialPage()
        {
            InitializeComponent();
            allReports = new List<Report>();
            filteredReports = new List<Report>();
            UpdateStatistics();
            DisplayReports();
        }

        public OfficialPage(User user)
        {
            InitializeComponent();
            currentUser = user;
            _firebaseService = DependencyService.Get<IFirebaseService>();
            allReports = new List<Report>();
            filteredReports = new List<Report>();

            // Show loading screen and load reports
            ShowLoading(true);
            LoadReportsFromFirebase();
        }
        
        private void ShowLoading(bool show)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                LoadingOverlay.IsVisible = show;
            });
        }
        
        private async void OnRefreshing(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("🔄 Pull-to-refresh triggered");
            
            try
            {
                // Reload reports from Firebase
                await Task.Run(() => LoadReportsFromFirebase());
            }
            finally
            {
                // Stop the refreshing animation
                Device.BeginInvokeOnMainThread(() =>
                {
                    RefreshView.IsRefreshing = false;
                });
            }
        }

        private void UpdateStatistics()
        {
            if (allReports == null || allReports.Count == 0)
            {
                PendingCountLabel.Text = "0";
                InProgressCountLabel.Text = "0";
                ResolvedCountLabel.Text = "0";
                TotalCountLabel.Text = "0";
                return;
            }

            var pendingCount = allReports.Count(r => r.Status == ReportStatus.Pending);
            var inProgressCount = allReports.Count(r => r.Status == ReportStatus.InProgress);
            var resolvedCount = allReports.Count(r => r.Status == ReportStatus.Resolved);
            var totalCount = allReports.Count;

            PendingCountLabel.Text = pendingCount.ToString();
            InProgressCountLabel.Text = inProgressCount.ToString();
            ResolvedCountLabel.Text = resolvedCount.ToString();
            TotalCountLabel.Text = totalCount.ToString();
        }

        private void DisplayReports()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"📊 DisplayReports: Displaying {filteredReports?.Count ?? 0} reports");
                
                ReportsContainer.Children.Clear();

                if (filteredReports == null || filteredReports.Count == 0)
                {
                    EmptyStateContainer.IsVisible = true;
                    System.Diagnostics.Debug.WriteLine("📦 No reports to display - showing empty state");
                    return;
                }

                EmptyStateContainer.IsVisible = false;
                
                int successCount = 0;
                int errorCount = 0;

                foreach (var report in filteredReports.OrderByDescending(r => r.DateReported))
                {
                    try
                    {
                        if (report == null)
                        {
                            System.Diagnostics.Debug.WriteLine("⚠️ Skipping null report in list");
                            errorCount++;
                            continue;
                        }
                        
                        var reportCard = CreateReportCard(report);
                        if (reportCard != null)
                        {
                            ReportsContainer.Children.Add(reportCard);
                            successCount++;
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"⚠️ CreateReportCard returned null for report {report.ReportId}");
                            errorCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"❌ Error creating card for report {report?.ReportId ?? "unknown"}: {ex.Message}");
                        errorCount++;
                    }
                }
                
                System.Diagnostics.Debug.WriteLine($"✅ DisplayReports complete: {successCount} successful, {errorCount} errors");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ DisplayReports fatal error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                
                // Show error to user
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await DisplayAlert("Error", 
                        "Failed to display reports. Please try refreshing the page.", 
                        "OK");
                });
            }
        }

        private Frame CreateReportCard(Report report)
        {
            try
            {
                // Validate report object
                if (report == null)
                {
                    System.Diagnostics.Debug.WriteLine("❌ CreateReportCard: report is null");
                    return CreateErrorCard("Invalid report data");
                }
                
                var statusColor = GetStatusColor(report.Status);
                var statusText = report.Status.ToString();

                Console.WriteLine(report.ImageUrl);

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

                // Header with ID and Status
                var headerGrid = new Grid
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                        new ColumnDefinition { Width = GridLength.Auto }
                    }
                };

                // Safely get report ID
                string reportIdDisplay = "Unknown";
                if (!string.IsNullOrEmpty(report.ReportId))
                {
                    reportIdDisplay = report.ReportId.Substring(0, Math.Min(8, report.ReportId.Length));
                }

                var idLabel = new Label
                {
                    Text = $"Report #{reportIdDisplay}",
                    FontSize = 16,
                    FontAttributes = FontAttributes.Bold,
                    TextColor = (Color)this.Resources["PrimaryBlue"]
                };
            headerGrid.Children.Add(idLabel, 0, 0);

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
                Text = report.Description ?? "No description provided",
                FontSize = 14,
                TextColor = (Color)this.Resources["DarkGray"],
                MaxLines = 2,
                LineBreakMode = LineBreakMode.TailTruncation
            };
            mainLayout.Children.Add(descriptionLabel);

            // Footer with location and date
            var footerGrid = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = GridLength.Auto }
                },
                Margin = new Thickness(0, 5, 0, 0)
            };

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
                Text = report.LocationAddress ?? "Location not specified",
                FontSize = 12,
                TextColor = (Color)this.Resources["DarkGray"],
                LineBreakMode = LineBreakMode.TailTruncation
            });
            footerGrid.Children.Add(locationStack, 0, 0);

            // Parse date string
            DateTime reportDate;
            if (DateTime.TryParse(report.DateReported, out reportDate))
            {
                var dateLabel = new Label
                {
                    Text = reportDate.ToString("MMM dd, yyyy"),
                    FontSize = 12,
                    TextColor = (Color)this.Resources["DarkGray"],
                    HorizontalOptions = LayoutOptions.End
                };
                footerGrid.Children.Add(dateLabel, 1, 0);
            }

            mainLayout.Children.Add(footerGrid);

            // Reported by
            var reportedByLabel = new Label
            {
                Text = $"By: {report.ReporterName ?? "Unknown"}",
                FontSize = 12,
                TextColor = (Color)this.Resources["DarkGray"],
                FontAttributes = FontAttributes.Italic
            };
            mainLayout.Children.Add(reportedByLabel);

            card.Content = mainLayout;
            return card;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ CreateReportCard error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Report ID: {report?.ReportId ?? "null"}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                return CreateErrorCard($"Error displaying report: {ex.Message}");
            }
        }
        
        private Frame CreateErrorCard(string errorMessage)
        {
            var card = new Frame
            {
                BackgroundColor = Color.FromHex("#FFF3CD"),
                CornerRadius = 15,
                Padding = 15,
                HasShadow = true,
                Margin = new Thickness(0, 5)
            };
            
            var layout = new StackLayout { Spacing = 5 };
            layout.Children.Add(new Label
            {
                Text = "⚠️ Error",
                FontSize = 16,
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.FromHex("#856404")
            });
            layout.Children.Add(new Label
            {
                Text = errorMessage,
                FontSize = 14,
                TextColor = Color.FromHex("#856404")
            });
            
            card.Content = layout;
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
            }

            DisplayReports();
        }

        private void ShowReportDetails(Report report)
        {
            selectedReport = report;

            // Set report details
            ModalDescription.Text = report.Description;
            ModalReportedBy.Text = report.ReporterName;
            
            // Parse and format date
            DateTime reportDate;
            if (DateTime.TryParse(report.DateReported, out reportDate))
            {
                ModalDateReported.Text = reportDate.ToString("MMMM dd, yyyy 'at' hh:mm tt");
            }
            
            ModalLocation.Text = report.LocationAddress;
            ModalStatus.Text = report.Status.ToString();
            ModalStatusFrame.BackgroundColor = GetStatusColor(report.Status);
            byte[] imageBytes = Convert.FromBase64String(report.ImageUrl);
            ModalReportImage.Source = ImageSource.FromStream(() => new MemoryStream(imageBytes));


            ModalLat.Text = report.Latitude.HasValue ? report.Latitude.Value.ToString() : "N/A";
            ModalLong.Text = report.Longitude.HasValue ? report.Longitude.Value.ToString() : "N/A";
            // Set map location using Latitude and Longitude
            //if (report.Latitude.HasValue && report.Longitude.HasValue && 
            //    report.Latitude.Value != 0 && report.Longitude.Value != 0)
            //{
            //    var position = new Position(report.Latitude.Value, report.Longitude.Value);
            //    var pin = new Pin
            //    {
            //        Label = "Report Location",
            //        Address = report.LocationAddress,
            //        Type = PinType.Place,
            //        Position = position
            //    };
            //    ModalMap.Pins.Clear();
            //    ModalMap.Pins.Add(pin);
            //    ModalMap.MoveToRegion(MapSpan.FromCenterAndRadius(pin.Position, Distance.FromMeters(500)));
            //}

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

            // Show/hide action buttons based on status
            if (report.Status == ReportStatus.Resolved || report.Status == ReportStatus.Rejected)
            {
                ActionButtonsContainer.IsVisible = false;
            }
            else
            {
                ActionButtonsContainer.IsVisible = true;
                ResolutionInputContainer.IsVisible = false;
                
                // Configure buttons based on current status
                if (report.Status == ReportStatus.Pending)
                {
                    MarkInProgressButton.IsVisible = true;
                    MarkResolvedButton.Text = "Mark as Resolved";
                }
                else if (report.Status == ReportStatus.InProgress)
                {
                    MarkInProgressButton.IsVisible = false;
                    MarkResolvedButton.Text = "Mark as Resolved";
                }
            }

            // Show modal
            ReportDetailsModal.IsVisible = true;
        }

        private void CloseModal_Clicked(object sender, EventArgs e)
        {
            ReportDetailsModal.IsVisible = false;
            ResolutionInputContainer.IsVisible = false;
            ResolutionNotesEditor.Text = string.Empty;
        }

        private async void MarkInProgress_Clicked(object sender, EventArgs e)
        {
            if (selectedReport == null) return;

            var confirm = await DisplayAlert("Confirm", 
                "Mark this report as In Progress?", 
                "Yes", "No");

            if (confirm)
            {
                selectedReport.Status = ReportStatus.InProgress;
                
                // Update in Firebase
                await _firebaseService.SaveDataAsync($"reports/{selectedReport.ReportId}", selectedReport);
                
                UpdateStatistics();
                DisplayReports();
                ShowReportDetails(selectedReport); // Refresh modal
                await DisplayAlert("Success", "Report marked as In Progress", "OK");
            }
        }

        private async void MarkResolved_Clicked(object sender, EventArgs e)
        {
            if (selectedReport == null) return;

            // Show resolution notes input if not already visible
            if (!ResolutionInputContainer.IsVisible)
            {
                ResolutionInputContainer.IsVisible = true;
                MarkResolvedButton.Text = "Confirm Resolution";
                return;
            }

            // Validate resolution notes
            if (string.IsNullOrWhiteSpace(ResolutionNotesEditor.Text))
            {
                await DisplayAlert("Required", "Please enter resolution notes before marking as resolved.", "OK");
                return;
            }

            var confirm = await DisplayAlert("Confirm", 
                "Mark this report as Resolved?", 
                "Yes", "No");

            if (confirm)
            {
                selectedReport.Status = ReportStatus.Resolved;
                selectedReport.ResolutionNotes = ResolutionNotesEditor.Text;
                selectedReport.ResolvedBy = currentUser != null ? 
                    $"{currentUser.FirstName} {currentUser.LastName}" : "Barangay Official";
                selectedReport.DateResolved = DateTime.Now.ToString("o");

                // Update in Firebase
                await _firebaseService.SaveDataAsync($"reports/{selectedReport.ReportId}", selectedReport);

                UpdateStatistics();
                DisplayReports();
                CloseModal_Clicked(null, null);
                await DisplayAlert("Success", "Report marked as Resolved", "OK");
            }
        }

        private async void RejectReport_Clicked(object sender, EventArgs e)
        {
            if (selectedReport == null) return;

            var confirm = await DisplayAlert("Confirm", 
                "Are you sure you want to reject this report? This action cannot be undone.", 
                "Yes", "No");

            if (confirm)
            {
                selectedReport.Status = ReportStatus.Rejected;
                
                // Update in Firebase
                await _firebaseService.SaveDataAsync($"reports/{selectedReport.ReportId}", selectedReport);
                
                UpdateStatistics();
                DisplayReports();
                CloseModal_Clicked(null, null);
                await DisplayAlert("Success", "Report has been rejected", "OK");
            }
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

        private async void LoadReportsFromFirebase()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("=== OFFICIAL PAGE: LOADING ALL REPORTS ===");
                
                // Show loading overlay (only if not refreshing)
                if (!RefreshView.IsRefreshing)
                {
                    ShowLoading(true);
                }
                
                if (currentUser == null)
                {
                    System.Diagnostics.Debug.WriteLine("⚠️ WARNING: currentUser is NULL in OfficialPage");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"👤 Official User: {currentUser.UserId} ({currentUser.FullName})");
                }
                
                // Get all reports from Firebase - returns Dictionary<string, object>
                var reportsData = await _firebaseService.GetDataAsync<Dictionary<string, object>>("reports");

                if (reportsData != null && reportsData.Count > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"📊 Retrieved {reportsData.Count} reports from Firebase");
                    

                    allReports = new List<Report>();
                    var reportsByUser = new Dictionary<string, int>();
                    foreach (var reportEntry in reportsData)
                    {
                        try
                        {
                            // Convert each report entry to JSON then to Report object
                            var reportJson = JsonConvert.SerializeObject(reportEntry.Value);
                            
                            System.Diagnostics.Debug.WriteLine($"📦 Raw JSON for {reportEntry.Key}:");
                            System.Diagnostics.Debug.WriteLine($"   {reportJson.Substring(0, Math.Min(500, reportJson.Length))}...");
                            
                            var report = JsonConvert.DeserializeObject<Report>(reportJson);
                            
                            System.Diagnostics.Debug.WriteLine($"🔍 After deserialization:");
                            
                            if (report != null)
                            {
                                // Validate critical fields
                                bool hasIssues = false;
                                if (string.IsNullOrEmpty(report.ReportId))
                                {
                                    System.Diagnostics.Debug.WriteLine($"⚠️ Report has null/empty ReportId");
                                    hasIssues = true;
                                }
                                if (string.IsNullOrEmpty(report.ReportedBy))
                                {
                                    System.Diagnostics.Debug.WriteLine($"⚠️ Report {report.ReportId ?? "unknown"} has null/empty ReportedBy");
                                    hasIssues = true;
                                }
                                if (string.IsNullOrEmpty(report.Description))
                                {
                                    System.Diagnostics.Debug.WriteLine($"⚠️ Report {report.ReportId ?? "unknown"} has null/empty Description");
                                    hasIssues = true;
                                }
                                
                                allReports.Add(report);
                                
                                // Track reports by user
                                string reportedBy = report.ReportedBy ?? "unknown";
                                if (!reportsByUser.ContainsKey(reportedBy))
                                    reportsByUser[reportedBy] = 0;
                                reportsByUser[reportedBy]++;
                                
                                System.Diagnostics.Debug.WriteLine($"📝 Report: {report.ReportId ?? "null"}");
                                System.Diagnostics.Debug.WriteLine($"   ReportedBy: '{report.ReportedBy ?? "null"}'");
                                System.Diagnostics.Debug.WriteLine($"   ReporterName: '{report.ReporterName ?? "null"}'");
                                System.Diagnostics.Debug.WriteLine($"   Description: '{(string.IsNullOrEmpty(report.Description) ? "null/empty" : report.Description.Substring(0, Math.Min(50, report.Description.Length)))}'");
                                System.Diagnostics.Debug.WriteLine($"   LocationAddress: '{(string.IsNullOrEmpty(report.LocationAddress) ? "null/empty" : report.LocationAddress)}'");
                                System.Diagnostics.Debug.WriteLine($"   Latitude: {report.Latitude}");
                                System.Diagnostics.Debug.WriteLine($"   Longitude: {report.Longitude}");
                                System.Diagnostics.Debug.WriteLine($"   Status: {report.Status}");
                                System.Diagnostics.Debug.WriteLine($"   {(hasIssues ? "⚠️" : "✅")} Added to official's view");
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine($"❌ Report {reportEntry.Key} deserialized to null");
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"❌ Error parsing report {reportEntry.Key}: {ex.Message}");
                            System.Diagnostics.Debug.WriteLine($"   Exception type: {ex.GetType().Name}");
                        }
                    }
                    
                    System.Diagnostics.Debug.WriteLine($"📊 Reports by User:");
                    foreach (var kvp in reportsByUser)
                    {
                        System.Diagnostics.Debug.WriteLine($"   User {kvp.Key}: {kvp.Value} reports");
                    }

                    filteredReports = new List<Report>(allReports);
                    
                    System.Diagnostics.Debug.WriteLine($"✅ Total reports loaded for official view: {allReports.Count}");
                    
                    // Update UI
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        UpdateStatistics();
                        DisplayReports();
                    });
                    
                    System.Diagnostics.Debug.WriteLine("=== OFFICIAL PAGE: LOAD COMPLETE ===");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("⚠️ No reports found in Firebase database");
                    allReports = new List<Report>();
                    filteredReports = new List<Report>();
                    
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        UpdateStatistics();
                        DisplayReports();
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ OFFICIAL PAGE: Load reports error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                
                // Show error to user
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await DisplayAlert("Error", 
                        "Failed to load reports from database. Please check your internet connection and try again.", 
                        "OK");
                });
                
                // Initialize empty lists
                allReports = new List<Report>();
                filteredReports = new List<Report>();
                
                Device.BeginInvokeOnMainThread(() =>
                {
                    UpdateStatistics();
                    DisplayReports();
                });
            }
            finally
            {
                // Hide loading overlay
                ShowLoading(false);
            }
        }
    }
}
