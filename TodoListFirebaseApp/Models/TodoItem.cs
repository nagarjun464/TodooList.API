using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Google.Cloud.Firestore;

namespace TodoListFirebaseApp.Models
{
    [FirestoreData]
    public class TodoItem
    {
        [FirestoreProperty]
        public string? Id { get; set; }

        [FirestoreProperty]
        [Required]
        public string Title { get; set; }

        [FirestoreProperty]
        public string? Description { get; set; }

        [FirestoreProperty]
        public bool IsCompleted { get; set; }

        [FirestoreProperty]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Important: UtcNow

        [FirestoreProperty]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    }
}
