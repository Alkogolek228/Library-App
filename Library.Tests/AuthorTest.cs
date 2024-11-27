using System;
using System.Collections.Generic;
using Library.Core.Entities;
using Xunit;

namespace Library.Tests
{
    public class AuthorTest
    {
        [Fact]
        public void CanCreateAuthor()
        {
            // Arrange
            var author = new Author
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = new DateTime(1980, 1, 1),
                Country = "USA"
            };

            // Act & Assert
            Assert.NotNull(author);
            Assert.Equal("John", author.FirstName);
            Assert.Equal("Doe", author.LastName);
            Assert.Equal(new DateTime(1980, 1, 1), author.DateOfBirth);
            Assert.Equal("USA", author.Country);
            Assert.Empty(author.Books);
        }

        [Fact]
        public void CanAddBooksToAuthor()
        {
            // Arrange
            var author = new Author
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = new DateTime(1980, 1, 1),
                Country = "USA"
            };

            var book1 = new Book
            {
                Id = Guid.NewGuid(),
                Title = "Test Book 1",
                ISBN = "1234567890125",
                AuthorId = Guid.NewGuid(),
                Description = "Test Description",
                Genre = "Test Genre"
            };

            var book2 = new Book
            {
                Id = Guid.NewGuid(),
                Title = "Test Book 2",
                ISBN = "1234567890126",
                AuthorId = Guid.NewGuid(),
                Description = "Test Description",
                Genre = "Test Genre"
            };

            // Act
            author.Books.Add(book1);
            author.Books.Add(book2);

            // Assert
            Assert.Equal(2, author.Books.Count);
            Assert.Contains(book1, author.Books);
            Assert.Contains(book2, author.Books);
        }

        [Fact]
        public void CanRemoveBooksFromAuthor()
        {
            // Arrange
            var author = new Author
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = new DateTime(1980, 1, 1),
                Country = "USA"
            };

            var book1 = new Book
            {
                Id = Guid.NewGuid(),
                Title = "Test Book 1",
                ISBN = "1234567890124",
                AuthorId = Guid.NewGuid(),
                Description = "Test Description 1",
                Genre = "Test Genre"
            };

            var book2 = new Book
            {
                Id = Guid.NewGuid(),
                Title = "Test Book 2",
                ISBN = "1234567890123",
                AuthorId = Guid.NewGuid(),
                Description = "Test Description 2",
                Genre = "Test Genre"
            };

            author.Books.Add(book1);
            author.Books.Add(book2);

            // Act
            author.Books.Remove(book1);

            // Assert
            Assert.Single(author.Books);
            Assert.DoesNotContain(book1, author.Books);
            Assert.Contains(book2, author.Books);
        }

        [Fact]
        public void CanUpdateAuthorDetails()
        {
            // Arrange
            var author = new Author
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = new DateTime(1980, 1, 1),
                Country = "USA"
            };

            // Act
            author.FirstName = "Jane";
            author.LastName = "Smith";
            author.DateOfBirth = new DateTime(1990, 1, 1);
            author.Country = "Canada";

            // Assert
            Assert.Equal("Jane", author.FirstName);
            Assert.Equal("Smith", author.LastName);
            Assert.Equal(new DateTime(1990, 1, 1), author.DateOfBirth);
            Assert.Equal("Canada", author.Country);
        }
    }
}