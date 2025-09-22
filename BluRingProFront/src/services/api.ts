import axios from "axios";

const api = axios.create({
  baseURL: "http://localhost:7123/api", // 👈 ajusta al puerto de tu API
  headers: {
    "Content-Type": "application/json",
  },
});

export default api;
