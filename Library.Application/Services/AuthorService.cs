using Library.Application.DTOs;
using Library.Application.Interfaces;
using Library.Core.Common;
using Library.Core.Entities;
using Library.Core.Interfaces;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Library.Application.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AuthorService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AuthorDTO>> GetAllAsync()
        {
            var authors = await _unitOfWork.Authors.GetAllAsync();
            return _mapper.Map<IEnumerable<AuthorDTO>>(authors);
        }

        public async Task<AuthorDTO> GetByIdAsync(Guid id)
        {
            var author = await _unitOfWork.Authors.GetByIdAsync(id);
            if (author == null)
                throw new KeyNotFoundException("Author not found");

            return _mapper.Map<AuthorDTO>(author);
        }

        public async Task AddAsync(AuthorDTO authorDto)
        {
            var author = _mapper.Map<Author>(authorDto);
            await _unitOfWork.Authors.AddAsync(author);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateAsync(Guid id, AuthorDTO authorDto)
        {
            var existingAuthor = await _unitOfWork.Authors.GetByIdAsync(id);
            if (existingAuthor == null)
                throw new KeyNotFoundException("Author not found");

            _mapper.Map(authorDto, existingAuthor);
            _unitOfWork.Authors.Update(existingAuthor);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var existingAuthor = await _unitOfWork.Authors.GetByIdAsync(id);
            if (existingAuthor == null)
                throw new KeyNotFoundException("Author not found");

            _unitOfWork.Authors.Delete(existingAuthor);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<PagedResult<AuthorDTO>> GetPagedAsync(int page, int pageSize)
        {
            var authors = await _unitOfWork.Authors.GetPagedAsync(page, pageSize);
            return _mapper.Map<PagedResult<AuthorDTO>>(authors);
        }

        public async Task<IEnumerable<BookDTO>> GetBooksByAuthorAsync(Guid authorId)
        {
            var books = await _unitOfWork.Books.GetBooksByAuthorAsync(authorId);
            return _mapper.Map<IEnumerable<BookDTO>>(books);
        }
    }
}