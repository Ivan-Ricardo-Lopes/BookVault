using BallastLane.BookVault.Application.Common;

namespace BallastLane.BookVault.Application.Features.Books.AddBook
{
    public sealed record AddBookCommand(string CreatedBy, string Title, string Author, string? ISBN) : IRequest;
}
