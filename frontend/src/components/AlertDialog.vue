<template>
    <v-dialog v-model="internalVisible" max-width="500" persistent>
      <v-card class="alert-dialog-card">
        <!-- Title Section -->
        <v-card-title class="alert-dialog-title">{{ title }}</v-card-title>
  
        <!-- Message Section -->
        <v-card-text class="alert-dialog-message">
          <p v-for="(message, index) in formattedMessages" :key="index">
            {{ message }}
          </p>
        </v-card-text>
  
        <!-- Action Section -->
        <v-card-actions class="alert-dialog-actions">
          <v-btn color="primary" class="alert-dialog-button" @click="closeDialog">
            Okay
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </template>
  
  <script>
  export default {
    name: "AlertDialog",
    props: {
      title: {
        type: String,
        default: "Notification",
      },
      message: {
        type: [String, Object, Array],
        default: "",
      },
      modelValue: {
        type: Boolean,
        required: true, 
      },
    },
    computed: {
      internalVisible: {
        get() {
          return this.modelValue;
        },
        set(value) {
          this.$emit("update:modelValue", value);
        },
      },
      formattedMessages() {
        if (typeof this.message === "string") {
          return [this.message];
        } else if (Array.isArray(this.message)) {
          return this.message;
        } else if (typeof this.message === "object" && this.message !== null) {
          return Object.entries(this.message).map(([key, value]) => `${key}: ${value}`);
        }
        return ["An unexpected error occurred."];
      },
    },
    methods: {
      closeDialog() {
        this.internalVisible = false;
      },
    },
  };
  </script>
  
  <style scoped>

  .alert-dialog-card {
    border-radius: 20px 20px 20px 20px; 
    overflow: hidden;
    padding: 0; 
    margin: 0; 
  }
  

  .alert-dialog-title {
    background-color: #9bd4f5; 
    color: black; 
    font-weight: bold;
    font-size: 18px;
    text-align: center;
    padding: 15px;

    margin: 0; 
  }
  

  .alert-dialog-message {
    text-align: center;
    font-size: 16px;
    color: #555;
    margin: 20px;
  }
  
 
  .alert-dialog-actions {
    display: flex;
    justify-content: center;
    padding: 20px;
    margin: 0; 
  }
  
  .alert-dialog-button {
    border-radius: 20px;
    font-weight: bold;
    padding: 10px 20px;
    font-size: 16px;
    width: 120px; 
    background-color: #2195f33a; 
    color: white;
    transition: background-color 0.3s;
  }
  

  .alert-dialog-button:hover {
    background-color: #f7f8f8; 
  }
  </style>
  