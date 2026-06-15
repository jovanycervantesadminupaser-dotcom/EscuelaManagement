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
}