using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MovieAPI.Data;
using MovieAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add Swagger generation services.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpClient<MovieApiService>();
builder.Services.AddDbContext<MovieDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MovieDB")));

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MovieAPI", Version = "v1" });
});

var app = builder.Build();

// Enable Swagger middleware in the development environment.
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