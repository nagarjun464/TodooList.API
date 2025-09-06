# 🔑 Firebase Environment Variable Setup Script
# This script helps you set up Firebase authentication without credentials files

Write-Host "🔑 Firebase Authentication Setup" -ForegroundColor Green
Write-Host "================================" -ForegroundColor Green

Write-Host "`n📋 Choose your authentication method:" -ForegroundColor Yellow
Write-Host "1. Set FIREBASE_SERVICE_ACCOUNT_KEY (Recommended for Docker/Cloud)" -ForegroundColor Cyan
Write-Host "2. Set GOOGLE_APPLICATION_CREDENTIALS (File path)" -ForegroundColor Cyan
Write-Host "3. Show current environment variables" -ForegroundColor Cyan
Write-Host "4. Exit" -ForegroundColor Cyan

$choice = Read-Host "`nEnter your choice (1-4)"

switch ($choice) {
    "1" {
        Write-Host "`n🚀 Setting FIREBASE_SERVICE_ACCOUNT_KEY" -ForegroundColor Green
        Write-Host "Paste your service account JSON key below (press Enter twice when done):" -ForegroundColor Yellow
        
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
            Write-Host "✅ FIREBASE_SERVICE_ACCOUNT_KEY set successfully!" -ForegroundColor Green
            Write-Host "Project ID: $($jsonObject.project_id)" -ForegroundColor Cyan
        }
        catch {
            Write-Host "❌ Invalid JSON format. Please check your service account key." -ForegroundColor Red
        }
    }
    
    "2" {
        Write-Host "`n📁 Setting GOOGLE_APPLICATION_CREDENTIALS" -ForegroundColor Green
        $filePath = Read-Host "Enter the full path to your credentials JSON file"
        
        if (Test-Path $filePath) {
            $env:GOOGLE_APPLICATION_CREDENTIALS = $filePath
            Write-Host "✅ GOOGLE_APPLICATION_CREDENTIALS set successfully!" -ForegroundColor Green
            Write-Host "Path: $filePath" -ForegroundColor Cyan
        }
        else {
            Write-Host "❌ File not found: $filePath" -ForegroundColor Red
        }
    }
    
    "3" {
        Write-Host "`n🔍 Current Firebase Environment Variables:" -ForegroundColor Green
        Write-Host "FIREBASE_SERVICE_ACCOUNT_KEY: $($env:FIREBASE_SERVICE_ACCOUNT_KEY)" -ForegroundColor Cyan
        Write-Host "GOOGLE_APPLICATION_CREDENTIALS: $($env:GOOGLE_APPLICATION_CREDENTIALS)" -ForegroundColor Cyan
        Write-Host "ASPNETCORE_ENVIRONMENT: $($env:ASPNETCORE_ENVIRONMENT)" -ForegroundColor Cyan
    }
    
    "4" {
        Write-Host "`n👋 Goodbye!" -ForegroundColor Green
        exit
    }
    
    default {
        Write-Host "❌ Invalid choice. Please enter 1-4." -ForegroundColor Red
    }
}

Write-Host "`n💡 Next steps:" -ForegroundColor Yellow
Write-Host "1. Run 'dotnet run' to test your Firebase connection" -ForegroundColor White
Write-Host "2. Check the console output for Firebase initialization status" -ForegroundColor White
Write-Host "3. Access Swagger UI at http://localhost:5144" -ForegroundColor White

Write-Host "`n🔗 For more help, see FIREBASE_AUTH_GUIDE.md" -ForegroundColor Cyan
