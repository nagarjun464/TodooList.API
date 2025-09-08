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
                

                _firestore = FirestoreDb.Create(configuration["Firebase:ProjectId"]);
                var projectId = configuration["Firebase:ProjectId"] ?? "todolistfirebaseapp-d4e9a";

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

                    var base64 = "ewogICJ0eXBlIjogInNlcnZpY2VfYWNjb3VudCIsCiAgInByb2plY3RfaWQiOiAidG9kb2xpc3RmaXJlYmFzZWFwcC1kNGU5YSIsCiAgInByaXZhdGVfa2V5X2lkIjogImExNWFmMGIyNDI3YWMyMjkyZTE5ZTE5ODQ2YzJiNTA5NmU3NjMyOTIiLAogICJwcml2YXRlX2tleSI6ICItLS0tLUJFR0lOIFBSSVZBVEUgS0VZLS0tLS1cbk1JSUV2Z0lCQURBTkJna3Foa2lHOXcwQkFRRUZBQVNDQktnd2dnU2tBZ0VBQW9JQkFRREZXbnZybU44d095TjhcbnEvclkvSzc3U0RrUzVvRE03b0V3VjVCaDhUd21OVlhsay9uYkxmM0VYT0dlYW9GbDdpSVpKVHBWb25uMHA3dFpcbkpLYmRnWG85dW1iY0Y5VHFTbjZuUTdxTkZFUXd1RFp5b0Jlckp0blM0Ymx5RUdIb0t6UE50Rm5ETVhUVjNYN1FcbnVhNTNicmsxczM3cFZiZkdzQkpRb2FXS1NvMlF4d1BZaVdwbmdoVTM1RDVPdEp1czhoTy9Xd3RRbEhlZUNTdnlcbmJWWTZjeG9WTFdGRUQxbEZGQ2FyS0luckFYMVVERCtuamNTZXFFeHlGb3BaSndPYXROMi9DQWU0cksvUU9oV3VcbkFZU2RpQ0RJNXNxTzNjQWZEZE5MTW9NRFM4eExPTE1qSFpqcXFQQWFZVWxyUTd3RE4xeUNJOERWRW83SUV6MGtcbjRRaUZ0WjNiQWdNQkFBRUNnZ0VBRExyNi9tWVVrNVJsQkNCRjVLUmNGenAvNy8rdGRFcTRTNm1DVjBjTDViMFFcbkJ1aW1OK1MrRjJJVWJiY2FGTHlwTjIwRzE4cE15cUxkTzNGWlVEV1BjSmJyRnF4MDRBNGhJR0lHa1I0ckRlZDhcbkg2WERFUmZHK2tyMFB1N2VtZTdiZnh2d3BGNDIxcXJjaU5OSmRVdlBwNjZsODZCRFpBNHNpRVg5R3pBVzlLMDhcbnZTTi9FU3JZd2poQVcrN2kvTllaSldSNlRGLzRTVzRnMVBTL0YweFMwSjJnZ2duYVVDbVZYZERIS0dlVjBhcU9cbjVzbmM5ViswVVlsZ3VRNEp0L1JpdnlvR3I4QVNScVV3ZElYaHVkS05aazBqZmpxUjdmSkY2V3FCd283MDNJODlcbml3WGxVLzJSWU1NQ3dLQ0V3Wk5XcjNsYmJPN1lFZGI0N3RpeU4yQTU4UUtCZ1FEdkF1TVdoMm5VOGJneFhBdTJcbmY1WmJSRStpN2pUVEpVem1MZktYWG54MkJrNnQ4U2g0OVYwUng3djBTbFRhdnh2Y0ZNc1BIT0VZR3BMdWJ6ZkRcblp5SzVqMXMxdzBJTVJKMy9Zd0pna0xRVncwME1WeXIwVnB3UkdqU0pjWTZBTm9VUXZWanlCR0VzTEpLekFPTU5cblM4dFNqa2NrcFlTQjh0UTdJMExKRHlnR2F3S0JnUURUWVpTREFob2gvZjBsTUhqd2xCWU5XYXUzeUM2YzJQNkJcbk9uQUdPTDEvNFM4Y2VCZHpkaFZsU0Uwd0dEb3J3WVA5WlA2ZE5hNElKdGgyWDR4djgwRVRPa0YrYjNPSitrY1hcblA3NDllS0dXS0dyNTM0WjFwQ2k4bTlrT0wwTXc2NnM5dk5ucWhGNG1FZTl3TWtsWVkyemU4R0NSUktSczVNNXlcbnZmZ3JhaXhDVVFLQmdRQzZzRENjaWt6WjdheUhzWFBjcXEvUXZmek9NTzNGOXg2bnRFQUdoK0VEZDAzQmc5cXZcbkVFZ1ArVWtRT1hjcUhaZ3ZRWGFTYVdaUW80RWRDSFl3Qk1FVTI2ck14YVV6VysxbGEvcVJYcldyUWc5T2trK1VcbjVvbWt1aWxMS2cyNmk5dElCZjRSbStZaTBQWU5KNll4M29BRURlbjM3K2tlYjRQaVpWcG5IWldQM1FLQmdRQzJcbkFzenRMYTh3MjlIUTV2Y01PSWJ3Z2VqeGU4WmZPaDJUU3ExUlBpUTE2OEUwSTFnV0RIU0ozSUxTU2RvMCtSRmNcbjdFeXRNNTZoMkVrRlRHclJyc1MyK0srU1hnYy9wS2o0Sk9QN2JGYSt4Qml4QmI1eXA2S0lIUzMrSDhZY1pxRHhcbnJSNTY1L1ZsbElYMnR1WWUzS1hQSlc0UlIrakk5dytwRDFpVWtYeUFzUUtCZ0VObkN2ckhRcVRrOVpFMU9aVGpcbnRPa3RQUC9JTnhEK3B1RVdjdzVVYjk1VzNIQ2daOFp1Q2M4NzJkOTdSbFBNSkJKNTJDdGFMUU5pU3k2ckJYeEhcbnN4cThRajFudzlQUmU2aTl0L0h6UHlXOHFCME1kOE82Qk91YTlMUEdoUTZyL3JzZTdwcjIvWTlKd3pLN0cwME5cbjdzQUQ4TStwT1c3QnBlNzM0OVo3b2ZUaVxuLS0tLS1FTkQgUFJJVkFURSBLRVktLS0tLVxuIiwKICAiY2xpZW50X2VtYWlsIjogImZpcmViYXNlLWFkbWluc2RrLWZic3ZjQHRvZG9saXN0ZmlyZWJhc2VhcHAtZDRlOWEuaWFtLmdzZXJ2aWNlYWNjb3VudC5jb20iLAogICJjbGllbnRfaWQiOiAiMTA5MDUzNDgzOTQ4MzEyOTk2MjYwIiwKICAiYXV0aF91cmkiOiAiaHR0cHM6Ly9hY2NvdW50cy5nb29nbGUuY29tL28vb2F1dGgyL2F1dGgiLAogICJ0b2tlbl91cmkiOiAiaHR0cHM6Ly9vYXV0aDIuZ29vZ2xlYXBpcy5jb20vdG9rZW4iLAogICJhdXRoX3Byb3ZpZGVyX3g1MDlfY2VydF91cmwiOiAiaHR0cHM6Ly93d3cuZ29vZ2xlYXBpcy5jb20vb2F1dGgyL3YxL2NlcnRzIiwKICAiY2xpZW50X3g1MDlfY2VydF91cmwiOiAiaHR0cHM6Ly93d3cuZ29vZ2xlYXBpcy5jb20vcm9ib3QvdjEvbWV0YWRhdGEveDUwOS9maXJlYmFzZS1hZG1pbnNkay1mYnN2YyU0MHRvZG9saXN0ZmlyZWJhc2VhcHAtZDRlOWEuaWFtLmdzZXJ2aWNlYWNjb3VudC5jb20iLAogICJ1bml2ZXJzZV9kb21haW4iOiAiZ29vZ2xlYXBpcy5jb20iCn0K";
                    var json = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(base64));
                    
                    Console.WriteLine("Decoded JSON (first 200 chars): " + json.Substring(0, Math.Min(200, json.Length)));

                    try
                    {
                        var credential = GoogleCredential.FromJson(json);

                        if (FirebaseApp.DefaultInstance == null)
                        {
                            FirebaseApp.Create(new AppOptions
                            {
                                Credential = credential,
                                ProjectId = projectId
                            });
                        }

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

