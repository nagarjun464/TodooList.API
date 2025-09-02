using Google.Cloud.Firestore;
using TodoListFirebaseApp.Models;

namespace TodoListFirebaseApp.Services
{
    public class FirebaseService
    {
        private readonly FirestoreDb _firestore;

        public FirebaseService()
        {
            string path = "C:\\Users\\smart\\source\\repos\\TodoListFirebaseApp\\todolistfirebaseapp-d4e9a-firebase-adminsdk-fbsvc-bf9834d081.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
            _firestore = FirestoreDb.Create("todolistfirebaseapp-d4e9a");
        }

        public async Task<List<TodoItem>> GetTodosAsync()
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

        public async Task<TodoItem?> GetTodoByIdAsync(string id)
        {
            var docRef = _firestore.Collection("todos").Document(id);
            var snapshot = await docRef.GetSnapshotAsync();
            return snapshot.Exists ? snapshot.ConvertTo<TodoItem>() : null;
        }


        internal async Task AddTodoAsync(TodoItem todo)
        {
            Console.WriteLine("Adding to Firebase: " + todo.Title);
            DocumentReference docRef = _firestore.Collection("todos").Document(todo.Id ?? Guid.NewGuid().ToString());
            await docRef.SetAsync(todo);
        }

        public async Task<bool> DeleteTodoAsync(string id)
        {
            var docRef = _firestore.Collection("todos").Document(id);
            var snapshot = await docRef.GetSnapshotAsync();
            if (!snapshot.Exists) return false;

            await docRef.DeleteAsync();
            return true;
        }

        public async Task<bool> UpdateTodoAsync(TodoItem todo)
        {
            var docRef = _firestore.Collection("todos").Document(todo.Id);
            var snapshot = await docRef.GetSnapshotAsync();
            if (!snapshot.Exists) return false;            
            await docRef.SetAsync(todo, SetOptions.Overwrite);
            return true;
        }


        // Add AddTodoAsync, UpdateTodoAsync, DeleteTodoAsync similarly
    }
}
