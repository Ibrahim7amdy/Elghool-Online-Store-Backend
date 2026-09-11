using API.Middlewares;
using FluentValidation.AspNetCore;
using Infrastructure;
using Application;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json.Serialization;




var builder = WebApplication.CreateBuilder(args);

// ── Database Connection ──────────────────────────────────────
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "Database connection string 'DefaultConnection' is missing.");
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString , sqlOptions => sqlOptions.EnableRetryOnFailure()));



// ── Dependencies ───────────────────────────────
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices();

// ── FluentValidation ──────────────────────────────────────
builder.Services.AddFluentValidationAutoValidation();

// ── JWT Authentication ────────────────────────────────────
var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrWhiteSpace(jwtKey)) throw new InvalidOperationException("JWT Key is missing.");
if (jwtKey.Length < 32) throw new InvalidOperationException("JWT Key must be at least 32 characters.");


var jwtIssuer = builder.Configuration["Jwt:Issuer"];
if (string.IsNullOrWhiteSpace(jwtIssuer)) throw new InvalidOperationException("JWT Issuer is missing.");


var jwtAudience = builder.Configuration["Jwt:Audience"];
if (string.IsNullOrWhiteSpace(jwtAudience)) throw new InvalidOperationException("JWT Audience is missing.");



builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });



builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("AdminOnly", policy =>
            policy.RequireClaim("type", "admin"));

        options.AddPolicy("CustomerOnly", policy =>
            policy.RequireClaim("type", "customer"));

        options.AddPolicy("SuperAdminOnly", policy =>
        policy.RequireClaim("type", "admin")
              .RequireClaim("is_super_admin", "true"));
    });


builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // السطر ده بيمنع الـ Serializer إنه يلف في حلقة مفرغة بسبب العلاقات الدائرية
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        // response JSON هيبقى فيه الـ Enums على شكل Strings بدل الأرقام    
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });


builder.Services.AddCors(options =>    // Add CORS policy
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});



JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();
app.UseCors("AllowAll"); // Add CORS middleware
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();