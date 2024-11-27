namespace Library.Application.Contracts.Books
{
    public record BorrowBookRequest(
        DateTime ReturnBy);
}