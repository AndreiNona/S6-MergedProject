<template>
  <v-container>
    <h2>Edit Top List: {{ topListName }}</h2>

    <!-- Search Bar -->
    <v-row class="search-bar">
      <v-col cols="12" sm="8">
        <v-text-field
          v-model="searchQuery"
          label="Search Movies by Name"
          outlined
          clearable
        ></v-text-field>
      </v-col>
      <v-col cols="12" sm="4">
        <v-btn color="#b59902" block @click="searchMovies">Search</v-btn>
      </v-col>
    </v-row>

    <!-- Search Results -->
    <v-row>
      <v-col
        v-for="movie in searchResults"
        :key="movie.id"
        cols="12"
        sm="6"
        md="4"
        lg="3"
      >
        <v-card outlined class="movie-card">
          <v-img
            class="movie-poster"
            :src="getMoviePoster(movie)"
            alt="Movie Poster"
            contain
          ></v-img>
          <v-card-title class="movie-title">{{ movie.title }}</v-card-title>
          <v-card-subtitle>
            <strong>Year:</strong> {{ movie.year }}
          </v-card-subtitle>
          <v-card-text>
            <p v-if="movie.genre"><strong>Genre:</strong> {{ movie.genre }}</p>
            <p v-if="movie.plot"><strong>Plot:</strong> {{ movie.plot }}</p>
          </v-card-text>
          <v-card-actions>
            <v-btn
              color="success"
              block
              @click="addMovieToList(movie)"
              :disabled="movieIds.includes(movie.id)"
            >
              Add to Top List
            </v-btn>
          </v-card-actions>
        </v-card>
      </v-col>
    </v-row>

    <!-- Top List -->
    <v-card outlined class="top-list-edit-card">
      <v-card-text>
        <ol>
          <li
            v-for="(movie, index) in movies"
            :key="index"
            class="movie-item"
          >
            <!-- Movie Details -->
            <div class="movie-details">
              <!-- Movie Poster -->
              <img
                :src="getMoviePoster(movie)"
                alt="Movie Poster"
                class="movie-poster"
              />
              <div class="movie-info">
                <h3>{{ movie.title }}</h3>
                <p v-if="movie.year"><strong>Year:</strong> {{ movie.year }}</p>
                <p v-if="movie.genre"><strong>Genre:</strong> {{ movie.genre }}</p>
                <p v-if="getMovieRating(movie)">
                  <strong>Rating:</strong> {{ getMovieRating(movie) }}
                </p>
                <p v-if="movie.plot && movie.plot !== 'N/A'">
                  <strong>Plot:</strong> {{ movie.plot }}
                </p>
                <p v-else>
                  <strong>Plot:</strong> We seem to be unable to find any
                  information on this.
                </p>
              </div>
            </div>
            <!-- Control Buttons -->
            <div class="button-container">
          <v-btn icon color="#b59902" @click="moveUp(index)" :disabled="index === 0">
            <v-icon>mdi-arrow-up</v-icon>
          </v-btn>
          <v-btn icon color="#b59902" @click="moveDown(index)" :disabled="index === movies.length - 1">
            <v-icon>mdi-arrow-down</v-icon>
          </v-btn>
          <v-btn icon color="error" @click="removeMovie(index)">
            <v-icon>mdi-close</v-icon>
          </v-btn>
        </div>
          </li>
        </ol>
      </v-card-text>
      <v-card-actions>
        <v-btn color="success" @click="confirmChanges">Confirm Changes</v-btn>
        <v-btn color="error" @click="discardChanges">Discard Changes</v-btn>
      </v-card-actions>
    </v-card>
  </v-container>
</template>

<script>
import axios from "axios";
import config from "@/config.js";

export default {
  name: "TopListEditView",
  data() {
    return {
      topListName: "Unnamed List",
      movieIds: [],
      movies: [],
      searchQuery: "",
      searchResults: [],
    };
  },
  async mounted() {
    try {
      const { id } = this.$route.params;
      const baseUrl = config.apiBaseUrl;
      // Fetch top list details
      const response = await axios.get(`${baseUrl}/api/toplist/${id}`, {
        headers: {
          Authorization: `Bearer ${localStorage.getItem("jwtToken")}`,
        },
      });

      const topList = response.data;
      this.topListName = topList.name;
      this.movieIds = topList.movieIds;

      // Fetch details for movies in the list
      await this.fetchMovieDetails();
    } catch (error) {
      console.error("Error fetching top list details:", error);
    }
  },
  methods: {
    async fetchMovieDetails() {
      try {
        const baseUrl = config.apiBaseUrl;
        this.movies = await Promise.all(
          this.movieIds.map(async (movieId) => {
            const response = await axios.get(
              `${baseUrl}/api/movies/omdb/${movieId}`
            );
            return response.data;
          })
        );
      } catch (error) {
        console.error("Error fetching movie details:", error);
      }
    },
    async searchMovies() {
      try {
        const baseUrl = config.apiBaseUrl;
        const response = await axios.get(
          `${baseUrl}/api/movies/name/${this.searchQuery}?smartSearch=true&wordComplete=true&limit=10&includeOmdbDetails=true`
        );
        this.searchResults = response.data;
      } catch (error) {
        console.error("Error searching movies:", error);
      }
    },
    addMovieToList(movie) {
      if (!this.movieIds.includes(movie.id)) {
        this.movies.push(movie);
        this.movieIds.push(movie.id);
      }
    },
    getMoviePoster(movie) {
      return movie.poster && movie.poster !== "N/A"
        ? movie.poster
        : "/images/noPoserAvailable.png";
    },
    getMovieRating(movie) {
      if (movie.ratings && movie.ratings.length > 0) {
        return movie.ratings[0].value;
      }
      return null;
    },
    removeMovie(index) {
      this.movies.splice(index, 1);
      this.movieIds.splice(index, 1);
    },
    moveUp(index) {
  
      const temp = this.movies[index - 1];
      this.movies[index - 1] = this.movies[index];
      this.movies[index] = temp;

      const tempId = this.movieIds[index - 1];
      this.movieIds[index - 1] = this.movieIds[index];
      this.movieIds[index] = tempId;
    },
    moveDown(index) {
      const temp = this.movies[index + 1];
      this.movies[index + 1] = this.movies[index];
      this.movies[index] = temp;

      const tempId = this.movieIds[index + 1];
      this.movieIds[index + 1] = this.movieIds[index];
      this.movieIds[index] = tempId;
    },
    async confirmChanges() {
      try {
        const { id } = this.$route.params;
        const baseUrl = config.apiBaseUrl;
        await axios.put(
          `${baseUrl}/api/toplist/${id}/update`,
          { movieIds: this.movieIds },
          {
            headers: {
              Authorization: `Bearer ${localStorage.getItem("jwtToken")}`,
            },
          }
        );
        alert("Changes saved successfully!");
        this.$router.push("/profile");
      } catch (error) {
        console.error("Error saving changes:", error);
        alert("Failed to save changes. Please try again.");
      }
    },
    discardChanges() {
      this.$router.push("/profile");
    },
  },
};
</script>

  
<style scoped>
/* General Styling for Top List */
.v-container {
  background-color: #121212; 
  color: #f0f0f0; 
  padding: 20px;
  border-radius: 10px;
}

/* Title Styling */
h2 {
  color: #ffd700; 
  font-weight: bold;
  margin-bottom: 20px;
}

/* Search Bar */
.search-bar v-col {
  
  margin-bottom: 15px;
}

.search-bar .v-text-field {
  background-color: #1e1e1e;
  color: #f0f0f0;
  border-radius: 5px;
}

/* Movie Cards */
.movie-card {
  background-color: #1e1e1e;
  color: #f0f0f0;
  border-radius: 8px;
  box-shadow: 0 4px 10px rgba(0, 0, 0, 0.3);
  margin: 0; 
}

/* List Styling */
ol {
  padding-left: 0;
  list-style-type: none;
}

.movie-item {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 10px;
  background-color: #1e1e1e; 
  border-radius: 5px;
  margin-bottom: 0; 
  color: #f0f0f0;
  border: 1px solid #333; 
}

.movie-details {
  display: flex;
  align-items: center;
}

.movie-info {
  margin-left: 10px;
}

.movie-poster {
  width: 100px;
  height: 150px;
  object-fit: cover;
  margin: 0; 
}

.movie-info h3 {
  color: #ffd700;
  font-size: 1rem;
  margin: 0;
}

/* Buttons */
.button-container {
  display: flex;
  align-items: center;
  gap: 10px; 
}

.v-btn {
  background-color: #333;
  color: #ffd700;
  font-weight: bold;
  border-radius: 5px;
  min-width: 40px;
  min-height: 40px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.v-btn:hover {
  background-color: #444;
}

.v-btn[disabled] {
  background-color: #555;
  color: #888;
  cursor: not-allowed;
}

.v-icon {
  font-size: 20px; 
  color: #f0f0f0;
}
</style>
