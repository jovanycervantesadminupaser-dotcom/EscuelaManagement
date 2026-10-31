using Google.Cloud.Firestore;
using EscuelaManagement.Data.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.IO;
using System.Text;
using Microsoft.Extensions.Caching.Memory;

namespace EscuelaManagement.Data.Services;

public class FirebaseService
{
    private const string ProjectId = "escuelamanager-d1fba";
    private readonly FirestoreDb _db;
    private readonly IMemoryCache _cache; // Inyección del Cache

    // Tiempos de vida en caché configurables
    private readonly TimeSpan _cacheDurationConfig = TimeSpan.FromHours(12);
    private readonly TimeSpan _cacheDurationCourses = TimeSpan.FromMinutes(10);
    private readonly TimeSpan _cacheDurationMaterias = TimeSpan.FromMinutes(10);

    // Modificamos el constructor para inyectar IMemoryCache
    public FirebaseService(IMemoryCache cache)
    {
        _cache = cache;
        string? base64Env = Environment.GetEnvironmentVariable("FIREBASE_CONFIG_BASE64");

        if (!string.IsNullOrEmpty(base64Env))
        {
            // ==========================================
            // --- MODO PRODUCCIÓN (NUBE / RENDER) ---
            // ==========================================
            byte[] data = Convert.FromBase64String(base64Env);
            string jsonCreds = Encoding.UTF8.GetString(data);

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
    // --- MÉTODOS PARA CURSOS (Caché Aplicado) ---
    // ==========================================
    public async Task AddCourseAsync(Course course)
    {
        if (string.IsNullOrEmpty(course.Id))
        {
            course.Id = Guid.NewGuid().ToString();
        }

        DocumentReference docRef = _db.Collection("courses").Document(course.Id);
        await docRef.SetAsync(course);
        
        // Limpiamos caché al insertar para mantener datos frescos
        _cache.Remove("Global_Courses"); 
    }

    public async Task<List<Course>> GetCoursesAsync()
    {
        // 1. Buscamos en caché primero
        if (_cache.TryGetValue("Global_Courses", out List<Course>? cachedCourses))
        {
            return cachedCourses ?? new List<Course>();
        }

        // 2. Si no existe, buscamos en Firestore
        QuerySnapshot snapshot = await _db.Collection("courses").GetSnapshotAsync();
        var courses = snapshot.Documents.Select(d => d.ConvertTo<Course>()).ToList();

        // 3. Guardamos en caché
        _cache.Set("Global_Courses", courses, _cacheDurationCourses);
        return courses;
    }

    public async Task DeleteCourseAsync(string id)
    {
        DocumentReference docRef = _db.Collection("courses").Document(id);
        await docRef.DeleteAsync();
        _cache.Remove("Global_Courses"); 
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
            return new List<Payment>(); 
        }
    }

    public async Task DeletePaymentAsync(string paymentId)
    {
        DocumentReference docRef = _db.Collection("payments").Document(paymentId);
        await docRef.DeleteAsync();
    }

    // ==========================================
    // --- MÉTODOS PARA MATERIAS (Caché Aplicado) ---
    // ==========================================
    public async Task AddMateriaAsync(Materia materia)
    {
        if (string.IsNullOrEmpty(materia.Id))
        {
            materia.Id = Guid.NewGuid().ToString();
        }

        DocumentReference docRef = _db.Collection("materias").Document(materia.Id);
        await docRef.SetAsync(materia);
        _cache.Remove("Global_Materias");
    }

    public async Task<List<Materia>> GetMateriasAsync()
    {
        if (_cache.TryGetValue("Global_Materias", out List<Materia>? cachedMaterias))
        {
            return cachedMaterias ?? new List<Materia>();
        }

        QuerySnapshot snapshot = await _db.Collection("materias").GetSnapshotAsync();
        var materias = snapshot.Documents.Select(d => d.ConvertTo<Materia>()).ToList();
        
        _cache.Set("Global_Materias", materias, _cacheDurationMaterias);
        return materias;
    }

    public async Task DeleteMateriaAsync(string id)
    {
        DocumentReference docRef = _db.Collection("materias").Document(id);
        await docRef.DeleteAsync();
        _cache.Remove("Global_Materias");
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
    // --- MÉTODOS DE CONFIGURACIÓN GLOBAL (Caché Alto Nivel) ---
    // ==========================================
    public async Task<ConfiguracionEscuela> GetConfiguracionAsync()
    {
        // Esta es la consulta que más recursos consumía (logo en base64).
        if (_cache.TryGetValue("Global_Config", out ConfiguracionEscuela? cachedConfig))
        {
            return cachedConfig ?? new ConfiguracionEscuela();
        }

        DocumentReference docRef = _db.Collection("configuracion").Document("global");
        DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

        ConfiguracionEscuela config = snapshot.Exists ? snapshot.ConvertTo<ConfiguracionEscuela>() : new ConfiguracionEscuela();
        
        _cache.Set("Global_Config", config, _cacheDurationConfig); // 12 horas en memoria
        return config;
    }

    public async Task SaveConfiguracionAsync(ConfiguracionEscuela config)
    {
        config.Id = "global";
        DocumentReference docRef = _db.Collection("configuracion").Document("global");
        await docRef.SetAsync(config, SetOptions.MergeAll); 
        
        // Eliminamos caché para que se actualice la vista al instante
        _cache.Remove("Global_Config");
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
        _cache.Remove("Global_CredencialDiseno");
    }

    public async Task<CredencialDiseno> GetDisenoCredencialAsync()
    {
        if (_cache.TryGetValue("Global_CredencialDiseno", out CredencialDiseno? cachedDiseno))
        {
            return cachedDiseno ?? new CredencialDiseno();
        }

        DocumentReference docRef = _db.Collection("configuraciones").Document("config_diseno_credencial");
        DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

        var diseno = snapshot.Exists ? snapshot.ConvertTo<CredencialDiseno>() : new CredencialDiseno();
        _cache.Set("Global_CredencialDiseno", diseno, _cacheDurationConfig);
        
        return diseno;
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
            return new List<Gasto>(); 
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
            return new List<Asistencia>();
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

    // ==========================================
    // --- MÉTODOS PARA MAESTROS / DOCENTES ---
    // ==========================================
    public async Task AddTeacherAsync(Teacher teacher)
    {
        if (string.IsNullOrEmpty(teacher.Id))
        {
            teacher.Id = Guid.NewGuid().ToString();
        }
        
        // Si la fecha no es UTC, la convertimos para evitar errores de Firestore
        if (teacher.FechaRegistro.Kind == DateTimeKind.Unspecified || teacher.FechaRegistro.Kind == DateTimeKind.Local)
        {
            teacher.FechaRegistro = teacher.FechaRegistro.ToUniversalTime();
        }

        DocumentReference docRef = _db.Collection("teachers").Document(teacher.Id);
        await docRef.SetAsync(teacher);
    }

    public async Task UpdateTeacherAsync(Teacher teacher)
    {
        DocumentReference docRef = _db.Collection("teachers").Document(teacher.Id);
        await docRef.SetAsync(teacher);
    }

    public async Task<List<Teacher>> GetAllTeachersAsync()
    {
        try
        {
            QuerySnapshot snapshot = await _db.Collection("teachers").GetSnapshotAsync();
            return snapshot.Documents.Select(d => d.ConvertTo<Teacher>()).OrderBy(t => t.NombreCompleto).ToList();
        }
        catch
        {
            return new List<Teacher>();
        }
    }

    public async Task DeleteTeacherAsync(string id)
    {
        DocumentReference docRef = _db.Collection("teachers").Document(id);
        await docRef.DeleteAsync();
    }
}