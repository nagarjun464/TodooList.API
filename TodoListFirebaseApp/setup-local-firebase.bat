@echo off
echo 🔑 Local Firebase Development Setup
echo ====================================
echo.

echo 📋 Choose your authentication method for LOCAL development:
echo 1. Set FIREBASE_SERVICE_ACCOUNT_KEY (Recommended)
echo 2. Set GOOGLE_APPLICATION_CREDENTIALS (File path)
echo 3. Show current environment variables
echo 4. Exit
echo.

set /p choice="Enter your choice (1-4): "

if "%choice%"=="1" goto set_key
if "%choice%"=="2" goto set_file
if "%choice%"=="3" goto show_vars
if "%choice%"=="4" goto exit
goto invalid

:set_key
echo.
echo 🚀 Setting FIREBASE_SERVICE_ACCOUNT_KEY for local development
echo Paste your Firebase service account JSON key below:
echo (Copy the entire JSON content from your firebase-key.json file)
echo.
set /p json_key="Enter JSON key: "
setx FIREBASE_SERVICE_ACCOUNT_KEY "%json_key%"
set FIREBASE_SERVICE_ACCOUNT_KEY=%json_key%
echo.
echo ✅ FIREBASE_SERVICE_ACCOUNT_KEY set successfully!
echo 💡 This will now work with 'dotnet run' and any new terminal sessions
goto next_steps

:set_file
echo.
echo 📁 Setting GOOGLE_APPLICATION_CREDENTIALS for local development
set /p file_path="Enter the full path to your credentials JSON file: "
if exist "%file_path%" (
    setx GOOGLE_APPLICATION_CREDENTIALS "%file_path%"
    set GOOGLE_APPLICATION_CREDENTIALS=%file_path%
    echo.
    echo ✅ GOOGLE_APPLICATION_CREDENTIALS set successfully!
    echo Path: %file_path%
    echo 💡 This will now work with 'dotnet run' and any new terminal sessions
) else (
    echo ❌ File not found: %file_path%
)
goto next_steps

:show_vars
echo.
echo 🔍 Current Firebase Environment Variables:
echo FIREBASE_SERVICE_ACCOUNT_KEY: %FIREBASE_SERVICE_ACCOUNT_KEY%
echo GOOGLE_APPLICATION_CREDENTIALS: %GOOGLE_APPLICATION_CREDENTIALS%
echo ASPNETCORE_ENVIRONMENT: %ASPNETCORE_ENVIRONMENT%
goto next_steps

:invalid
echo ❌ Invalid choice. Please enter 1-4.
goto next_steps

:next_steps
echo.
echo 💡 Next steps for local development:
echo 1. Run 'dotnet run' to start your app with real Firebase data
echo 2. Check the console output for Firebase initialization status
echo 3. Access Swagger UI at http://localhost:5144
echo 4. Test your API endpoints with real Firebase data
echo.
echo 🔗 For more help, see FIREBASE_AUTH_GUIDE.md
pause

:exit
echo 👋 Goodbye!

