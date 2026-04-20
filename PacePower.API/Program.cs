using Microsoft.EntityFrameworkCore;
using PacePower.API.Application.Services;
using PacePower.API.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddScoped<LeadService>();

builder.Services.AddHttpClient();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();
    app.UseSwagger();
    app.UseSwaggerUI();
app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.MapControllers();

app.Run();