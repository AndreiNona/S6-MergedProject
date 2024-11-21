using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MovieAPI.Data;
using MovieAPI.Services;
using MovieAPI.Data.Repository;
using MovieAPI.Data.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Register MovieApiService with HttpClient for making OMDb API requests
builder.Services.AddHttpClient<MovieApiService>();

// Register MovieDbContext with SQL Server configuration
builder.Services.AddDbContext<MovieDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MovieDB")));

// Register repository and service dependencies
builder.Services.AddScoped<IPeopleService, PeopleService>();
builder.Services.AddScoped<IPeopleRepository, PeopleRepository>();

builder.Services.AddScoped<IMovieRepository, MovieRepository>();
builder.Services.AddScoped<IMovieService, MovieService>();

builder.Services.AddScoped<IMovieDetailsService, MovieDetailsService>();

// Add Swagger generation services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MovieAPI", Version = "v1" });
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

app.UseAuthorization();
app.MapControllers();

app.Run();