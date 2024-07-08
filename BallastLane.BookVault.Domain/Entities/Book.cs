namespace BallastLane.BookVault.Domain.Entities
{
    public class Book : BaseEntity
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = default!;
        public string Author { get; set; } = default!;
        public string? ISBN { get; set; }
    }
}
