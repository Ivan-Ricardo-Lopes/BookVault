using BallastLane.BookVault.Domain.Entities;

namespace BallastLane.BookVault.Domain.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetUserByUsername(string username, CancellationToken cancellationToken);
        Task<bool> ExistsByUsername(string username, CancellationToken cancellationToken);
        Task<int> AddUser(User user, CancellationToken cancellationToken);
    }
}
