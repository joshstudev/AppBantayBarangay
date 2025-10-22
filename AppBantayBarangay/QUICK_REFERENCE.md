# Official Page - Quick Reference Card

## 🚀 Quick Start

### Files Modified/Created
```
✓ AppBantayBarangay/Models/Report.cs              [NEW]
✓ AppBantayBarangay/Views/OfficialPage.xaml       [UPDATED]
✓ AppBantayBarangay/Views/OfficialPage.xaml.cs    [UPDATED]
```

### To Run
1. Open solution in Visual Studio
2. Build the project
3. Run on Android/iOS emulator or device
4. Login as an official
5. Dashboard loads with 5 sample reports

---

## 📊 Key Components

### Statistics Cards
```csharp
PendingCountLabel.Text      // Orange - Pending reports count
InProgressCountLabel.Text   // Blue - In Progress count
ResolvedCountLabel.Text     // Green - Resolved count
TotalCountLabel.Text        // Blue - Total reports count
```

### Filter Buttons
```csharp
AllButton          // Shows all reports
PendingButton      // Shows only pending
InProgressButton   // Shows only in progress
```

### Report Status
```csharp
ReportStatus.Pending      // Orange
ReportStatus.InProgress   // Blue
ReportStatus.Resolved     // Green
ReportStatus.Rejected     // Red
```

---

## 🎨 Color Reference

| Element | Color Code | Usage |
|---------|-----------|--------|
| Primary Blue | #007AFF | Headers, primary actions |
| Accent Orange | #FF9500 | Pending status |
| Accent Green | #34C759 | Resolved status |
| Accent Red | #FF3B30 | Reject action |
| Light Gray | #F5F5F5 | Background |
| Dark Gray | #666666 | Text |

---

## 🔧 Key Methods

### Display & Filter
```csharp
LoadSampleReports()        // Loads 5 sample reports
UpdateStatistics()         // Refreshes count labels
DisplayReports()           // Renders report cards
FilterButton_Clicked()     // Handles filter selection
```

### Report Actions
```csharp
ShowReportDetails(report)  // Opens modal with details
MarkInProgress_Clicked()   // Updates to InProgress
MarkResolved_Clicked()     // Updates to Resolved (requires notes)
RejectReport_Clicked()     // Updates to Rejected
CloseModal_Clicked()       // Closes detail modal
```

---

## 📱 User Actions

### View Reports
1. Tap filter button (All/Pending/In Progress)
2. Scroll through report cards
3. Tap any card to view details

### Resolve Report
1. Tap report card
2. Review details
3. Click "Mark as Resolved"
4. Enter resolution notes
5. Click "Confirm Resolution"
6. Confirm in dialog

### Mark In Progress
1. Tap pending report card
2. Click "Mark as In Progress"
3. Confirm in dialog

### Reject Report
1. Tap report card
2. Click "Reject Report"
3. Confirm in dialog

---

## 🗂️ Data Structure

### Report Model
```csharp
public class Report
{
    public int Id { get; set; }
    public string Description { get; set; }
    public string ImagePath { get; set; }
    public Position Location { get; set; }
    public string LocationAddress { get; set; }
    public DateTime DateReported { get; set; }
    public ReportStatus Status { get; set; }
    public string ReportedBy { get; set; }
    public string ResolvedBy { get; set; }
    public DateTime? DateResolved { get; set; }
    public string ResolutionNotes { get; set; }
}
```

---

## 🔌 Backend Integration Points

### Replace Sample Data
```csharp
// In InitializePage()
// OLD:
LoadSampleReports();

// NEW:
await LoadReportsFromApi();
```

### API Methods Needed
```csharp
Task<List<Report>> GetAllReportsAsync()
Task<List<Report>> GetReportsByStatusAsync(ReportStatus status)
Task<bool> UpdateReportStatusAsync(int id, ReportStatus status)
Task<bool> ResolveReportAsync(int id, string notes, int userId)
Task<bool> RejectReportAsync(int id, int userId)
```

---

## 🎯 Common Customizations

### Change Header Text
```xml
<!-- In OfficialPage.xaml -->
<Label Text="Official Dashboard" />  <!-- Change this -->
```

### Modify Statistics Layout
```xml
<!-- In OfficialPage.xaml, find Grid with statistics cards -->
<Grid ColumnSpacing="10" RowSpacing="10">
    <!-- Modify grid structure here -->
</Grid>
```

### Add New Filter
```xml
<!-- Add new button in filter section -->
<Button x:Name="ResolvedButton"
        Text="Resolved"
        Clicked="FilterButton_Clicked"/>
```

```csharp
// Add case in FilterButton_Clicked
case "Resolved":
    filteredReports = allReports.Where(r => r.Status == ReportStatus.Resolved).ToList();
    break;
```

### Change Card Design
```csharp
// Modify CreateReportCard() method in OfficialPage.xaml.cs
private Frame CreateReportCard(Report report)
{
    // Customize card layout here
}
```

---

## 🐛 Troubleshooting

### Maps Not Showing
```csharp
// Ensure in MainActivity.cs (Android):
Xamarin.FormsMaps.Init(this, savedInstanceState);

// Ensure in AppDelegate.cs (iOS):
Xamarin.FormsMaps.Init();
```

### Statistics Not Updating
```csharp
// Always call after data changes:
UpdateStatistics();
DisplayReports();
```

### Modal Not Closing
```csharp
// Check:
ReportDetailsModal.IsVisible = false;
```

### Filter Not Working
```csharp
// Ensure filteredReports is updated:
filteredReports = allReports.Where(...).ToList();
DisplayReports();  // Must call this!
```

---

## 📚 Documentation Files

| File | Purpose |
|------|---------|
| OFFICIAL_PAGE_SUMMARY.md | Project overview |
| OFFICIAL_PAGE_DESIGN.md | Design specifications |
| OFFICIAL_PAGE_VISUAL_GUIDE.md | Visual diagrams |
| IMPLEMENTATION_GUIDE.md | Backend integration |
| QUICK_REFERENCE.md | This file |

---

## ✅ Testing Checklist

- [ ] Dashboard loads with statistics
- [ ] All 5 sample reports display
- [ ] Filter buttons work (All, Pending, In Progress)
- [ ] Tapping report card opens modal
- [ ] Map displays in modal
- [ ] "Mark as In Progress" updates status
- [ ] "Mark as Resolved" requires notes
- [ ] Resolution notes are saved
- [ ] "Reject Report" works with confirmation
- [ ] Statistics update after status changes
- [ ] Modal closes properly
- [ ] Logout button works

---

## 🎓 Learning Path

1. **Start Here**: OFFICIAL_PAGE_SUMMARY.md
2. **Understand Design**: OFFICIAL_PAGE_DESIGN.md
3. **See Visuals**: OFFICIAL_PAGE_VISUAL_GUIDE.md
4. **Integrate Backend**: IMPLEMENTATION_GUIDE.md
5. **Quick Lookup**: QUICK_REFERENCE.md (this file)

---

## 💡 Pro Tips

1. **Use Filters**: Quickly find reports by status
2. **Resolution Notes**: Be detailed - they're permanent
3. **Confirmation Dialogs**: Prevent accidental actions
4. **Statistics**: Monitor at a glance
5. **Map View**: Verify location before resolving

---

## 🔑 Key Features Summary

✅ Real-time statistics dashboard
✅ Color-coded status system
✅ One-tap filtering
✅ Detailed report modal
✅ Interactive maps
✅ Resolution tracking
✅ Confirmation dialogs
✅ Professional UI/UX
✅ Sample data included
✅ Backend-ready architecture

---

**Need Help?** Check the full documentation files for detailed explanations!
