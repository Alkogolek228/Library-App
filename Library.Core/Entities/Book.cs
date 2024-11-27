using System;
using System.Collections.Generic;

namespace Library.Core.Entities
{
    public class Book
    {
        public Guid Id { get; set; }
        public string ISBN { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Genre { get; set; } = null!;
        public string Description { get; set; } = null!;
        public Guid AuthorId { get; set; }
        public Author Author { get; set; } = null!;
        public DateTime? BorrowedOn { get; set; }
        public DateTime? ReturnBy { get; set; }
        public string? ImagePath { get; set; }
        public ICollection<BookPick> BookPicks { get; set; } = new List<BookPick>();
    }
}