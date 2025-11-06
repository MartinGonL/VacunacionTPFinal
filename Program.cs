using Microsoft.AspNetCore.Authentication.Cookies;
using VacunacionTPFinal.Models; // ¡Importante! Aquí están todos nuestros reposDitorios e interfaces

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// -----------------------------------------------------------------
// 1. CONFIGURACIÓN DE INYECCIÓN DE DEPENDENCIAS (REPOSITORIOS)
// -----------------------------------------------------------------
// Le decimos a la app que use AddScoped (una instancia por solicitud HTTP)
// Cuando un controlador pida "IRepositorioEscuela", le daremos "RepositorioEscuela".
builder.Services.AddScoped<IRepositorioEscuela, RepositorioEscuela>();
builder.Services.AddScoped<IRepositorioAlumno, RepositorioAlumno>();
builder.Services.AddScoped<IRepositorioAgenteSanitario, RepositorioAgenteSanitario>();
builder.Services.AddScoped<IRepositorioVacuna, RepositorioVacuna>();
builder.Services.AddScoped<IRepositorioUsuario, RepositorioUsuario>();
builder.Services.AddScoped<IRepositorioRegistroVacunacion, RepositorioRegistroVacunacion>();

// -----------------------------------------------------------------
// 2. CONFIGURACIÓN DE SEGURIDAD (LOGIN / ROLES)
// -----------------------------------------------------------------
// Agregamos servicios de autenticación y autorización
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Usuarios/Login";      // Ruta a la acción de Login
        options.LogoutPath = "/Usuarios/Logout";     // Ruta a la acción de Logout
        options.AccessDeniedPath = "/Home/AccesoDenegado"; // Ruta si no tiene permisos
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Tiempo de la sesión
    });

builder.Services.AddAuthorization(options =>
{
    // Aquí es donde defines tus roles (los nombres deben coincidir con lo que guardes en la DB)
    options.AddPolicy("Administrador", policy => policy.RequireRole("Administrador"));
    options.AddPolicy("Agente", policy => policy.RequireRole("Agente", "Administrador")); // Un admin también es agente
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// -----------------------------------------------------------------
// 3. HABILITAR AUTENTICACIÓN Y AUTORIZACIÓN
// -----------------------------------------------------------------
// ¡El orden es MUY importante! Debe ir después de UseRouting.
app.UseAuthentication();
app.UseAuthorization();
// -----------------------------------------------------------------


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();