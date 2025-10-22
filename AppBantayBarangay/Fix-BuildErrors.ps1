# Fix Build Errors - Comprehensive Script
# This script fixes missing assembly references and restores packages

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Fixing Build Errors" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$projectRoot = "C:\Users\ASUS_VX16\source\repos\AppBantayBarangay\AppBantayBarangay"

# Step 1: Close Visual Studio if running
Write-Host "Step 1: Checking for Visual Studio..." -ForegroundColor Cyan
$vsProcess = Get-Process devenv -ErrorAction SilentlyContinue
if ($vsProcess) {
    Write-Host "⚠ Visual Studio is running. Please close it first." -ForegroundColor Yellow
    Write-Host "Press any key after closing Visual Studio..."
    $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
}

# Step 2: Clean bin and obj folders
Write-Host ""
Write-Host "Step 2: Cleaning bin and obj folders..." -ForegroundColor Cyan
$foldersToDelete = @(
    "$projectRoot\AppBantayBarangay.Android\bin",
    "$projectRoot\AppBantayBarangay.Android\obj",
    "$projectRoot\AppBantayBarangay\bin",
    "$projectRoot\AppBantayBarangay\obj"
)

foreach ($folder in $foldersToDelete) {
    if (Test-Path $folder) {
        Remove-Item $folder -Recurse -Force -ErrorAction SilentlyContinue
        Write-Host "  ✓ Deleted: $folder" -ForegroundColor Green
    }
}

# Step 3: Clear NuGet cache
Write-Host ""
Write-Host "Step 3: Clearing NuGet cache..." -ForegroundColor Cyan
dotnet nuget locals all --clear
Write-Host "  ✓ NuGet cache cleared" -ForegroundColor Green

# Step 4: Restore NuGet packages
Write-Host ""
Write-Host "Step 4: Restoring NuGet packages..." -ForegroundColor Cyan
Set-Location $projectRoot
dotnet restore
Write-Host "  ✓ Packages restored" -ForegroundColor Green

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Fix Complete!" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Next Steps:" -ForegroundColor Yellow
Write-Host "1. Open Visual Studio" -ForegroundColor White
Write-Host "2. Open the solution" -ForegroundColor White
Write-Host "3. Build → Rebuild Solution" -ForegroundColor White
Write-Host "4. If errors persist, try:" -ForegroundColor White
Write-Host "   - Right-click Solution → Restore NuGet Packages" -ForegroundColor Gray
Write-Host "   - Tools → Options → NuGet → Clear All NuGet Cache(s)" -ForegroundColor Gray
Write-Host "   - Rebuild again" -ForegroundColor Gray
Write-Host ""
