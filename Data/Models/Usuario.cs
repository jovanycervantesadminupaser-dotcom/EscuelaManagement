using Google.Cloud.Firestore;
using System;

namespace EscuelaManagement.Data.Models
{
    [FirestoreData]
    public class Usuario
    {
        [FirestoreDocumentId]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [FirestoreProperty]
        public string Correo { get; set; } = string.Empty;

        [FirestoreProperty]
        public string Nombre { get; set; } = string.Empty;

        [FirestoreProperty]
        public string Rol { get; set; } = string.Empty;

        [FirestoreProperty]
        public string Password { get; set; } = string.Empty;
        
        [FirestoreProperty]
        public bool Activo { get; set; } = true;
    }
}