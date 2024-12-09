<template>
   <body>
  <v-app>
    <!-- App Bar -->
    <v-app-bar app>
  <!-- Hamburger Icon for Navigation Drawer -->
  <v-app-bar-nav-icon @click="drawer = !drawer" class="d-sm-none" />

  <!-- Title -->
  <router-link to="/" class="title-link">
    <v-toolbar-title>Movie Enthusiast</v-toolbar-title>
  </router-link>

  <v-spacer></v-spacer>

  <!-- Top Bar Buttons (Visible on Larger Screens) -->
  <v-btn class="nav-btn d-none d-sm-flex" text to="/movies" v-bind="$router-link">Movies</v-btn>
  <v-btn class="nav-btn d-none d-sm-flex" text to="/people" v-bind="$router-link">Talents</v-btn>
  <v-btn
    v-if="isAuthenticated"
    class="nav-btn d-none d-sm-flex"
    text
    to="/profile"
    v-bind="$router-link"
  >Profile</v-btn>
  <v-btn
    v-if="!isAuthenticated"
    class="nav-btn d-none d-sm-flex"
    text
    @click="dialog = true"
  >Login</v-btn>
  <v-btn
    v-else
    class="nav-btn d-none d-sm-flex"
    text
    @click="logout"
  >Logout</v-btn>
</v-app-bar>

    <!-- Navigation Drawer for Small Screens -->
    <v-navigation-drawer
      v-model="drawer"
      app
      temporary
    >
      <v-list>
        <v-list-item>
          <v-list-item-title>Navigation</v-list-item-title>
        </v-list-item>
        <v-divider></v-divider>
        <v-list-item
          v-for="(item, index) in filteredMenuItems"
          :key="index"
          :to="item.route"
          v-bind="$router-link"
          @click="drawer = false"
        >
          <v-list-item-title>{{ item.label }}</v-list-item-title>
        </v-list-item>
        <v-divider></v-divider>
        <v-list-item
          v-if="!isAuthenticated"
          @click="openLogin()"
        >
          <v-list-item-title>Login</v-list-item-title>
        </v-list-item>
        <v-list-item
          v-else
          @click="logout"
        >
          <v-list-item-title>Logout</v-list-item-title>
        </v-list-item>
      </v-list>
    </v-navigation-drawer>

    <!-- Main Content -->
    <v-main>
      <router-view />
    </v-main>

    <!-- Login/Register Dialog -->
    <LoginRegister v-model="dialog" v-if="dialog" @authenticated="handleAuthenticated" />

    <!-- Alert Dialog -->
    <AlertDialog
      :modelValue="alertDialogVisible"
      :title="alertTitle"
      :message="alertMessage"
      @update:modelValue="alertDialogVisible = $event"
    />
  </v-app>
 </body>
</template>

<script>

import { ref, computed, onMounted } from "vue";
import LoginRegister from "./components/LoginRegister.vue";
import AlertDialog from "./components/AlertDialog.vue";


export default {
  name: "App",
  components: {
    LoginRegister,
    AlertDialog,
  },
  setup() {
    const isAuthenticated = ref(false);
    const dialog = ref(false);
    const drawer = ref(false);

  
    const alertDialogVisible = ref(false);
    const alertTitle = ref("");
    const alertMessage = ref("");

    const menuItems = [
      { label: "Movies", route: "/movies" },
      { label: "Talents", route: "/people" },
      { label: "Profile", route: "/profile", requiresAuth: true },
    ];

    // Filter menu items
    const filteredMenuItems = computed(() =>
      menuItems.filter((item) => !(item.requiresAuth && !isAuthenticated.value))
    );



    const showAlert = (title, message) => {
      alertTitle.value = title;
      alertMessage.value = message;
      alertDialogVisible.value = true;
    };

    const logout = () => {
      localStorage.removeItem("jwtToken");
      isAuthenticated.value = false;
      showAlert("Logout", "Logged out successfully!");
    };

    const handleAuthenticated = (token) => {
      if (token) {
        localStorage.setItem("jwtToken", token);
        isAuthenticated.value = true;
        dialog.value = false;
        showAlert("Login", "Logged in successfully!");
      }
    };

    const openLogin = () => {
      dialog.value = true;
      drawer.value = false;
    };

    onMounted(() => {
      const token = localStorage.getItem("jwtToken");
      if (token) {
        isAuthenticated.value = true;
      }
    });

    return {
      isAuthenticated,
      dialog,
      logout,
      handleAuthenticated,
      alertDialogVisible,
      alertTitle,
      alertMessage,
      drawer,
      filteredMenuItems,
      openLogin,
    };
  },
};
</script>

<style scoped>
/* Sone work some get overwritten  */

.v-app-bar {
  background-color: #000 !important; 
  color: #fff !important; 
  padding: 0 20px !important;
  box-shadow: none !important;
}

/* Title Styling */
.title-link {
  text-decoration: none;
  display: flex;
  align-items: center;
}

.title-link .v-toolbar-title {
  font-family: "Roboto Condensed", sans-serif !important; 
  font-size: 24px !important;
  font-weight: bold !important;
  color: #fff !important;
  text-transform: uppercase !important;
}


.nav-btn {
  font-size: 14px !important;
  color: #fff !important;
  text-transform: uppercase !important;
  margin: 0 10px !important;
  padding: 5px 10px !important;
  border-radius: 10px !important;
  transition: background-color 0.3s ease !important;
}

.nav-btn:hover {
  background-color: #ffb8b8 !important; 
  color: #000 !important; 
}


.v-app-bar .v-app-bar-nav-icon {
  color: #fff !important;
}


.v-navigation-drawer {
  background-color: #ffffff; 
}
</style>
