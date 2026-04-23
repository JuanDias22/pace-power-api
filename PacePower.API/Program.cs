using Microsoft.EntityFrameworkCore;
using PacePower.API.Application.Services;
using PacePower.API.Controllers;
using PacePower.API.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

var allowedOrigins = new[]
{
    "https://pace-power-landing-page-3ih0x8wu8-juandias22s-projects.vercel.app",
    "https://pace-power-landing-page.vercel.app"
};

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var conn = builder.Configuration.GetConnectionString("DefaultConnection");

Console.WriteLine("CONNECTION STRING:");
Console.WriteLine(conn ?? "NULL");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(conn));

builder.Services.AddControllers();
builder.Services.AddHttpClient();

builder.Services.AddScoped<LeadService>();
builder.Services.AddScoped<PaymentsRepository>();
builder.Services.AddScoped<UserRepository>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

    app.UseSwagger();
    app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseCors("CorsPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();