using MovieAPI.Data.Repository;
using MovieAPI.Data.Service;

namespace MovieAPI.Tests.Services;

using Xunit;
using Moq;
using MovieAPI.Services;
using MovieAPI.Models;

public class MovieServiceTests
    {
        private readonly Mock<IMovieRepository> _mockMovieRepository;
        private readonly Mock<MovieApiService> _mockMovieApiService; // Mock for MovieApiService
        private readonly Mock<IMovieDetailsService> _mockMovieDetailsService; // Mock for MovieDetailsService
        private readonly MovieService _movieService;

        public MovieServiceTests()
        {
            _mockMovieRepository = new Mock<IMovieRepository>();
            _mockMovieApiService = new Mock<MovieApiService>();
            _mockMovieDetailsService = new Mock<IMovieDetailsService>();

            // Pass the dependencies to MovieService
            _movieService = new MovieService(_mockMovieRepository.Object, _mockMovieApiService.Object, _mockMovieDetailsService.Object);
        }

        // Test 1: No Movies Exist for the Given Year
        [Fact]
        public async Task GetMoviesByYear_ShouldReturnNotFound_WhenNoMoviesExist()
        {
            // Arrange
            int year = 1999;
            int limit = 10;
            bool includeOmdbDetails = false;

            _mockMovieRepository
                .Setup(repo => repo.GetMoviesByYear(year, limit))
                .ReturnsAsync(new List<Movie>());

            // Act
            var result = await _movieService.GetMoviesByYear(year, limit, includeOmdbDetails);

            // Assert
            Assert.Empty(result);
        }

        // Test 2: Invalid Parameters - Negative Year
        [Fact]
        public async Task GetMoviesByYear_ShouldReturnNotFound_WhenYearIsNegative()
        {
            // Arrange
            int year = -100;
            int limit = 10;
            bool includeOmdbDetails = false;

            _mockMovieRepository
                .Setup(repo => repo.GetMoviesByYear(year, limit))
                .ReturnsAsync(new List<Movie>());

            // Act
            var result = await _movieService.GetMoviesByYear(year, limit, includeOmdbDetails);

            // Assert
            Assert.Empty(result);
        }

        // Test 3: Invalid Parameters - Very Large Limit
        [Fact]
        public async Task GetMoviesByYear_ShouldLimitResults_WhenLimitIsVeryLarge()
        {
            // Arrange
            int year = 2001;
            int limit = 1000000;
            bool includeOmdbDetails = false;

            var movies = new List<Movie>
            {
                new Movie { Id = 1, Title = "Shrek", Year = year },
                new Movie { Id = 2, Title = "The Lord of the Rings: The Fellowship of the Ring", Year = year }
            };

            _mockMovieRepository
                .Setup(repo => repo.GetMoviesByYear(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(movies);

            // Act
            var result = await _movieService.GetMoviesByYear(year, limit, includeOmdbDetails);

            // Assert
            Assert.Equal(2, result.Count()); // The limit should not affect the results 
        }

        // Test 4: Movies Exist for the Given Year
        [Fact]
        public async Task GetMoviesByYear_ShouldReturnMovies_WhenMoviesExist()
        {
            // Arrange
            int year = 2001;
            int limit = 10;
            bool includeOmdbDetails = false;

            var movies = new List<Movie>
            {
                new Movie { Id = 1, Title = "Shrek", Year = year },
                new Movie { Id = 2, Title = "The Lord of the Rings: The Fellowship of the Ring", Year = year }
            };

            _mockMovieRepository
                .Setup(repo => repo.GetMoviesByYear(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(movies);

            // Act
            var result = await _movieService.GetMoviesByYear(year, limit, includeOmdbDetails);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Equal("Shrek", result.First().Title);
        }

        // Test 5: Include OMDb Details
        [Fact]
        public async Task GetMoviesByYear_ShouldIncludeOmdbDetails_WhenIncludeOmdbDetailsIsTrue()
        {
            // Arrange
            int year = 2001;
            int limit = 10;
            bool includeOmdbDetails = true;

            var movies = new List<Movie>
            {
                new Movie { Id = 1, Title = "Shrek", Year = year },
                new Movie { Id = 2, Title = "The Lord of the Rings: The Fellowship of the Ring", Year = year }
            };

            _mockMovieRepository
                .Setup(repo => repo.GetMoviesByYear(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(movies);

            _mockMovieDetailsService
                .Setup(service => service.AddOmdbDetailsToMovies(It.IsAny<IEnumerable<Movie>>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _movieService.GetMoviesByYear(year, limit, includeOmdbDetails);

            // Assert
            _mockMovieDetailsService.Verify(service => service.AddOmdbDetailsToMovies(It.IsAny<IEnumerable<Movie>>()), Times.Once);
        }
    }