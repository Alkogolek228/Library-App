import axios from 'axios';

const API_URL = 'http://localhost:5242/api/books'; // Убедитесь, что URL правильный

const getAuthHeaders = () => {
  const token = localStorage.getItem('token');
  if (!token) {
    throw new Error('No token found');
  }
  return {
    headers: {
      Authorization: `Bearer ${token}`,
    },
  };
};

const refreshToken = async () => {
  const refreshToken = localStorage.getItem('refreshToken');
  if (!refreshToken) {
    throw new Error('No refresh token found');
  }
  try {
    const response = await axios.post('http://localhost:5242/api/auth/refresh', { refreshToken });
    localStorage.setItem('token', response.data.accessToken);
    localStorage.setItem('refreshToken', response.data.refreshToken);
  } catch (error) {
    console.error('Failed to refresh token', error);
    throw error;
  }
};

const handleRequest = async (requestFunc) => {
  try {
    return await requestFunc();
  } catch (error) {
    if (error.response && error.response.status === 401) {
      try {
        await refreshToken();
        return await requestFunc();
      } catch (refreshError) {
        console.error('Failed to refresh token', refreshError);
        throw refreshError;
      }
    } else {
      throw error;
    }
  }
};

export const getAllBooks = async (page, pageSize, search, author, genre) => {
  return handleRequest(async () => {
    const response = await axios.get(API_URL, {
      params: { page, pageSize, search, author, genre },
      ...getAuthHeaders(),
    });
    console.log("get books data:", response.data);
    return response.data;
  });
};

export const getBookById = async (id) => {
  return handleRequest(async () => {
    const response = await axios.get(`${API_URL}/${id}`, getAuthHeaders());
    return response.data;
  });
};

export const createBook = async (bookData) => {
  return handleRequest(async () => {
    const response = await axios.post(API_URL, bookData, {
      ...getAuthHeaders(),
      headers: {
        ...getAuthHeaders().headers,
        'Content-Type': 'application/json',
      },
    });
    return response.data;
  });
};

export const updateBook = async (id, bookData) => {
  return handleRequest(async () => {
    const response = await axios.put(`${API_URL}/${id}`, bookData, {
      ...getAuthHeaders(),
      headers: {
        ...getAuthHeaders().headers,
        'Content-Type': 'application/json',
      },
    });
    return response.data;
  });
};

export const deleteBook = async (id) => {
  return handleRequest(async () => {
    const response = await axios.delete(`${API_URL}/${id}`, getAuthHeaders());
    return response.data;
  });
};

export const borrowBook = async (id, returnBy) => {
  return handleRequest(async () => {
    const response = await axios.post(`${API_URL}/${id}/borrow`, { returnBy }, getAuthHeaders());
    return response.data;
  });
};