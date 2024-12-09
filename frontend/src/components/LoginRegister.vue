<template>
  <v-dialog v-model="dialog" max-width="500">
    <v-card>
      <v-card-title class="title-container">
        <span class="title-text">{{ isLogin ? 'Login' : 'Register' }}</span>
        <v-spacer></v-spacer>
        <v-btn
          icon
          class="close-button"
          @click="closeDialog"
        >
          <v-icon>mdi-close</v-icon>
        </v-btn>
      </v-card-title>
      <v-card-text>
        <v-text-field label="Username" v-model="username" />
        <v-text-field v-if="!isLogin" label="Email" v-model="email" />
        <v-text-field label="Password" v-model="password" type="password" />
      </v-card-text>
      <v-card-actions>
        <v-spacer></v-spacer>
        <v-btn color="primary" @click="isLogin ? login() : register()">
          {{ isLogin ? 'Login' : 'Register' }}
        </v-btn>
      </v-card-actions>
      <v-card-actions>
        <v-spacer></v-spacer>
        <v-btn text @click="toggleForm">
          {{ isLogin ? 'Register?' : 'Login?' }}
        </v-btn>
      </v-card-actions>
    </v-card>
    <!-- Alert Dialog Component -->
    <AlertDialog
      v-model="showAlert"
      :message="alertMessage"
    />
  </v-dialog>
</template>

<script>
import { ref, watch } from 'vue';
import axios from 'axios';
import AlertDialog from './AlertDialog.vue';
import config from "@/config.js";

export default {
  name: 'LoginRegister',
  components: {
    AlertDialog,
  },
  props: {
    modelValue: {
      type: Boolean,
      required: true,
    },
  },
  emits: ['update:modelValue', 'authenticated'],
  setup(props, { emit }) {
    const dialog = ref(props.modelValue);
    const isLogin = ref(true);
    const username = ref('');
    const email = ref('');
    const password = ref('');
    const showAlert = ref(false);
    const alertMessage = ref('');

    const baseUrl = `${config.apiBaseUrl}/api/Auth`;

    const toggleForm = () => {
      isLogin.value = !isLogin.value;
    };

    const login = async () => {
      try {
        const credentials = {
          username: username.value,
          password: password.value,
        };
        const response = await axios.post(`${baseUrl}/login`, credentials);
        if (response.data && response.data.token) {
          emit('authenticated', response.data.token);
          dialog.value = false; // Close dialog
          alertMessage.value = 'Logged in successfully!';
          showAlert.value = true;
        }
      } catch (error) {
        console.error('Login failed:', error);
        alertMessage.value = error.response?.data?.message || 'Login failed. Please check your credentials.';
        showAlert.value = true;
      }
    };

    const register = async () => {
      try {
        const registrationData = {
          username: username.value,
          email: email.value,
          password: password.value,
        };
        const response = await axios.post(`${baseUrl}/register`, registrationData);

        if (response.status === 200) {
          // Automatically log in after successful registration
          const loginResponse = await axios.post(`${baseUrl}/login`, {
            username: username.value,
            password: password.value,
          });

          if (loginResponse.data && loginResponse.data.token) {
            emit('authenticated', loginResponse.data.token);
            dialog.value = false; // Close dialog
            alertMessage.value = 'Registration successful! You are now logged in.';
            showAlert.value = true;
          }
        }
      } catch (error) {
        console.error('Registration failed:', error);
        alertMessage.value = error.response?.data?.message || 'Registration failed. Please check your inputs.';
        showAlert.value = true;
      }
    };

    const closeDialog = () => {
      emit('update:modelValue', false);
    };

    // Watch for changes in the parent and sync dialog state
    watch(
      () => props.modelValue,
      (newVal) => {
        dialog.value = newVal;
      }
    );

    // Watch for changes in dialog state and emit updates
    watch(
      dialog,
      (newVal) => {
        emit('update:modelValue', newVal);
      }
    );

    return {
      dialog,
      isLogin,
      username,
      email,
      password,
      showAlert,
      alertMessage,
      toggleForm,
      login,
      register,
      closeDialog,
    };
  },
};
</script>



  
  <style scoped>
  .title-container {
    display: flex;
    align-items: center; 
    justify-content: space-between; 
  }
  
  .title-text {
    font-size: 20px; 
    font-weight: bold; 
  }
  
  .close-button {
    background-color: gray;
    border-radius: 0; 
    width: 36px;
    height: 36px;
    display: flex;
    justify-content: center;
    align-items: center;
    transition: background-color 0.3s, border-radius 0.3s;
  }
  
  .close-button:hover {
    background-color: red;
    border-radius: 50%; 
    cursor: pointer;
  }
  </style>  