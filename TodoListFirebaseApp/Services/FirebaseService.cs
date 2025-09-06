using Google.Cloud.Firestore;
using TodoListFirebaseApp.Models;
using Microsoft.Extensions.Configuration;
using Google.Apis.Auth.OAuth2;

namespace TodoListFirebaseApp.Services
{
    public class FirebaseService
    {
        private readonly FirestoreDb? _firestore;
        private readonly bool _isDevelopmentMode;

        public FirebaseService(IConfiguration configuration)
        {
            _isDevelopmentMode = configuration["ASPNETCORE_ENVIRONMENT"] == "Development";
            
            try
            {
                var projectId = configuration["Firebase:ProjectId"] ?? "todolistfirebaseapp-d4e9a";
                
                // Method 1: Try credentials file first
                var credentialsPath = ".\\todolistfirebaseapp-d4e9a-firebase-adminsdk-fbsvc-91c5c30adc.json";
                if (!string.IsNullOrEmpty(credentialsPath) && File.Exists(credentialsPath))
                {
                    Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialsPath);
                    _firestore = FirestoreDb.Create(projectId);
                    Console.WriteLine("✅ Firebase initialized with credentials file");
                    return;
                }

                // Method 2: Try environment variable for credentials file path
                var envCredentialsPath = Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS");
                if (!string.IsNullOrEmpty(envCredentialsPath) && File.Exists(envCredentialsPath))
                {
                    _firestore = FirestoreDb.Create(projectId);
                    Console.WriteLine("✅ Firebase initialized with environment credentials file");
                    return;
                }

                // Method 3: Try service account key as environment variable (for Docker/Cloud)
                var serviceAccountKey = Environment.GetEnvironmentVariable("FIREBASE_SERVICE_ACCOUNT_KEY");
                if (!string.IsNullOrEmpty(serviceAccountKey))
                {
                    try
                    {
                        var credential = GoogleCredential.FromJson(serviceAccountKey);
                        _firestore = new FirestoreDbBuilder
                        {
                            ProjectId = projectId,
                            Credential = credential
                        }.Build();
                        Console.WriteLine("✅ Firebase initialized with service account key from environment");
                        return;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"⚠️ Failed to parse service account key: {ex.Message}");
                    }
                }

                // Method 4: Try default credentials (for local development with gcloud CLI)
                try
                {
                    _firestore = FirestoreDb.Create(projectId);
                    Console.WriteLine("✅ Firebase initialized with default credentials (gcloud CLI)");
                    return;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Default credentials failed: {ex.Message}");
                }

                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error initializing Firebase: {ex.Message}");
                if (_isDevelopmentMode)
                {
                    Console.WriteLine("🔄 Falling back to mock mode for development");
                    _firestore = null;
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<List<TodoItem>> GetTodosAsync()
        {
            try
            {
                var todos = new List<TodoItem>();
                var snapshot = await _firestore.Collection("todos").GetSnapshotAsync();

                foreach (var doc in snapshot.Documents)
                {
                    TodoItem todo = doc.ConvertTo<TodoItem>();
                    todos.Add(todo);
                }

                return todos;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting todos: {ex.Message}");
                return new List<TodoItem>();
            }
        }

        public async Task<TodoItem?> GetTodoByIdAsync(string id)
        {
            if (_firestore == null)
            {
                if (_isDevelopmentMode)
                {
                    Console.WriteLine("🔄 Development mode: Returning mock todo by id");
                    
                }
                else
                {
                    Console.WriteLine("Warning: Firebase not initialized, returning null");
                    return null;
                }
            }

            try
            {
                var docRef = _firestore.Collection("todos").Document(id);
                var snapshot = await docRef.GetSnapshotAsync();
                return snapshot.Exists ? snapshot.ConvertTo<TodoItem>() : null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting todo by id: {ex.Message}");
                return null;
            }
        }

        internal async Task AddTodoAsync(TodoItem todo)
        {
            if (_firestore == null)
            {
                if (_isDevelopmentMode)
                {
                    Console.WriteLine("🔄 Development mode: Adding to mock storage");
                    todo.Id = todo.Id ?? Guid.NewGuid().ToString();
                    todo.CreatedAt = DateTime.UtcNow;
                    
                    return;
                }
                else
                {
                    Console.WriteLine("Warning: Firebase not initialized, skipping add operation");
                    return;
                }
            }

            try
            {
                Console.WriteLine("Adding to Firebase: " + todo.Title);
                DocumentReference docRef = _firestore.Collection("todos").Document(todo.Id ?? Guid.NewGuid().ToString());
                await docRef.SetAsync(todo);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding todo: {ex.Message}");
            }
        }

        public async Task<bool> DeleteTodoAsync(string id)
        {
            

            try
            {
                var docRef = _firestore.Collection("todos").Document(id);
                var snapshot = await docRef.GetSnapshotAsync();
                if (!snapshot.Exists) return false;

                await docRef.DeleteAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting todo: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateTodoAsync(TodoItem todo)
        {
            if (_firestore == null)
            {
                if (_isDevelopmentMode)
                {
                    Console.WriteLine("🔄 Development mode: Updating in mock storage");
                    
                    return false;
                }
                else
                {
                    Console.WriteLine("Warning: Firebase not initialized, returning false");
                    return false;
                }
            }

            try
            {
                var docRef = _firestore.Collection("todos").Document(todo.Id);
                var snapshot = await docRef.GetSnapshotAsync();
                if (!snapshot.Exists) return false;            
                await docRef.SetAsync(todo, SetOptions.Overwrite);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating todo: {ex.Message}");
                return false;
            }
        }


        // Add AddTodoAsync, UpdateTodoAsync, DeleteTodoAsync similarly
    }
}

