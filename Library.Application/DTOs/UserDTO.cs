using Library.Core.Entities;

namespace Library.Application.DTOs
{
    public record UserDTO(
        Guid Id,
        string UserName,
        string Email,
        ICollection<BookPick>? Picks,
        UserRole Role);
}