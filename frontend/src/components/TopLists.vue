<template>
  <v-container>
    <h2>Your Top Lists</h2>
    <p v-if="topLists.length === 0" class="no-top-lists">You have no top lists yet.</p>
    <v-row v-else>
      <v-col
        v-for="(topList, index) in topLists"
        :key="index"
        cols="12"
        sm="6"
        md="4"
      >
        <v-card outlined class="top-list-card">
          <!-- Top Left X Button -->
          <v-btn
            icon
            class="delete-button"
            @click="showConfirmDeleteDialog(topList.id)"
          >
            <v-icon>mdi-close</v-icon>
          </v-btn>

          <!-- Top List Name -->
          <v-card-title class="top-list-title">{{ topList.name }}</v-card-title>

          <!-- Movie Posters -->
          <div class="poster-container" v-if="topList.movies && topList.movies.length > 0">
            <img
              v-for="(movie, idx) in topList.movies.slice(0, 3)"
              :key="idx"
              :src="getMoviePoster(movie)"
              alt="Movie Poster"
              class="movie-poster"
            />
          </div>
          <div v-else class="loading-placeholder">Loading posters...</div>

          <!-- Movie Titles -->
          <v-card-text class="movie-titles">
            <ol>
              <li v-for="(title, idx) in topList.movieTitles" :key="idx">
                {{ title }}
              </li>
            </ol>
            <p class="total-movies">Total Movies: {{ topList.movieIds.length }}</p>
          </v-card-text>

          <!-- Bottom Actions -->
          <v-card-actions class="bottom-actions">
            <v-btn text color="primary" @click="navigateToEditTopList(topList)">Edit</v-btn>
            <v-btn icon color="primary" @click="navigateToMovies">
              <v-icon>mdi-plus</v-icon>
            </v-btn>
          </v-card-actions>
        </v-card>
      </v-col>
    </v-row>

    <!-- Alert Dialog -->
    <AlertDialog v-model="alertVisible" :message="alertMessage" />

    <!-- Confirm Dialog -->
    <ConfirmDialog
      v-model="confirmVisible"
      :message="confirmMessage"
      @confirm="handleConfirm"
      @cancel="handleCancel"
    />
  </v-container>
</template>


<script>
import AlertDialog from "@/components/AlertDialog.vue";
import ConfirmDialog from "@/components/ConfirmDialog.vue";
import config from "@/config.js"; 
export default {
  props: {
    topLists: {
      type: Array,
      required: true,
    },
  },
  components: {
    AlertDialog, 
    ConfirmDialog, 
  },
  data() {
    return {
      alertVisible: false, 
      alertMessage: "", 
      confirmVisible: false, 
      confirmMessage: "", 
      confirmCallback: null, 
    };
  },
  methods: {
    async fetchMovieTitlesForTopLists() {
      for (const topList of this.topLists) {
        topList.movieTitles = [];
        topList.movies = [];

        for (const movieId of topList.movieIds.slice(0, 3)) {
          try {
            const baseUrl = config.apiBaseUrl;
            const response = await fetch(`${baseUrl}/api/movies/omdb/${movieId}`);
            const data = await response.json();
            topList.movieTitles.push(data.title);

            if (data.poster && data.poster !== "N/A") {
              topList.movies.push(data);
            }
          } catch (error) {
            console.error(`Error fetching movie for ID ${movieId}:`, error);
          }
        }

        if (topList.movies.length === 0) {
          topList.movies.push({ poster: "/images/noPoserAvailable.png" });
        }
      }
    },
    getMoviePoster(movie) {
      return movie.poster || "/images/noPoserAvailable.png";
    },
    navigateToMovies() {
      this.$router.push("/movies");
    },
    navigateToEditTopList(topList) {
      this.$router.push({
        name: "TopListEdit",
        params: {
          id: topList.id,
        },
      });
    },
    showAlertDialog(message) {
      this.alertMessage = message;
      this.alertVisible = true;
    },
    showConfirmDeleteDialog(topListId) {
      this.confirmMessage = "Are you sure you want to delete this top list?";
      this.confirmCallback = () => this.deleteTopList(topListId);
      this.confirmVisible = true;
    },
    handleConfirm() {
      if (this.confirmCallback) {
        this.confirmCallback();
      }
      this.confirmVisible = false;
    },
    handleCancel() {
      this.confirmVisible = false;
    },
    async deleteTopList(topListId) {
      try {
        const baseUrl = config.apiBaseUrl;
        await fetch(`${baseUrl}/api/toplist/${topListId}`, {
          method: "DELETE",
          headers: {
            Authorization: `Bearer ${localStorage.getItem("jwtToken")}`,
          },
        });
        this.$emit("delete-toplist", topListId);
        this.showAlertDialog("Top list deleted successfully.");
      } catch (error) {
        console.error("Error deleting top list:", error);
        this.showAlertDialog("Failed to delete top list. Please try again.");
      }
    },
  },
  async mounted() {
    console.log("Fetching movie titles and posters for top lists...");
    await this.fetchMovieTitlesForTopLists();
  },
};
</script>

<style scoped>
/* General Container */
.v-container {
  background-color: #121212;
  color: #f0f0f0;
  padding: 20px;
  border-radius: 10px;
}

/* Top List Card */
.top-list-card {
  background-color: #1e1e1e;
  color: #f0f0f0;
  border-radius: 8px;
  box-shadow: 0 4px 10px rgba(0, 0, 0, 0.3);
  padding: 10px;
  display: flex;
  flex-direction: column;
  justify-content: space-between;
}

.top-list-title {
  color: #ffd700;
  font-weight: bold;
  font-size: 1.2rem;
  margin-bottom: 10px;
}

/* Poster Container */
.poster-container {
  display: flex;
  justify-content: space-evenly;
  align-items: center;
  margin-bottom: 10px;
}

.movie-poster {
  width: 80px;
  height: 120px;
  object-fit: cover;
  border: 1px solid #444;
  border-radius: 5px;
}

/* Movie Titles */
.movie-titles ol {
  padding-left: 20px;
  list-style-type: decimal;
}

.movie-titles li {
  color: #f0f0f0;
  margin-bottom: 5px;
}

.total-movies {
  text-align: right;
  font-weight: bold;
  color: #ffd700;
  margin-top: 10px;
}

/* Buttons */
.delete-button {
  position: absolute;
  top: 10px;
  right: 10px;
  background-color: transparent;
  color: #f0f0f0;
  z-index: 10;
}

.delete-button:hover {
  color: #ff0000;
}

.bottom-actions {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.v-btn {
  background-color: #333;
  color: #f0f0f0;
}

.v-btn:hover {
  background-color: #444;
}
</style>

  