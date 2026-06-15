using EscuelaManagement.Components;
using MudBlazor.Services;
using QuestPDF.Infrastructure;

QuestPDF.Settings.License = LicenseType.Community;

var builder = WebApplication.CreateBuilder(args);

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