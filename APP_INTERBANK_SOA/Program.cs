using APP_INTERBANK_SOA.Models;
using APP_INTERBANK_SOA.Servicios.Implementaciones;
using APP_INTERBANK_SOA.Servicios.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<InterbankContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("INTERBANK")));
builder.Services.AddScoped<ServicioMovimiento, IMovimiento>();
builder.Services.AddScoped<ServicioGuardar, IGuardar>();
builder.Services.AddScoped<ServicioProducto, IProducto>();
builder.Services.AddScoped<ServicioCuentaAhorro, ICuentaAhorro>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
