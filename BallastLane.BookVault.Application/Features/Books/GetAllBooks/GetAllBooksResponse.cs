using BallastLane.BookVault.Domain.Entities;

namespace BallastLane.BookVault.Application.Features.Books.GetAllBooks
{
    public sealed record GetAllBooksResponse(IEnumerable<Book> Books);
}
