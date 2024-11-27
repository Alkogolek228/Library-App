using Library.Application.Contracts.Authors;
using Library.Application.DTOs;
using Library.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Library.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorService _authorService;

        public AuthorsController(IAuthorService authorService)
        {
            _authorService = authorService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var authors = await _authorService.GetAllAsync();
            return Ok(authors);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var author = await _authorService.GetByIdAsync(id);
            if (author == null) return NotFound();

            return Ok(author);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CreateAuthorRequest request)
        {
            var authorDto = new AuthorDTO
            {
                Id = Guid.NewGuid(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                DateOfBirth = DateTime.SpecifyKind(request.DateOfBirth, DateTimeKind.Utc),
                Country = request.Country
            };
            await _authorService.AddAsync(authorDto);
            return CreatedAtAction(nameof(GetById), new { id = authorDto.Id }, authorDto);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAuthorRequest request)
        {
            var authorDto = new AuthorDTO
            {
                Id = id,
                FirstName = request.FirstName,
                LastName = request.LastName,
                DateOfBirth = DateTime.SpecifyKind(request.DateOfBirth, DateTimeKind.Utc),
                Country = request.Country
            };
            await _authorService.UpdateAsync(id, authorDto);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _authorService.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet("{id:guid}/books")]
        public async Task<IActionResult> GetBooksByAuthor(Guid id)
        {
            var books = await _authorService.GetBooksByAuthorAsync(id);
            return Ok(books);
        }
    }
}