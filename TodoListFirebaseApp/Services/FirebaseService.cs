using Google.Cloud.Firestore;
using TodoListFirebaseApp.Models;
using Microsoft.Extensions.Configuration;
using Google.Apis.Auth.OAuth2;
using FirebaseAdmin;
using System.Net;

namespace TodoListFirebaseApp.Services
{
    public class FirebaseService
    {
        private readonly FirestoreDb? _firestore;
        private readonly bool _isDevelopmentMode;

        public string? Credential { get; private set; }


        public FirebaseService(IConfiguration configuration)
        {
            _isDevelopmentMode = configuration["ASPNETCORE_ENVIRONMENT"] == "Development";
            
            try
            {
                

                //_firestore = FirestoreDb.Create(configuration["Firebase:ProjectId"]);
                Console.WriteLine("######Initial########");
                var projectId = configuration["Firebase:ProjectId"] ?? "todolistfirebaseapp-d4e9a";
                Console.WriteLine("######projectId########");

                try
                {
                    //var credential = GoogleCredential.GetApplicationDefault();

                    //string json = "{"
                    //+ "\"type\": \"service_account\","
                    //+ "\"project_id\": \"todolistfirebaseapp-d4e9a\","
                    //+ "\"private_key_id\": \"a2e2880012da9b1d33860a32df49ffce050ec0f7\","
                    //+ "\"private_key\": \"-----BEGIN PRIVATE KEY-----\\nMIIEvgIBADANBgkqhki...\\n-----END PRIVATE KEY-----\\n\","
                    //+ "\"client_email\": \"firebase-adminsdk-fbsvc@todolistfirebaseapp-d4e9a.iam.gserviceaccount.com\","
                    //+ "\"client_id\": \"109053483948312996260\","
                    //+ "\"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\","
                    //+ "\"token_uri\": \"https://oauth2.googleapis.com/token\","
                    //+ "\"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\","
                    //+ "\"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/firebase-adminsdk-fbsvc%40todolistfirebaseapp-d4e9a.iam.gserviceaccount.com\","
                    //+ "\"universe_domain\": \"googleapis.com\""
                    //+ "}";
                    //var json = File.ReadAllText("firebase-key.json");
                    //var credential = GoogleCredential.FromJson(json);

                    //if (FirebaseApp.DefaultInstance == null)
                    //{
                    //    FirebaseApp.Create(new AppOptions
                    //    {
                    //        Credential = credential
                    //    });
                    //}

                    // 🔹 Get secret JSON from environment
                    //var keyJson = Environment.GetEnvironmentVariable("FIREBASE_SERVICE_ACCOUNT_KEY");

                    var keyJson = Environment.GetEnvironmentVariable("FIREBASE_SERVICE_ACCOUNT_KEY");
                    Console.WriteLine("######FIREBASE_SERVICE_ACCOUNT_KEY########");
                    Console.WriteLine(keyJson);
                    Console.WriteLine("######FIREBASE_SERVICE_ACCOUNT_KEY########");
                    if (string.IsNullOrEmpty(keyJson))
                        throw new InvalidOperationException("Firebase service account not found in environment");

                    try
                    {
                        var credential = GoogleCredential.FromJson(keyJson);
                        Console.WriteLine("######Credentail########"+ credential);

                        if (FirebaseApp.DefaultInstance == null)
                        {
                            FirebaseApp.Create(new AppOptions
                            {
                                Credential = credential,
                                ProjectId = projectId
                            });
                        }
                        Console.WriteLine("######Firebase_Instance_Saved########");
                        // Firestore – IMPORTANT: pass the credential explicitly (don’t use ADC)
                        _firestore = new FirestoreDbBuilder
                        {
                            ProjectId = projectId,
                            Credential = credential
                        }.Build();
                        Console.WriteLine("######FirestoreDbBuilder########");
                        _firestore = FirestoreDb.Create(projectId);
                        Console.WriteLine("✅ Firebase initialized successfully");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"❌ Error creating Firebase app: {ex.Message}");
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Error initializing Firebase: {ex.Message}");
                    throw;
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

