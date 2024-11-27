using Library.Application.Contracts.Books;
using Library.Application.DTOs;
using Library.Core.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Library.Application.Interfaces
{
    public interface IBookService
    {
        Task<PagedResult<BookDTO>> GetAllAsync(int page, int pageSize, string? search = null);
        Task<GetBookResponse?> GetByIdAsync(Guid id);
        Task<GetBookResponse?> GetByISBNAsync(string isbn);
        Task AddAsync(BookDTO bookDto);
        Task UpdateAsync(Guid id, BookDTO bookDto);
        Task DeleteAsync(Guid id);
        Task BorrowBookAsync(Guid bookId, Guid userId, DateTime returnBy);
        Task CheckBookReturnDatesAsync();
    }
}