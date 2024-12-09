using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MovieAPI.Data;
using MovieAPI.Services;
using MovieAPI.Data.Repository;
using MovieAPI.Data.Service;
using MovieAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// Load environment-specific configurations
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

// Configure Authentication with JWT Bearer Tokens
var jwtSettings = builder.Configuration.GetSection("JWT");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

// Add services to the container
builder.Services.AddControllers();

// Register MovieApiService with HttpClient for making OMDb API requests
builder.Services.AddHttpClient<MovieApiService>();

// Register MovieDbContext with SQL Server configuration
builder.Services.AddDbContext<MovieDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MovieDB")));

// Add ASP.NET Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<MovieDbContext>()
    .AddDefaultTokenProviders();

// Configure Authentication with JWT Bearer Tokens
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
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
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

// Register repository and service dependencies
builder.Services.AddScoped<IPeopleService, PeopleService>();
builder.Services.AddScoped<IPeopleRepository, PeopleRepository>();

builder.Services.AddScoped<IMovieRepository, MovieRepository>();
builder.Services.AddScoped<IMovieService, MovieService>();

builder.Services.AddScoped<IMovieDetailsService, MovieDetailsService>();

builder.Services.AddScoped<ITopListService, TopListService>();
builder.Services.AddScoped<ITopListRepository, TopListRepository>();

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", builder =>
    {
        builder
            .AllowAnyOrigin()   // Allow requests from any origin
            .AllowAnyMethod()   // Allow all HTTP methods
            .AllowAnyHeader();  // Allow all headers
    });
});

// Add Swagger generation services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MovieAPI", Version = "v1" });

    // Add JWT Bearer Authentication
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' followed by a space and your JWT token",
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

// Enable Swagger middleware in the development environment
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MovieAPI v1");
    });
}

// Enable CORS middleware
app.UseCors("AllowAllOrigins");

app.UseAuthentication(); // Add Authentication Middleware
app.UseAuthorization();

app.MapControllers();

app.Run();
