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

        // --- Nuevos campos de Domicilio ---
        [FirestoreProperty]
        public string Calle { get; set; } = string.Empty;

        [FirestoreProperty]
        public string NumeroExterior { get; set; } = string.Empty;

        [FirestoreProperty]
        public string NumeroInterior { get; set; } = string.Empty;

        [FirestoreProperty]
        public string Colonia { get; set; } = string.Empty;

        [FirestoreProperty]
        public string Municipio { get; set; } = string.Empty;

        [FirestoreProperty]
        public string CodigoPostal { get; set; } = string.Empty;
        // -----------------------------------

        [FirestoreProperty]
        public string Telefono { get; set; } = string.Empty;

        [FirestoreProperty]
        public string CorreoElectronico { get; set; } = string.Empty;

        [FirestoreProperty]
        public string EspecialidadCarrera { get; set; } = string.Empty;

        // --- Nuevo campo ---
        [FirestoreProperty]
        public string UniversidadEgreso { get; set; } = string.Empty;

        [FirestoreProperty]
        public string FotoBase64 { get; set; } = string.Empty;
        
        [FirestoreProperty]
        public bool Activo { get; set; } = true;
        
        [FirestoreProperty]
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        // Propiedad calculada para mantener compatibilidad si tenías código que usaba "Direccion"
        public string DireccionCompleta => $"{Calle} {NumeroExterior}{(string.IsNullOrEmpty(NumeroInterior) ? "" : $" Int {NumeroInterior}")}, {Colonia}, {Municipio}, C.P. {CodigoPostal}".Trim();
    }
}