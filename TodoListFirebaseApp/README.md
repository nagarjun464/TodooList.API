# TodoList Firebase App

A .NET Web API application for managing todo items with Firebase integration.

## 🔑 **Firebase Authentication**

This application supports multiple authentication methods for Firebase:

### **🚀 Quick Start (No Credentials File Needed):**

1. **Use the setup script:**
   ```powershell
   .\set-firebase-env.ps1
   ```

2. **Or set environment variable manually:**
   ```powershell
   $env:FIREBASE_SERVICE_ACCOUNT_KEY='{"type":"service_account","project_id":"your-project-id",...}'
   ```

3. **Run the application:**
   ```bash
   dotnet run
   ```

### **🔧 Alternative Authentication Methods:**

- **Environment Variables** - Set `FIREBASE_SERVICE_ACCOUNT_KEY` with your JSON key
- **Google Cloud CLI** - Run `gcloud auth application-default login`
- **Credentials File** - Set `GOOGLE_APPLICATION_CREDENTIALS` path
- **Mock Mode** - Automatically falls back to mock data in development

📖 **See [FIREBASE_AUTH_GUIDE.md](FIREBASE_AUTH_GUIDE.md) for detailed instructions**

## 💻 **Local Development with Real Firebase Data**

### **🚀 Quick Setup for Local Development:**

1. **PowerShell (Recommended):**
   ```powershell
   .\setup-local-firebase.ps1
   ```

2. **Command Prompt:**
   ```cmd
   .\setup-local-firebase.bat
   ```

3. **Manual Setup:**
   ```powershell
   # Set Firebase credentials for local development
   $env:FIREBASE_SERVICE_ACCOUNT_KEY='{"type":"service_account","project_id":"todolistfirebaseapp-d4e9a",...}'
   
   # Run your app with real Firebase data
   dotnet run
   ```

### **🔍 What You'll Get:**

- ✅ **Real Firebase Data** - Connect to your actual Firestore database
- ✅ **Live CRUD Operations** - Create, read, update, delete real todos
- ✅ **Swagger UI** - Test real API endpoints with actual data
- ✅ **Development Mode** - Full debugging and logging capabilities
- ✅ **No Mock Data** - Work with your production Firebase data locally

### **📱 Test Your Local Firebase Connection:**

1. **Start the app:**
   ```bash
   dotnet run
   ```

2. **Check console output** - You should see:
   ```
   ✅ Firebase initialized with service account key from environment
   ```

3. **Access Swagger UI:** `http://localhost:5144`

4. **Test API endpoints:**
   - GET `/api/todo` - Get real todos from Firebase
   - POST `/api/todo` - Create real todos in Firebase
   - PUT `/api/todo/{id}` - Update real todos in Firebase
   - DELETE `/api/todo/{id}` - Delete real todos from Firebase

## 🚀 **Swagger Setup**

This application includes Swagger/OpenAPI documentation that is automatically generated from your API controllers.

### **How to Access Swagger UI**

1. **Run the application:**
   ```bash
   dotnet run
   ```

2. **Access Swagger UI:**
   - **Development Environment:** Navigate to `http://localhost:5144` (Swagger UI is set as the root page)
   - **Alternative:** Navigate to `http://localhost:5144/swagger` for the traditional Swagger path

3. **API Documentation:**
   - Swagger will automatically document all your API endpoints
   - You can test API calls directly from the Swagger UI
   - View request/response schemas and examples

### **Available API Endpoints**

The following endpoints are documented in Swagger:

- **GET** `/api/todo` - Get all todos
- **GET** `/api/todo/{Title}` - Get todo by title
- **POST** `/api/todo` - Create new todo
- **PUT** `/api/todo/{id}` - Update existing todo
- **DELETE** `/api/todo/{id}` - Delete todo

### **Environment**

- Swagger is enabled in all environments
- Firebase authentication is flexible and supports multiple methods
- Mock data available in development mode when Firebase is not configured

## ☁️ **Google Cloud Run Deployment**

### **🚀 Quick Deploy:**

```powershell
# Deploy to Cloud Run (replace YOUR_PROJECT_ID)
.\deploy-to-cloud-run.ps1 -ProjectId "YOUR_PROJECT_ID"
```

### **🔧 Manual Deployment:**

```bash
# Build and push
gcloud builds submit --tag gcr.io/YOUR_PROJECT_ID/todolist-app

# Deploy to Cloud Run
gcloud run deploy todolist-app \
    --image gcr.io/YOUR_PROJECT_ID/todolist-app \
    --platform managed \
    --region us-central1 \
    --allow-unauthenticated \
    --set-env-vars="FIREBASE_SERVICE_ACCOUNT_KEY=YOUR_JSON_KEY"
```

### **📚 Cloud Run Features:**

- ✅ **Auto-scaling** - Scales to zero when not in use
- ✅ **HTTPS by default** - Automatic SSL certificates
- ✅ **Global CDN** - Fast response times worldwide
- ✅ **Pay-per-use** - Only pay for actual usage
- ✅ **Health checks** - Built-in monitoring
- ✅ **Secret management** - Secure credential storage

📖 **See [CLOUD_RUN_DEPLOYMENT.md](CLOUD_RUN_DEPLOYMENT.md) for complete deployment guide**

## 🐳 **Docker Support**

```bash
# Build the image
docker build -t todolist-app .

# Run with Firebase credentials
docker run -e FIREBASE_SERVICE_ACCOUNT_KEY='{"type":"service_account",...}' -p 8080:80 todolist-app

# Or use a .env file
docker run --env-file .env -p 8080:80 todolist-app
```

## 🛠️ **Development**

- **Framework:** .NET 9.0
- **Database:** Firebase Firestore (with mock fallback)
- **Documentation:** Swagger/OpenAPI 3.0
- **Authentication:** Multiple Firebase auth methods supported
- **Deployment:** Cloud Run, Docker, Local
- **Local Development:** Full Firebase integration with real data

## 📚 **Documentation**

- **[FIREBASE_AUTH_GUIDE.md](FIREBASE_AUTH_GUIDE.md)** - Complete Firebase authentication guide
- **[CLOUD_RUN_DEPLOYMENT.md](CLOUD_RUN_DEPLOYMENT.md)** - Google Cloud Run deployment guide
- **[set-firebase-env.ps1](set-firebase-env.ps1)** - PowerShell setup script for environment variables
- **[setup-local-firebase.ps1](setup-local-firebase.ps1)** - Local development Firebase setup (PowerShell)
- **[setup-local-firebase.bat](setup-local-firebase.bat)** - Local development Firebase setup (Command Prompt)
- **[deploy-to-cloud-run.ps1](deploy-to-cloud-run.ps1)** - Automated Cloud Run deployment script
