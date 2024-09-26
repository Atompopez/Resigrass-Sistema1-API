using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ResiGrass_API.Logic; // Asegúrate de que este espacio de nombres sea correcto
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Inicializa la conexión utilizando el espacio de nombres correcto
ResiGrass_API.Logic.Globals.Initialize(builder.Configuration);

// Asegúrate de que la clase DbQuery esté en el espacio de nombres correcto
builder.Services.AddSingleton<DbQuery>(new DbQuery(ResiGrass_API.Logic.Globals.ConnectionString));

// Configuración de la autenticación JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "api.santiago.com",
        ValidAudience = "api.resigrass.com",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("G8j!q%Z7pT@x$3Rw^eY&f2*BnL9k#4Hs"))
    };
});

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ResiGrass API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Ingrese el token JWT en este formato: Bearer {token}",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Agrega la autenticación
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Urls.Add("http://0.0.0.0:5023");
app.Run();
