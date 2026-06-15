namespace EscuelaManagement.Data.Services
{
    public class UserSessionService
    {
        public bool IsLoggedIn { get; private set; } = false;
        public string Nombre { get; private set; } = string.Empty;
        public string Correo { get; private set; } = string.Empty;
        public string Rol { get; private set; } = string.Empty;

        public void IniciarSesion(string nombre, string correo, string rol)
        {
            Nombre = nombre;
            Correo = correo;
            Rol = rol;
            IsLoggedIn = true;
        }

        public void CerrarSesion()
        {
            Nombre = string.Empty;
            Correo = string.Empty;
            Rol = string.Empty;
            IsLoggedIn = false;
        }

        // --- REGLAS DE PODER (Roles) ---
        // El Administrador siempre tiene acceso a todo.
        public bool EsAdministrador => Rol == "Administrador";
        public bool EsControlEscolar => Rol == "Control Escolar" || Rol == "Administrador"; 
        public bool EsDocente => Rol == "Docente" || Rol == "Administrador";
    }
}