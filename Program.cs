//using SmartBus.Data;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// EF Core + MySQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("MySQL"),
        new MySqlServerVersion(new Version(8, 0, 0))
    //ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("MySQL"))
    ));
builder.Services.AddScoped<IKorisnikService, KorisnikService>();
builder.Services.AddScoped<IAutobusService, AutobusService>();
builder.Services.AddScoped<IPolazakService, PolazakService>();
builder.Services.AddScoped<IKartaService, KartaService>();
builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
builder.Services.AddScoped<IStanicaService, StanicaService>();
builder.Services.AddScoped<ISedisteService, SedisteService>();
builder.Services.AddScoped<IObavestenjeService, ObavestenjeService>();
builder.Services.AddScoped<ISignatureService, SignatureService>();
builder.Services.AddScoped<IDnevniIzvestajService, DnevniIzvestajService>();
builder.Services.AddScoped<IOcenaAutobusService, OcenaAutobusService>();

builder.Services.AddAuthentication()
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
            ),
            ClockSkew = TimeSpan.Zero
        };
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"❌ FAILED: {context.Exception.GetType().Name}: {context.Exception.Message}");
                return Task.CompletedTask;
            },
            OnMessageReceived = context =>
            {
                Console.WriteLine($"📨 Token primljen: {context.Token?.Substring(0, 20)}...");
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

// Podrska za CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("CORS", policy =>
    {
        policy.AllowAnyHeader()
            .AllowAnyMethod()
            .WithOrigins(
                        "http://localhost:5500",
                        "https://localhost:5500",
                        "http://127.0.0.1:5500",
                        "https://127.0.0.1:5500"
                        );
    });
});

// Servisi se registruju ovde
// builder.Services.AddScoped<ILinijaService, LinijaService>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Upiši: Bearer {tvoj_token}"
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
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SmartBus API v1");
        c.RoutePrefix = "swagger";
    });
    //app.UseSwaggerUI();
}

app.UseCors("CORS");
// Serviranje HTML/CSS/JS iz wwwroot
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();