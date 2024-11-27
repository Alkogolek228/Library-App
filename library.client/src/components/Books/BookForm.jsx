import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { getBookById, createBook, updateBook } from '../../services/bookService';
import { getAllAuthors, createAuthor } from '../../services/authorService';
import { uploadFile } from '../../services/fileService';

const BookForm = () => {
  const { id } = useParams();
  const [book, setBook] = useState({ title: '', description: '', authorId: '', genre: '', isbn: '', borrowedOn: null, returnBy: null, imagePath: '' });
  const [authors, setAuthors] = useState([]); 
  const [newAuthor, setNewAuthor] = useState({ firstName: '', lastName: '', dateOfBirth: '', country: '' });
  const [userRole, setUserRole] = useState(null);
  const [imageFile, setImageFile] = useState(null);
  const navigate = useNavigate();

  useEffect(() => {
    const fetchAuthors = async () => {
      try {
        const data = await getAllAuthors();
        setAuthors(data);
      } catch (error) {
        console.error('Failed to fetch authors', error);
        setAuthors([]);
      }
    };

    fetchAuthors();

    if (id) {
      const fetchBook = async () => {
        try {
          const data = await getBookById(id);
          setBook(data);
        } catch (error) {
          console.error('Failed to fetch book', error);
        }
      };
      fetchBook();
    }

    const fetchUserRole = () => {
      const role = localStorage.getItem('userRole');
      setUserRole(role);
    };

    fetchUserRole();
  }, [id]);

  const handleAuthorChange = (e) => {
    const { name, value } = e.target;
    setNewAuthor({ ...newAuthor, [name]: value });
  };

  const handleCreateAuthor = async () => {
    try {
      const createdAuthor = await createAuthor(newAuthor);
      setAuthors([...authors, createdAuthor]);
      setBook({ ...book, authorId: createdAuthor.id });
      setNewAuthor({ firstName: '', lastName: '', dateOfBirth: '', country: '' });
    } catch (error) {
      console.error('Failed to create author', error);
    }
  };

  const handleImageChange = (e) => {
    const file = e.target.files[0];
    setImageFile(file);
    const reader = new FileReader();
    reader.onloadend = () => {
      setBook({ ...book, imagePath: reader.result });
    };
    reader.readAsDataURL(file);
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    let imagePath = book.imagePath;

    if (imageFile) {
      try {
        const uploadResponse = await uploadFile(imageFile);
        imagePath = uploadResponse.filePath;
      } catch (error) {
        console.error('Failed to upload image', error);
        return;
      }
    }

    const bookData = {
      ...book,
      imagePath,
    };

    try {
      if (id) {
        await updateBook(id, bookData);
      } else {
        await createBook(bookData);
      }
      navigate('/books');
    } catch (error) {
      console.error('Failed to save book', error);
    }
  };

  if (userRole !== '0') return <div>Access denied</div>;

  return (
    <form onSubmit={handleSubmit}>
      <input type="text" value={book.title} onChange={(e) => setBook({ ...book, title: e.target.value })} placeholder="Title" required />
      <input type="text" value={book.description} onChange={(e) => setBook({ ...book, description: e.target.value })} placeholder="Description" required />
      <select value={book.authorId} onChange={(e) => setBook({ ...book, authorId: e.target.value })} required>
        <option value="">Select Author</option>
        {authors.map(author => (
          <option key={author.id} value={author.id}>{author.firstName} {author.lastName}</option>
        ))}
      </select>
      {book.authorId === '' && (
        <div>
          <h3>Create New Author</h3>
          <input type="text" name="firstName" value={newAuthor.firstName} onChange={handleAuthorChange} placeholder="First Name" required />
          <input type="text" name="lastName" value={newAuthor.lastName} onChange={handleAuthorChange} placeholder="Last Name" required />
          <input type="date" name="dateOfBirth" value={newAuthor.dateOfBirth} onChange={handleAuthorChange} required />
          <input type="text" name="country" value={newAuthor.country} onChange={handleAuthorChange} placeholder="Country" required />
          <button type="button" onClick={handleCreateAuthor}>Create Author</button>
        </div>
      )}
      <input type="text" value={book.genre} onChange={(e) => setBook({ ...book, genre: e.target.value })} placeholder="Genre" required />
      <input type="text" value={book.isbn} onChange={(e) => setBook({ ...book, isbn: e.target.value })} placeholder="ISBN" required />
      <input type="file" onChange={handleImageChange} />
      {book.imagePath && <img src={book.imagePath} alt="Book cover" style={{ width: '100px', height: 'auto' }} />}
      <button type="submit">Save</button>
    </form>
  );
};

export default BookForm;