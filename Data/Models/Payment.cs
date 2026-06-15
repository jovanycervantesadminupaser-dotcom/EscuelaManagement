using Google.Cloud.Firestore;
using System;

namespace EscuelaManagement.Data.Models;

[FirestoreData]
public class Payment
{
    [FirestoreProperty]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [FirestoreProperty]
    public string? StudentId { get; set; }

    [FirestoreProperty]
    public string? CourseId { get; set; } // Identificador del curso al que se abona

    [FirestoreProperty]
    public string? Concept { get; set; } 

    [FirestoreProperty]
    public double Amount { get; set; } 

    [FirestoreProperty]
    public DateTime PaymentDate { get; set; }
}