namespace BallastLane.BookVault.Application.Features.UpdateBook
{
    public sealed record UpdateBookRequest(Guid Id, string Title, string Author, string? ISBN);
}
