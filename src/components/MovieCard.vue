<template>
  <v-card class="movie-card" outlined>
    <!-- Poster Image (Clickable for Movie Details) -->
    <router-link
      :to="{ name: 'MovieDetails', params: { id: movieId } }"
      class="movie-poster-link"
    >
      <v-img
        class="movie-poster"
        :src="isValidPoster(poster) ? poster : defaultPoster"
        alt="Movie Poster"
        contain
      ></v-img>
    </router-link>

    <!-- Movie Details -->
    <v-card-title class="movie-title">
      <div class="title-text">{{ title }}</div>
      <span class="movie-rating">
        {{ displayRating }}
      </span>
    </v-card-title>

    <v-card-subtitle class="movie-details">
      <div v-if="genre && genre !== 'Unknown' && genre !== 'N/A'">
        <strong>Genre:</strong> {{ genre }}
      </div>
      <div v-if="year && year !== 'N/A'">
        <strong>Year:</strong> {{ year }}
      </div>
    </v-card-subtitle>

    <!-- Add to Top List Button -->
    <v-card-actions>
      <v-btn
        color="success"
        block
        @click.stop="fetchUserTopLists"
      >
        Add to Top List
      </v-btn>
    </v-card-actions>

    <!-- Dialog for Selecting Top List -->
    <v-dialog v-model="topListDialog" max-width="400">
      <v-card>
        <v-card-title>Select a Top List</v-card-title>
        <v-card-text>
          <v-list>
            <v-list-item
              v-for="topList in userTopLists"
              :key="topList.id"
              @click.stop="addMovieToTopList(topList)"
              class="top-list-item"
            >
              {{ topList.name }}
            </v-list-item>
          </v-list>
        </v-card-text>
        <v-card-actions>
          <v-btn text color="error" @click="topListDialog = false">Cancel</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <!-- Alert Dialog -->
    <AlertDialog
      v-model="alertDialogVisible"
      :message="alertDialogMessage"
    />
  </v-card>
</template>




<script>
import axios from "axios";
import AlertDialog from "@/components/AlertDialog.vue";
import config from "@/config.js"; 

export default {
  name: "MovieCard",
  components: {
    AlertDialog, 
  },
  props: {
    title: {
      type: String,
      required: true,
    },
    genre: {
      type: String,
      default: null,
    },
    year: {
      type: Number,
      default: null,
    },
    rating: {
      type: String,
      default: null,
    },
    poster: {
      type: String,
      default: null,
    },
    movieId: {
      type: Number,
      required: true,
    },
  },
  data() {
    return {
      topListDialog: false,
      userTopLists: [],
      alertDialogVisible: false, 
      alertDialogMessage: "", 
    };
  },
  computed: {
    defaultPoster() {
      return "/images/noPoserAvailable.png";
    },
    displayRating() {
      return this.rating && this.rating !== "N/A" ? this.rating : "No rating";
    },
  },
  methods: {
    isValidPoster(poster) {
      return poster && poster !== "N/A";
    },
    async fetchUserTopLists() {
      try {
        const baseUrl = config.apiBaseUrl;
        const response = await axios.get(`${baseUrl}/api/toplist/my-toplists`, {
          headers: {
            Authorization: `Bearer ${localStorage.getItem("jwtToken")}`,
          },
        });
        this.userTopLists = response.data;
        this.topListDialog = true; // Open the dialog
      } catch (error) {
        if (error.response && error.response.status === 401) {
          this.showAlert("You can't add a top list while not logged in.");
        } else {
          console.error("Error fetching user top lists:", error);
          this.showAlert("Failed to load top lists. Please try again.");
        }
      }
    },
    async addMovieToTopList(topList) {
      if (topList.movieIds.includes(this.movieId)) {
        this.showAlert(`"${this.title}" is already in "${topList.name}"`);
        return;
      }

      try {
        const baseUrl = config.apiBaseUrl;
        await axios.put(
          `${baseUrl}/api/toplist/${topList.id}/update`,
          {
            movieIds: [...topList.movieIds, this.movieId],
          },
          {
            headers: {
              Authorization: `Bearer ${localStorage.getItem("jwtToken")}`,
            },
          }
        );
        this.showAlert(`"${this.title}" added to "${topList.name}" successfully!`);
        this.topListDialog = false;
      } catch (error) {
        if (error.response && error.response.status === 401) {
          this.showAlert("Unauthorized action. Please log in again.");
        } else {
          console.error("Error adding movie to top list:", error);
          this.showAlert("Failed to add movie to top list. Please try again.");
        }
      }
    },
    showAlert(message) {
      this.alertDialogMessage = message;
      this.alertDialogVisible = true;
    },
  },
};
</script>

<style scoped>
.movie-card {
  width: 320px;
  margin: 10px;
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  border-radius: 10px;
  box-shadow: 0 4px 10px rgba(0, 0, 0, 0.1);
  background-color: #000; 
  color: #fff; 
}

.movie-poster-link {
  text-decoration: none;
  display: block;
}

.movie-poster {
  width: 100%;
  height: 250px;
  object-fit: cover;
  border-bottom: 2px solid #444; 
}

.movie-title {
  display: flex;
  justify-content: space-between;
  align-items: center;
  font-size: 20px;
  font-weight: bold;
  margin: 10px;
  text-align: left;
  white-space: normal;
  word-wrap: break-word;
  color: #fff; /* Ensure title text is visible */
}

.movie-rating {
  color: #2196f3; /* Blue color for rating */
  font-weight: bold;
  margin-left: 10px;
  white-space: nowrap;
}

.movie-details {
  display: flex;
  flex-direction: column;
  font-size: 14px;
  color: #ccc; /* Light gray for subtle contrast */
  margin: 0 10px 10px;
}

.movie-footer {
  font-size: 14px;
  color: gray;
  margin: 0 10px;
  display: flex;
  justify-content: space-between;
}

.footer-item {
  flex: 1;
  text-align: left;
}

.v-btn {
  text-transform: uppercase;
  font-weight: bold;
  background-color: #333; /* Dark button background for consistency */
  color: #fff; /* White text */
}
</style>