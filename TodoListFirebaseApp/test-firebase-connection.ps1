# 🔍 Firebase Connection Test Script
# This script tests your Firebase connection for local development

Write-Host "🔍 Testing Firebase Connection for Local Development" -ForegroundColor Green
Write-Host "=====================================================" -ForegroundColor Green

# Check if Firebase credentials are set
$firebaseKey = $env:FIREBASE_SERVICE_ACCOUNT_KEY
$credentialsPath = $env:GOOGLE_APPLICATION_CREDENTIALS

Write-Host "`n📋 Current Firebase Configuration:" -ForegroundColor Yellow

if (![string]::IsNullOrEmpty($firebaseKey)) {
    Write-Host "✅ FIREBASE_SERVICE_ACCOUNT_KEY is set" -ForegroundColor Green
    try {
        $jsonObject = $firebaseKey | ConvertFrom-Json
        Write-Host "   Project ID: $($jsonObject.project_id)" -ForegroundColor Cyan
        Write-Host "   Service Account: $($jsonObject.client_email)" -ForegroundColor Cyan
        Write-Host "   Private Key ID: $($jsonObject.private_key_id)" -ForegroundColor Cyan
    }
    catch {
        Write-Host "   ⚠️  JSON format appears invalid" -ForegroundColor Yellow
    }
} else {
    Write-Host "❌ FIREBASE_SERVICE_ACCOUNT_KEY is not set" -ForegroundColor Red
}

if (![string]::IsNullOrEmpty($credentialsPath)) {
    if (Test-Path $credentialsPath) {
        Write-Host "✅ GOOGLE_APPLICATION_CREDENTIALS is set and file exists" -ForegroundColor Green
        Write-Host "   Path: $credentialsPath" -ForegroundColor Cyan
    } else {
        Write-Host "⚠️  GOOGLE_APPLICATION_CREDENTIALS is set but file not found" -ForegroundColor Yellow
        Write-Host "   Path: $credentialsPath" -ForegroundColor Cyan
    }
} else {
    Write-Host "❌ GOOGLE_APPLICATION_CREDENTIALS is not set" -ForegroundColor Red
}

# Check if app is running
Write-Host "`n🔍 Testing Application Connection:" -ForegroundColor Yellow

try {
    $response = Invoke-WebRequest -Uri "http://localhost:5144" -Method GET -TimeoutSec 5 -ErrorAction Stop
    if ($response.StatusCode -eq 200) {
        Write-Host "✅ Application is running on http://localhost:5144" -ForegroundColor Green
        Write-Host "   Status: $($response.StatusCode)" -ForegroundColor Cyan
        
        # Try to get todos to test Firebase connection
        try {
            $todosResponse = Invoke-WebRequest -Uri "http://localhost:5144/api/todo" -Method GET -TimeoutSec 10 -ErrorAction Stop
            if ($todosResponse.StatusCode -eq 200) {
                Write-Host "✅ API endpoint /api/todo is responding" -ForegroundColor Green
                $todos = $todosResponse.Content | ConvertFrom-Json
                Write-Host "   Found $($todos.Count) todos in database" -ForegroundColor Cyan
                
                if ($todos.Count -gt 0) {
                    Write-Host "   Sample todo: $($todos[0].Title)" -ForegroundColor Cyan
                }
            }
        }
        catch {
            Write-Host "⚠️  API endpoint /api/todo is not responding" -ForegroundColor Yellow
            Write-Host "   Error: $($_.Exception.Message)" -ForegroundColor Red
        }
    }
} catch {
    Write-Host "❌ Application is not running on http://localhost:5144" -ForegroundColor Red
    Write-Host "   Error: $($_.Exception.Message)" -ForegroundColor Red
}

# Recommendations
Write-Host "`n💡 Recommendations:" -ForegroundColor Yellow

if ([string]::IsNullOrEmpty($firebaseKey) -and [string]::IsNullOrEmpty($credentialsPath)) {
    Write-Host "1. Set up Firebase credentials using:" -ForegroundColor White
    Write-Host "   .\setup-local-firebase.ps1" -ForegroundColor Cyan
    Write-Host "   OR" -ForegroundColor White
    Write-Host "   .\setup-local-firebase.bat" -ForegroundColor Cyan
} else {
    Write-Host "1. Your Firebase credentials are configured!" -ForegroundColor Green
}

Write-Host "2. Start your application:" -ForegroundColor White
Write-Host "   dotnet run" -ForegroundColor Cyan

Write-Host "3. Test in browser:" -ForegroundColor White
Write-Host "   http://localhost:5144" -ForegroundColor Cyan

Write-Host "4. Check console output for Firebase initialization status" -ForegroundColor White

Write-Host "`n🔗 For more help, see FIREBASE_AUTH_GUIDE.md" -ForegroundColor Cyan

