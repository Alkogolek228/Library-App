import React, { useState } from 'react';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';

const Login = () => {
  const [userName, setUserName] = useState('');
  const [password, setPassword] = useState('');
  const navigate = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      const response = await axios.post('http://localhost:5242/api/auth/login', { userName, password });
      console.log("Response data:", response.data); // Вывод данных ответа в консоль
      localStorage.setItem('token', response.data.accessToken);
      localStorage.setItem('refreshToken', response.data.refreshToken);
      localStorage.setItem('userId', response.data.user.id); // Сохранение userId
      localStorage.setItem('userRole', response.data.user.role); // Сохранение userRole
      console.log('userId', response.data.user.id);
      console.log('userRole', response.data.user.role);
      navigate('/'); // Перенаправление на домашнюю страницу
    } catch (error) {
      console.error('Login failed', error);
    }
  };

  return (
    <form onSubmit={handleSubmit}>
      <input type="text" value={userName} onChange={(e) => setUserName(e.target.value)} placeholder="Username" required />
      <input type="password" value={password} onChange={(e) => setPassword(e.target.value)} placeholder="Password" required />
      <button type="submit">Login</button>
    </form>
  );
};

export default Login;