<template>
  <v-app>
    <div class="movie-search-wrapper">
      <!-- Film Strip Left -->
      <div class="film-strip left"></div>

      <!-- Content Area -->
      <div class="content-wrapper">
        <v-container>
          <h1 class="page-title">Movies</h1>
          <p class="page-description">This is the Movies page.</p>

          <!-- Search Form -->
          <v-row class="search-form">
            <!-- Name Search -->
            <v-col cols="12" sm="6" md="4">
              <v-text-field
                v-model="searchQuery"
                label="Search by Name"
                outlined
                clearable
                class="search-input"
              ></v-text-field>
            </v-col>

            <!-- Year Range Picker -->
            <v-col cols="12" sm="6" md="4">
              <div class="text-field-wrapper">
                <v-btn icon class="calendar-button" @click="menu = true">
                  <v-icon>mdi-calendar</v-icon>
                </v-btn>
                <v-text-field
                  v-model="formattedYearRange"
                  label="Select Year Range"
                  readonly
                  outlined
                  clearable
                  class="year-picker-field"
                ></v-text-field>
              </div>
            </v-col>

            <!-- Results Per Page -->
            <v-col cols="12" sm="6" md="4">
              <v-text-field
                v-model="limit"
                label="Results Per Page"
                outlined
                clearable
                type="number"
                class="search-input"
              ></v-text-field>
            </v-col>
          </v-row>

          <!-- Year Picker Dialog -->
          <v-dialog v-model="menu" persistent max-width="90%">
            <template #default>
              <div class="year-picker">
                <div class="year-picker-header">
                  <v-btn icon @click="scrollYears(1)">
                    <v-icon>mdi-chevron-up</v-icon>
                  </v-btn>
                  <span class="year-picker-title">
                    {{ displayYearRange }}
                  </span>
                  <v-btn icon @click="scrollYears(-1)">
                    <v-icon>mdi-chevron-down</v-icon>
                  </v-btn>
                </div>
                <div class="year-picker-body">
                  <v-list>
                    <v-list-item
                      v-for="year in years"
                      :key="year"
                      :class="{
                        selected: selectedYearRange.includes(year),
                        inRange: isYearInRange(year),
                      }"
                      @click="toggleYear(year)"
                    >
                      {{ year }}
                    </v-list-item>
                  </v-list>
                </div>
                <div class="year-picker-actions">
                  <v-btn text class="clear-button" @click="clearYearSelection">Clear</v-btn>
                  <v-btn text class="close-button" @click="menu = false">Confirm</v-btn>
                </div>
              </div>
            </template>
          </v-dialog>

          <!-- Search Button -->
          <v-row>
            <v-btn class="search-button" block @click="searchMovies">
              Search
            </v-btn>
          </v-row>

          <!-- Loading Indicator -->
          <LoadingRadar :loading="isLoading" />

          <!-- Movie Cards -->
          <v-row>
            <v-col
              v-for="(movie, index) in movies"
              :key="index"
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
                :movieId="movie.id"
              />
            </v-col>
          </v-row>
        </v-container>
      </div>

      <!-- Film Strip Right -->
      <div class="film-strip right"></div>
    </div>
  </v-app>
</template>

<script>
import LoadingRadar from "@/components/LoadingRadar.vue";
import MovieCard from "@/components/MovieCard.vue";
import axios from "axios";
import config from "@/config.js"; 

export default {
  name: "MovieView",
  components: {
    LoadingRadar,
    MovieCard,
  },
  data() {
    return {
      menu: false, // Controls dialog visibility
      years: Array.from({ length: 2024 - 1900 + 1 }, (_, i) => 2024 - i), // List of years in descending order
      displayYearRange: "2024 - 1900", // Shown year range
      selectedYearRange: [], // Array for selected year range
      formattedYearRange: "", // Display formatted year range in the text field
      searchQuery: "", // Search term for movie names
      limit: 5, // Number of results per page
      isLoading: false, // Loading state
      movies: [], // Array to store returned movies
    };
  },
  methods: {
    /**
     * Fetches the first rating of a movie.
     * @param {Object} movie - The movie object.
     * @returns {String} - The movie rating or "N/A" if not available.
     */
    getMovieRating(movie) {
      if (movie.ratings && movie.ratings.length > 0) {
        return movie.ratings[0].value || "N/A"; // Use the first rating
      }
      return "N/A";
    },
    scrollYears(direction) {
      const offset = 10 * direction;
      this.years = this.years.map((year) => year - offset);
      this.displayYearRange = `${this.years[0]} - ${this.years[this.years.length - 1]}`;
    },
    toggleYear(year) {
      if (this.selectedYearRange.includes(year)) {
        // Deselect the clicked year
        this.selectedYearRange = this.selectedYearRange.filter((y) => y !== year);
      } else if (this.selectedYearRange.length === 0) {
        // Select the first year
        this.selectedYearRange = [year];
      } else if (this.selectedYearRange.length === 1) {
        // Add the second year and sort the range
        this.selectedYearRange = [this.selectedYearRange[0], year].sort((a, b) => a - b);
      } else {
        // Replace the closest year
        const [start, end] = this.selectedYearRange;
        const distanceToStart = Math.abs(start - year);
        const distanceToEnd = Math.abs(end - year);
        if (distanceToStart < distanceToEnd || (distanceToStart === distanceToEnd && year < start)) {
          this.selectedYearRange = [year, end].sort((a, b) => a - b);
        } else {
          this.selectedYearRange = [start, year].sort((a, b) => a - b);
        }
      }

      // Update formatted year range
      this.updateFormattedYearRange();
    },
    updateFormattedYearRange() {
      if (this.selectedYearRange.length === 2) {
        this.formattedYearRange = `${this.selectedYearRange[0]} - ${this.selectedYearRange[1]}`;
      } else if (this.selectedYearRange.length === 1) {
        this.formattedYearRange = `${this.selectedYearRange[0]}`;
      } else {
        this.formattedYearRange = "";
      }
    },
    isYearInRange(year) {
      if (this.selectedYearRange.length === 2) {
        const [start, end] = this.selectedYearRange;
        return year > start && year < end;
      }
      return false;
    },
    clearYearSelection() {
      // Clear the selected year range
      this.selectedYearRange = [];
      this.formattedYearRange = "";
    },
    async searchMovies() {
      const baseUrl = config.apiBaseUrl; 
      const includeOmdbDetails = true;
      let response;

      try {
        this.isLoading = true;

        if (this.searchQuery && this.selectedYearRange.length > 0) {
          const expandedLimit = 50;
          response = await axios.get(
            `${baseUrl}/api/Movies/name/${this.searchQuery}?smartSearch=true&wordComplete=true&limit=${expandedLimit}&includeOmdbDetails=${includeOmdbDetails}`
          );
          const filteredResults = this.filterMoviesByYear(response.data, this.selectedYearRange);
          this.movies = filteredResults.slice(0, this.limit).map(this.cleanMovieData);
        } else if (this.searchQuery) {
          response = await axios.get(
            `${baseUrl}/api/Movies/name/${this.searchQuery}?smartSearch=true&wordComplete=true&limit=${this.limit}&includeOmdbDetails=${includeOmdbDetails}`
          );
          this.movies = response.data.map(this.cleanMovieData);
        } else if (this.selectedYearRange.length > 0) {
          if (this.selectedYearRange.length === 1) {
            response = await axios.get(
              `${baseUrl}/api/Movies/year/${this.selectedYearRange[0]}?limit=${this.limit}&includeOmdbDetails=${includeOmdbDetails}`
            );
          } else if (this.selectedYearRange.length === 2) {
            const [start, end] = this.selectedYearRange;
            response = await axios.get(
              `${baseUrl}/api/Movies/range?start=${start}&end=${end}&limit=${this.limit}&includeOmdbDetails=${includeOmdbDetails}`
            );
          }
          this.movies = response.data.map(this.cleanMovieData);
        } else {
          console.log("No search criteria provided.");
        }

        if (response) {
          console.log("API Response:", response.data);
          console.log("Displayed Movies:", this.movies);
        }
      } catch (error) {
        console.error("Error fetching movies:", error);
      } finally {
        this.isLoading = false;
      }
    },
    cleanMovieData(movie) {
      return {
        ...movie,
        poster: movie.poster === "N/A" ? null : movie.poster, // Replace "N/A" with null
      };
    },
    filterMoviesByYear(movies, selectedYearRange) {
      if (selectedYearRange.length === 1) {
        // Single Year
        return movies.filter((movie) => movie.year === selectedYearRange[0]);
      } else if (selectedYearRange.length === 2) {
        const [start, end] = selectedYearRange;
        return movies.filter((movie) => movie.year >= start && movie.year <= end);
      }
      return movies;
    },
  },
};
</script>



<style scoped>
.movie-search-wrapper {
  display: flex;
  position: relative;
  min-height: 100vh;
  background-color: #121212;
  color: #f0f0f0;
}

.film-strip {
  position: relative;
  width: 80px;
  background-color: #000;
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
  padding: 20px;
}

.page-title {
  text-align: center;
  color: #ffd700;
  font-size: 2rem;
  font-weight: bold;
  margin-bottom: 10px;
}

.page-description {
  text-align: center;
  color: #c0c0c0;
  margin-bottom: 20px;
}

.text-field-wrapper {
  display: flex;
  align-items: center;
  gap: 10px; 
}

.year-picker-field {
  flex-grow: 1;
  max-width: calc(100% - 60px); 
}

.calendar-button {
  background-color: #1e1e1e;
  color: #ffffff;
  border-radius: 50%;
  width: 50px;
  height: 50px;
  display: flex;
  justify-content: center;
  align-items: center;
}


.year-picker {
  background-color: #dddddd;
  border-radius: 8px;
  padding: 16px;
  display: flex;
  flex-direction: column;
  align-items: center;
}

.year-picker-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  width: 100%;
  margin-bottom: 8px;
}

.year-picker-body {
  max-height: 60vh;
  overflow-y: scroll;
}

.year-picker-body::-webkit-scrollbar {
  width: 8px;
}

.year-picker-body::-webkit-scrollbar-thumb {
  background-color: #b0b0b0;
  border-radius: 4px;
}

.year-picker-body .v-list-item {
  width: 100%;
  padding: 10px;
  text-align: center;
  cursor: pointer;
  transition: background-color 0.2s, color 0.2s;
}

.year-picker-body .v-list-item.selected {
  background-color: #b99e00; 
  color: #ffffff;
  border-radius: 4px;
}

.year-picker-body .v-list-item.inRange {
  background-color: #b99e003b; 
  color: #000000;
  border-radius: 4px;
}

.year-picker-actions {
  display: flex;
  justify-content: space-between;
  width: 100%;
  padding: 10px 16px;
}

.clear-button {
  background-color: rgba(255, 0, 0, 0.2);
  color: red;
  font-weight: bold;
  border-radius: 8px;
  padding: 8px 12px;
}

.close-button {
  background-color: transparent;
  color: blue;
}
</style>
