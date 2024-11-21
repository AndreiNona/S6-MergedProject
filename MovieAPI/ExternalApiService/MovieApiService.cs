using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using MovieAPI.Models;

namespace MovieAPI.Services
{
     public class MovieApiService
    {
        private readonly HttpClient _httpClient;

        public MovieApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Helper method to format the IMDb ID
        private string FormatOmdbId(int id)
        {
            return $"tt{id.ToString().PadLeft(7, '0')}";
        }

        // Method to get a complete OMDb movie object
        public async Task<OmdbMovie> GetMovieFromOmdb(int databaseId, string apiKey)
        {
            // Format the ID
            string formattedId = FormatOmdbId(databaseId);

            // Construct the OMDb API URL
            var url = $"http://www.omdbapi.com/?i={formattedId}&apikey={apiKey}";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Failed to fetch movie data: {response.StatusCode}");
            }

            // Deserialize the API response into OmdbMovie
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var omdbMovie = JsonSerializer.Deserialize<OmdbMovie>(jsonResponse, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return omdbMovie;
        }

        // Method to get specific movie details (e.g., Poster, Genre, Ratings)
        public async Task<(string Poster, string Genre, List<Rating> Ratings)> GetMovieDetailsFromOmdb(int databaseId, string apiKey)
        {
            // Format the ID
            string formattedId = FormatOmdbId(databaseId);

            // Construct the OMDb API URL
            var url = $"http://www.omdbapi.com/?i={formattedId}&apikey={apiKey}";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Failed to fetch movie data: {response.StatusCode}");
            }

            // Deserialize the API response
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var omdbMovie = JsonSerializer.Deserialize<OmdbMovie>(jsonResponse, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            // Return only the necessary fields
            return (omdbMovie.Poster, omdbMovie.Genre, omdbMovie.Ratings);
        }
    }
}