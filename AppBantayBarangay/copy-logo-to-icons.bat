@echo off
echo ========================================
echo   Copy Logo to App Icons
echo ========================================
echo.

set LOGO=AppBantayBarangay.Android\Resources\drawable\logo.png
set BASE=AppBantayBarangay.Android\Resources

if not exist "%LOGO%" (
    echo ERROR: Logo not found at %LOGO%
    echo Please ensure logo.png exists in the drawable folder.
    pause
    exit /b 1
)

echo Found logo: %LOGO%
echo.
echo Copying logo to all icon locations...
echo.

copy /Y "%LOGO%" "%BASE%\mipmap-mdpi\icon.png" >nul 2>&1
if %errorlevel% equ 0 (
    echo   [OK] Copied to mipmap-mdpi\icon.png
) else (
    echo   [FAIL] Failed to copy to mipmap-mdpi\icon.png
)

copy /Y "%LOGO%" "%BASE%\mipmap-hdpi\icon.png" >nul 2>&1
if %errorlevel% equ 0 (
    echo   [OK] Copied to mipmap-hdpi\icon.png
) else (
    echo   [FAIL] Failed to copy to mipmap-hdpi\icon.png
)

copy /Y "%LOGO%" "%BASE%\mipmap-xhdpi\icon.png" >nul 2>&1
if %errorlevel% equ 0 (
    echo   [OK] Copied to mipmap-xhdpi\icon.png
) else (
    echo   [FAIL] Failed to copy to mipmap-xhdpi\icon.png
)

copy /Y "%LOGO%" "%BASE%\mipmap-xxhdpi\icon.png" >nul 2>&1
if %errorlevel% equ 0 (
    echo   [OK] Copied to mipmap-xxhdpi\icon.png
) else (
    echo   [FAIL] Failed to copy to mipmap-xxhdpi\icon.png
)

copy /Y "%LOGO%" "%BASE%\mipmap-xxxhdpi\icon.png" >nul 2>&1
if %errorlevel% equ 0 (
    echo   [OK] Copied to mipmap-xxxhdpi\icon.png
) else (
    echo   [FAIL] Failed to copy to mipmap-xxxhdpi\icon.png
)

echo.
echo ========================================
echo   Done!
echo ========================================
echo.
echo All icons have been updated with your logo.
echo.
echo NEXT STEPS:
echo 1. In Visual Studio: Build -^> Clean Solution
echo 2. Then: Build -^> Rebuild Solution
echo 3. Uninstall the old app from your device/emulator
echo 4. Run the app to see the new icon
echo.
echo NOTE: For best results, use properly sized icons.
echo See CHANGE_APP_ICON_GUIDE.md for details.
echo.
pause
