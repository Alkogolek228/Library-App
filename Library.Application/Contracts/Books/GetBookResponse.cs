using System;

namespace Library.Application.Contracts.Books
{
    public class GetBookResponse
    {
        public Guid Id { get; set; }
        public string ISBN { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid AuthorId { get; set; }
        public DateTime? BorrowedOn { get; set; }
        public DateTime? ReturnBy { get; set; }
        public string? ImagePath { get; set; } = string.Empty;
    }
}