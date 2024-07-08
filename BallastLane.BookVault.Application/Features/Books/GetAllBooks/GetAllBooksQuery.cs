using BallastLane.BookVault.Application.Common;

namespace BallastLane.BookVault.Application.Features.Books.GetAllBooks
{
    public sealed record GetAllBooksQuery(int? PageNumber = 1, int? PageSize = 50) : IRequest;
}
