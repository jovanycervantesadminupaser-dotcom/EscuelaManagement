using Google.Cloud.Firestore;
using System;

namespace EscuelaManagement.Data.Models;

[FirestoreData]
public class Enrollment
{
    [FirestoreProperty]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [FirestoreProperty]
    public string? StudentId { get; set; }

    [FirestoreProperty]
    public string? CourseId { get; set; }

    [FirestoreProperty]
    public DateTime EnrollmentDate { get; set; }

    // Cambiado a double para compatibilidad nativa con Firestore
    [FirestoreProperty]
    public double DiscountAmount { get; set; }

    [FirestoreProperty]
    public string Turno { get; set; } = "";

    // ==============================================================
    // NUEVAS PROPIEDADES PARA EL CÁLCULO DINÁMICO DE MOROSIDAD
    // ==============================================================

    [FirestoreProperty]
    public string TipoPlazo { get; set; } = ""; // Guardará: "Semanas", "Quincenas", "Meses" o "Unico"

    [FirestoreProperty]
    public int NumeroPlazos { get; set; }       // Guardará la cantidad total de pagos (Ej. 4, 12)

    [FirestoreProperty]
    public double MontoPlazo { get; set; }      // Guardará el costo individual de cada pago
}