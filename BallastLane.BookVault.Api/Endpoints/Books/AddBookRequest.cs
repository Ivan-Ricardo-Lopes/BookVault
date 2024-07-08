namespace BallastLane.BookVault.Api.Endpoints.Books
{
    public sealed record AddBookRequest(string Title, string Author, string? ISBN);
}
