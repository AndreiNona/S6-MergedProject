const config = {
    development: {
      apiBaseUrl: "http://localhost:5205", 
    },
    production: {
      apiBaseUrl: "https://movieapi-app.azurewebsites.net", 
    },
  };
  
  // Export the correct configuration 
  const environment = process.env.NODE_ENV || "development";
  export default config[environment];