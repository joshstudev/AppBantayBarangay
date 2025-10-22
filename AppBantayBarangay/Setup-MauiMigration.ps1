# .NET MAUI Migration Setup Script
# This script helps automate the initial setup for migrating from Xamarin.Forms to .NET MAUI

param(
    [switch]$CreateBackup = $true,
    [switch]$CreateNewProject = $true,
    [switch]$SetupFolders = $true
)

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  .NET MAUI Migration Setup Script" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$projectRoot = "C:\Users\ASUS_VX16\source\repos\AppBantayBarangay\AppBantayBarangay"
$backupPath = "C:\Users\ASUS_VX16\source\repos\AppBantayBarangay\AppBantayBarangay_Backup"
$mauiProjectPath = "$projectRoot\AppBantayBarangay.Maui"

# Function to check prerequisites
function Test-Prerequisites {
    Write-Host "Checking prerequisites..." -ForegroundColor Yellow
    
    # Check .NET SDK
    try {
        $dotnetVersion = dotnet --version
        Write-Host "✓ .NET SDK found: $dotnetVersion" -ForegroundColor Green
        
        if ($dotnetVersion -lt "8.0") {
            Write-Host "⚠ Warning: .NET 8.0 or later is recommended" -ForegroundColor Yellow
        }
    }
    catch {
        Write-Host "✗ .NET SDK not found. Please install .NET 8.0 SDK" -ForegroundColor Red
        return $false
    }
    
    # Check Visual Studio
    $vsPath = "C:\Program Files\Microsoft Visual Studio\2022"
    if (Test-Path $vsPath) {
        Write-Host "✓ Visual Studio 2022 found" -ForegroundColor Green
    }
    else {
        Write-Host "⚠ Visual Studio 2022 not found at default location" -ForegroundColor Yellow
    }
    
    return $true
}

# Function to create backup
function New-ProjectBackup {
    Write-Host ""
    Write-Host "Creating backup..." -ForegroundColor Yellow
    
    if (Test-Path $backupPath) {
        Write-Host "⚠ Backup already exists at: $backupPath" -ForegroundColor Yellow
        $response = Read-Host "Overwrite existing backup? (y/n)"
        if ($response -ne 'y') {
            Write-Host "Skipping backup..." -ForegroundColor Yellow
            return
        }
        Remove-Item $backupPath -Recurse -Force
    }
    
    try {
        Copy-Item $projectRoot $backupPath -Recurse -Force
        Write-Host "✓ Backup created at: $backupPath" -ForegroundColor Green
    }
    catch {
        Write-Host "✗ Failed to create backup: $_" -ForegroundColor Red
    }
}

# Function to create new MAUI project
function New-MauiProject {
    Write-Host ""
    Write-Host "Creating new .NET MAUI project..." -ForegroundColor Yellow
    
    if (Test-Path $mauiProjectPath) {
        Write-Host "⚠ MAUI project already exists at: $mauiProjectPath" -ForegroundColor Yellow
        $response = Read-Host "Overwrite existing project? (y/n)"
        if ($response -ne 'y') {
            Write-Host "Skipping project creation..." -ForegroundColor Yellow
            return
        }
        Remove-Item $mauiProjectPath -Recurse -Force
    }
    
    try {
        Set-Location $projectRoot
        dotnet new maui -n AppBantayBarangay.Maui -o AppBantayBarangay.Maui
        Write-Host "✓ MAUI project created at: $mauiProjectPath" -ForegroundColor Green
    }
    catch {
        Write-Host "✗ Failed to create MAUI project: $_" -ForegroundColor Red
    }
}

# Function to setup folder structure
function New-FolderStructure {
    Write-Host ""
    Write-Host "Setting up folder structure..." -ForegroundColor Yellow
    
    $folders = @(
        "$mauiProjectPath\Platforms\Android\Services",
        "$mauiProjectPath\Platforms\Android\Resources\values",
        "$mauiProjectPath\Platforms\Android\Resources\mipmap-hdpi",
        "$mauiProjectPath\Platforms\Android\Resources\mipmap-mdpi",
        "$mauiProjectPath\Platforms\Android\Resources\mipmap-xhdpi",
        "$mauiProjectPath\Platforms\Android\Resources\mipmap-xxhdpi",
        "$mauiProjectPath\Platforms\Android\Resources\mipmap-xxxhdpi",
        "$mauiProjectPath\Resources\Images",
        "$mauiProjectPath\Resources\Fonts",
        "$mauiProjectPath\Resources\AppIcon",
        "$mauiProjectPath\Resources\Splash",
        "$mauiProjectPath\Models",
        "$mauiProjectPath\Services",
        "$mauiProjectPath\Views",
        "$mauiProjectPath\ViewModels"
    )
    
    foreach ($folder in $folders) {
        if (-not (Test-Path $folder)) {
            New-Item -ItemType Directory -Path $folder -Force | Out-Null
            Write-Host "  Created: $folder" -ForegroundColor Gray
        }
    }
    
    Write-Host "✓ Folder structure created" -ForegroundColor Green
}

# Function to copy existing files
function Copy-ExistingFiles {
    Write-Host ""
    Write-Host "Copying existing files..." -ForegroundColor Yellow
    
    # Copy Services
    if (Test-Path "$projectRoot\AppBantayBarangay\Services") {
        Copy-Item "$projectRoot\AppBantayBarangay\Services\*" "$mauiProjectPath\Services\" -Recurse -Force
        Write-Host "  ✓ Copied Services" -ForegroundColor Gray
    }
    
    # Copy Models
    if (Test-Path "$projectRoot\AppBantayBarangay\Models") {
        Copy-Item "$projectRoot\AppBantayBarangay\Models\*" "$mauiProjectPath\Models\" -Recurse -Force
        Write-Host "  ✓ Copied Models" -ForegroundColor Gray
    }
    
    # Copy Views
    if (Test-Path "$projectRoot\AppBantayBarangay\Views") {
        Copy-Item "$projectRoot\AppBantayBarangay\Views\*" "$mauiProjectPath\Views\" -Recurse -Force
        Write-Host "  ✓ Copied Views" -ForegroundColor Gray
    }
    
    # Copy Android Resources
    if (Test-Path "$projectRoot\AppBantayBarangay.Android\Resources\values") {
        Copy-Item "$projectRoot\AppBantayBarangay.Android\Resources\values\*" "$mauiProjectPath\Platforms\Android\Resources\values\" -Force
        Write-Host "  ✓ Copied Android Resources" -ForegroundColor Gray
    }
    
    # Copy google-services.json
    if (Test-Path "$projectRoot\AppBantayBarangay.Android\Assets\google-services.json") {
        Copy-Item "$projectRoot\AppBantayBarangay.Android\Assets\google-services.json" "$mauiProjectPath\Platforms\Android\" -Force
        Write-Host "  ✓ Copied google-services.json" -ForegroundColor Gray
    }
    
    # Copy Firebase Service
    if (Test-Path "$projectRoot\AppBantayBarangay.Android\Services\FirebaseService.cs") {
        Copy-Item "$projectRoot\AppBantayBarangay.Android\Services\FirebaseService.cs" "$mauiProjectPath\Platforms\Android\Services\" -Force
        Write-Host "  ✓ Copied FirebaseService" -ForegroundColor Gray
    }
    
    Write-Host "✓ Files copied" -ForegroundColor Green
}

# Function to display next steps
function Show-NextSteps {
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "  Migration Setup Complete!" -ForegroundColor Cyan
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Next Steps:" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "1. Open the MAUI project in Visual Studio 2022:" -ForegroundColor White
    Write-Host "   $mauiProjectPath\AppBantayBarangay.Maui.csproj" -ForegroundColor Gray
    Write-Host ""
    Write-Host "2. Review and follow the migration guide:" -ForegroundColor White
    Write-Host "   XAMARIN_TO_MAUI_MIGRATION_GUIDE.md" -ForegroundColor Gray
    Write-Host ""
    Write-Host "3. Use the migration checklist to track progress:" -ForegroundColor White
    Write-Host "   MAUI_MIGRATION_CHECKLIST.md" -ForegroundColor Gray
    Write-Host ""
    Write-Host "4. Update namespaces in all XAML files:" -ForegroundColor White
    Write-Host "   xmlns=`"http://schemas.microsoft.com/dotnet/2021/maui`"" -ForegroundColor Gray
    Write-Host ""
    Write-Host "5. Update using statements in C# files:" -ForegroundColor White
    Write-Host "   using Microsoft.Maui.Controls;" -ForegroundColor Gray
    Write-Host ""
    Write-Host "6. Create MauiProgram.cs, AppShell.xaml, and platform-specific files" -ForegroundColor White
    Write-Host ""
    Write-Host "7. Test and validate the migration" -ForegroundColor White
    Write-Host ""
    Write-Host "Good luck with your migration! 🚀" -ForegroundColor Green
    Write-Host ""
}

# Main execution
try {
    # Check prerequisites
    if (-not (Test-Prerequisites)) {
        Write-Host ""
        Write-Host "Please install required prerequisites and try again." -ForegroundColor Red
        exit 1
    }
    
    # Create backup
    if ($CreateBackup) {
        New-ProjectBackup
    }
    
    # Create new MAUI project
    if ($CreateNewProject) {
        New-MauiProject
    }
    
    # Setup folder structure
    if ($SetupFolders -and (Test-Path $mauiProjectPath)) {
        New-FolderStructure
        Copy-ExistingFiles
    }
    
    # Show next steps
    Show-NextSteps
}
catch {
    Write-Host ""
    Write-Host "✗ An error occurred: $_" -ForegroundColor Red
    Write-Host ""
    exit 1
}
