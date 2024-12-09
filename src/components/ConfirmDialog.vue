<template>
    <v-dialog v-model="visible" max-width="500" persistent>
      <v-card class="confirm-dialog-card">
        <!-- Title Section -->
        <v-card-title class="confirm-dialog-title">
          {{ title }}
        </v-card-title>
  
        <!-- Message Section -->
        <v-card-text class="confirm-dialog-message">
          {{ message }}
        </v-card-text>
  
        <!-- Buttons Section -->
        <v-card-actions class="confirm-dialog-actions">
          <v-btn
            color="error"
            class="confirm-dialog-button"
            @click="confirm"
          >
            {{ confirmText || "CONFIRM" }}
          </v-btn>
          <v-btn
            color="success"
            class="confirm-dialog-button"
            @click="cancel"
          >
            {{ cancelText || "CANCEL" }}
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </template>
  
  
  <script>
  export default {
    name: "ConfirmDialog",
    props: {
      modelValue: {
        type: Boolean,
        required: true,
      },
      title: {
        type: String,
        default: "Are you sure?",
      },
      message: {
        type: String,
        required: true,
      },
      confirmText: {
        type: String,
        default: "Confirm",
      },
      cancelText: {
        type: String,
        default: "Cancel",
      },
    },
    emits: ["update:modelValue", "confirm", "cancel"],
    data() {
      return {
        visible: this.modelValue,
      };
    },
    watch: {
      modelValue(newValue) {
        this.visible = newValue;
      },
      visible(newValue) {
        this.$emit("update:modelValue", newValue);
      },
    },
    methods: {
      confirm() {
        this.$emit("confirm");
        this.visible = false;
      },
      cancel() {
        this.$emit("cancel");
        this.visible = false;
      },
    },
  };
  </script>
  
  
  
  
  
  <style scoped>

.confirm-dialog-card {
  border-radius: 20px 20px 20px 20px; 
  overflow: hidden; 
  padding: 0; 
  margin: 0; 
}

.confirm-dialog-title {
  background-color: #ffb8b8; 
  color: black; 
  font-weight: bold;
  font-size: 18px;
  text-align: center;
  padding: 15px;

  margin: 0; 
}

.confirm-dialog-message {
  text-align: center;
  font-size: 16px;
  color: #555;
  margin: 20px;
}

.confirm-dialog-actions {
  display: flex;
  justify-content: space-evenly;
  padding: 20px;
  margin: 0; 
}


.confirm-dialog-button {
  border-radius: 20px;
  font-weight: bold;
  padding: 10px 20px;
  font-size: 16px;
  width: 120px; 
}


.confirm-dialog-button[color="error"] {
  background-color: #f44336; 
  color: white;
  transition: background-color 0.3s;
}

.confirm-dialog-button[color="error"]:hover {
  background-color: #d32f2f; 
}


.confirm-dialog-button[color="success"] {
  background-color: #4caf50; 
  color: white;
  transition: background-color 0.3s;
}

.confirm-dialog-button[color="success"]:hover {
  background-color: #388e3c; 
}
</style>
