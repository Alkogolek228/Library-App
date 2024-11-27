using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Library.Application.Contracts.Books;
using Library.Application.Mapping;
using Library.Application.Services;
using Library.Core.Entities;
using Library.Core.Interfaces;
using Library.Infrastructure.Data;
using Library.Infrastructure.Hubs;
using Library.Infrastructure.Repositories;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Library.Tests
{
    public class BookRepositoryTests
    {
        private readonly DbContextOptions<LibraryDbContext> _options;

        public BookRepositoryTests()
        {
            _options = new DbContextOptionsBuilder<LibraryDbContext>()
                .UseInMemoryDatabase(databaseName: "LibraryTestDb")
                .Options;
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnBook_WhenBookExists()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var book = new Book
            {
                Id = bookId,
                Title = "Test Book",
                ISBN = "1234567890123",
                AuthorId = Guid.NewGuid(),
                Description = "Test Description",
                Genre = "Test Genre"
            };

            using (var context = new LibraryDbContext(_options))
            {
                context.Books.Add(book);
                await context.SaveChangesAsync();
            }

            // Act
            Book result;
            using (var context = new LibraryDbContext(_options))
            {
                var repository = new BookRepository(context);
                result = await repository.GetByIdAsync(bookId);
            }

            // Assert
            Assert.NotNull(result);
            Assert.Equal(bookId, result.Id);
            Assert.Equal("Test Book", result.Title);
        }

        [Fact]
        public async Task AddAsync_ShouldAddBook()
        {
            // Arrange
            var book = new Book
            {
                Id = Guid.NewGuid(),
                Title = "New Book",
                ISBN = "1234567890123",
                AuthorId = Guid.NewGuid(),
                Description = "New Description",
                Genre = "New Genre"
            };

            // Act
            using (var context = new LibraryDbContext(_options))
            {
                var repository = new BookRepository(context);
                await repository.AddAsync(book);
                await context.SaveChangesAsync();
            }

            // Assert
            using (var context = new LibraryDbContext(_options))
            {
                var result = await context.Books.FindAsync(book.Id);
                Assert.NotNull(result);
                Assert.Equal("New Book", result.Title);
            }
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateBook()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var book = new Book
            {
                Id = bookId,
                Title = "Old Book",
                ISBN = "1234567890123",
                AuthorId = Guid.NewGuid(),
                Description = "Old Description",
                Genre = "Old Genre"
            };

            using (var context = new LibraryDbContext(_options))
            {
                context.Books.Add(book);
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = new LibraryDbContext(_options))
            {
                var repository = new BookRepository(context);
                book.Title = "Updated Book";
                repository.Update(book);
                await context.SaveChangesAsync();
            }

            // Assert
            using (var context = new LibraryDbContext(_options))
            {
                var result = await context.Books.FindAsync(bookId);
                Assert.NotNull(result);
                Assert.Equal("Updated Book", result.Title);
            }
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveBook()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var book = new Book
            {
                Id = bookId,
                Title = "Test Book",
                ISBN = "1234567890123",
                AuthorId = Guid.NewGuid(),
                Description = "Test Description",
                Genre = "Test Genre"
            };

            using (var context = new LibraryDbContext(_options))
            {
                context.Books.Add(book);
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = new LibraryDbContext(_options))
            {
                var repository = new BookRepository(context);
                repository.Delete(book);
                await context.SaveChangesAsync();
            }

            // Assert
            using (var context = new LibraryDbContext(_options))
            {
                var result = await context.Books.FindAsync(bookId);
                Assert.Null(result);
            }
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllBooks()
        {
            // Arrange
            var book1 = new Book
            {
                Id = Guid.NewGuid(),
                Title = "Book 1",
                ISBN = "1234567890123",
                AuthorId = Guid.NewGuid(),
                Description = "Description 1",
                Genre = "Genre 1"
            };
            var book2 = new Book
            {
                Id = Guid.NewGuid(),
                Title = "Book 2",
                ISBN = "1234567890124",
                AuthorId = Guid.NewGuid(),
                Description = "Description 2",
                Genre = "Genre 2"
            };

            using (var context = new LibraryDbContext(_options))
            {
                context.Books.AddRange(book1, book2);
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = new LibraryDbContext(_options))
            {
                var repository = new BookRepository(context);
                var result = await repository.GetAllAsync();
                // Assert
                Assert.Equal(6, result.Count());
            }
        }

        [Fact]
        public async Task GetByISBNAsync_ShouldReturnBook_WhenBookExists()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var book = new Book
            {
                Id = bookId,
                Title = "Test Book",
                ISBN = "1234517890123",
                AuthorId = Guid.NewGuid(),
                Description = "Test Description",
                Genre = "Test Genre"
            };

            using (var context = new LibraryDbContext(_options))
            {
                context.Books.Add(book);
                await context.SaveChangesAsync();
            }

            // Act
            GetBookResponse result;
            using (var context = new LibraryDbContext(_options))
            {
                var repository = new BookRepository(context);
                var unitOfWork = new Mock<IUnitOfWork>();
                unitOfWork.Setup(u => u.Books).Returns(repository);
                var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());
                var mapper = mapperConfig.CreateMapper();
                var hubContext = new Mock<IHubContext<NotificationHub>>().Object;
                var notificationService = new NotificationService(hubContext);
                var bookService = new BookService(unitOfWork.Object, mapper, notificationService);
                result = await bookService.GetByISBNAsync("1234517890123");
            }

            // Assert
            Assert.NotNull(result);
            Assert.Equal(bookId, result.Id);
            Assert.Equal("Test Book", result.Title);
        }

        [Fact]
        public async Task BorrowBookAsync_ShouldUpdateBookBorrowedOnAndReturnBy()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var book = new Book
            {
                Id = bookId,
                Title = "Test Book",
                ISBN = "1234567890123",
                AuthorId = Guid.NewGuid(),
                Description = "Test Description",
                Genre = "Test Genre"
            };

            using (var context = new LibraryDbContext(_options))
            {
                context.Books.Add(book);
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = new LibraryDbContext(_options))
            {
                var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());
                var mapper = mapperConfig.CreateMapper();
                var hubContext = new Mock<IHubContext<NotificationHub>>().Object;
                var notificationService = new NotificationService(hubContext);
                var unitOfWorkMock = new Mock<IUnitOfWork>();
                unitOfWorkMock.Setup(u => u.Books).Returns(new BookRepository(context));
                unitOfWorkMock.Setup(u => u.BookPicks).Returns(new Repository<BookPick>(context));
                var bookService = new BookService(unitOfWorkMock.Object, mapper, notificationService);
                var returnBy = DateTime.UtcNow.AddDays(7);
                var userId = Guid.NewGuid(); // Add a userId for the test
                await bookService.BorrowBookAsync(bookId, userId, returnBy);
                await context.SaveChangesAsync();
            }

            // Assert
            using (var context = new LibraryDbContext(_options))
            {
                var result = await context.Books.FindAsync(bookId);
                Assert.NotNull(result);
                Assert.NotNull(result.BorrowedOn);
                Assert.NotNull(result.ReturnBy);
                Assert.Equal(DateTime.UtcNow.AddDays(7).Date, result.ReturnBy.Value.Date);
            }
        }           
    }
}