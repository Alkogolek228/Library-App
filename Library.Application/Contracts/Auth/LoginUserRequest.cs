using System.ComponentModel.DataAnnotations;

namespace Library.Application.Contracts.Auth
{
    public record LoginUserRequest(
        [Required]string UserName, 
        [Required]string Password);
}