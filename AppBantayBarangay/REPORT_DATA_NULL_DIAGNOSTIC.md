# Report Data Showing as Null/Unknown - Diagnostic Guide

## 🐛 Problem

Reports are displaying but showing default values:
- "Unknown" for reporter name
- "Location not specified" for address
- "No description provided" for description

Even though the resident provided all this information when submitting.

## 🔍 Diagnostic Steps

### Step 1: Check What's Being Saved

When you submit a report as a Resident, check the **Debug Output** for these lines:

```
📋 Values to be saved:
   description variable: '[YOUR DESCRIPTION]'
   currentAddress variable: '[YOUR ADDRESS]'
   currentLatitude: [NUMBER]
   currentLongitude: [NUMBER]
   imageData length: [NUMBER]

📝 Report Object Created:
   ReportId: [GUID]
   Description: '[YOUR DESCRIPTION]' (length: XX)
   LocationAddress: '[YOUR ADDRESS]'
   Coordinates: (XX.XXXXXX, XX.XXXXXX)
   ReportedBy (UserId): [USER ID]
   ReporterName: [YOUR NAME]
```

**What to Look For:**
- ✅ **GOOD**: Description shows your actual text
- ✅ **GOOD**: LocationAddress shows actual address or coordinates
- ❌ **BAD**: Description shows 'NULL' or empty
- ❌ **BAD**: LocationAddress shows 'NULL'

### Step 2: Check What's Being Retrieved

When you view reports as an Official, check the **Debug Output** for:

```
📦 Raw JSON for [report-id]:
   {"ReportId":"...","Description":"...","LocationAddress":"..."}

🔍 After deserialization:
📝 Report: [report-id]
   ReportedBy: '[user-id]'
   ReporterName: '[name]'
   Description: '[first 50 chars]'
   LocationAddress: '[address]'
```

**What to Look For:**
- ✅ **GOOD**: JSON shows actual values
- ✅ **GOOD**: After deserialization, fields have values
- ❌ **BAD**: JSON shows null or empty strings
- ❌ **BAD**: After deserialization, fields are null/empty

## 🎯 Common Causes & Solutions

### Cause 1: Data Not Being Captured

**Symptom**: Debug output shows NULL values when saving

**Check**:
```
📋 Values to be saved:
   description variable: 'NULL'  ← PROBLEM
   currentAddress variable: 'NULL'  ← PROBLEM
```

**Solution**:
1. Make sure you're typing in the Description field
2. Make sure you're clicking "Get Current Location" button
3. Check that the location frame becomes visible after clicking

**Code to Check**: ResidentPage.xaml.cs
- Line ~189: `var description = DescriptionEditor.Text;`
- Line ~136-148: Location capture code

### Cause 2: Firebase Not Saving Properly

**Symptom**: Debug shows correct values when saving, but NULL when retrieving

**Check**:
```
✅ SUCCESS: Report saved to Firebase at path: reports/[id]
   (values look correct when saving)

But later when retrieving:
📦 Raw JSON: {"Description":null,"LocationAddress":null}  ← PROBLEM
```

**Solution**:
1. Check Firebase Console to see actual data
2. Verify Firebase service is working
3. Check for serialization issues

**How to Check Firebase Console**:
1. Go to Firebase Console
2. Navigate to Realtime Database
3. Look at `reports/[report-id]`
4. Verify fields have values

### Cause 3: Deserialization Issue

**Symptom**: JSON shows values, but after deserialization they're null

**Check**:
```
📦 Raw JSON: {"Description":"My description","LocationAddress":"123 Main St"}
🔍 After deserialization:
   Description: 'null/empty'  ← PROBLEM
   LocationAddress: 'null/empty'  ← PROBLEM
```

**Solution**:
- Check Report model property names match JSON
- Check for JsonProperty attributes
- Verify no custom converters interfering

**Code to Check**: AppBantayBarangay/Models/Report.cs

### Cause 4: Old Reports in Database

**Symptom**: New reports work, but old reports show null

**Check**: Look at the report IDs and dates

**Solution**:
1. Delete old test reports from Firebase Console
2. Submit fresh reports with all data
3. Verify new reports display correctly

## 🧪 Testing Procedure

### Test 1: Fresh Report Submission

1. **Log in as Resident**
2. **Fill out form**:
   - Description: "Test report with all data"
   - Upload a photo
   - Click "Get Current Location"
   - Wait for location to appear
3. **Submit report**
4. **Check Debug Output** for:
   ```
   📋 Values to be saved:
      description variable: 'Test report with all data'  ← Should NOT be NULL
      currentAddress variable: 'Lat: XX, Long: XX'  ← Should NOT be NULL
   ```

### Test 2: Verify Firebase Storage

1. **Open Firebase Console**
2. **Navigate to**: Realtime Database → reports → [latest report ID]
3. **Verify fields**:
   - Description: Should have your text
   - LocationAddress: Should have address or coordinates
   - ReportedBy: Should have user ID
   - ReporterName: Should have your name

### Test 3: Verify Retrieval

1. **Log in as Official**
2. **Check Debug Output** for:
   ```
   📦 Raw JSON for [report-id]:
      Should show actual values, not null
   
   🔍 After deserialization:
      Description: 'Test report with all data'  ← Should match
      LocationAddress: 'Lat: XX, Long: XX'  ← Should match
   ```

## 📊 Debug Output Interpretation

### ✅ Healthy Output (Saving)

```
=== Starting Report Submission ===
✅ Current User Validated:
   UserId: abc123
   Name: Juan Dela Cruz

📋 Values to be saved:
   description variable: 'Broken streetlight on Main St'
   currentAddress variable: 'Main St, Manila, Metro Manila'
   currentLatitude: 14.5995
   currentLongitude: 120.9842
   imageData length: 50000

📝 Report Object Created:
   Description: 'Broken streetlight on Main St' (length: 30)
   LocationAddress: 'Main St, Manila, Metro Manila'
   Coordinates: (14.5995, 120.9842)
   ReportedBy (UserId): abc123
   ReporterName: Juan Dela Cruz

💾 Firebase Save Result: True
✅ SUCCESS: Report saved to Firebase
```

### ✅ Healthy Output (Retrieving)

```
=== OFFICIAL PAGE: LOADING ALL REPORTS ===
📊 Retrieved 1 reports from Firebase

📦 Raw JSON for abc-123:
   {"ReportId":"abc-123","Description":"Broken streetlight on Main St","LocationAddress":"Main St, Manila, Metro Manila",...}

🔍 After deserialization:
📝 Report: abc-123
   Description: 'Broken streetlight on Main St'
   LocationAddress: 'Main St, Manila, Metro Manila'
   Latitude: 14.5995
   Longitude: 120.9842
   ✅ Added to official's view
```

### ❌ Unhealthy Output (Problem)

```
📋 Values to be saved:
   description variable: 'NULL'  ← PROBLEM!
   currentAddress variable: 'NULL'  ← PROBLEM!
   currentLatitude: 0
   currentLongitude: 0
```

OR

```
📦 Raw JSON for abc-123:
   {"ReportId":"abc-123","Description":null,"LocationAddress":null,...}  ← PROBLEM!
```

## 🔧 Quick Fixes

### Fix 1: Ensure Location is Captured

**Check**: Is the location frame visible before submitting?

**Code Location**: ResidentPage.xaml.cs, line ~206
```csharp
if (!hasLocation)
{
    await DisplayAlert("Missing Information", "Please capture your current location.", "OK");
    return;
}
```

This validation should prevent submission without location.

### Fix 2: Verify Description is Entered

**Check**: Is the description editor populated?

**Code Location**: ResidentPage.xaml.cs, line ~193
```csharp
if (string.IsNullOrWhiteSpace(description))
{
    await DisplayAlert("Missing Information", "Please provide a description of the issue.", "OK");
    return;
}
```

This validation should prevent submission without description.

### Fix 3: Check XAML Bindings

**File**: ResidentPage.xaml

Verify the Editor has the correct name:
```xml
<Editor x:Name="DescriptionEditor" ... />
```

And location label:
```xml
<Label x:Name="LocationLabel" ... />
```

## 📞 Next Steps

1. **Submit a new report** with the enhanced logging
2. **Copy the debug output** and analyze it
3. **Check Firebase Console** to verify data
4. **Compare** what's saved vs what's retrieved

### If Values are NULL When Saving:
- Problem is in ResidentPage data capture
- Check UI controls are properly named
- Verify location button is working

### If Values are NULL When Retrieving:
- Problem is in Firebase or deserialization
- Check Firebase Console for actual data
- Verify Report model matches JSON structure

### If Values are Correct in Both:
- Problem might be in display logic
- Check CreateReportCard method
- Verify null-coalescing operators aren't hiding real values

---

**Status**: Diagnostic logging added  
**Action Required**: Submit a test report and check debug output  
**Expected**: Debug output will show exactly where data is being lost
