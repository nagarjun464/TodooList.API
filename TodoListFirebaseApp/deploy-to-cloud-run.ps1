# 🚀 Cloud Run Deployment Script
# This script automates deployment to Google Cloud Run

param(
    [Parameter(Mandatory=$true)]
    [string]$ProjectId,
    
    [Parameter(Mandatory=$false)]
    [string]$Region = "us-central1",
    
    [Parameter(Mandatory=$false)]
    [string]$ServiceName = "todolist-app",
    
    [Parameter(Mandatory=$false)]
    [string]$ImageTag = "latest"
)

Write-Host "🚀 Deploying to Google Cloud Run" -ForegroundColor Green
Write-Host "=================================" -ForegroundColor Green

# Check if gcloud is installed
try {
    $gcloudVersion = gcloud --version 2>$null
    if ($LASTEXITCODE -ne 0) {
        throw "gcloud not found"
    }
    Write-Host "✅ Google Cloud CLI found" -ForegroundColor Green
} catch {
    Write-Host "❌ Google Cloud CLI not found. Please install it first:" -ForegroundColor Red
    Write-Host "   https://cloud.google.com/sdk/docs/install" -ForegroundColor Yellow
    exit 1
}

# Check if user is authenticated
try {
    $authInfo = gcloud auth list --filter=status:ACTIVE --format="value(account)" 2>$null
    if ([string]::IsNullOrEmpty($authInfo)) {
        Write-Host "❌ Not authenticated with Google Cloud. Please run:" -ForegroundColor Red
        Write-Host "   gcloud auth login" -ForegroundColor Yellow
        exit 1
    }
    Write-Host "✅ Authenticated as: $authInfo" -ForegroundColor Green
} catch {
    Write-Host "❌ Authentication check failed" -ForegroundColor Red
    exit 1
}

# Set project
Write-Host "🔧 Setting project to: $ProjectId" -ForegroundColor Cyan
gcloud config set project $ProjectId

# Enable required APIs
Write-Host "🔌 Enabling required APIs..." -ForegroundColor Cyan
gcloud services enable run.googleapis.com
gcloud services enable containerregistry.googleapis.com

# Build and push Docker image
Write-Host "🐳 Building and pushing Docker image..." -ForegroundColor Cyan
$imageName = "gcr.io/$ProjectId/$ServiceName"
gcloud builds submit --tag $imageName

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Docker build failed" -ForegroundColor Red
    exit 1
}

Write-Host "✅ Docker image built and pushed successfully" -ForegroundColor Green

# Check if Firebase credentials are set
$firebaseKey = $env:FIREBASE_SERVICE_ACCOUNT_KEY
if ([string]::IsNullOrEmpty($firebaseKey)) {
    Write-Host "⚠️  FIREBASE_SERVICE_ACCOUNT_KEY not set. Using mock mode." -ForegroundColor Yellow
    Write-Host "💡 To use real Firebase, set the environment variable first." -ForegroundColor Cyan
    
    # Deploy without Firebase credentials (will use mock mode)
    Write-Host "🚀 Deploying to Cloud Run (mock mode)..." -ForegroundColor Cyan
    gcloud run deploy $ServiceName `
        --image $imageName `
        --platform managed `
        --region $Region `
        --allow-unauthenticated `
        --set-env-vars="ASPNETCORE_ENVIRONMENT=Production" `
        --port=8080 `
        --memory=512Mi `
        --cpu=1 `
        --min-instances=0 `
        --max-instances=10
} else {
    Write-Host "✅ Firebase credentials found. Deploying with real Firebase..." -ForegroundColor Green
    
    # Deploy with Firebase credentials
    Write-Host "🚀 Deploying to Cloud Run (with Firebase)..." -ForegroundColor Cyan
    gcloud run deploy $ServiceName `
        --image $imageName `
        --platform managed `
        --region $Region `
        --allow-unauthenticated `
        --set-env-vars="FIREBASE_SERVICE_ACCOUNT_KEY=$firebaseKey" `
        --set-env-vars="ASPNETCORE_ENVIRONMENT=Production" `
        --port=8080 `
        --memory=512Mi `
        --cpu=1 `
        --min-instances=0 `
        --max-instances=10
}

if ($LASTEXITCODE -eq 0) {
    Write-Host "🎉 Deployment successful!" -ForegroundColor Green
    
    # Get the service URL
    $serviceUrl = gcloud run services describe $ServiceName --region=$Region --format="value(status.url)"
    Write-Host "🌐 Your app is available at: $serviceUrl" -ForegroundColor Cyan
    
    Write-Host "`n📱 Test your deployment:" -ForegroundColor Yellow
    Write-Host "   Health Check: $serviceUrl" -ForegroundColor White
    Write-Host "   Swagger UI: $serviceUrl" -ForegroundColor White
    Write-Host "   API: $serviceUrl/api/todo" -ForegroundColor White
    
    Write-Host "`n📊 Monitor your app:" -ForegroundColor Yellow
    Write-Host "   gcloud run logs read --service=$ServiceName --region=$Region" -ForegroundColor White
    Write-Host "   gcloud console" -ForegroundColor White
} else {
    Write-Host "❌ Deployment failed" -ForegroundColor Red
    exit 1
}

Write-Host "`n🔗 For more help, see CLOUD_RUN_DEPLOYMENT.md" -ForegroundColor Cyan
