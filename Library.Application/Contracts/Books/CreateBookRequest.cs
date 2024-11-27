using System;
using System.ComponentModel.DataAnnotations;

namespace Library.Application.Contracts.Books
{
    public record CreateBookRequest
    {
        public required string ISBN { get; init; }
        public required string Title { get; init; }
        public required string Genre { get; init; }
        public required string Description { get; init; }
        public Guid AuthorId { get; init; }
        public DateTime? BorrowedOn { get; init; }
        public DateTime? ReturnBy { get; init; }
        public string? ImagePath { get; init; }
    }
}