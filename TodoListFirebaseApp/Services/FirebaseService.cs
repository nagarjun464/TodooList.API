using Google.Cloud.Firestore;
using TodoListFirebaseApp.Models;
using Microsoft.Extensions.Configuration;
using Google.Apis.Auth.OAuth2;

namespace TodoListFirebaseApp.Services
{
    public class FirebaseService
    {
        private readonly FirestoreDb? _firestore;
        private readonly List<TodoItem> _mockTodos = new List<TodoItem>
        {
            new TodoItem { Id = "1", Title = "Mock Todo 1", IsCompleted = false, CreatedAt = DateTime.UtcNow },
            new TodoItem { Id = "2", Title = "Mock Todo 2", IsCompleted = true, CreatedAt = DateTime.UtcNow.AddDays(-1) },
            new TodoItem { Id = "3", Title = "Mock Todo 3", IsCompleted = false, CreatedAt = DateTime.UtcNow.AddHours(-2) }
        };
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

                // Method 5: Development mode fallback
                if (_isDevelopmentMode)
                {
                    Console.WriteLine("🔄 Development mode: Running with mock Firebase service");
                    Console.WriteLine("💡 To use real Firebase, set one of these:");
                    Console.WriteLine("   - FIREBASE_SERVICE_ACCOUNT_KEY environment variable");
                    Console.WriteLine("   - GOOGLE_APPLICATION_CREDENTIALS environment variable");
                    Console.WriteLine("   - Run 'gcloud auth application-default login' locally");
                    _firestore = null;
                }
                else
                {
                    throw new InvalidOperationException(
                        "Firebase credentials not found. Please set one of:\n" +
                        "- FIREBASE_SERVICE_ACCOUNT_KEY environment variable\n" +
                        "- GOOGLE_APPLICATION_CREDENTIALS environment variable\n" +
                        "- Run 'gcloud auth application-default login' locally"
                    );
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
            if (_firestore == null)
            {
                if (_isDevelopmentMode)
                {
                    Console.WriteLine("🔄 Development mode: Returning mock todos");
                    return _mockTodos.ToList();
                }
                else
                {
                    Console.WriteLine("Warning: Firebase not initialized, returning empty list");
                    return new List<TodoItem>();
                }
            }

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
                if (_isDevelopmentMode)
                {
                    Console.WriteLine("🔄 Falling back to mock data due to Firebase error");
                    return _mockTodos.ToList();
                }
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
                    return _mockTodos.FirstOrDefault(t => t.Id == id);
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
                    _mockTodos.Add(todo);
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
            if (_firestore == null)
            {
                if (_isDevelopmentMode)
                {
                    Console.WriteLine("🔄 Development mode: Deleting from mock storage");
                    var todo = _mockTodos.FirstOrDefault(t => t.Id == id);
                    if (todo != null)
                    {
                        _mockTodos.Remove(todo);
                        return true;
                    }
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
                    var existingTodo = _mockTodos.FirstOrDefault(t => t.Id == todo.Id);
                    if (existingTodo != null)
                    {
                        existingTodo.Title = todo.Title;
                        existingTodo.IsCompleted = todo.IsCompleted;
                        existingTodo.CreatedAt = todo.CreatedAt;
                        return true;
                    }
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

