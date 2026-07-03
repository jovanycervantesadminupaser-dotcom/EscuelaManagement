using EscuelaManagement.Components;
using MudBlazor.Services;
using QuestPDF.Infrastructure;
using System.Globalization;

QuestPDF.Settings.License = LicenseType.Community;

var builder = WebApplication.CreateBuilder(args);

// =================================================================
// 1. FORZAR LA CULTURA MEXICANA Y EL SÍMBOLO DE PESOS ($) EN EL SERVIDOR
// =================================================================
var cultureInfo = new CultureInfo("es-MX");
cultureInfo.NumberFormat.CurrencySymbol = "$"; // Forzamos el símbolo clásico

CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
// =================================================================

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddHubOptions(options => 
    {
        // Aumentamos el límite de SignalR a 10MB para permitir la captura de fotos
        options.MaximumReceiveMessageSize = 10 * 1024 * 1024;
    });

builder.Services.AddMudServices();

builder.Services.AddScoped<EscuelaManagement.Data.Services.FirebaseService>();
builder.Services.AddScoped<EscuelaManagement.Data.Services.PdfService>();
builder.Services.AddScoped<EscuelaManagement.Data.Services.UserSessionService>();

var app = builder.Build();

// =================================================================
// 2. APLICAR LA LOCALIZACIÓN AL PIPELINE DE PETICIONES (BLAZOR)
// =================================================================
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture(cultureInfo)
});
// =================================================================

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();