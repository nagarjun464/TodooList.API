# 🚀 Google Cloud Run Deployment Guide

This guide shows you how to deploy your TodoList Firebase app to Google Cloud Run.

## 🎯 **Prerequisites**

1. **Google Cloud CLI installed and authenticated:**
   ```bash
   gcloud auth login
   gcloud config set project YOUR_PROJECT_ID
   ```

2. **Enable required APIs:**
   ```bash
   gcloud services enable run.googleapis.com
   gcloud services enable containerregistry.googleapis.com
   ```

## 🐳 **Step 1: Build and Push Docker Image**

### **Option A: Using Cloud Build (Recommended)**
```bash
# Build and push in one command
gcloud builds submit --tag gcr.io/YOUR_PROJECT_ID/todolist-app

# Or build locally and push
docker build -t gcr.io/YOUR_PROJECT_ID/todolist-app .
docker push gcr.io/YOUR_PROJECT_ID/todolist-app
```

### **Option B: Using Artifact Registry**
```bash
# Create repository
gcloud artifacts repositories create todolist-repo \
    --repository-format=docker \
    --location=us-central1

# Build and push
gcloud builds submit --tag us-central1-docker.pkg.dev/YOUR_PROJECT_ID/todolist-repo/todolist-app
```

## ☁️ **Step 2: Deploy to Cloud Run**

### **Deploy with Environment Variables:**
```bash
gcloud run deploy todolist-app \
    --image gcr.io/YOUR_PROJECT_ID/todolist-app \
    --platform managed \
    --region us-central1 \
    --allow-unauthenticated \
    --set-env-vars="FIREBASE_SERVICE_ACCOUNT_KEY=YOUR_JSON_KEY" \
    --set-env-vars="ASPNETCORE_ENVIRONMENT=Production"
```

### **Deploy with Secret Manager (Recommended for Production):**
```bash
# Create secret
echo '{"type":"service_account","project_id":"your-project-id",...}' | \
gcloud secrets create firebase-key --data-file=-

# Deploy with secret
gcloud run deploy todolist-app \
    --image gcr.io/YOUR_PROJECT_ID/todolist-app \
    --platform managed \
    --region us-central1 \
    --allow-unauthenticated \
    --set-secrets="FIREBASE_SERVICE_ACCOUNT_KEY=firebase-key:latest" \
    --set-env-vars="ASPNETCORE_ENVIRONMENT=Production"
```

## 🔐 **Step 3: Set Up Firebase Authentication**

### **Method 1: Environment Variable (Quick Start)**
```bash
# Get your service account key from Firebase Console
# Then deploy with:
gcloud run deploy todolist-app \
    --image gcr.io/YOUR_PROJECT_ID/todolist-app \
    --set-env-vars="FIREBASE_SERVICE_ACCOUNT_KEY=YOUR_JSON_KEY"
```

### **Method 2: Secret Manager (Production)**
```bash
# Create secret
gcloud secrets create firebase-key --data-file=path/to/your/firebase-key.json

# Deploy with secret reference
gcloud run deploy todolist-app \
    --image gcr.io/YOUR_PROJECT_ID/todolist-app \
    --set-secrets="FIREBASE_SERVICE_ACCOUNT_KEY=firebase-key:latest"
```

## 🌐 **Step 4: Access Your App**

After deployment, you'll get a URL like:
```
https://todolist-app-abc123-uc.a.run.app
```

- **Swagger UI:** `https://your-app-url/`
- **API Endpoints:** `https://your-app-url/api/todo`
- **Health Check:** `https://your-app-url/`

## 🔧 **Cloud Run Optimizations**

### **Update Dockerfile for Cloud Run:**
```dockerfile
# Use the official .NET runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0

# Set environment for Cloud Run
ENV ASPNETCORE_URLS=http://0.0.0.0:8080
ENV ASPNETCORE_ENVIRONMENT=Production
ENV PORT=8080

# Copy your app
WORKDIR /app
COPY --from=build /app/out ./

# Expose port 8080 (Cloud Run standard)
EXPOSE 8080

# Health check endpoint
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/ || exit 1

ENTRYPOINT ["dotnet", "TodoListFirebaseApp.dll"]
```

### **Update Program.cs for Cloud Run:**
```csharp
// Add this to your Program.cs
app.MapGet("/", () => Results.Ok(new { 
    service = "todo-api", 
    ok = true, 
    ts = DateTime.UtcNow,
    environment = app.Environment.EnvironmentName
}));
```

## 📊 **Monitoring and Logging**

### **View Logs:**
```bash
gcloud run logs read --service=todolist-app --region=us-central1
```

### **View Metrics:**
```bash
# Open Cloud Console
gcloud console
# Navigate to Cloud Run > todolist-app > Metrics
```

## 🔄 **Continuous Deployment**

### **GitHub Actions Example:**
```yaml
name: Deploy to Cloud Run
on:
  push:
    branches: [main]

jobs:
  deploy:
    runs-on: ubuntu-latest
    steps:
    - uses: actions/checkout@v2
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v1
      with:
        dotnet-version: '9.0.x'
    
    - name: Build and Push
      run: |
        gcloud builds submit --tag gcr.io/${{ secrets.GCP_PROJECT_ID }}/todolist-app
    
    - name: Deploy to Cloud Run
      run: |
        gcloud run deploy todolist-app \
          --image gcr.io/${{ secrets.GCP_PROJECT_ID }}/todolist-app \
          --region us-central1 \
          --platform managed \
          --allow-unauthenticated \
          --set-secrets="FIREBASE_SERVICE_ACCOUNT_KEY=firebase-key:latest"
      env:
        GCP_PROJECT_ID: ${{ secrets.GCP_PROJECT_ID }}
```

## 💰 **Cost Optimization**

- **Min Instances:** 0 (scales to zero when not in use)
- **Max Instances:** 10 (adjust based on your needs)
- **CPU:** 1 (minimum for .NET apps)
- **Memory:** 512Mi (minimum for .NET apps)

## 🚨 **Security Best Practices**

1. **Use Secret Manager** for Firebase credentials
2. **Set up IAM** with least privilege
3. **Enable VPC Connector** if needed
4. **Use Cloud Armor** for DDoS protection

## 🔍 **Troubleshooting**

### **Common Issues:**

1. **Port Binding Error:**
   - Ensure your app listens on `0.0.0.0:8080`
   - Check `ASPNETCORE_URLS` environment variable

2. **Firebase Authentication Error:**
   - Verify service account key is correct
   - Check project ID matches
   - Ensure service account has Firestore permissions

3. **Cold Start Issues:**
   - Use min instances > 0 for critical services
   - Optimize Docker image size

## 📱 **Testing Your Deployment**

```bash
# Test health endpoint
curl https://your-app-url/

# Test API endpoints
curl https://your-app-url/api/todo
curl https://your-app-url/swagger
```

Your app is now running on Google Cloud Run! 🎉
