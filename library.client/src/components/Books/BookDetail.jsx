import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { getBookById, borrowBook, deleteBook } from '../../services/bookService';
import axios from 'axios';

const BookDetail = () => {
  const { id } = useParams();
  const [book, setBook] = useState(null);
  const [userRole, setUserRole] = useState(null); // Добавьте состояние для роли пользователя
  const [showModal, setShowModal] = useState(false); // Состояние для отображения модального окна
  const [returnBy, setReturnBy] = useState(''); // Состояние для даты возврата
  const navigate = useNavigate();

  useEffect(() => {
    const fetchBook = async () => {
      try {
        const data = await getBookById(id);
        const token = localStorage.getItem('token'); // Получение токена из localStorage
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

    const fetchUserRole = () => {
      const role = localStorage.getItem('userRole');
      setUserRole(role);
    };

    fetchBook();
    fetchUserRole();
  }, [id]);

  const handleBorrow = async () => {
    if (new Date(returnBy) < new Date()) {
      alert('Return date cannot be in the past.');
      return;
    }
    try {
      const data = await borrowBook(id, returnBy);
      console.log('Borrowed book:', data); // Вывод результатов в консоль
      navigate('/user/books');
    } catch (error) {
      console.error('Borrow failed', error);
    }
  };

  const handleDelete = async () => {
    try {
      await deleteBook(id);
      navigate('/books');
    } catch (error) {
      console.error('Failed to delete book', error);
    }
  };

  if (!book) return <div>Loading...</div>;

  return (
    <div>
      <h1>{book.title}</h1>
      {book.imagePath && <img src={`http://localhost:5242${book.imagePath}`} alt={book.title} style={{ width: '100px', height: '150px' }} />}
      <p>{book.description}</p>
      <p>Author: {book.author.firstName} {book.author.lastName}</p>
      <p>Genre: {book.genre}</p>
      <p>ISBN: {book.isbn}</p>
      <p>Borrowed On: {book.borrowedOn}</p>
      <p>Return By: {book.returnBy}</p>
      {book.borrowedOn ? (
        <p>This book is currently borrowed and cannot be borrowed again until it is returned.</p>
      ) : (
        <>
          <input
            type="date"
            value={returnBy}
            onChange={(e) => setReturnBy(e.target.value)}
            min={new Date().toISOString().split('T')[0]} // Установка минимальной даты на сегодня
            required
          />
          <button onClick={handleBorrow}>Borrow</button>
        </>
      )}
      {userRole === '0' && ( // Проверка числового значения роли
        <>
          <button onClick={() => navigate(`/admin/books/${id}/edit`)}>Edit</button>
          <button onClick={() => setShowModal(true)}>Delete</button>
        </>
      )}
      {showModal && (
        <div className="modal">
          <div className="modal-content">
            <h2>Confirm Deletion</h2>
            <p>Are you sure you want to delete this book?</p>
            <button onClick={handleDelete}>Yes, Delete</button>
            <button onClick={() => setShowModal(false)}>Cancel</button>
          </div>
        </div>
      )}
    </div>
  );
};

export default BookDetail;