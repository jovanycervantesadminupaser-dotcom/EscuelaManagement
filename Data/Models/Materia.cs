using Google.Cloud.Firestore;
using System;

namespace EscuelaManagement.Data.Models
{
    [FirestoreData]
    public class Materia
    {
        [FirestoreDocumentId]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [FirestoreProperty]
        public string Nombre { get; set; } = string.Empty;

        [FirestoreProperty]
        public string CursoId { get; set; } = string.Empty;

        [FirestoreProperty]
        public string Profesor { get; set; } = string.Empty;
    }
}