using System;
using System.ComponentModel.DataAnnotations;

namespace Library.Core.Entities
{
    public class BookPick
    {
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        [Required]
        public Guid BookId { get; set; }
        public Book Book { get; set; } = null!;

        [Required]
        public DateTime BorrowedOn { get; set; }

        public DateTime? ReturnBy { get; set; }
    }
}