using Microsoft.AspNetCore.Authentication.Cookies;
using NegocioPDF.Repositories; // Importar el repositorio de usuarios

var builder = WebApplication.CreateBuilder(args);

// Obtener la cadena de conexión desde appsettings.json
string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Verificar que la cadena de conexión no sea nula o vacía
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("La cadena de conexión 'DefaultConnection' no se encontró o es nula.");
}

// Registrar UsuarioRepository y DetalleSuscripcionRepository como servicios utilizando la cadena de conexión
builder.Services.AddSingleton<UsuarioRepository>(provider => new UsuarioRepository(connectionString));
builder.Services.AddSingleton<DetalleSuscripcionRepository>(provider => new DetalleSuscripcionRepository(connectionString));
builder.Services.AddSingleton<OperacionesPDFRepository>(provider => new OperacionesPDFRepository(connectionString)); // <--- Agregar esta línea
// Configurar la autenticación con cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login"; // Ruta de inicio de sesión
        options.LogoutPath = "/Auth/Logout"; // Ruta para cerrar sesión
        options.AccessDeniedPath = "/Auth/AccessDenied"; // Ruta de acceso denegado
    });

// Agregar servicios MVC a la aplicación
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configurar el pipeline de manejo de solicitudes HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

// Configuración para servir archivos estáticos como imágenes, CSS, etc.
app.UseStaticFiles();

app.UseRouting();

// Añadir el middleware de autenticación y autorización
app.UseAuthentication(); // Agregar autenticación
app.UseAuthorization();  // Agregar autorización

// Configurar la ruta principal para redirigir a Auth/Login
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

// Ejecutar la aplicación
app.Run();