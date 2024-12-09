<template>
  <v-container>
    <h1>People</h1>
    <p>Search for people in the database.</p>

    <!-- Compact Search Bar -->
    <v-row align="center" justify="center" class="compact-search-bar">
      <v-col cols="12" sm="6" md="4">
        <v-text-field
          v-model="searchQuery"
          label="Search by Name"
          outlined
          hide-details
          dense
          class="search-field"
        ></v-text-field>
      </v-col>
      <v-col cols="12" sm="4" md="2">
        <v-text-field
          v-model="maxResults"
          label="Max Results"
          type="number"
          outlined
          hide-details
          dense
          class="search-field"
        ></v-text-field>
      </v-col>
      <v-col cols="12" sm="4" md="2" class="search-button-wrapper">
        <v-btn class="search-button" block @click="searchPeople">Search</v-btn>
      </v-col>
    </v-row>

    <!-- Display Results -->
    <v-row>
      <v-col
        v-for="person in people"
        :key="person.id"
        cols="12"
        sm="6"
        md="4"
        lg="3"
      >
        <PersonCard
          :personId="person.id"
          :name="person.name"
          :birth="person.birth"
        />
      </v-col>
    </v-row>
  </v-container>
</template>

<script>
import PersonCard from "@/components/PersonCard.vue";
import axios from "axios";
import config from "@/config.js"; 

export default {
  name: "PeopleView",
  components: {
    PersonCard,
  },
  data() {
    return {
      searchQuery: "",
      maxResults: 10,
      people: [],
    };
  },
  methods: {
    async searchPeople() {
      try {
        const baseUrl = config.apiBaseUrl;
        const response = await axios.get(
          `${baseUrl}/api/people/name/${this.searchQuery}?maxResults=${this.maxResults}`
        );
        this.people = response.data;
        console.log("Search Results:", this.people);
      } catch (error) {
        console.error("Error searching people:", error);
      }
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
  border-radius: 4px; /* Rounded edges */
  box-shadow: none !important; 
  transition: background-color 0.3s ease, color 0.3s ease;
}

.search-button:hover {
  background-color: #e0e0e0 !important; 
  color: #000 !important; 
}
</style>