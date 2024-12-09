<template>
    <div v-if="loading" class="loading-radar">
      <div class="circle"></div>
    </div>
  </template>
  
  <script>
  export default {
    name: "LoadingRadar",
    props: {
      loading: {
        type: Boolean,
        required: true,
      },
    },
  };
  </script>
  
  <style scoped>
  .loading-radar {
    display: flex;
    justify-content: center;
    align-items: center;
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    background-color: rgba(0, 0, 0, 0.5); 
    z-index: 1000;
  }
  
  .circle {
    position: relative;
    width: 80px;
    height: 80px;
    border-radius: 50%;
    background: conic-gradient(
      white 0%,
      red 0% 25%,
      white 25% 100%
    );
    animation: radar-spin 1s linear infinite;
  }
  
  @keyframes radar-spin {
    from {
      transform: rotate(0deg);
    }
    to {
      transform: rotate(360deg);
    }
  }
  </style>
  