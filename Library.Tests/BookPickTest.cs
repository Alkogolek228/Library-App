using System;
using Library.Core.Entities;
using Xunit;

namespace Library.Tests
{
    public class BookPickTest
    {
        [Fact]
        public void CanCreateBookPick()
        {
            // Arrange
            var bookPick = new BookPick
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                BookId = Guid.NewGuid(),
                BorrowedOn = DateTime.UtcNow,
                ReturnBy = DateTime.UtcNow.AddDays(14)
            };

            // Act & Assert
            Assert.NotNull(bookPick);
            Assert.NotEqual(Guid.Empty, bookPick.Id);
            Assert.NotEqual(Guid.Empty, bookPick.UserId);
            Assert.NotEqual(Guid.Empty, bookPick.BookId);
            Assert.NotEqual(default(DateTime), bookPick.BorrowedOn);
            Assert.NotNull(bookPick.ReturnBy);
        }

        [Fact]
        public void CanUpdateBookPickDetails()
        {
            // Arrange
            var bookPick = new BookPick
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                BookId = Guid.NewGuid(),
                BorrowedOn = DateTime.UtcNow,
                ReturnBy = DateTime.UtcNow.AddDays(14)
            };

            // Act
            bookPick.BorrowedOn = DateTime.UtcNow.AddDays(-1);
            bookPick.ReturnBy = DateTime.UtcNow.AddDays(7);

            // Assert
            Assert.Equal(DateTime.UtcNow.AddDays(-1).Date, bookPick.BorrowedOn.Date);
            Assert.Equal(DateTime.UtcNow.AddDays(7).Date, bookPick.ReturnBy?.Date);
        }

        [Fact]
        public void CanSetReturnByToNull()
        {
            // Arrange
            var bookPick = new BookPick
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                BookId = Guid.NewGuid(),
                BorrowedOn = DateTime.UtcNow,
                ReturnBy = DateTime.UtcNow.AddDays(14)
            };

            // Act
            bookPick.ReturnBy = null;

            // Assert
            Assert.Null(bookPick.ReturnBy);
        }

        [Fact]
        public void CanAssignUserAndBookToBookPick()
        {
            // Arrange
            var user = new User(
                id: Guid.NewGuid(),
                userName: "testuser",
                passwordHash: "hashedpassword",
                email: "testuser@example.com",
                role: UserRole.User
            );

            var book = new Book
            {
                Id = Guid.NewGuid(),
                Title = "Test Book 2",
                ISBN = "1234567890123",
                AuthorId = Guid.NewGuid(),
                Description = "Test Description 2",
                Genre = "Test Genre"
            };

            var bookPick = new BookPick
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                BookId = book.Id,
                BorrowedOn = DateTime.UtcNow,
                ReturnBy = DateTime.UtcNow.AddDays(14)
            };

            // Act
            bookPick.User = user;
            bookPick.Book = book;

            // Assert
            Assert.Equal(user, bookPick.User);
            Assert.Equal(book, bookPick.Book);
        }
    }
}