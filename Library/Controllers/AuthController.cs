using Library.Application.Contracts.Auth;
using Library.Application.DTOs;
using Library.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Library.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
        {
            try
            {
                var existingUser = await _authService.GetUserByUsernameAsync(request.UserName);
                if (existingUser != null)
                {
                    return BadRequest(new { Message = "Пользователь с таким логином уже существует." });
                }

                await _authService.RegisterUserAsync(request);
                return Ok(new { Message = "Пользователь успешно зарегистрирован." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Ошибка регистрации. Попробуйте снова." });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserRequest request)
        {
            var tokens = await _authService.LoginUserAsync(request);

            var user = await _authService.GetUserByUsernameAsync(request.UserName);

            var userDTO = new UserDTO(user.Id,
                                user.UserName, 
                                user.Email,  
                                user.BookPicks, 
                                user.Role);

            return Ok(new
            {
                AccessToken = tokens.AccessToken,
                RefreshToken = tokens.RefreshToken,
                User = userDTO
            });
        }

        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
        {
            try
            {
                var tokens = await _authService.RefreshTokensAsync(request.RefreshToken);
                return Ok(tokens);
            }
            catch(Exception ex)
            {
                return Unauthorized(new { ex.Message });
            }
        }
    }
}