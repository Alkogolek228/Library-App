import axios from 'axios';

const API_URL = 'http://localhost:5242/api/authors';

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

export const getAllAuthors = async () => {
  const response = await axios.get(API_URL, getAuthHeaders());
  return response.data;
};

export const createAuthor = async (author) => {
  const response = await axios.post(API_URL, author, getAuthHeaders());
  return response.data;
};