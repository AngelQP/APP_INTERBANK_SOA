using APP_INTERBANK_SOA.Models;
using APP_INTERBANK_SOA.Servicios.Implementaciones;
using APP_INTERBANK_SOA.Servicios.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// JWT Auth
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

// Add services to the container.
builder.Services.AddDbContext<InterbankContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Interbank")));


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// configuración de Swagger con soporte para JWT Bearer
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Interbank API",
        Version = "v1"
    });

    // 1. Definición del esquema de seguridad Bearer
    var securityScheme = new OpenApiSecurityScheme
    {
        Description = "Introduce el token JWT usando el formato: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer", // Asegúrate de que el esquema sea 'Bearer'
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer" // Debe coincidir con el nombre de la definición (paso 2)
        }
    };

    // 2. Añade la definición de seguridad
    c.AddSecurityDefinition("Bearer", securityScheme);

    // 3. Establece el requisito de seguridad global (ESTO ES LO QUE TE FALTA)
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            securityScheme, // Usa el objeto de esquema que definiste
            new List<string>()
        }
    });

});

// Configuración de Token

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddScoped<ServicioMovimiento, IMovimiento>();
builder.Services.AddScoped<ServicioGuardar, IGuardar>();
builder.Services.AddScoped<ServicioProducto, IProducto>();
builder.Services.AddScoped<ServicioCuentaAhorro, ICuentaAhorro>();
builder.Services.AddScoped<CuentaSaldo, ICuentaSaldo>();
builder.Services.AddScoped<Transferencia, ITransferencia>();
builder.Services.AddScoped<Pagos, IPagos>();





var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication(); // uso de auth
app.UseAuthorization();

app.MapControllers();

app.Run();
