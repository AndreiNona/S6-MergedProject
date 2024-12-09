# Use the .NET SDK image for building the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory
WORKDIR /src

# Copy project file(s) and restore dependencies
COPY ["MovieAPI.csproj", "./"]
RUN dotnet restore "./MovieAPI.csproj"

# Copy the rest of the application files
COPY . .

# Build the application
RUN dotnet build "./MovieAPI.csproj" -c Release -o /app/build

# Publish the application
RUN dotnet publish "./MovieAPI.csproj" -c Release -o /app/publish

# Use the ASP.NET Core Runtime image for running the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# Set the working directory
WORKDIR /app

# Copy the published application from the build stage
COPY --from=build /app/publish .

# Expose port 8080
EXPOSE 8080

# Set the entry point for the container
ENTRYPOINT ["dotnet", "MovieAPI.dll"]
