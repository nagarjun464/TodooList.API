# 🔑 Local Firebase Setup Script
# This script sets up Firebase credentials for local development

Write-Host "🔑 Local Firebase Development Setup" -ForegroundColor Green
Write-Host "====================================" -ForegroundColor Green

Write-Host "`n📋 Choose your authentication method for LOCAL development:" -ForegroundColor Yellow
Write-Host "1. Set FIREBASE_SERVICE_ACCOUNT_KEY (Recommended)" -ForegroundColor Cyan
Write-Host "2. Set GOOGLE_APPLICATION_CREDENTIALS (File path)" -ForegroundColor Cyan
Write-Host "3. Use Google Cloud CLI (gcloud auth)" -ForegroundColor Cyan
Write-Host "4. Show current environment variables" -ForegroundColor Cyan
Write-Host "5. Exit" -ForegroundColor Cyan

$choice = Read-Host "`nEnter your choice (1-5)"

switch ($choice) {
    "1" {
        Write-Host "`n🚀 Setting FIREBASE_SERVICE_ACCOUNT_KEY for local development" -ForegroundColor Green
        Write-Host "Paste your Firebase service account JSON key below (press Enter twice when done):" -ForegroundColor Yellow
        
        $jsonLines = @()
        do {
            $line = Read-Host
            if ($line -ne "") {
                $jsonLines += $line
            }
        } while ($line -ne "")
        
        $jsonContent = $jsonLines -join "`n"
        
        try {
            # Validate JSON
            $jsonObject = $jsonContent | ConvertFrom-Json
            $env:FIREBASE_SERVICE_ACCOUNT_KEY = $jsonContent
            
            # Also set it permanently for this session
            [Environment]::SetEnvironmentVariable("FIREBASE_SERVICE_ACCOUNT_KEY", $jsonContent, "User")
            
            Write-Host "✅ FIREBASE_SERVICE_ACCOUNT_KEY set successfully!" -ForegroundColor Green
            Write-Host "Project ID: $($jsonObject.project_id)" -ForegroundColor Cyan
            Write-Host "Service Account: $($jsonObject.client_email)" -ForegroundColor Cyan
            Write-Host "`n💡 This will now work with 'dotnet run' and any new terminal sessions" -ForegroundColor Yellow
        }
        catch {
            Write-Host "❌ Invalid JSON format. Please check your service account key." -ForegroundColor Red
        }
    }
    
    "2" {
        Write-Host "`n📁 Setting GOOGLE_APPLICATION_CREDENTIALS for local development" -ForegroundColor Green
        $filePath = Read-Host "Enter the full path to your credentials JSON file"
        
        if (Test-Path $filePath) {
            $env:GOOGLE_APPLICATION_CREDENTIALS = $filePath
            [Environment]::SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", $filePath, "User")
            Write-Host "✅ GOOGLE_APPLICATION_CREDENTIALS set successfully!" -ForegroundColor Green
            Write-Host "Path: $filePath" -ForegroundColor Cyan
            Write-Host "`n💡 This will now work with 'dotnet run' and any new terminal sessions" -ForegroundColor Yellow
        }
        else {
            Write-Host "❌ File not found: $filePath" -ForegroundColor Red
        }
    }
    
    "3" {
        Write-Host "`n🔧 Setting up Google Cloud CLI authentication" -ForegroundColor Green
        Write-Host "This will use your personal Google account for development" -ForegroundColor Yellow
        
        try {
            # Check if gcloud is installed
            $gcloudVersion = gcloud --version 2>$null
            if ($LASTEXITCODE -eq 0) {
                Write-Host "✅ Google Cloud CLI found. Running authentication..." -ForegroundColor Green
                gcloud auth application-default login
                
                if ($LASTEXITCODE -eq 0) {
                    Write-Host "✅ Google Cloud authentication successful!" -ForegroundColor Green
                    Write-Host "💡 Your app will now use your personal Google account for Firebase access" -ForegroundColor Yellow
                } else {
                    Write-Host "❌ Google Cloud authentication failed" -ForegroundColor Red
                }
            } else {
                Write-Host "❌ Google Cloud CLI not found. Please install it first:" -ForegroundColor Red
                Write-Host "   https://cloud.google.com/sdk/docs/install" -ForegroundColor Yellow
            }
        }
        catch {
            Write-Host "❌ Error with Google Cloud CLI" -ForegroundColor Red
        }
    }
    
    "4" {
        Write-Host "`n🔍 Current Firebase Environment Variables:" -ForegroundColor Green
        Write-Host "FIREBASE_SERVICE_ACCOUNT_KEY: $($env:FIREBASE_SERVICE_ACCOUNT_KEY)" -ForegroundColor Cyan
        Write-Host "GOOGLE_APPLICATION_CREDENTIALS: $($env:GOOGLE_APPLICATION_CREDENTIALS)" -ForegroundColor Cyan
        Write-Host "ASPNETCORE_ENVIRONMENT: $($env:ASPNETCORE_ENVIRONMENT)" -ForegroundColor Cyan
        
        Write-Host "`n🔍 User Environment Variables (persistent):" -ForegroundColor Green
        Write-Host "FIREBASE_SERVICE_ACCOUNT_KEY: $([Environment]::GetEnvironmentVariable('FIREBASE_SERVICE_ACCOUNT_KEY', 'User'))" -ForegroundColor Cyan
        Write-Host "GOOGLE_APPLICATION_CREDENTIALS: $([Environment]::GetEnvironmentVariable('GOOGLE_APPLICATION_CREDENTIALS', 'User'))" -ForegroundColor Cyan
    }
    
    "5" {
        Write-Host "`n👋 Goodbye!" -ForegroundColor Green
        exit
    }
    
    default {
        Write-Host "❌ Invalid choice. Please enter 1-5." -ForegroundColor Red
    }
}

Write-Host "`n💡 Next steps for local development:" -ForegroundColor Yellow
Write-Host "1. Run 'dotnet run' to start your app with real Firebase data" -ForegroundColor White
Write-Host "2. Check the console output for Firebase initialization status" -ForegroundColor White
Write-Host "3. Access Swagger UI at http://localhost:5144" -ForegroundColor White
Write-Host "4. Test your API endpoints with real Firebase data" -ForegroundColor White

Write-Host "`n🔗 For more help, see FIREBASE_AUTH_GUIDE.md" -ForegroundColor Cyan

