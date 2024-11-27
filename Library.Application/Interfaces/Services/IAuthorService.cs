using Library.Application.DTOs;
using Library.Core.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Library.Application.Interfaces
{
    public interface IAuthorService
    {
        Task<IEnumerable<AuthorDTO>> GetAllAsync();
        Task<AuthorDTO> GetByIdAsync(Guid id);
        Task AddAsync(AuthorDTO authorDto);
        Task UpdateAsync(Guid id, AuthorDTO authorDto);
        Task DeleteAsync(Guid id);
        Task<PagedResult<AuthorDTO>> GetPagedAsync(int page, int pageSize); // Add this line
        Task<IEnumerable<BookDTO>> GetBooksByAuthorAsync(Guid authorId); // Add this line
    }
}