using BallastLane.BookVault.Application.Common;

namespace BallastLane.BookVault.Application.Features.Books.UpdateBook
{
    public sealed record UpdateBookCommand(string ModifiedBy, Guid Id, string Title, string Author, string? ISBN) : IRequest;
}
