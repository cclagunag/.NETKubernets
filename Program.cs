using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NetKubernetes.Data;
using NetKubernetes.Data.Inmuebles;
using NetKubernetes.Data.Usuarios;
using NetKubernetes.Middleware;
using NetKubernetes.Models;
using NetKubernetes.Profiles;
using NetKubernetes.Token;
// 
var builder = WebApplication.CreateBuilder(args);

// String Connection Database registrar conexión de bases de datos
// builder.Services.AddDbContext<AppDbContext>(opt =>
// {
//     opt.LogTo(Console.WriteLine, new[] { DbLoggerCategory.Database.Command.Name }, LogLevel.Information)
//     .EnableSensitiveDataLogging();
//     // UseSql Server Registrar string de bases de datos
//     opt.UseSqlServer(builder.Configuration.GetConnectionString("SQLServerConnection"));
// });

var connectionMySqlString = builder.Configuration.GetConnectionString("MySqlConnection");
builder.Services.AddDbContext<AppDbContext>(options=> {
    options.UseMySql(connectionMySqlString, ServerVersion.AutoDetect(connectionMySqlString));
});


// AddScoped -> registrar injección de dependencias (IInmuebleRepository, InmuebleRepository)
builder.Services.AddScoped<IInmuebleRepository, InmuebleRepository>();

// registrar controller de Autenticación de usuario
builder.Services.AddControllers(opt =>
{
    // Crear Objeto de AuthorizationPolicyBuilder para registrar Autenticación de usuario
    var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
    // Registrar objeto de Autenticación
    opt.Filters.Add(new AuthorizeFilter(policy));
});


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// agregar mappers registrados en InmuebleProfile para transformar información entre clases
var mapperConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new InmuebleProfile());
});

// Configurar mapperConfig para crear el objeto mapper y hacer singleton para registrarlos en program
IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

// Registrar la clase Usuario como autenticación para crear objeto en bases de datos
var buiderSecurity = builder.Services.AddIdentityCore<Usuario>();

// Crear autenticación de usuario
var identityBuider = new IdentityBuilder(buiderSecurity.UserType, builder.Services);

// registrar autenticación de usuario con la clase context y no registrar usuario en la clase context
identityBuider.AddEntityFrameworkStores<AppDbContext>(); // registrar clase Context Bases de datos en programs.cs

// registrar la clase usuario como autenticación para la base de datos
identityBuider.AddSignInManager<SignInManager<Usuario>>();

// crear una instancia del objeto SystemClock PARA CONTROLAR tiempo
builder.Services.AddSingleton<ISystemClock, SystemClock>();

// crear injección de dependencias para generar JSON WEB TOKEN
builder.Services.AddScoped<IJwtGenerador, JwtGenerador>();

// crear injección de dependencias para generar UsuarioSesion
builder.Services.AddScoped<IUsuarioSesion, UsuarioSesion>();

// crear injección de dependencias para generar UsuarioRepository
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

// codificar en bytes la clave secreta del JSON WEB TOKEN para agregar autenticación de usuario
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("M1Pa1abraS3cr3ta3sM1a"));

// agregar AddAuthentication (Agregar validacion de JWT para guardar sección de usuario en la API)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(opt =>
{
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = key,
        ValidateAudience = false,
        ValidateIssuer = false
    };
});

// declarar CORS en la aplicación para habilitar todos los:
/* 
    -WithOrigins: habilitar todos los origenes en este caso tambien permiten conexión con Angular
    -AllowAnyMethod: habilitar todos los metodos PUT, GET, POST, DELETE, ETC.
    -AllowAnyHeader: habilitar todas las cabeceras
 */
builder.Services.AddCors(o => o.AddPolicy("corsapp", builder =>
{
    builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// registrar los middlewares para crear excepciones
app.UseMiddleware<ManagerMiddleware>();

app.UseAuthentication(); // habilitar autenticación
app.UseCors("corsapp"); // habilitar la configuracion de CORS

//app.UseHttpsRedirection();

app.UseAuthorization(); // habilitar las autorizaciones

app.MapControllers();


// injectar datos a la base de datos si no hay datos
using (var ambiente = app.Services.CreateScope())
{
    var services = ambiente.ServiceProvider;
    try
    {
        // obtener todos los usuarios de la base de datos
        var userManager = services.GetRequiredService<UserManager<Usuario>>();
        // obtener todos los datos de la base de datos en el context
        var context = services.GetRequiredService<AppDbContext>();
        // crear migraciones en la base de datos
        await context.Database.MigrateAsync();
        // cargar datos a la base de datos insertando el context y los usuarios registrados en la aplicacion
        await LoadDatabase.InsertarData(context, userManager);
    }
    catch (Exception e)
    {
        var logging = services.GetRequiredService<ILogger<Program>>();
        logging.LogError(e, "Ocurrio un error en la migracion");
    }
}


app.Run();
