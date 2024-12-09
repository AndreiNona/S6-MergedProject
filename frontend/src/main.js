import { createApp } from 'vue';
import App from './App.vue';
import { createVuetify } from 'vuetify';
import 'vuetify/styles'; 
import '@mdi/font/css/materialdesignicons.css'; // Material Design Icons
import * as components from 'vuetify/components'; 
import * as directives from 'vuetify/directives'; 

import AlertDialog from "@/components/AlertDialog.vue"; // AlertDialog Component
import ConfirmDialog from "@/components/ConfirmDialog.vue"; // ConfirmDialog Component

import router from './router'; // Vue Router setup

import './assets/main.css'; //Not working for some reason

const vuetify = createVuetify({
  components,
  directives,
});

const app = createApp(App);

app.use(vuetify); // Use Vuetify
app.use(router);  // Use Vue Router

// Globally register components
app.component("AlertDialog", AlertDialog);
app.component("ConfirmDialog", ConfirmDialog);

app.mount('#app');
