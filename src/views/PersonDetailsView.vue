<template>
  <div class="person-details-wrapper">
    <!-- Film Strip Left -->
    <div class="film-strip left"></div>

    <!-- Content Area -->
    <div class="content-wrapper">
      <v-container v-if="personDetails">
        <!-- Person Name -->
        <h1 class="person-name">{{ personDetails.name }}</h1>

        <!-- Birth Year -->
        <p v-if="personDetails.birthYear" class="birth-year">
          <strong>Birth Year:</strong> {{ personDetails.birthYear }}
        </p>

        <!-- Directed Section -->
        <v-card
          v-if="directedMovies.length > 0"
          outlined
          class="person-details-card"
        >
          <v-card-title>Directed Movies</v-card-title>
          <v-card-text>
            <p
              v-if="averageRatings.averageRatingDirected && averageRatings.averageRatingDirected !== 'N/A'"
              class="average-rating"
            >
              <strong>Average Rating (Directed):</strong> {{ averageRatings.averageRatingDirected }}
            </p>
            <v-row>
              <v-col
                v-for="movie in visibleDirectedMovies"
                :key="movie.databaseId"
                cols="12"
                sm="6"
                md="4"
                lg="3"
              >
                <MovieCard
                  :title="movie.title"
                  :genre="movie.genre || 'N/A'"
                  :year="movie.year"
                  :rating="getMovieRating(movie)"
                  :poster="movie.poster || ''"
                  :movieId="movie.databaseId"
                />
              </v-col>
            </v-row>
            <v-btn
              v-if="directedMovies.length > visibleDirectedMovies.length"
              text
              class="load-more-btn"
              @click="loadMoreDirectedMovies"
            >
              Load More
            </v-btn>
          </v-card-text>
        </v-card>

        <!-- Starred Section -->
        <v-card
          v-if="starredMovies.length > 0"
          outlined
          class="person-details-card"
        >
          <v-card-title>
            Starred Movies of {{ personDetails.name }}
          </v-card-title>
          <v-card-text>
            <p
              v-if="averageRatings.averageRatingStarred && averageRatings.averageRatingStarred !== 'N/A'"
              class="average-rating"
            >
              <strong>Average Rating (Starred):</strong> {{ averageRatings.averageRatingStarred }}
            </p>
            <v-row>
              <v-col
                v-for="movie in visibleStarredMovies"
                :key="movie.databaseId"
                cols="12"
                sm="6"
                md="4"
                lg="3"
              >
                <MovieCard
                  :title="movie.title"
                  :genre="movie.genre || 'N/A'"
                  :year="movie.year"
                  :rating="getMovieRating(movie)"
                  :poster="movie.poster || ''"
                  :movieId="movie.databaseId"
                />
              </v-col>
            </v-row>
            <v-btn
              v-if="starredMovies.length > visibleStarredMovies.length"
              text
              class="load-more-btn"
              @click="loadMoreStarredMovies"
            >
              Load More
            </v-btn>
          </v-card-text>
        </v-card>
      </v-container>
      <v-container v-else>
        <p class="loading-message">Loading person details...</p>
      </v-container>
    </div>

    <!-- Film Strip Right -->
    <div class="film-strip right"></div>
  </div>
</template>


<script>
import axios from "axios";
import MovieCard from "@/components/MovieCard.vue";
import config from "@/config.js"; 

export default {
  name: "PersonDetailsView",
  components: { MovieCard },
  data() {
    return {
      personDetails: null, // Ensure personDetails is null initially TODO: Load person name
      directedMovies: [],
      starredMovies: [],
      averageRatings: {},
      visibleDirectedMoviesCount: 4,
      visibleStarredMoviesCount: 4,
      moviesPerPage: 4,
    };
  },
  computed: {
    visibleDirectedMovies() {
      return this.directedMovies.slice(0, this.visibleDirectedMoviesCount);
    },
    visibleStarredMovies() {
      return this.starredMovies.slice(0, this.visibleStarredMoviesCount);
    },
  },
  async mounted() {
    try {
      const { id } = this.$route.params;
      const baseUrl = config.apiBaseUrl;
      const personResponse = await axios.get(`${baseUrl}/api/People/${id}/role`);
      this.personDetails = personResponse.data;

      if (personResponse.data.directedMovies) {
        this.directedMovies = await this.fetchMovieDetails(personResponse.data.directedMovies);
      }

      if (personResponse.data.starredMovies) {
        this.starredMovies = await this.fetchMovieDetails(personResponse.data.starredMovies);
      }

      const ratingsResponse = await axios.get(`${baseUrl}/api/People/${id}/average-ratings`);
      this.averageRatings = ratingsResponse.data;
    } catch (error) {
      console.error("Error fetching person details or movies:", error);
      alert("Failed to load person details. Please try again.");
    }
  },
  methods: {
    async fetchMovieDetails(movies) {
      try {
        const baseUrl = config.apiBaseUrl;
        const detailedMovies = await Promise.all(
          movies.map(async (movie) => {
            const response = await axios.get(`${baseUrl}/api/movies/omdb/${movie.id}`);
            return response.data;
          })
        );
        return detailedMovies;
      } catch (error) {
        console.error("Error fetching detailed movie information:", error);
        return [];
      }
    },
    loadMoreDirectedMovies() {
      this.visibleDirectedMoviesCount += this.moviesPerPage;
    },
    loadMoreStarredMovies() {
      this.visibleStarredMoviesCount += this.moviesPerPage;
    },
    getMovieRating(movie) {
      if (movie.ratings && movie.ratings.length > 0) {
        return movie.ratings[0].value || "N/A";
      }
      return "N/A";
    },
  },
};
</script>

<style scoped>
.person-details-wrapper {
  display: flex;
  position: relative;
  min-height: 100vh;
  background-color: #121212; /* Dark background */
}

.film-strip {
  position: relative;
  width: 80px;
  background-color: #000;
  z-index: 1;
}

.film-strip.left {
  left: 0;
}

.film-strip.right {
  right: 0;
}

.film-strip::before {
  content: '';
  position: absolute;
  top: 0;
  width: 100%;
  height: 100%;
  background: repeating-linear-gradient(
    transparent 0,
    transparent 80px,
    #ffffff 80px,
    #ffffff 100px
  );
  border-radius: 20px;
}

@media screen and (max-width: 768px) {
  .film-strip {
    display: none; 
  }
}

.content-wrapper {
  flex: 1;
  max-width: calc(100% - 160px); 
  margin: 0 auto;
}

.person-name {
  text-align: center;
  color: #fff;
  font-weight: bold;
  margin-bottom: 20px;
}

.birth-year {
  text-align: center;
  color: #c0c0c0;
}

.person-details-card {
  margin-bottom: 20px;
  background-color: #1e1e1e;
  border-radius: 10px;
  padding: 20px;
}

.v-card-title {
  color: #ffd700;
  font-size: 18px;
  margin-bottom: 10px;
}

.average-rating {
  color: #ffffff;
  margin-bottom: 10px;
}

.load-more-btn {
  margin-top: 20px;
  color: #000;
  background-color: #ffffff;
  font-weight: bold;
  border-radius: 5px;
  transition: background-color 0.3s ease, color 0.3s ease;
}

.load-more-btn:hover {
  background-color: #e0e0e0; 
}
</style>
