import { createRouter, createWebHistory } from "vue-router";
import LoginRegister from "@/components/LoginRegister.vue";
import PeopleView from "@/views/PeopleView.vue";
import ProfileView from "@/views/ProfileView.vue";
import MovieView from "@/views/MovieView.vue";
import TopListEditView from "@/views/TopListEditView.vue";


const routes = [
  {
    path: "/",
    name: "Home",
    component: LoginRegister, 
  },
  {
    path: "/movies",
    name: "Movies",
    component: MovieView, 
  },
  {
    path: "/people",
    name: "People",
    component: PeopleView, 
  },
  {
    path: "/profile",
    name: "Profile",
    component: ProfileView,
    meta: { requiresAuth: true },
  },
  {
    path: "/toplist/:id/edit",
    name: "TopListEdit",
    component: TopListEditView,
    props: true, 
  },
  {
    path: "/movie/:id",
    name: "MovieDetails",
    component: () => import("@/views/MovieDetailsView.vue"), // Lazy-loaded route
  },
  {
    path: "/person/:id",
    name: "PersonDetails",
    component: () => import("@/views/PersonDetailsView.vue"),
  },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

// Navigation Guard
router.beforeEach((to, from, next) => {
  const isAuthenticated = !!localStorage.getItem("jwtToken"); // Check if token exists
  if (to.meta.requiresAuth && !isAuthenticated) {
    alert("You need to log in to access this page!");
    next({ name: "Home" }); // Redirect to the home page
  } else {
    next(); // Allow navigation
  }
});

export default router;
