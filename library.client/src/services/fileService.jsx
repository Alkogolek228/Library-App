import axios from 'axios';

const API_URL = 'http://localhost:5242/api/files'; // Убедитесь, что URL правильный

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

export const uploadFile = async (file) => {
  const formData = new FormData();
  formData.append('file', file);

  return handleRequest(async () => {
    const response = await axios.post(`${API_URL}/upload`, formData, {
      ...getAuthHeaders(),
      headers: {
        ...getAuthHeaders().headers,
        'Content-Type': 'multipart/form-data',
      },
    });
    return response.data;
  });
};

export const deleteFile = async (filePath) => {
  return handleRequest(async () => {
    const response = await axios.delete(`${API_URL}/delete`, {
      ...getAuthHeaders(),
      data: { filePath },
    });
    return response.data;
  });
};