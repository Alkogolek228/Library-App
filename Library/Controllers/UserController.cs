using System.Security.Claims;
using Library.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("books")]
        public async Task<IActionResult> GetUserBooks()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var books = await _userService.GetUserBooksAsync(Guid.Parse(userId));
            return Ok(books);
        }

        [HttpGet("{userId:guid}/books")]
        public async Task<IActionResult> GetUserBooks(Guid userId)
        {
            var books = await _userService.GetUserBooksAsync(userId);
            return Ok(books);
        }
    }
}