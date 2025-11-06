using Microsoft.AspNetCore.Authentication.Cookies;
using VacunacionTPFinal.Models; 

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// -----------------------------------------------------------------
// CONFIGURACIÓN DE INYECCIÓN DE DEPENDENCIAS 
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
    
    options.AddPolicy("Administrador", policy => policy.RequireRole("Administrador"));
    options.AddPolicy("Agente", policy => policy.RequireRole("Agente", "Administrador")); 
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// -----------------------------------------------------------------
app.UseAuthentication();
app.UseAuthorization();
// -----------------------------------------------------------------


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();