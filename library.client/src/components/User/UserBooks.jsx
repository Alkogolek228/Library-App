import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import axios from 'axios';

const UserBooks = () => {
  const [books, setBooks] = useState([]);

  useEffect(() => {
    const fetchBooks = async () => {
      try {
        const userId = localStorage.getItem('userId'); 
        const token = localStorage.getItem('token'); 
        if (!userId) {
          throw new Error('User ID not found');
        }
        if (!token) {
          throw new Error('Token not found');
        }
        const response = await axios.get(`http://localhost:5242/api/user/${userId}/books`, {
          headers: {
            Authorization: `Bearer ${token}`
          }
        });
        const booksWithAuthors = await Promise.all(response.data.map(async (book) => {
          const authorResponse = await axios.get(`http://localhost:5242/api/authors/${book.authorId}`, {
            headers: {
              Authorization: `Bearer ${token}`
            }
          });
          return { ...book, author: authorResponse.data };
        }));
        setBooks(booksWithAuthors);
      } catch (error) {
        console.error('Failed to fetch user books', error);
        setBooks([]); 
      }
    };
    fetchBooks();
  }, []);

  return (
    <div>
      <h1>Your Books</h1>
      <ul>
        {Array.isArray(books) ? books.map(book => (
          <li key={book.id}>
            <Link to={`/user/books/${book.id}`}>{book.title} by {book.author.firstName} {book.author.lastName}</Link>
          </li>
        )) : <p>No books found</p>}
      </ul>
    </div>
  );
};

export default UserBooks;