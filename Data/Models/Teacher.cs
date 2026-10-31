using Google.Cloud.Firestore;
using System;

namespace EscuelaManagement.Data.Models
{
    [FirestoreData]
    public class Teacher
    {
        [FirestoreDocumentId]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [FirestoreProperty]
        public string NombreCompleto { get; set; } = string.Empty;

        [FirestoreProperty]
        public string Direccion { get; set; } = string.Empty;

        [FirestoreProperty]
        public string Telefono { get; set; } = string.Empty;

        [FirestoreProperty]
        public string CorreoElectronico { get; set; } = string.Empty;

        [FirestoreProperty]
        public string EspecialidadCarrera { get; set; } = string.Empty;

        [FirestoreProperty]
        public string FotoBase64 { get; set; } = string.Empty;
        
        [FirestoreProperty]
        public bool Activo { get; set; } = true;
        
        [FirestoreProperty]
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
    }
}