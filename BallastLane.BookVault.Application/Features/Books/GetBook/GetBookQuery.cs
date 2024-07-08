using BallastLane.BookVault.Application.Common;

namespace BallastLane.BookVault.Application.Features.Books.GetBook
{
    public sealed record GetBookQuery(Guid Id) : IRequest;
}
