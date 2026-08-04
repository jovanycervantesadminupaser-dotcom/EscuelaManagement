using Microsoft.AspNetCore.Http;
using System;

namespace EscuelaManagement.Data.Services
{
    public class UserSessionService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserSessionService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        // --- PROPIEDADES CONECTADAS A COOKIES (Sobreviven al F5) ---
        public bool IsLoggedIn
        {
            get
            {
                var val = _httpContextAccessor.HttpContext?.Request.Cookies["IsLoggedIn"];
                return bool.TryParse(val, out var result) && result;
            }
        }

        public string Nombre
        {
            get => _httpContextAccessor.HttpContext?.Request.Cookies["UserName"] ?? string.Empty;
        }

        public string Correo
        {
            get => _httpContextAccessor.HttpContext?.Request.Cookies["UserEmail"] ?? string.Empty;
        }

        public string Rol
        {
            get => _httpContextAccessor.HttpContext?.Request.Cookies["UserRol"] ?? string.Empty;
        }

        // --- MÉTODO PARA INICIAR SESIÓN Y GUARDAR COOKIES ---
        public void IniciarSesion(string nombre, string correo, string rol)
        {
            var options = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7) // La sesión dura 7 días activa
            };

            var context = _httpContextAccessor.HttpContext;
            if (context != null)
            {
                context.Response.Cookies.Append("IsLoggedIn", "true", options);
                context.Response.Cookies.Append("UserName", nombre ?? string.Empty, options);
                context.Response.Cookies.Append("UserEmail", correo ?? string.Empty, options);
                context.Response.Cookies.Append("UserRol", rol ?? string.Empty, options);
            }
        }

        // --- MÉTODO PARA CERRAR SESIÓN Y BORRAR COOKIES ---
        public void CerrarSesion()
        {
            var options = new CookieOptions
            {
                Expires = DateTime.UtcNow.AddDays(-1),
                Secure = true,
                HttpOnly = true
            };

            var context = _httpContextAccessor.HttpContext;
            if (context != null)
            {
                context.Response.Cookies.Append("IsLoggedIn", "false", options);
                context.Response.Cookies.Append("UserName", string.Empty, options);
                context.Response.Cookies.Append("UserEmail", string.Empty, options);
                context.Response.Cookies.Append("UserRol", string.Empty, options);
            }
        }

        // --- REGLAS DE PODER (Roles) ---
        public bool EsAdministrador => Rol == "Administrador";
        public bool EsControlEscolar => Rol == "Control Escolar" || Rol == "Administrador"; 
        public bool EsDocente => Rol == "Docente" || Rol == "Administrador";
    }
}