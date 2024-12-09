<template>
  <v-card class="person-card" outlined>
    <!-- Flipping Content -->
    <div class="flip-container">
      <!-- Front Side -->
      <div class="card-side front-side" :class="{ flipped: flipped }" @click="flipCard">
        <v-card-text>
          <h3 class="person-name">{{ name }}</h3>
          <p class="person-birth">Birth Year: {{ birth || "Unknown" }}</p>
        </v-card-text>
      </div>

      <!-- Back Side -->
      <div class="card-side back-side" :class="{ flipped: flipped }" @click="flipCard">
        <v-card-text>
          <h3 class="back-title">{{ primaryRoleText }}</h3>
          <ul>
            <li v-for="(movie, index) in primaryMovies.slice(0, 3)" :key="index">
              {{ movie.title }} ({{ movie.year }})
            </li>
          </ul>
          <p v-if="secondaryMovies.length > 0" class="secondary-role-text">
            {{ secondaryRoleText }} {{ secondaryMovies[0].title }}
          </p>
        </v-card-text>
      </div>
    </div>

    <!-- View Details Button -->
    <v-card-actions class="view-details">
      <v-btn color="primary" text @click.stop="goToPersonDetails">View Details</v-btn>
    </v-card-actions>
  </v-card>
</template>

<script>
import config from "@/config.js"; 

export default {
  name: "PersonCard",
  props: {
    name: {
      type: String,
      required: true,
    },
    birth: {
      type: [Number, String],
      default: "Unknown",
    },
    personId: {
      type: Number,
      required: true,
    },
  },
  data() {
    return {
      flipped: false,
      primaryRoleText: "",
      secondaryRoleText: "",
      primaryMovies: [],
      secondaryMovies: [],
    };
  },
  methods: {
    async flipCard() {
      this.flipped = !this.flipped;
      if (this.flipped && this.primaryMovies.length === 0) {
        await this.fetchRoleAndMovies();
      }
    },
    async fetchRoleAndMovies() {
      const baseUrl = `${config.apiBaseUrl}/api/people`;
      try {
        const response = await fetch(`${baseUrl}/${this.personId}/role`);
        const data = await response.json();

        if (data.role === "Director" && data.directedMovies.length > 0) {
          this.primaryRoleText = "Directed";
          this.primaryMovies = data.directedMovies;
          if (data.starredMovies && data.starredMovies.length > 0) {
            this.secondaryRoleText = "They also starred in";
            this.secondaryMovies = data.starredMovies;
          }
        } else if (data.role === "Star" && data.starredMovies.length > 0) {
          this.primaryRoleText = "Starred in";
          this.primaryMovies = data.starredMovies;
          if (data.directedMovies && data.directedMovies.length > 0) {
            this.secondaryRoleText = "They also directed";
            this.secondaryMovies = data.directedMovies;
          }
        } else {
          this.primaryRoleText = "No recent works found";
        }
      } catch (error) {
        console.error("Error fetching role and movies:", error);
        this.primaryRoleText = "Error fetching works";
      }
    },
    goToPersonDetails() {
      this.$router.push({ name: "PersonDetails", params: { id: this.personId } });
    },
  },
};
</script>

<style scoped>
.person-card {
  margin: 10px;
  padding: 10px;
  text-align: center;
  max-width: 300px;
  perspective: 1000px; /* Enable 3D effect */
}

.flip-container {
  position: relative;
  width: 100%;
  height: 200px;
}

.card-side {
  backface-visibility: hidden;
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  transition: transform 0.6s;
}

.front-side {
  transform: rotateY(0deg);
}

.back-side {
  transform: rotateY(180deg);
}

.person-card .flipped.front-side {
  transform: rotateY(180deg);
}

.person-card .flipped.back-side {
  transform: rotateY(0deg);
}

.view-details {
  margin-top: 10px;
}

.back-title {
  font-size: 1.2rem;
  font-weight: bold;
  margin-bottom: 10px;
}

.secondary-role-text {
  margin-top: 10px;
  font-style: italic;
  color: gray;
}
</style>
