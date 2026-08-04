using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace EscuelaManagement.Data.Services
{
    public class UserSessionService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IJSRuntime _jsRuntime;

        public UserSessionService(IHttpContextAccessor httpContextAccessor, IJSRuntime jsRuntime)
        {
            _httpContextAccessor = httpContextAccessor;
            _jsRuntime = jsRuntime;
        }

        // --- PROPIEDADES QUE LEEN DESDE LAS COOKIES (Sobreviven al F5) ---
        public bool IsLoggedIn
        {
            get
            {
                var val = _httpContextAccessor.HttpContext?.Request.Cookies["IsLoggedIn"];
                return bool.TryParse(val, out var result) && result;
            }
        }

        public string Nombre => _httpContextAccessor.HttpContext?.Request.Cookies["UserName"] ?? string.Empty;
        public string Correo => _httpContextAccessor.HttpContext?.Request.Cookies["UserEmail"] ?? string.Empty;
        public string Rol => _httpContextAccessor.HttpContext?.Request.Cookies["UserRol"] ?? string.Empty;

        // --- MÉTODO ASÍNCRONO PARA INICIAR SESIÓN (Usa JS para evitar el error de headers) ---
        public async Task IniciarSesionAsync(string nombre, string correo, string rol)
        {
            string cookieOptions = "path=/; secure; samesite=strict; max-age=604800"; // 7 días de duración

            await _jsRuntime.InvokeVoidAsync("eval", $"document.cookie = 'IsLoggedIn=true; {cookieOptions}';");
            await _jsRuntime.InvokeVoidAsync("eval", $"document.cookie = 'UserName={nombre}; {cookieOptions}';");
            await _jsRuntime.InvokeVoidAsync("eval", $"document.cookie = 'UserEmail={correo}; {cookieOptions}';");
            await _jsRuntime.InvokeVoidAsync("eval", $"document.cookie = 'UserRol={rol}; {cookieOptions}';");
        }

        // --- MÉTODO ASÍNCRONO PARA CERRAR SESIÓN ---
        public async Task CerrarSesionAsync()
        {
            string expiredOptions = "path=/; secure; samesite=strict; max-age=0";

            await _jsRuntime.InvokeVoidAsync("eval", $"document.cookie = 'IsLoggedIn=false; {expiredOptions}';");
            await _jsRuntime.InvokeVoidAsync("eval", $"document.cookie = 'UserName=; {expiredOptions}';");
            await _jsRuntime.InvokeVoidAsync("eval", $"document.cookie = 'UserEmail=; {expiredOptions}';");
            await _jsRuntime.InvokeVoidAsync("eval", $"document.cookie = 'UserRol=; {expiredOptions}';");
        }

        // --- REGLAS DE PODER (Roles) ---
        public bool EsAdministrador => Rol == "Administrador";
        public bool EsControlEscolar => Rol == "Control Escolar" || Rol == "Administrador"; 
        public bool EsDocente => Rol == "Docente" || Rol == "Administrador";
    }
}