using BallastLane.BookVault.Domain.Entities;

namespace BallastLane.BookVault.Domain.Repositories
{
    public interface IBookRepository
    {
        Task<IEnumerable<Book>> GetAllAsync(int? pageNumber, int? pageSize, CancellationToken cancellationToken);
        Task<Book?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<int> AddAsync(Book book, CancellationToken cancellationToken);
        Task<int> UpdateAsync(Book book, CancellationToken cancellationToken);
        Task<int> DeleteAsync(Guid id, string deletedBy, CancellationToken cancellationToken);
    }
}
