namespace BallastLane.BookVault.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = default!;
        public string PasswordHash { get; set; } = default!;
        public string Salt { get; set; } = default!;
    }
}
