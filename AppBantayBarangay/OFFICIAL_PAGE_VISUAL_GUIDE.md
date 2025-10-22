# Official Main Page - Visual Guide

## 📱 Page Layout Structure

```
┌─────────────────────────────────────────────────────────┐
│  ┌──┐  OFFICIAL DASHBOARD        Welcome, Juan!    [⎋] │ ← Header (Blue)
│  │BB│                                                    │
│  └──┘                                                    │
├─────────────────────────────────────────────────────────┤
│                                                          │
│  Overview                                                │
│                                                          │
│  ┌──────────────┐  ┌──────────────┐                    │
│  │      ⏳      │  │      🔄      │                    │
│  │      5       │  │      2       │                    │ ← Statistics Cards
│  │   Pending    │  │  In Progress │                    │
│  └──────────────┘  └──────────────┘                    │
│                                                          │
│  ┌──────────────┐  ┌──────────────┐                    │
│  │      ✅      │  │      📊      │                    │
│  │      3       │  │      10      │                    │
│  │   Resolved   │  │ Total Reports│                    │
│  └──────────────┘  └──────────────┘                    │
│                                                          │
├─────────────────────────────────────────────────────────┤
│                                                          │
│  Filter:  [  All  ] [ Pending ] [ In Progress ]         │ ← Filter Buttons
│                                                          │
├─────────────────────────────────────────────────────────┤
│                                                          │
│  Reports                                                 │
│                                                          │
│  ┌─────────────────────────────────────────────────┐   │
│  │ Report #1                          [Pending]    │   │
│  │                                                  │   │
│  │ Broken streetlight on Main Street causing...    │   │
│  │                                                  │   │ ← Report Cards
│  │ 📍 Main Street, Barangay Centro   May 15, 2024  │   │   (Scrollable)
│  │ By: Juan Dela Cruz                              │   │
│  └─────────────────────────────────────────────────┘   │
│                                                          │
│  ┌─────────────────────────────────────────────────┐   │
│  │ Report #2                      [In Progress]    │   │
│  │                                                  │   │
│  │ Large pothole on the road near the market...    │   │
│  │                                                  │   │
│  │ 📍 Market Road, Barangay Centro   May 17, 2024  │   │
│  │ By: Maria Santos                                │   │
│  └─────────────────────────────────────────────────┘   │
│                                                          │
│  ┌─────────────────────────────────────────────────┐   │
│  │ Report #3                         [Resolved]    │   │
│  │                                                  │   │
│  │ Overflowing garbage bins attracting pests...    │   │
│  │                                                  │   │
│  │ 📍 Park Avenue, Barangay Centro   May 13, 2024  │   │
│  │ By: Pedro Reyes                                 │   │
│  └─────────────────────────────────────────────────┘   │
│                                                          │
└─────────────────────────────────────────────────────────┘
```

---

## 🔍 Report Details Modal

```
┌─────────────────────────────────────────────────────────┐
│                                                          │
│    ┌───────────────────────────────────────────────┐   │
│    │                                             [X]│   │
│    │         Report Details                         │   │
│    │                                                │   │
│    │  ┌──────────────────────────────────────────┐ │   │
│    │  │                                          │ │   │
│    │  │         [Report Image]                   │ │   │
│    │  │                                          │ │   │
│    │  └──────────────────────────────────────────┘ │   │
│    │                                                │   │
│    │  Description                                   │   │
│    │  Broken streetlight on Main Street causing     │   │
│    │  safety concerns for pedestrians at night.     │   │
│    │                                                │   │
│    │  Reported By                                   │   │
│    │  Juan Dela Cruz                                │   │
│    │                                                │   │
│    │  Date Reported                                 │   │
│    │  May 15, 2024 at 03:30 PM                      │   │
│    │                                                │   │
│    │  Location                                      │   │
│    │  Main Street, Barangay Centro                  │   │
│    │                                                │   │
│    │  ┌──────────────────────────────────────────┐ │   │
│    │  │                                          │ │   │
│    │  │         [Interactive Map]                │ │   │
│    │  │              📍                          │ │   │
│    │  └──────────────────────────────────────────┘ │   │
│    │                                                │   │
│    │  Status                                        │   │
│    │  ┌─────────┐                                  │   │
│    │  │ Pending │                                  │   │
│    │  └─────────┘                                  │   │
│    │                                                │   │
│    │  ┌──────────────────────────────────────────┐ │   │
│    │  │    Mark as In Progress                   │ │   │
│    │  └──────────────────────────────────────────┘ │   │
│    │                                                │   │
│    │  ┌──────────────────────────────────────────┐ │   │
│    │  │    Mark as Resolved                      │ │   │
│    │  └──────────────────────────────────────────┘ │   │
│    │                                                │   │
│    │  ┌──────────────────────────────────────────┐ │   │
│    │  │    Reject Report                         │ │   │
│    │  └──────────────────────────────────────────┘ │   │
│    │                                                │   │
│    └───────────────────────────────────────────────┘   │
│                                                          │
└─────────────────────────────────────────────────────────┘
```

---

## 🔄 User Flow Diagram

### Flow 1: Viewing Reports

```
┌─────────────┐
│   Login     │
│  as Official│
└──────┬──────┘
       │
       ▼
┌─────────────────┐
│   Dashboard     │
│   Loads with    │
│   Statistics    │
└──────┬──────────┘
       │
       ▼
┌─────────────────┐      ┌──────────────┐
│  View Reports   │◄─────┤ Apply Filter │
│     List        │      │ (Optional)   │
└──────┬──────────┘      └──────────────┘
       │
       ▼
┌─────────────────┐
│  Tap Report     │
│     Card        │
└──────┬──────────┘
       │
       ▼
┌─────────────────┐
│  View Detailed  │
│   Information   │
└─────────────────┘
```

### Flow 2: Resolving a Report

```
┌─────────────────┐
│  Open Report    │
│    Details      │
└──────┬──────────┘
       │
       ▼
┌─────────────────┐
│  Review Image,  │
│  Description,   │
│  & Location     │
└──────┬──────────┘
       │
       ▼
┌─────────────────┐      ┌──────────────────┐
│ Mark as "In     │      │   (Optional)     │
│ Progress"       │◄─────┤  Skip this step  │
└──────┬──────────┘      └──────────────────┘
       │
       ▼
┌─────────────────┐
│  Click "Mark    │
│  as Resolved"   │
└──────┬──────────┘
       │
       ▼
┌─────────────────┐
│ Resolution      │
│ Notes Field     │
│   Appears       │
└──────┬──────────┘
       │
       ▼
┌─────────────────┐
│  Enter Notes    │
│  (Required)     │
└──────┬──────────┘
       │
       ▼
┌─────────────────┐
│ Click "Confirm  │
│  Resolution"    │
└──────┬──────────┘
       │
       ▼
┌─────────────────┐
│  Confirmation   │
│     Dialog      │
└──────┬──────────┘
       │
       ▼
┌─────────────────┐
│  Status Updated │
│  Statistics     │
│   Refreshed     │
└──────┬──────────┘
       │
       ▼
┌─────────────────┐
│  Modal Closes   │
│  Success Alert  │
└─────────────────┘
```

### Flow 3: Filtering Reports

```
┌─────────────────┐
│   Dashboard     │
│    Loaded       │
└──────┬──────────┘
       │
       ▼
┌─────────────────┐
│  Click Filter   │
│     Button      │
└──────┬──────────┘
       │
       ├──────────────┬──────────────┬──────────────┐
       │              │              │              │
       ▼              ▼              ▼              ▼
┌──────────┐   ┌──────────┐   ┌──────────┐   ┌──────────┐
│   All    │   │ Pending  │   │    In    │   │ Resolved │
│ Reports  │   │  Only    │   │ Progress │   │   Only   │
└────┬─────┘   └────┬─────┘   └────┬─────┘   └────┬─────┘
     │              │              │              │
     └──────────────┴──────────────┴──────────────┘
                    │
                    ▼
            ┌───────────────┐
            │ Reports List  │
            │   Filtered    │
            │  & Displayed  │
            └───────────────┘
```

---

## 🎨 Color-Coded Status System

```
┌─────────────────────────────────────────────────────┐
│                                                      │
│  Status Colors & Meanings:                          │
│                                                      │
│  ┌──────────┐                                       │
│  │ Pending  │  Orange (#FF9500)                     │
│  └──────────┘  → New report, needs attention        │
│                                                      │
│  ┌──────────────┐                                   │
│  │ In Progress  │  Blue (#007AFF)                   │
│  └──────────────┘  → Official is working on it      │
│                                                      │
│  ┌──────────┐                                       │
│  │ Resolved │  Green (#34C759)                      │
│  └──────────┘  → Issue has been fixed               │
│                                                      │
│  ┌──────────┐                                       │
│  │ Rejected │  Red (#FF3B30)                        │
│  └──────────┘  → Invalid or duplicate report        │
│                                                      │
└─────────────────────────────────────────────────────┘
```

---

## 📊 Statistics Card Layout

```
┌─────────────────────────────────────────────────────┐
│                                                      │
│  ┌──────────────┐  ┌──────────────┐                │
│  │              │  │              │                │
│  │      ⏳      │  │      🔄      │                │
│  │              │  │              │                │
│  │      5       │  │      2       │  ← Large Number│
│  │              │  │              │                │
│  │   Pending    │  │  In Progress │  ← Label       │
│  │              │  │              │                │
│  └──────────────┘  └──────────────┘                │
│       Orange            Blue                        │
│                                                      │
│  ┌──────────────┐  ┌──────────────┐                │
│  │              │  │              │                │
│  │      ✅      │  │      📊      │                │
│  │              │  │              │                │
│  │      3       │  │      10      │                │
│  │              │  │              │                │
│  │   Resolved   │  │ Total Reports│                │
│  │              │  │              │                │
│  └──────────────┘  └──────────────┘                │
│       Green             Blue                        │
│                                                      │
└─────────────────────────────────────────────────────┘
```

---

## 🗺️ Interactive Map Display

```
┌─────────────────────────────────────────────────────┐
│                                                      │
│  Location: Main Street, Barangay Centro             │
│                                                      │
│  ┌────────────────────────────────────────────────┐ │
│  │                                                │ │
│  │         ╔═══════════════════════╗              │ │
│  │         ║                       ║              │ │
│  │         ║    Interactive Map    ║              │ │
│  │         ║                       ║              │ │
│  │         ║         📍            ║              │ │
│  │         ║    (Pin Marker)       ║              │ │
│  │         ║                       ║              │ │
│  │         ║   Zoom & Pan Enabled  ║              │ │
│  │         ║                       ║              │ │
│  │         ╚═══════════════════════╝              │ │
│  │                                                │ │
│  │  • Shows exact location of reported issue     │ │
│  │  • Centered on pin with 500m radius           │ │
│  │  • Helps officials plan response              │ │
│  │                                                │ │
│  └────────────────────────────────────────────────┘ │
│                                                      │
└─────────────────────────────────────────────────────┘
```

---

## 🎯 Action Button States

### Pending Report
```
┌─────────────────────────────────────────┐
│                                          │
│  ┌────────────────────────────────────┐ │
│  │  Mark as In Progress         [Blue]│ │ ← Available
│  └────────────────────────────────────┘ │
│                                          │
│  ┌────────────────────────────────────┐ │
│  │  Mark as Resolved           [Green]│ │ ← Available
│  └────────────────────────────────────┘ │
│                                          │
│  ┌────────────────────────────────────┐ │
│  │  Reject Report                [Red]│ │ ← Available
│  └────────────────────────────────────┘ │
│                                          │
└─────────────────────────────────────────┘
```

### In Progress Report
```
┌─────────────────────────────────────────┐
│                                          │
│  ┌────────────────────────────────────┐ │
│  │  Mark as Resolved           [Green]│ │ ← Available
│  └────────────────────────────────────┘ │
│                                          │
│  ┌────────────────────────────────────┐ │
│  │  Reject Report                [Red]│ │ ← Available
│  └────────────────────────────────────┘ │
│                                          │
└─────────────────────────────────────────┘
```

### Resolved Report
```
┌─────────────────────────────────────────┐
│                                          │
│  Resolution Notes:                       │
│  Garbage has been collected and bins     │
│  have been replaced with larger capacity │
│  containers. Scheduled collection        │
│  frequency increased.                    │
│                                          │
│  Resolved by Barangay Official           │
│  on May 19, 2024                         │
│                                          │
│  [No action buttons - report is closed]  │
│                                          │
└─────────────────────────────────────────┘
```

---

## 📝 Resolution Notes Input

### Step 1: Initial State
```
┌─────────────────────────────────────────┐
│                                          │
│  ┌────────────────────────────────────┐ │
│  │  Mark as Resolved           [Green]│ │ ← Click to show input
│  └────────────────────────────────────┘ │
│                                          │
└─────────────────────────────────────────┘
```

### Step 2: Input Field Appears
```
┌─────────────────────────────────────────┐
│                                          │
│  Resolution Notes                        │
│  ┌────────────────────────────────────┐ │
│  │ Enter resolution details...        │ │
│  │                                    │ │
│  │ [Text input area]                  │ │ ← Type notes here
│  │                                    │ │
│  │                                    │ │
│  └────────────────────────────────────┘ │
│                                          │
│  ┌────────────────────────────────────┐ │
│  │  Confirm Resolution         [Green]│ │ ← Click to submit
│  └────────────────────────────────────┘ │
│                                          │
└─────────────────────────────────────────┘
```

---

## 🔔 Confirmation Dialogs

### Mark as Resolved
```
┌─────────────────────────────────────────┐
│                                          │
│              Confirm                     │
│                                          │
│  Mark this report as Resolved?           │
│                                          │
│  ┌──────────┐         ┌──────────┐      │
│  │   Yes    │         │    No    │      │
│  └──────────┘         └──────────┘      │
│                                          │
└─────────────────────────────────────────┘
```

### Reject Report
```
┌─────────────────────────────────────────┐
│                                          │
│              Confirm                     │
│                                          │
│  Are you sure you want to reject this    │
│  report? This action cannot be undone.   │
│                                          │
│  ┌──────────┐         ┌──────────┐      │
│  │   Yes    │         │    No    │      │
│  └──────────┘         └──────────┘      │
│                                          │
└─────────────────────────────────────────┘
```

### Logout
```
┌─────────────────────────────────────────┐
│                                          │
│              Logout                      │
│                                          │
│  Are you sure you want to logout?        │
│                                          │
│  ┌──────────┐         ┌──────────┐      │
│  │   Yes    │         │    No    │      │
│  └──────────┘         └──────────┘      │
│                                          │
└─────────────────────────────────────────┘
```

---

## 📱 Responsive Behavior

### Portrait Mode (Default)
```
┌─────────────┐
│   Header    │
├─────────────┤
│ Statistics  │
│  (2x2 Grid) │
├─────────────┤
│   Filters   │
├─────────────┤
│             │
│   Reports   │
│    List     │
│  (Scroll)   │
│             │
└─────────────┘
```

### Landscape Mode (Adaptive)
```
┌────────────────────────────────────┐
│           Header                   │
├────────────┬───────────────────────┤
│Statistics  │                       │
│ (2x2 Grid) │    Reports List       │
│            │     (Scroll)          │
│  Filters   │                       │
└────────────┴───────────────────────┘
```

---

## 🎨 Visual Hierarchy

```
Level 1: Header (Blue Background)
   │
   ├─ Logo & Title (Most Prominent)
   └─ User Welcome & Logout
   
Level 2: Statistics Overview
   │
   ├─ Large Numbers (Primary Focus)
   └─ Status Labels (Secondary)
   
Level 3: Filters
   │
   └─ Active/Inactive States (Clear Distinction)
   
Level 4: Reports List
   │
   ├─ Report ID & Status (Header)
   ├─ Description (Body)
   └─ Metadata (Footer)
   
Level 5: Modal Details
   │
   ├─ Image (Visual Anchor)
   ├─ Information Sections (Organized)
   └─ Action Buttons (Call to Action)
```

---

## ✨ Animation & Transitions

### Card Tap Animation
```
Normal State → Tap → Slight Scale Down → Release → Modal Appears
   100%         95%         100%           Fade In
```

### Filter Button Toggle
```
Inactive → Tap → Active
  Gray         Blue
  
Active → Tap Another → Inactive
 Blue                    Gray
```

### Modal Appearance
```
Hidden → Triggered → Fade In + Scale Up
  0%                    100%
```

### Statistics Update
```
Old Value → Update → Brief Highlight → New Value
   "5"                  Flash Green      "6"
```

---

This visual guide provides a comprehensive overview of the Official Main Page design, making it easy to understand the layout, user flows, and interactive elements at a glance.
