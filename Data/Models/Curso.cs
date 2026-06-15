using Google.Cloud.Firestore;
namespace EscuelaManagement.Models
{
    public class Curso
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Nombre { get; set; } = string.Empty; // Ej: "1° A - Primaria"
        public string CicloEscolar { get; set; } = string.Empty; // Ej: "2025-2026"

        // Opcional: Lista para saber qué materias pertenecen a este curso
        public List<string> MateriasIds { get; set; } = new();
    }
}