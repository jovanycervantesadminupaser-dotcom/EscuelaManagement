using Google.Cloud.Firestore;

namespace EscuelaManagement.Data.Models
{
    [FirestoreData]
    public class ConfiguracionEscuela
    {
        [FirestoreDocumentId]
        public string Id { get; set; } = "global";

        [FirestoreProperty]
        public string NombreEscuela { get; set; } = "EscuelaManager";

        [FirestoreProperty]
        public string LogoBase64 { get; set; } = string.Empty;

        [FirestoreProperty]
        public string BackgroundLoginBase64 { get; set; } = "";
    }
}