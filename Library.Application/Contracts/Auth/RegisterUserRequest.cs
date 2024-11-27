using System.ComponentModel.DataAnnotations;

namespace Library.Application.Contracts.Auth
{
    public record RegisterUserRequest(
        [Required]string UserName, 
        [Required]string Email,
        [Required]string Password,
        [Required]string PasswordConfirm); 
}