using AutoMapper;
using Library.Application.Contracts.Books;
using Library.Application.DTOs;
using Library.Application.Interfaces;
using Library.Core.Common;
using Library.Core.Entities;
using Library.Core.Interfaces;

namespace Library.Application.Services
{
    public class BookService : IBookService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly NotificationService _notificationService;

        public BookService(IUnitOfWork unitOfWork, IMapper mapper, NotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _notificationService = notificationService;
        }

        public async Task<PagedResult<BookDTO>> GetAllAsync(int page, int pageSize, string? search = null)
        {
            var books = await _unitOfWork.Books.GetAllAsync();
            if (!string.IsNullOrEmpty(search))
            {
                books = books.Where(b => b.Title.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            var pagedBooks = books.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return new PagedResult<BookDTO>(
                _mapper.Map<IEnumerable<BookDTO>>(pagedBooks),
                books.Count(),
                page,
                pageSize
            );
        }

        public async Task<GetBookResponse?> GetByIdAsync(Guid id)
        {
            var book = await _unitOfWork.Books.GetByIdAsync(id);
            return book == null ? null : _mapper.Map<GetBookResponse>(book);
        }

        public async Task<GetBookResponse?> GetByISBNAsync(string isbn)
        {
            var book = (await _unitOfWork.Books.GetAllAsync()).FirstOrDefault(b => b.ISBN == isbn);
            return book == null ? null : _mapper.Map<GetBookResponse>(book);
        }

        public async Task AddAsync(BookDTO bookDto)
        {
            var book = _mapper.Map<Book>(bookDto);
            await _unitOfWork.Books.AddAsync(book);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateAsync(Guid id, BookDTO bookDto)
        {
            var book = await _unitOfWork.Books.GetByIdAsync(id);
            if (book == null) throw new KeyNotFoundException("Book not found");

            _mapper.Map(bookDto, book);
            _unitOfWork.Books.Update(book);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var book = await _unitOfWork.Books.GetByIdAsync(id);
            if (book == null) throw new KeyNotFoundException("Book not found");

            _unitOfWork.Books.Delete(book);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task BorrowBookAsync(Guid bookId, Guid userId, DateTime returnBy)
        {
            var book = await _unitOfWork.Books.GetByIdAsync(bookId);
            if (book == null) throw new KeyNotFoundException("Book not found");

            var bookPick = new BookPick
            {
                Id = Guid.NewGuid(),
                BookId = bookId,
                UserId = userId,
                BorrowedOn = DateTime.UtcNow,
                ReturnBy = DateTime.SpecifyKind(returnBy, DateTimeKind.Utc)
            };

            book.BorrowedOn = DateTime.UtcNow;
            book.ReturnBy = DateTime.SpecifyKind(returnBy, DateTimeKind.Utc);

            await _unitOfWork.BookPicks.AddAsync(bookPick);
            _unitOfWork.Books.Update(book);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task CheckBookReturnDatesAsync()
        {
            var books = await _unitOfWork.Books.GetAllAsync();
            var overdueBooks = books.Where(b => b.ReturnBy.HasValue && b.ReturnBy.Value < DateTime.UtcNow).ToList();

            foreach (var book in overdueBooks)
            {
                var bookPick = await _unitOfWork.BookPicks.GetByIdAsync(book.Id);
                var user = bookPick != null ? await _unitOfWork.Users.GetByIdAsync(bookPick.UserId) : null;
                if (user != null)
                {
                    var message = $"The return date for the book '{book.Title}' has passed. Please return the book as soon as possible.";
                    await _notificationService.SendNotificationAsync(user.Id.ToString(), message);
                }

                // Обновление статуса книги, чтобы она снова была доступна для взятия
                book.BorrowedOn = null;
                book.ReturnBy = null;
                _unitOfWork.Books.Update(book);
            }

            await _unitOfWork.SaveChangesAsync();
        }
    }
}