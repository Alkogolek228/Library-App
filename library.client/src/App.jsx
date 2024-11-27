import React, { useEffect } from 'react';
import { BrowserRouter as Router, Routes, Route, Link } from 'react-router-dom';
import { HubConnectionBuilder } from '@microsoft/signalr';
import Home from './components/Home/Home';
import Login from './components/Auth/Login';
import Register from './components/Auth/Register';
import BookList from './components/Books/BookList';
import BookDetail from './components/Books/BookDetail';
import BookForm from './components/Books/BookForm';
import UserBooks from './components/User/UserBooks';
import UserBookDetail from './components/User/UserBookDetail';

const App = () => {
  const userRole = localStorage.getItem('userRole');

  useEffect(() => {
    const connection = new HubConnectionBuilder()
      .withUrl('http://localhost:5242/notificationHub', {
        accessTokenFactory: () => localStorage.getItem('token')
      })
      .withAutomaticReconnect()
      .build();

    connection.start()
      .then(() => {
        console.log('Connected to SignalR hub');
      })
      .catch(error => console.error('Connection to SignalR hub failed', error));

    connection.on('ReceiveNotification', message => {
      alert(message);
    });

    return () => {
      connection.stop();
    };
  }, []);

  return (
    <Router>
      <div>
        <nav>
          <ul>
            <li><Link to="/">Home</Link></li>
            <li><Link to="/login">Login</Link></li>
            <li><Link to="/register">Register</Link></li>
            <li><Link to="/books">Books</Link></li>
            {userRole === '0' && (
              <>
                <li><Link to="/admin/books/new">Add Book</Link></li>
              </>
            )}
            <li><Link to="/user/books">My Books</Link></li>
          </ul>
        </nav>
        <Routes>
          <Route path="/" element={<Home />} />
          <Route path="/login" element={<Login />} />
          <Route path="/register" element={<Register />} />
          <Route path="/books" element={<BookList />} />
          <Route path="/books/:id" element={<BookDetail />} />
          <Route path="/admin/books/new" element={<BookForm />} />
          <Route path="/admin/books/:id/edit" element={<BookForm />} />
          <Route path="/user/books" element={<UserBooks />} />
          <Route path="/user/books/:id" element={<UserBookDetail />} />
        </Routes>
      </div>
    </Router>
  );
};

export default App;