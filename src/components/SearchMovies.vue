<template>
    <v-container>
      <h2>Search for Movies</h2>
      <v-row align="center" justify="center" class="compact-search-bar">
        <!-- Search Bar -->
        <v-col cols="12" sm="6" md="4">
          <v-text-field
            v-model="searchQuery"
            label="Search by Name"
            outlined
            dense
            hide-details
            class="search-field"
          ></v-text-field>
        </v-col>
        <!-- Search Button -->
        <v-col cols="12" sm="4" md="2" class="search-button-wrapper">
          <v-btn class="search-button" block @click="searchMovies">Search</v-btn>
        </v-col>
      </v-row>
  
      <!-- Search Results -->
      <v-row>
        <v-col
          v-for="(movie, index) in filteredMovies"
          :key="index"
          cols="12"
          sm="6"
          md="4"
        >
          <MovieCard
            :title="movie.title"
            :genre="movie.genre || 'N/A'"
            :year="parseInt(movie.year) || 'N/A'"
            :rating="getMovieRating(movie)"
            :poster="movie.poster || null"
          >
            <!-- Add to List Button -->
            <template #actions>
              <v-btn
                text
                color="success"
                @click="addToTopList(movie)"
                :disabled="isInTopList(movie)"
              >
                Add to List
              </v-btn>
            </template>
          </MovieCard>
        </v-col>
      </v-row>
    </v-container>
  </template>
  
  <script>
  import MovieCard from "@/components/MovieCard.vue";
  import axios from "axios";
  
  export default {
    name: "SearchMovies",
    components: {
      MovieCard,
    },
    props: {
      topList: {
        type: Array,
        required: true, 
      },
    },
    data() {
      return {
        searchQuery: "",
        searchResults: [],
      };
    },
    computed: {
      filteredMovies() {
        // Remove movies already in the top list from the search results
        return this.searchResults.filter(
          (movie) => !this.isInTopList(movie)
        );
      },
    },
    methods: {
      async searchMovies() {
        if (!this.searchQuery.trim()) {
          alert("Please enter a movie name to search.");
          return;
        }
  
        try {
          const response = await axios.get(
            `https://movieapi-app.azurewebsites.net/api/movies/name/${this.searchQuery}?limit=50`
          );
          this.searchResults = response.data;
        } catch (error) {
          console.error("Error fetching movies:", error);
          alert("Failed to fetch movies. Please try again.");
        }
      },
      isInTopList(movie) {
        // Check if the movie is already in the top list
        return this.topList.some((topListMovie) => topListMovie.id === movie.id);
      },
      addToTopList(movie) {
        if (!this.isInTopList(movie)) {
          this.$emit("add-to-toplist", movie);
          this.searchResults = this.searchResults.filter(
            (result) => result.id !== movie.id
          ); // Remove movie from results after adding
        }
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
  .compact-search-bar {
    margin-bottom: 20px;
  }
  
  .search-field {
    background-color: #f9f9f9;
    border-radius: 4px;
  }
  
  .search-button-wrapper {
    display: flex;
    align-items: center;
    justify-content: center;
  }
  
  .search-button {
    background-color: #f9f9f9 !important;
    color: #000 !important;
    border: 1px solid #dcdcdc !important;
    border-radius: 4px;
  }
  
  .search-button:hover {
    background-color: #e0e0e0 !important;
    color: #000 !important;
  }
  
  .movie-card {
    position: relative;
  }
  
  .movie-card .actions {
    display: flex;
    justify-content: flex-end;
    margin-top: 10px;
  }
  </style>
  