using Google.Cloud.Firestore;

namespace EscuelaManagement.Data.Models
{
    [FirestoreData]
    public class Gasto
    {
        [FirestoreProperty]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [FirestoreProperty]
        public string Concepto { get; set; } = "";

        [FirestoreProperty]
        public double Monto { get; set; }

        [FirestoreProperty]
        public DateTime Fecha { get; set; } = DateTime.UtcNow;

        [FirestoreProperty]
        public string Categoria { get; set; } = "Operativo";

        [FirestoreProperty]
        public string Notas { get; set; } = "";
    }
}