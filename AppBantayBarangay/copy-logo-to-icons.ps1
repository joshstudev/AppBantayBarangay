# PowerShell Script to Copy Logo to All Icon Locations
# This is a quick solution - for best results, use properly sized icons

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Copy Logo to App Icons" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Paths
$logoPath = "AppBantayBarangay.Android\Resources\drawable\logo.png"
$basePath = "AppBantayBarangay.Android\Resources"

# Check if logo exists
if (-not (Test-Path $logoPath)) {
    Write-Host "ERROR: Logo not found at $logoPath" -ForegroundColor Red
    Write-Host "Please ensure logo.png exists in the drawable folder." -ForegroundColor Yellow
    exit 1
}

Write-Host "Found logo: $logoPath" -ForegroundColor Green
$logoSize = (Get-Item $logoPath).Length / 1KB
Write-Host "Logo size: $([math]::Round($logoSize, 2)) KB" -ForegroundColor Gray
Write-Host ""

# Icon folders
$iconFolders = @(
    "mipmap-mdpi",
    "mipmap-hdpi",
    "mipmap-xhdpi",
    "mipmap-xxhdpi",
    "mipmap-xxxhdpi"
)

Write-Host "Copying logo to icon locations..." -ForegroundColor Yellow
Write-Host ""

$successCount = 0
foreach ($folder in $iconFolders) {
    $targetPath = Join-Path $basePath "$folder\icon.png"
    
    try {
        Copy-Item $logoPath $targetPath -Force
        Write-Host "  ✓ Copied to $folder\icon.png" -ForegroundColor Green
        $successCount++
    }
    catch {
        Write-Host "  ✗ Failed to copy to $folder\icon.png" -ForegroundColor Red
        Write-Host "    Error: $($_.Exception.Message)" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Summary" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Successfully copied to $successCount out of $($iconFolders.Count) locations" -ForegroundColor $(if ($successCount -eq $iconFolders.Count) { "Green" } else { "Yellow" })
Write-Host ""

if ($successCount -eq $iconFolders.Count) {
    Write-Host "✓ All icons updated successfully!" -ForegroundColor Green
    Write-Host ""
    Write-Host "NEXT STEPS:" -ForegroundColor Cyan
    Write-Host "1. In Visual Studio: Build → Clean Solution" -ForegroundColor White
    Write-Host "2. Then: Build → Rebuild Solution" -ForegroundColor White
    Write-Host "3. Uninstall the old app from your device/emulator" -ForegroundColor White
    Write-Host "4. Run the app to see the new icon" -ForegroundColor White
    Write-Host ""
    Write-Host "NOTE: For best results, use properly sized icons." -ForegroundColor Yellow
    Write-Host "See CHANGE_APP_ICON_GUIDE.md for details." -ForegroundColor Yellow
}
else {
    Write-Host "⚠ Some icons failed to update. Check errors above." -ForegroundColor Yellow
}

Write-Host ""
Write-Host "Press any key to exit..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
