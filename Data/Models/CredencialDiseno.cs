using Google.Cloud.Firestore;

namespace EscuelaManagement.Data.Models
{
    [FirestoreData]
    public class CredencialDiseno
    {
        [FirestoreDocumentId]
        public string Id { get; set; } = "config_diseno_credencial";

        // ==========================================
        // --- COORDENADAS DEL FRENTE (ANVERSO) ---
        // ==========================================
        
        [FirestoreProperty] public float FotoX { get; set; } = 3.0f;
        [FirestoreProperty] public float FotoY { get; set; } = 15.0f;
        [FirestoreProperty] public float FotoW { get; set; } = 22.0f;
        [FirestoreProperty] public float FotoH { get; set; } = 26.0f;

        [FirestoreProperty] public float NombreX { get; set; } = 29.0f;
        [FirestoreProperty] public float NombreY { get; set; } = 18.0f;
        [FirestoreProperty] public int NombreSize { get; set; } = 8;

        [FirestoreProperty] public float MatriculaX { get; set; } = 29.0f;
        [FirestoreProperty] public float MatriculaY { get; set; } = 35.0f;
        [FirestoreProperty] public int MatriculaSize { get; set; } = 9;

        [FirestoreProperty] public float CursoX { get; set; } = 55.0f;
        [FirestoreProperty] public float CursoY { get; set; } = 35.0f;
        [FirestoreProperty] public int CursoSize { get; set; } = 7;

        [FirestoreProperty] public float VigenciaX { get; set; } = 28.0f;
        [FirestoreProperty] public float VigenciaY { get; set; } = 48.0f;
        [FirestoreProperty] public int VigenciaSize { get; set; } = 6;

        // ==========================================
        // --- COORDENADAS DEL REVERSO (ATRÁS) ---
        // ==========================================
        
        [FirestoreProperty] public float FirmaLineaX { get; set; } = 12.0f;
        [FirestoreProperty] public float FirmaLineaY { get; set; } = 30.0f;
        [FirestoreProperty] public float FirmaLineaW { get; set; } = 61.0f;

        [FirestoreProperty] public float DirectorX { get; set; } = 12.0f;
        [FirestoreProperty] public float DirectorY { get; set; } = 32.0f;
        [FirestoreProperty] public int DirectorSize { get; set; } = 6;

        // ==========================================
        // --- IMÁGENES DE PLANTILLAS EN BASE64 ---
        // ==========================================
        
        [FirestoreProperty] public string PlantillaFrenteBase64 { get; set; } = string.Empty;
        [FirestoreProperty] public string PlantillaReversoBase64 { get; set; } = string.Empty;
    }
}