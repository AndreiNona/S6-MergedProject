<template>
  <div class="movie-details-wrapper">
    <!-- Film Strip Left -->
    <div class="film-strip left"></div>

    <!-- Content Area -->
    <div class="content-wrapper">
      <v-container>
        <h1 class="movie-title">{{ movieDetails.title }}</h1>

        <!-- Poster Section -->
        <div class="movie-poster-container">
          <img
            v-if="movieDetails.poster && movieDetails.poster !== 'N/A'"
            :src="movieDetails.poster"
            alt="Movie Poster"
            class="movie-poster"
          />
          <p v-else class="no-poster">No Poster Available</p>
        </div>

        <!-- General Information Section -->
        <v-card outlined class="movie-details-card">
          <v-card-title>General</v-card-title>
          <v-card-text>
            <div v-for="(value, key) in generalFields" :key="key" class="detail-item">
              <strong>{{ formatFieldName(key) }}:</strong> {{ value }}
            </div>
          </v-card-text>
        </v-card>

        <!-- Production Information Section -->
        <v-card outlined class="movie-details-card">
          <v-card-title>Production</v-card-title>
          <v-card-text>
            <div v-if="directors.length > 0" class="detail-item">
              <strong>Director:</strong>
              <span v-for="(person, index) in directors" :key="person.id">
                <a
                  href="#"
                  @click.prevent="navigateToPerson(person.id)"
                  class="clickable-person"
                >
                  {{ person.name }}
                </a
                >{{ index < directors.length - 1 ? ', ' : '' }}
              </span>
            </div>
            <div v-if="writers" class="detail-item">
              <strong>Writer:</strong> {{ writers }}
            </div>
            <div v-if="actors.length > 0" class="detail-item">
              <strong>Actors:</strong>
              <span v-for="(person, index) in actors" :key="person.id">
                <a
                  href="#"
                  @click.prevent="navigateToPerson(person.id)"
                  class="clickable-person"
                >
                  {{ person.name }}
                </a
                >{{ index < actors.length - 1 ? ', ' : '' }}
              </span>
            </div>
          </v-card-text>
        </v-card>

        <!-- Performance Information Section -->
        <v-card outlined class="movie-details-card">
          <v-card-title>Performance</v-card-title>
          <v-card-text>
            <div v-for="(value, key) in performanceFields" :key="key" class="detail-item">
              <strong>{{ formatFieldName(key) }}:</strong> {{ value }}
            </div>
            <!-- Ratings Section -->
            <div v-if="movieDetails.ratings && movieDetails.ratings.length > 0" class="ratings-section">
              <p class="ratings-text">
                <strong>Ratings:</strong> {{ displayRatings }}
              </p>
              <p class="ratings-text">
                <strong>Average Rating:</strong> {{ averageRating || "N/A" }}/100
              </p>
            </div>
          </v-card-text>
        </v-card>
      </v-container>
    </div>

    <!-- Film Strip Right -->
    <div class="film-strip right"></div>
  </div>
</template>



<script>
import axios from "axios";
import config from "@/config.js"; 

export default {
  name: "MovieDetailsView",
  data() {
    return {
      movieDetails: {},
      directors: [],
      writers: null, // Writers aren't fetched via endpoint, keeping the original field
      actors: [],
    };
  },
  computed: {
    generalFields() {
      const keys = ["runtime", "genre", "plot", "language", "country", "released"];
      return this.filterFields(keys);
    },
    performanceFields() {
      const keys = ["boxOffice", "awards"];
      return this.filterFields(keys);
    },
    displayRatings() {
      if (this.movieDetails.ratings && this.movieDetails.ratings.length > 0) {
        return this.movieDetails.ratings
          .map((rating) => `${rating.source}: ${rating.value}`)
          .join(" || ");
      }
      return null;
    },
    averageRating() {
      if (this.movieDetails.ratings && this.movieDetails.ratings.length > 0) {
        const normalizedRatings = this.movieDetails.ratings.map((rating) => {
          const value = rating.value.trim();
          if (value.includes("/100")) {
            return parseFloat(value.split("/")[0]);
          } else if (value.includes("/10")) {
            return parseFloat(value.split("/")[0]) * 10;
          } else if (value.includes("%")) {
            return parseFloat(value);
          }
          return 0;
        });
        const validRatings = normalizedRatings.filter((rating) => rating > 0);
        const total = validRatings.reduce((sum, rating) => sum + rating, 0);
        return (total / validRatings.length).toFixed(1);
      }
      return null;
    },
  },
  methods: {
    filterFields(keys) {
      return keys.reduce((result, key) => {
        if (this.movieDetails[key] && this.movieDetails[key] !== "N/A") {
          result[key] = this.movieDetails[key];
        }
        return result;
      }, {});
    },
    async fetchDirectors() {
      try {
        const { id } = this.$route.params;
        const baseUrl = config.apiBaseUrl;
        const response = await axios.get(`${baseUrl}/api/Movies/${id}/directors`);
        this.directors = response.data;
      } catch (error) {
        console.error("Error fetching directors:", error);
      }
    },
    async fetchActors() {
      try {
        const { id } = this.$route.params;
        const baseUrl = config.apiBaseUrl;
        const response = await axios.get(`${baseUrl}/api/Movies/${id}/stars`);
        this.actors = response.data;
      } catch (error) {
        console.error("Error fetching actors:", error);
      }
    },
    async navigateToPerson(personId) {
      try {
        this.$router.push({ name: "PersonDetails", params: { id: personId } });
      } catch (error) {
        console.error(`Error navigating to person with ID ${personId}:`, error);
      }
    },
    formatFieldName(fieldName) {
      return fieldName
        .replace(/([A-Z])/g, " $1")
        .replace(/^./, (str) => str.toUpperCase());
    },
  },
  async mounted() {
    try {
      const { id } = this.$route.params;
      const baseUrl = config.apiBaseUrl;
      // Fetch movie details
      const movieResponse = await axios.get(`${baseUrl}/api/movies/omdb/${id}`);
      this.movieDetails = movieResponse.data;

      // Fetch directors and actors
      await this.fetchDirectors();
      await this.fetchActors();
    } catch (error) {
      alert("Failed to load movie details. Please try again.");
    }
  },
};
</script>



<style scoped>
.movie-details-wrapper {
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

.v-container {
  margin: 20px;
  color: #f0f0f0;
}

.movie-title {
  text-align: center;
  color: #fff;
  font-weight: bold;
  margin-bottom: 20px;
}

.movie-poster-container {
  display: flex;
  justify-content: center;
  margin-bottom: 20px;
}

.movie-poster {
  max-width: 100%;
  max-height: 500px;
  border-radius: 5px;
}

.no-poster {
  color: gray;
  font-style: italic;
  text-align: center;
}

.movie-details-card {
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

.detail-item {
  color: #c0c0c0;
  margin-bottom: 10px;
}

.detail-item strong {
  color: #ffffff;
}

.clickable-person {
  color: #42a5f5;
  text-decoration: underline;
  cursor: pointer;
}

.clickable-person:hover {
  color: #64b5f6;
}

.ratings-section .ratings-text {
  color: #ffffff;
  margin-top: 10px;
  line-height: 1.5;
}
</style>
