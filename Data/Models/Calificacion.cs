using Google.Cloud.Firestore;
using System;

namespace EscuelaManagement.Data.Models
{
    [FirestoreData]
    public class Calificacion
    {
        [FirestoreDocumentId]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [FirestoreProperty]
        public string AlumnoId { get; set; } = string.Empty;

        [FirestoreProperty]
        public string MateriaId { get; set; } = string.Empty;

        [FirestoreProperty]
        public double Valor { get; set; } 

        [FirestoreProperty]
        public string Periodo { get; set; } = string.Empty;
    }
}