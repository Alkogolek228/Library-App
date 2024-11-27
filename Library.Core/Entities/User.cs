using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Library.Core.Entities
{
    public class User
    {
        public User(Guid id, string userName, string passwordHash, string email, UserRole role)
        {
            Id = id;
            UserName = userName;
            PasswordHash = passwordHash;
            Email = email;
            Role = role;
        }

        public Guid Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Имя пользователя должно быть в пределах от 5 до 50 символов.")]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Неправильный формат электронной почты.")]
        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public ICollection<BookPick> BookPicks { get; set; } = new List<BookPick>();

        public UserRole Role { get; set; } // Добавьте это свойство

        public string RefreshToken { get; set; } = string.Empty;
        public DateTime RefreshTokenExpiryTime { get; set; } = DateTime.UtcNow.AddDays(7);

        public static User Create(Guid id, string userName, string passwordHash, string email, UserRole role = UserRole.User)
        {
            return new User(id, userName, passwordHash, email, role);
        }
    }

    public enum UserRole
    {
        Admin,
        User
    }
}