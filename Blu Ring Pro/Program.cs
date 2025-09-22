using System.Text.Json.Serialization;
using Blu_Ring_Pro.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// 1) DbContext (Pomelo + MySQL/MariaDB)
var conn = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<BluRingProContext>(options =>
    options.UseMySql(conn, ServerVersion.AutoDetect(conn))
);

// 2) Controllers + JSON (evitar ciclos)
builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        o.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// 3) Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BluRing API", Version = "v1" });
});

// 4) CORS (política con nombre)
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins(
                "http://localhost:5173", // Vite dev
                "http://localhost:50035",
                "http://192.168.10.24:50035",
                "http://192.168.137.1:50035",
                "http://192.168.1.98:50035",
                "https://f9f7d3cd7ed8.ngrok-free.app"// otro puerto si usas extensión o preview
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
        });
});

var app = builder.Build();

// 5) Swagger UI
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "BluRing API v1");
    c.RoutePrefix = string.Empty;
});

// 6) Middleware
app.UseHttpsRedirection();
app.UseRouting();
app.UseCors(MyAllowSpecificOrigins); // 👉 ahora sí usa la política registrada
app.UseAuthorization();

// 7) Map Controllers
app.MapControllers();

app.Run();
