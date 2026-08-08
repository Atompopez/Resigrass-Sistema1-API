using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Resend;
using ResiGrass_API.Logic;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Inicializa configuraciones globales
ResiGrass_API.Logic.Globals.Initialize(builder.Configuration);

// Agrega la l�gica de base de datos
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<DbQuery>();
builder.Services.AddHostedService<SendTokenUser>();
builder.Services.AddHostedService<EmailNotificationService>();

// Habilita CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowResigrassAndLocalhost", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "https://www.resigrass.com.co", "https://api-resigrass.page.resigrass.com.co",  "http://46.62.175.112:8081") // Or�genes permitidos
              .AllowAnyHeader()    // Permite cualquier encabezado
              .AllowAnyMethod();   // Permite cualquier m�todo (GET, POST, etc.)
    });
});

// Configura servicios
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "ResiGrass API", Version = "v1" });

    // Configuraci�n de JWT en Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Ingrese el token JWT en este formato: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Configuraci�n de autenticaci�n con JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

builder.Services.AddTransient<EmailNotificationService>();

var app = builder.Build();

// Swagger disponible en desarrollo y producci�n
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ResiGrass API V1");
        c.RoutePrefix = string.Empty;
    });
}

// Habilita CORS antes de Authentication y Authorization
app.UseCors("AllowResigrassAndLocalhost");

app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();

app.UseHttpsRedirection();
app.MapControllers();
app.Urls.Add("http://0.0.0.0:5023");

app.Run();
