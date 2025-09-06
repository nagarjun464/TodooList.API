# 🔑 Firebase Authentication Alternatives

This guide shows you different ways to authenticate with Firebase without using a credentials file.

## 🚀 **Method 1: Environment Variable (Recommended for Docker/Cloud)**

Set the service account key as an environment variable:

### **Windows PowerShell:**
```powershell
$env:FIREBASE_SERVICE_ACCOUNT_KEY='{"type":"service_account","project_id":"your-project-id",...}'
```

### **Windows Command Prompt:**
```cmd
set FIREBASE_SERVICE_ACCOUNT_KEY={"type":"service_account","project_id":"your-project-id",...}
```

### **Linux/macOS:**
```bash
export FIREBASE_SERVICE_ACCOUNT_KEY='{"type":"service_account","project_id":"your-project-id",...}'
```

### **Docker:**
```bash
docker run -e FIREBASE_SERVICE_ACCOUNT_KEY='{"type":"service_account",...}' your-app
```

## 🔧 **Method 2: Google Cloud CLI (Local Development)**

Install Google Cloud CLI and authenticate:

### **Install gcloud CLI:**
- **Windows:** Download from https://cloud.google.com/sdk/docs/install
- **macOS:** `brew install google-cloud-sdk`
- **Linux:** Follow official docs

### **Authenticate:**
```bash
gcloud auth application-default login
```

This will use your personal Google account for development.

## 📁 **Method 3: Environment Variable for File Path**

Set the path to your credentials file:

### **Windows:**
```powershell
$env:GOOGLE_APPLICATION_CREDENTIALS="C:\path\to\your\credentials.json"
```

### **Linux/macOS:**
```bash
export GOOGLE_APPLICATION_CREDENTIALS="/path/to/your/credentials.json"
```

## 🐳 **Method 4: Docker Environment Variables**

Create a `.env` file:
```env
FIREBASE_SERVICE_ACCOUNT_KEY={"type":"service_account","project_id":"your-project-id",...}
```

Or pass directly:
```bash
docker run -e FIREBASE_SERVICE_ACCOUNT_KEY='{"type":"service_account",...}' your-app
```

## 🌐 **Method 5: Cloud Platform Integration**

### **Google Cloud Run:**
Set environment variables in the service configuration.

### **Kubernetes:**
```yaml
env:
- name: FIREBASE_SERVICE_ACCOUNT_KEY
  valueFrom:
    secretKeyRef:
      name: firebase-secret
      key: service-account-key
```

### **Azure App Service:**
Set in Application Settings.

## 📋 **How to Get Your Service Account Key:**

1. Go to [Firebase Console](https://console.firebase.google.com/)
2. Select your project
3. Go to Project Settings → Service Accounts
4. Click "Generate New Private Key"
5. Download the JSON file
6. Copy the entire content to use as environment variable

## 🧪 **Testing Your Setup:**

1. **Set the environment variable**
2. **Run your application:**
   ```bash
   dotnet run
   ```
3. **Check the console output** - you should see:
   ```
   ✅ Firebase initialized with service account key from environment
   ```

## 🚨 **Security Notes:**

- **Never commit credentials to source control**
- **Use environment variables in production**
- **Rotate keys regularly**
- **Use least-privilege service accounts**

## 🔍 **Troubleshooting:**

### **"Invalid JSON" Error:**
- Make sure the entire JSON is on one line
- Escape quotes properly in your shell
- Use single quotes around the entire value

### **"Permission Denied" Error:**
- Check if the service account has Firestore permissions
- Verify the project ID is correct

### **"Project Not Found" Error:**
- Verify your Firebase project ID
- Check if the service account belongs to the correct project

## 💡 **Best Practices:**

1. **Development:** Use `gcloud auth application-default login`
2. **Docker:** Use `FIREBASE_SERVICE_ACCOUNT_KEY` environment variable
3. **Production:** Use cloud platform secrets management
4. **CI/CD:** Use encrypted environment variables

## 🎯 **Quick Start for Local Development:**

```bash
# Install Google Cloud CLI
gcloud auth application-default login

# Run your app
dotnet run
```

Your app will now work with real Firebase data!
