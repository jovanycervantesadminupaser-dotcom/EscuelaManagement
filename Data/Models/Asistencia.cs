using Google.Cloud.Firestore;

namespace EscuelaManagement.Data.Models
{
    [FirestoreData]
    public class Asistencia
    {
        [FirestoreProperty]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [FirestoreProperty]
        public string StudentId { get; set; } = "";

        [FirestoreProperty]
        public string StudentName { get; set; } = "";

        [FirestoreProperty]
        public string CourseId { get; set; } = "";

        [FirestoreProperty]
        public DateTime Fecha { get; set; } = DateTime.UtcNow.Date;

        [FirestoreProperty]
        public string Estado { get; set; } = "Presente"; // Presente, Ausente, Retardo, Justificado
    }
}