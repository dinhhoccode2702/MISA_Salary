import axios from 'axios';

const baseService = axios.create({
  baseURL:import.meta.env.VITE_API_URL,
  timeout: 30000,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Request Interceptor
baseService.interceptors.request.use(
  (config) => {
    // You can add auth tokens here if needed
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Response Interceptor
baseService.interceptors.response.use(
  (response) => {
    return response; // return full axios response so FE can inspect ServiceResult/Data
  },
  (error) => {
    // Global error handling can be done here with a Toast service
    // Normalize error for FE: if backend returns ServiceResult with IsSuccess=false,
    // axios will still resolve; but network/500 errors come here.
    console.error('API Error:', error.response || error.message);
    return Promise.reject(error);
  }
);

export default baseService;
