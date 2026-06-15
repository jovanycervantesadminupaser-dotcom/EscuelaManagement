using Google.Cloud.Firestore;
using EscuelaManagement.Data.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace EscuelaManagement.Data.Services;

public class FirebaseService
{
    private readonly FirestoreDb _db;

    public FirebaseService()
    {
        // Nota: Asegúrate de que el archivo de credenciales esté en la raíz del proyecto.
        string path = "firebase-credentials.json.json";
        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
        _db = FirestoreDb.Create("escuelamanager-d1fba");
    }

    // ==========================================
    // --- MÉTODOS PARA ALUMNOS ---
    // ==========================================
    public async Task AddStudentAsync(Student student)
    {
        DocumentReference docRef = _db.Collection("students").Document(student.Id);
        await docRef.SetAsync(student);
    }

    public async Task UpdateStudentAsync(Student student)
    {
        DocumentReference docRef = _db.Collection("students").Document(student.Id);
        await docRef.SetAsync(student); // SetAsync sobrescribe con los nuevos datos
    }

    public async Task<List<Student>> GetAllStudentsAsync()
    {
        QuerySnapshot snapshot = await _db.Collection("students").GetSnapshotAsync();
        return snapshot.Documents.Select(d => d.ConvertTo<Student>()).ToList();
    }

    public async Task DeleteStudentAsync(string id)
    {
        DocumentReference docRef = _db.Collection("students").Document(id);
        await docRef.DeleteAsync();
    }

    // ==========================================
    // --- MÉTODOS PARA CURSOS ---
    // ==========================================
    public async Task AddCourseAsync(Course course)
    {
        if (string.IsNullOrEmpty(course.Id))
        {
            course.Id = Guid.NewGuid().ToString();
        }

        DocumentReference docRef = _db.Collection("courses").Document(course.Id);
        await docRef.SetAsync(course);
    }

    public async Task<List<Course>> GetCoursesAsync()
    {
        QuerySnapshot snapshot = await _db.Collection("courses").GetSnapshotAsync();
        return snapshot.Documents.Select(d => d.ConvertTo<Course>()).ToList();
    }

    public async Task DeleteCourseAsync(string id)
    {
        DocumentReference docRef = _db.Collection("courses").Document(id);
        await docRef.DeleteAsync();
    }

    // ==========================================
    // --- MÉTODOS PARA INSCRIPCIONES ---
    // ==========================================
    public async Task AddEnrollmentAsync(Enrollment enrollment)
    {
        if (string.IsNullOrEmpty(enrollment.Id))
        {
            enrollment.Id = Guid.NewGuid().ToString();
        }

        DocumentReference docRef = _db.Collection("enrollments").Document(enrollment.Id);
        await docRef.SetAsync(enrollment);
    }

    public async Task<List<Enrollment>> GetEnrollmentsByStudentAsync(string studentId)
    {
        Query query = _db.Collection("enrollments").WhereEqualTo("StudentId", studentId);
        QuerySnapshot snapshot = await query.GetSnapshotAsync();
        return snapshot.Documents.Select(d => d.ConvertTo<Enrollment>()).ToList();
    }

    public async Task<List<Enrollment>> GetAllEnrollmentsAsync()
    {
        QuerySnapshot snapshot = await _db.Collection("enrollments").GetSnapshotAsync();
        return snapshot.Documents.Select(d => d.ConvertTo<Enrollment>()).ToList();
    }

    // ==========================================
    // --- MÉTODOS PARA PAGOS ---
    // ==========================================
    public async Task AddPaymentAsync(Payment payment)
    {
        if (string.IsNullOrEmpty(payment.Id))
        {
            payment.Id = Guid.NewGuid().ToString();
        }

        DocumentReference docRef = _db.Collection("payments").Document(payment.Id);
        await docRef.SetAsync(payment);
    }

    public async Task<List<Payment>> GetPaymentsByStudentAsync(string studentId)
    {
        Query query = _db.Collection("payments").WhereEqualTo("StudentId", studentId);
        QuerySnapshot snapshot = await query.GetSnapshotAsync();
        
        return snapshot.Documents
            .Select(d => d.ConvertTo<Payment>())
            .OrderByDescending(p => p.PaymentDate) 
            .ToList();
    }

    // ==========================================
    // --- MÉTODOS PARA MATERIAS ---
    // ==========================================
    public async Task AddMateriaAsync(Materia materia)
    {
        if (string.IsNullOrEmpty(materia.Id))
        {
            materia.Id = Guid.NewGuid().ToString();
        }

        DocumentReference docRef = _db.Collection("materias").Document(materia.Id);
        await docRef.SetAsync(materia);
    }

    public async Task<List<Materia>> GetMateriasAsync()
    {
        QuerySnapshot snapshot = await _db.Collection("materias").GetSnapshotAsync();
        return snapshot.Documents.Select(d => d.ConvertTo<Materia>()).ToList();
    }

    public async Task DeleteMateriaAsync(string id)
    {
        DocumentReference docRef = _db.Collection("materias").Document(id);
        await docRef.DeleteAsync();
    }

    // ==========================================
    // --- MÉTODOS PARA CALIFICACIONES ---
    // ==========================================
    public async Task AddCalificacionAsync(Calificacion calificacion)
    {
        if (string.IsNullOrEmpty(calificacion.Id))
        {
            calificacion.Id = Guid.NewGuid().ToString();
        }

        DocumentReference docRef = _db.Collection("calificaciones").Document(calificacion.Id);
        await docRef.SetAsync(calificacion);
    }

    public async Task<List<Calificacion>> GetCalificacionesByAlumnoAsync(string alumnoId)
    {
        Query query = _db.Collection("calificaciones").WhereEqualTo("AlumnoId", alumnoId);
        QuerySnapshot snapshot = await query.GetSnapshotAsync();
        return snapshot.Documents.Select(d => d.ConvertTo<Calificacion>()).ToList();
    }

    public async Task<List<Calificacion>> GetAllCalificacionesAsync()
    {
        QuerySnapshot snapshot = await _db.Collection("calificaciones").GetSnapshotAsync();
        return snapshot.Documents.Select(d => d.ConvertTo<Calificacion>()).ToList();
    }

    // ==========================================
    // --- MÉTODOS DE CONFIGURACIÓN GLOBAL ---
    // ==========================================
    public async Task<ConfiguracionEscuela> GetConfiguracionAsync()
    {
        DocumentReference docRef = _db.Collection("configuracion").Document("global");
        DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
        
        if (snapshot.Exists)
        {
            return snapshot.ConvertTo<ConfiguracionEscuela>();
        }
        
        // Retorna valores por defecto si aún no se ha configurado nada
        return new ConfiguracionEscuela();
    }

    public async Task SaveConfiguracionAsync(ConfiguracionEscuela config)
    {
        config.Id = "global"; // Forzamos a que siempre sea el mismo documento
        DocumentReference docRef = _db.Collection("configuracion").Document("global");
        await docRef.SetAsync(config);
    }

    public async Task AddUsuarioAsync(Usuario usuario)
    {
        if (string.IsNullOrEmpty(usuario.Id))
        {
            usuario.Id = Guid.NewGuid().ToString();
        }
        DocumentReference docRef = _db.Collection("usuarios").Document(usuario.Id);
        await docRef.SetAsync(usuario);
    }

    public async Task UpdateUsuarioAsync(Usuario usuario)
    {
        DocumentReference docRef = _db.Collection("usuarios").Document(usuario.Id);
        await docRef.SetAsync(usuario);
    }

    public async Task<List<Usuario>> GetUsuariosAsync()
    {
        QuerySnapshot snapshot = await _db.Collection("usuarios").GetSnapshotAsync();
        return snapshot.Documents.Select(d => d.ConvertTo<Usuario>()).ToList();
    }

    public async Task DeleteUsuarioAsync(string id)
    {
        DocumentReference docRef = _db.Collection("usuarios").Document(id);
        await docRef.DeleteAsync();
    }

    public async Task SaveDisenoCredencialAsync(CredencialDiseno diseno)
    {
        DocumentReference docRef = _db.Collection("configuraciones").Document(diseno.Id);
        await docRef.SetAsync(diseno);
    }

    public async Task<CredencialDiseno> GetDisenoCredencialAsync()
    {
        DocumentReference docRef = _db.Collection("configuraciones").Document("config_diseno_credencial");
        DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
        
        if (snapshot.Exists)
        {
            return snapshot.ConvertTo<CredencialDiseno>();
        }
        return new CredencialDiseno(); // Devuelve valores por defecto si no existe
    }
}