# Fix App Not Running - Automated Script
# This script performs common fixes for Xamarin.Android apps that won't run

param(
    [switch]$CleanAll = $true,
    [switch]$ClearNuGet = $true,
    [switch]$RestorePackages = $true
)

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  App Not Running - Automated Fix" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$projectRoot = "C:\Users\ASUS_VX16\source\repos\AppBantayBarangay\AppBantayBarangay"
$androidProject = "$projectRoot\AppBantayBarangay.Android"
$sharedProject = "$projectRoot\AppBantayBarangay"

# Function to check if Visual Studio is running
function Test-VisualStudioRunning {
    $vsProcess = Get-Process devenv -ErrorAction SilentlyContinue
    return $null -ne $vsProcess
}

# Function to delete folder safely
function Remove-FolderSafely {
    param([string]$Path, [string]$Name)
    
    if (Test-Path $Path) {
        try {
            Write-Host "  Deleting $Name..." -ForegroundColor Yellow
            Remove-Item $Path -Recurse -Force -ErrorAction Stop
            Write-Host "  ✓ Deleted $Name" -ForegroundColor Green
        }
        catch {
            Write-Host "  ✗ Failed to delete $Name : $_" -ForegroundColor Red
        }
    }
    else {
        Write-Host "  ○ $Name not found (already clean)" -ForegroundColor Gray
    }
}

# Check if Visual Studio is running
if (Test-VisualStudioRunning) {
    Write-Host "⚠ WARNING: Visual Studio is currently running!" -ForegroundColor Yellow
    Write-Host "For best results, close Visual Studio before running this script." -ForegroundColor Yellow
    Write-Host ""
    $response = Read-Host "Continue anyway? (y/n)"
    if ($response -ne 'y') {
        Write-Host "Script cancelled. Please close Visual Studio and run again." -ForegroundColor Yellow
        exit 0
    }
    Write-Host ""
}

# Step 1: Clean bin and obj folders
if ($CleanAll) {
    Write-Host "Step 1: Cleaning bin and obj folders..." -ForegroundColor Cyan
    Write-Host ""
    
    Remove-FolderSafely "$androidProject\bin" "Android bin"
    Remove-FolderSafely "$androidProject\obj" "Android obj"
    Remove-FolderSafely "$sharedProject\bin" "Shared bin"
    Remove-FolderSafely "$sharedProject\obj" "Shared obj"
    Remove-FolderSafely "$projectRoot\.vs" ".vs folder"
    
    Write-Host ""
}

# Step 2: Clear NuGet cache
if ($ClearNuGet) {
    Write-Host "Step 2: Clearing NuGet cache..." -ForegroundColor Cyan
    Write-Host ""
    
    try {
        Write-Host "  Running: dotnet nuget locals all --clear" -ForegroundColor Yellow
        $output = dotnet nuget locals all --clear 2>&1
        Write-Host "  ✓ NuGet cache cleared" -ForegroundColor Green
    }
    catch {
        Write-Host "  ✗ Failed to clear NuGet cache: $_" -ForegroundColor Red
    }
    
    Write-Host ""
}

# Step 3: Restore NuGet packages
if ($RestorePackages) {
    Write-Host "Step 3: Restoring NuGet packages..." -ForegroundColor Cyan
    Write-Host ""
    
    try {
        Set-Location $projectRoot
        Write-Host "  Running: dotnet restore" -ForegroundColor Yellow
        $output = dotnet restore 2>&1
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "  ✓ Packages restored successfully" -ForegroundColor Green
        }
        else {
            Write-Host "  ⚠ Package restore completed with warnings" -ForegroundColor Yellow
        }
    }
    catch {
        Write-Host "  ✗ Failed to restore packages: $_" -ForegroundColor Red
    }
    
    Write-Host ""
}

# Step 4: Verify google-services.json
Write-Host "Step 4: Verifying Firebase configuration..." -ForegroundColor Cyan
Write-Host ""

$googleServicesPath = "$androidProject\Assets\google-services.json"
if (Test-Path $googleServicesPath) {
    Write-Host "  ✓ google-services.json found" -ForegroundColor Green
}
else {
    Write-Host "  ✗ google-services.json NOT found!" -ForegroundColor Red
    Write-Host "    Expected location: $googleServicesPath" -ForegroundColor Yellow
}

Write-Host ""

# Step 5: Check project files
Write-Host "Step 5: Checking project files..." -ForegroundColor Cyan
Write-Host ""

$androidCsproj = "$androidProject\AppBantayBarangay.Android.csproj"
$sharedCsproj = "$sharedProject\AppBantayBarangay.csproj"
$mainActivity = "$androidProject\MainActivity.cs"
$appXaml = "$sharedProject\App.xaml.cs"
$loginPage = "$sharedProject\Views\LoginPage.xaml"

$files = @(
    @{Path = $androidCsproj; Name = "Android project file"},
    @{Path = $sharedCsproj; Name = "Shared project file"},
    @{Path = $mainActivity; Name = "MainActivity.cs"},
    @{Path = $appXaml; Name = "App.xaml.cs"},
    @{Path = $loginPage; Name = "LoginPage.xaml"}
)

foreach ($file in $files) {
    if (Test-Path $file.Path) {
        Write-Host "  ✓ $($file.Name) exists" -ForegroundColor Green
    }
    else {
        Write-Host "  ✗ $($file.Name) NOT found!" -ForegroundColor Red
    }
}

Write-Host ""

# Summary and next steps
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Fix Complete!" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Next Steps:" -ForegroundColor Yellow
Write-Host ""
Write-Host "1. Open Visual Studio 2022" -ForegroundColor White
Write-Host ""
Write-Host "2. Open the solution:" -ForegroundColor White
Write-Host "   $projectRoot\AppBantayBarangay.sln" -ForegroundColor Gray
Write-Host ""
Write-Host "3. Set startup project:" -ForegroundColor White
Write-Host "   - Right-click 'AppBantayBarangay.Android'" -ForegroundColor Gray
Write-Host "   - Select 'Set as Startup Project'" -ForegroundColor Gray
Write-Host ""
Write-Host "4. Select Android emulator or device:" -ForegroundColor White
Write-Host "   - Click dropdown next to green 'Play' button" -ForegroundColor Gray
Write-Host "   - Select an emulator or connected device" -ForegroundColor Gray
Write-Host ""
Write-Host "5. Build the solution:" -ForegroundColor White
Write-Host "   - Build → Rebuild Solution" -ForegroundColor Gray
Write-Host "   - Check for errors in Error List" -ForegroundColor Gray
Write-Host ""
Write-Host "6. Run the app:" -ForegroundColor White
Write-Host "   - Press F5 or click green 'Play' button" -ForegroundColor Gray
Write-Host ""
Write-Host "If the app still won't run:" -ForegroundColor Yellow
Write-Host "- Check APP_WONT_RUN_TROUBLESHOOTING.md for detailed solutions" -ForegroundColor Gray
Write-Host "- Check Output window (View → Output) for error messages" -ForegroundColor Gray
Write-Host "- Check Error List (View → Error List) for build errors" -ForegroundColor Gray
Write-Host ""
Write-Host "Good luck! 🚀" -ForegroundColor Green
Write-Host ""
