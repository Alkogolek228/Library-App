import React, { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import { getBookById } from '../../services/bookService';
import axios from 'axios';

const UserBookDetail = () => {
  const { id } = useParams();
  const [book, setBook] = useState(null);

  useEffect(() => {
    const fetchBook = async () => {
      try {
        const token = localStorage.getItem('token'); // Получение токена из localStorage
        const data = await getBookById(id);
        const authorResponse = await axios.get(`http://localhost:5242/api/authors/${data.authorId}`, {
          headers: {
            Authorization: `Bearer ${token}`
          }
        });
        setBook({ ...data, author: authorResponse.data });
        console.log('Fetched book:', data); // Вывод результатов в консоль
      } catch (error) {
        console.error('Failed to fetch book', error);
      }
    };

    fetchBook();
  }, [id]);

  if (!book) return <div>Loading...</div>;

  return (
    <div>
      <h1>{book.title}</h1>
      <p>{book.description}</p>
      <p>Author: {book.author.firstName} {book.author.lastName}</p>
      <p>Genre: {book.genre}</p>
      <p>ISBN: {book.isbn}</p>
      <p>Borrowed On: {book.borrowedOn}</p>
      <p>Return By: {book.returnBy}</p>
    </div>
  );
};

export default UserBookDetail;