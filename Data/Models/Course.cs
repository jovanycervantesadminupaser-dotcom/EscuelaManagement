using Google.Cloud.Firestore;
using System.Collections.Generic;

namespace EscuelaManagement.Data.Models;

[FirestoreData]
public class Course
{
    [FirestoreProperty]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [FirestoreProperty]
    public string? Name { get; set; }

    [FirestoreProperty]
    public string? Schedule { get; set; }

    [FirestoreProperty]
    public double SinglePaymentCost { get; set; }

    [FirestoreProperty]
    public bool AllowInstallments { get; set; }

    // Ahora soportamos múltiples esquemas de pago a plazos por curso
    [FirestoreProperty]
    public List<InstallmentOption> InstallmentOptions { get; set; } = new();
}

[FirestoreData]
public class InstallmentOption
{
    [FirestoreProperty]
    public int InstallmentCount { get; set; }

    [FirestoreProperty]
    public double InstallmentAmount { get; set; }

    [FirestoreProperty]
    public string InstallmentPeriod { get; set; } = "Semanas";
}