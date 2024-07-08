using BallastLane.BookVault.Application.Common;

namespace BallastLane.BookVault.Application.Features.Books.DeleteBook
{
    public sealed record DeleteBookCommand(string DeletedBy, Guid Id) : IRequest;
}
