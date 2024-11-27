import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { getAllBooks } from '../../services/bookService';
import axios from 'axios';

const BookList = () => {
  const [books, setBooks] = useState([]);
  const [filteredBooks, setFilteredBooks] = useState([]);
  const [page, setPage] = useState(1);
  const [pageSize] = useState(10);
  const [search, setSearch] = useState('');
  const [author, setAuthor] = useState('');
  const [genre, setGenre] = useState('');

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

  const fetchBooks = async () => {
    try {
      const data = await getAllBooks(page, pageSize, search, author, genre);
      if (data && data.items) {
        console.log('Fetched books:', data.items); // Вывод результатов в консоль
        const booksWithAuthors = await Promise.all(data.items.map(async (book) => {
          try {
            const authorResponse = await axios.get(`http://localhost:5242/api/authors/${book.authorId}`, getAuthHeaders());
            return { ...book, author: authorResponse.data };
          } catch (error) {
            console.error('Failed to fetch author', error);
            return book;
          }
        }));
        setBooks(booksWithAuthors); // Отображение всех книг, включая взятые
        setFilteredBooks(booksWithAuthors); // Инициализация отфильтрованных книг
      } else {
        console.error('Unexpected response:', data);
        setBooks([]);
        setFilteredBooks([]);
      }
    } catch (error) {
      console.error('Failed to fetch books', error);
      setBooks([]); // Ensure books is always an array
      setFilteredBooks([]); // Ensure filteredBooks is always an array
    }
  };

  useEffect(() => {
    fetchBooks();
  }, [page, pageSize]);

  const handleSearch = (e) => {
    e.preventDefault();
    console.log('Search:', search);
    console.log('Author:', author);
    console.log('Genre:', genre);
    const filtered = books.filter(book => 
      (search === '' || book.title.toLowerCase().includes(search.toLowerCase())) &&
      (author === '' || (book.author && `${book.author.firstName} ${book.author.lastName}`.toLowerCase().includes(author.toLowerCase()))) &&
      (genre === '' || book.genre.toLowerCase().includes(genre.toLowerCase()))
    );
    setFilteredBooks(filtered);
  };

  return (
    <div>
      <form onSubmit={handleSearch}>
        <input type="text" value={search} onChange={(e) => { setSearch(e.target.value); console.log('Search input:', e.target.value); }} placeholder="Search by title" />
        <input type="text" value={author} onChange={(e) => { setAuthor(e.target.value); console.log('Author input:', e.target.value); }} placeholder="Filter by author" />
        <input type="text" value={genre} onChange={(e) => { setGenre(e.target.value); console.log('Genre input:', e.target.value); }} placeholder="Filter by genre" />
        <button type="submit">Search</button>
      </form>
      <ul>
        {filteredBooks.length > 0 ? filteredBooks.map(book => (
          <li key={book.id}>
            <Link to={`/books/${book.id}`}>
              {book.imagePath && <img src={`http://localhost:5242${book.imagePath}`} alt="Book cover" style={{ width: '50px', height: 'auto', marginRight: '10px' }} />}
              {book.title} by {book.author ? `${book.author.firstName} ${book.author.lastName}` : 'Unknown Author'}
            </Link>
          </li>
        )) : <p>No books found</p>}
      </ul>
      <button onClick={() => setPage(page - 1)} disabled={page === 1}>Previous</button>
      <button onClick={() => setPage(page + 1)}>Next</button>
    </div>
  );
};

export default BookList;