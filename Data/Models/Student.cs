using Google.Cloud.Firestore;

namespace EscuelaManagement.Data.Models;

[FirestoreData]
public class Student
{
    [FirestoreProperty]
    public string? PhotoBase64 { get; set; }
    
    [FirestoreProperty]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [FirestoreProperty]
    public string? Name { get; set; }
    
    [FirestoreProperty]
    public string? PaternalLastName { get; set; }
    
    [FirestoreProperty]
    public string? MaternalLastName { get; set; }
    
    [FirestoreProperty]
    public Timestamp? DateOfBirth { get; set; } // Asegúrate de usar Timestamp aquí

    [FirestoreProperty]
    public string? Phone { get; set; }

    [FirestoreProperty]
    public GuardianInfo Guardian { get; set; } = new();
    
    [FirestoreProperty]
    public AddressInfo Address { get; set; } = new();
}

[FirestoreData]
public class GuardianInfo
{
    [FirestoreProperty]
    public string? Name { get; set; }
    [FirestoreProperty]
    public string? PaternalLastName { get; set; }
    [FirestoreProperty]
    public string? MaternalLastName { get; set; }
    [FirestoreProperty]
    public int Age { get; set; }
    [FirestoreProperty]
    public string? Relationship { get; set; }
    [FirestoreProperty]
    public string? Phone { get; set; }
}

[FirestoreData]
public class AddressInfo
{
    [FirestoreProperty]
    public string? Street { get; set; }
    [FirestoreProperty]
    public string? ExtNumber { get; set; }
    [FirestoreProperty]
    public string? IntNumber { get; set; }
    [FirestoreProperty]
    public string? Neighborhood { get; set; }
    [FirestoreProperty]
    public string? Municipality { get; set; }
    [FirestoreProperty]
    public string? State { get; set; }
    [FirestoreProperty]
    public string? ZipCode { get; set; }
}