using Google.Cloud.Firestore;
using EscuelaManagement.Data.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.IO;
using System.Text;

namespace EscuelaManagement.Data.Services;

public class FirebaseService
{
    private const string ProjectId = "escuelamanager-d1fba";
    private readonly FirestoreDb _db;

    public FirebaseService()
    {
        string? base64Env = Environment.GetEnvironmentVariable("FIREBASE_CONFIG_BASE64");

        if (!string.IsNullOrEmpty(base64Env))
        {
            // ==========================================
            // --- MODO PRODUCCIÓN (NUBE / RENDER) ---
            // ==========================================
            byte[] data = Convert.FromBase64String(base64Env);
            string jsonCreds = Encoding.UTF8.GetString(data);

            // SOLUCIÓN DEFINITIVA: Guardar en un archivo temporal seguro
            // Esto evita cualquier advertencia de seguridad o métodos obsoletos de Google
            string tempAuthFile = Path.Combine(Path.GetTempPath(), "firebase_auth_render.json");
            File.WriteAllText(tempAuthFile, jsonCreds);

            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", tempAuthFile);
        }
        else
        {
            // ==========================================
            // --- MODO DESARROLLO (TU COMPUTADORA) ---
            // ==========================================
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", "firebase-credentials.json");
        }

        // Construcción nativa recomendada. Google leerá la variable de entorno automáticamente.
        _db = FirestoreDb.Create(ProjectId);
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
        await docRef.SetAsync(student);
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

    public async Task<List<Payment>> GetAllPaymentsAsync()
    {
        try
        {
            QuerySnapshot snapshot = await _db.Collection("payments").GetSnapshotAsync();
            return snapshot.Documents
                .Select(d => d.ConvertTo<Payment>())
                .OrderByDescending(p => p.PaymentDate)
                .ToList();
        }
        catch
        {
            return []; // Inicialización de colección simplificada
        }
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
        return new();
    }

    public async Task SaveConfiguracionAsync(ConfiguracionEscuela config)
    {
        config.Id = "global";
        DocumentReference docRef = _db.Collection("configuracion").Document("global");
        // Agregamos SetOptions.MergeAll para que actualice los botones sin borrar tu Logo u otros datos
        await docRef.SetAsync(config, SetOptions.MergeAll); 
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
        return new();
    }

    // ==========================================
    // --- MÉTODOS PARA CONTROL DE GASTOS (EGRESOS) ---
    // ==========================================
    public async Task<List<Gasto>> GetGastosAsync()
    {
        try
        {
            QuerySnapshot snapshot = await _db.Collection("Gastos").OrderByDescending("Fecha").GetSnapshotAsync();
            return snapshot.Documents.Select(d => d.ConvertTo<Gasto>()).ToList();
        }
        catch
        {
            return []; // Inicialización de colección simplificada
        }
    }

    public async Task AddGastoAsync(Gasto gasto)
    {
        DocumentReference docRef = _db.Collection("Gastos").Document(gasto.Id);

        if (gasto.Fecha.Kind == DateTimeKind.Unspecified || gasto.Fecha.Kind == DateTimeKind.Local)
        {
            gasto.Fecha = gasto.Fecha.ToUniversalTime();
        }

        await docRef.SetAsync(gasto);
    }

    public async Task DeleteGastoAsync(string id)
    {
        DocumentReference docRef = _db.Collection("Gastos").Document(id);
        await docRef.DeleteAsync();
    }

    // ==========================================
    // --- MÉTODOS PARA ASISTENCIAS ---
    // ==========================================
    public async Task<List<Asistencia>> GetAsistenciasByCursoYFechaAsync(string courseId, DateTime fecha)
    {
        try
        {
            var inicioDia = fecha.Date.ToUniversalTime();
            var finDia = fecha.Date.AddDays(1).AddTicks(-1).ToUniversalTime();

            Query query = _db.Collection("asistencias")
                .WhereEqualTo("CourseId", courseId)
                .WhereGreaterThanOrEqualTo("Fecha", inicioDia)
                .WhereLessThanOrEqualTo("Fecha", finDia);

            QuerySnapshot snapshot = await query.GetSnapshotAsync();
            return snapshot.Documents.Select(d => d.ConvertTo<Asistencia>()).ToList();
        }
        catch
        {
            return [];
        }
    }

    public async Task SaveAsistenciasBatchAsync(List<Asistencia> asistencias)
    {
        foreach (var item in asistencias)
        {
            if (string.IsNullOrEmpty(item.Id))
            {
                item.Id = Guid.NewGuid().ToString();
            }
            if (item.Fecha.Kind == DateTimeKind.Unspecified || item.Fecha.Kind == DateTimeKind.Local)
            {
                item.Fecha = item.Fecha.ToUniversalTime();
            }
            DocumentReference docRef = _db.Collection("asistencias").Document(item.Id);
            await docRef.SetAsync(item);
        }
    }
}