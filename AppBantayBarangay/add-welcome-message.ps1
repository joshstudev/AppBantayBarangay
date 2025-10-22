# Script to add welcome message code to OfficialPage.xaml.cs

$filePath = "AppBantayBarangay\Views\OfficialPage.xaml.cs"

# Read the file
$content = Get-Content $filePath -Raw

# Find the line to insert after
$searchText = "                    System.Diagnostics.Debug.WriteLine(`$`"👤 Official User: {currentUser.UserId} ({currentUser.FullName})`");
                }"

$insertText = @"
                    System.Diagnostics.Debug.WriteLine(`$`"👤 Official User: {currentUser.UserId} ({currentUser.FullName})`");
                    
                    // Update welcome message with first name
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        if (!string.IsNullOrEmpty(currentUser.FirstName))
                        {
                            WelcomeLabel.Text = `$`"Welcome, {currentUser.FirstName}!`";
                        }
                    });
                }
"@

# Replace
$newContent = $content -replace [regex]::Escape($searchText), $insertText

# Write back
Set-Content -Path $filePath -Value $newContent -NoNewline

Write-Host "✅ Welcome message code added successfully!" -ForegroundColor Green
Write-Host ""
Write-Host "The OfficialPage will now show 'Welcome, [FirstName]!' instead of 'Welcome, Official!'" -ForegroundColor Cyan
