# Bantay Barangay - Design Quick Reference

## 🎨 Color Palette

```xml
<Color x:Key="PrimaryBlue">#007AFF</Color>
<Color x:Key="AccentGreen">#34C759</Color>
<Color x:Key="AccentRed">#FF3B30</Color>
<Color x:Key="AccentOrange">#FF9500</Color>
<Color x:Key="AccentYellow">#FFD700</Color>
<Color x:Key="LightGray">#F5F5F5</Color>
<Color x:Key="MediumGray">#E0E0E0</Color>
<Color x:Key="DarkGray">#666666</Color>
```

---

## 📏 Typography

| Element | Size | Weight | Color |
|---------|------|--------|-------|
| Page Title | 22px | Bold | White |
| Page Subtitle | 12px | Regular | White (90%) |
| Section Title | 16-20px | Bold | Primary Blue |
| Field Label | 13-14px | Bold | Dark Gray |
| Input Text | 15-16px | Regular | Dark Gray |
| Button Text | 14-18px | Bold | White/Blue |
| Info Text | 12px | Regular | Dark Gray |

---

## 📐 Spacing

```
5px   - Extra Small (related items)
10px  - Small (form elements)
15px  - Medium (sections)
20px  - Large (major sections)
30px  - Extra Large (page padding)
```

---

## 🔲 Standard Components

### Header (All Pages)
```xml
<Grid BackgroundColor="{StaticResource PrimaryBlue}" 
      Padding="20,10" HeightRequest="70">
    <!-- BB Logo (50px circle) -->
    <!-- Title + Subtitle -->
    <!-- Action Button -->
</Grid>
```

### Card
```xml
<Frame BackgroundColor="White"
       CornerRadius="15"
       Padding="20"
       HasShadow="True">
    <!-- Content -->
</Frame>
```

### Input Field
```xml
<Label Text="Label" FontSize="13" FontAttributes="Bold"/>
<Frame BorderColor="{StaticResource PrimaryBlue}"
       CornerRadius="10" Padding="0">
    <Entry Placeholder="..." Margin="10,0"/>
</Frame>
```

### Primary Button
```xml
<Button Text="🔐 ACTION"
        BackgroundColor="{StaticResource PrimaryBlue}"
        TextColor="White"
        CornerRadius="10"
        Padding="15"
        FontSize="18"
        FontAttributes="Bold"/>
```

### Info Section
```xml
<Frame BackgroundColor="#E3F2FD"
       BorderColor="{StaticResource PrimaryBlue}"
       CornerRadius="15" Padding="15">
    <StackLayout>
        <Label Text="ℹ️ Title" FontSize="14" FontAttributes="Bold"/>
        <Label Text="• Point 1" FontSize="12"/>
    </StackLayout>
</Frame>
```

---

## 🎯 Icons

```
🔐 Login          📝 Register       👋 Welcome
⎋  Logout         ←  Back           ✓  Submit
🔄 Update         ⏳ Loading        📍 Location
📁 Upload         📷 Camera         ℹ️ Info
⏳ Pending        🔄 In Progress    ✅ Resolved
```

---

## 📱 Pages Overview

### LoginPage
- Header with BB logo
- Welcome card (👋)
- User type picker
- Phone + Password inputs
- Login button (🔐)
- Register button (📝)
- Info section

### RegistrationPage
- Header with back button (←)
- Registration card (📝)
- 3 sections: Personal, Contact, Security
- 8 input fields
- Register button (✓)
- Login button (🔐)
- Info section

### OfficialPage
- Header with logout (⎋)
- 4 statistics cards
- 3 filter buttons
- Reports list
- Report details modal
- Action buttons

### ResidentPage
- Header with logout (⎋)
- Photo upload (📁 📷)
- Description editor
- Location capture (📍)
- Coordinate frame
- Submit button (✓)
- Info section

---

## ✅ Validation Patterns

### Required Field
```csharp
if (string.IsNullOrWhiteSpace(Entry.Text))
{
    await DisplayAlert("Required", "Please enter...", "OK");
    return false;
}
```

### Email
```csharp
var pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
if (!Regex.IsMatch(email, pattern))
{
    await DisplayAlert("Invalid", "Please enter valid email", "OK");
    return false;
}
```

### Phone (PH)
```csharp
var clean = phone.Replace(" ", "").Replace("-", "");
var pattern = @"^(09|\+639)\d{9}$";
if (!Regex.IsMatch(clean, pattern))
{
    await DisplayAlert("Invalid", "Please enter valid PH number", "OK");
    return false;
}
```

### Password
```csharp
if (password.Length < 6)
{
    await DisplayAlert("Weak", "Min 6 characters", "OK");
    return false;
}
```

---

## 🔄 Loading States

```csharp
// Show loading
button.Text = "⏳ Processing...";
button.IsEnabled = false;

// Do work
await Task.Delay(1000);

// Reset
button.Text = "✓ SUBMIT";
button.IsEnabled = true;
```

---

## 🎨 Status Colors

| Status | Color | Hex |
|--------|-------|-----|
| Pending | Orange | #FF9500 |
| In Progress | Blue | #007AFF |
| Resolved | Green | #34C759 |
| Rejected | Red | #FF3B30 |

---

## 📊 Common Patterns

### Section Header
```xml
<Label Text="Section Title"
       FontSize="16"
       FontAttributes="Bold"
       TextColor="{StaticResource PrimaryBlue}"
       Margin="0,10,0,5"/>
```

### Divider
```xml
<Grid Margin="0,15,0,10">
    <Grid.ColumnDefinitions>
        <ColumnDefinition Width="*" />
        <ColumnDefinition Width="Auto" />
        <ColumnDefinition Width="*" />
    </Grid.ColumnDefinitions>
    <BoxView Grid.Column="0" HeightRequest="1" 
             BackgroundColor="{StaticResource MediumGray}"/>
    <Label Grid.Column="1" Text="OR" Margin="10,0"/>
    <BoxView Grid.Column="2" HeightRequest="1" 
             BackgroundColor="{StaticResource MediumGray}"/>
</Grid>
```

### Empty State
```xml
<StackLayout IsVisible="False" Padding="20">
    <Label Text="📭" FontSize="60" HorizontalOptions="Center"/>
    <Label Text="No items found" FontSize="18" 
           FontAttributes="Bold" HorizontalOptions="Center"/>
    <Label Text="Description..." FontSize="14" 
           HorizontalOptions="Center"/>
</StackLayout>
```

---

## 🚀 Quick Start Checklist

### New Page Setup
- [ ] Add color ResourceDictionary
- [ ] Create header (70px, blue)
- [ ] Add BB logo (50px circle)
- [ ] Set background (#F5F5F5)
- [ ] Use ScrollView for content
- [ ] Add padding (15-20px)
- [ ] Include info section
- [ ] Implement validation
- [ ] Add loading states
- [ ] Test on device

---

## 📝 File Structure

```
AppBantayBarangay/
├── Models/
│   ├── Report.cs
│   ├── User.cs
│   └── UserType.cs
├── Views/
│   ├── LoginPage.xaml
│   ├── LoginPage.xaml.cs
│   ├── RegistrationPage.xaml
│   ├── RegistrationPage.xaml.cs
│   ├── OfficialPage.xaml
│   ├── OfficialPage.xaml.cs
│   ├── ResidentPage.xaml
│   └── ResidentPage.xaml.cs
└── Documentation/
    ├── COMPLETE_DESIGN_SYSTEM.md
    ├── DESIGN_QUICK_REFERENCE.md
    ├── LOGIN_REGISTRATION_UPDATES.md
    ├── RESIDENT_PAGE_UPDATES.md
    └── OFFICIAL_PAGE_DESIGN.md
```

---

## 🎯 Key Principles

1. **Consistency** - Same design across all pages
2. **Clarity** - Clear visual hierarchy
3. **Feedback** - Loading states and messages
4. **Accessibility** - High contrast, large targets
5. **Professionalism** - Polished, production-ready

---

## 📞 Common Issues

### Colors not working?
```csharp
// Use this.Resources instead of Application.Current.Resources
var color = (Color)this.Resources["PrimaryBlue"];
```

### ImageButton error?
```xml
<!-- Don't use Text property on ImageButton -->
<!-- Use Button with Text instead -->
<Button Text="✕" BackgroundColor="Transparent"/>
```

### Map not showing?
```csharp
// Initialize in MainActivity (Android)
Xamarin.FormsMaps.Init(this, savedInstanceState);

// Initialize in AppDelegate (iOS)
Xamarin.FormsMaps.Init();
```

---

**Quick Reference Complete! 🎨**
