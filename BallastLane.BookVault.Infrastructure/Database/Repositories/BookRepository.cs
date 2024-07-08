using BallastLane.BookVault.Domain.Entities;
using BallastLane.BookVault.Domain.Repositories;
using Npgsql;

namespace BallastLane.BookVault.Infrastructure.Database.Repositories
{
    public class BookRepository(ConnectionStringAccessor connectionStringAccessor) : IBookRepository
    {
        private readonly ConnectionStringAccessor _connectionStringAccessor = connectionStringAccessor;

        public async Task<IEnumerable<Book>> GetAllAsync(int? pageNumber, int? pageSize, CancellationToken cancellationToken)
        {
            var books = new List<Book>();
            using (var connection = new NpgsqlConnection(_connectionStringAccessor.ConnectionString))
            {
                await connection.OpenAsync(cancellationToken);
                using var command = new NpgsqlCommand();

                command.Connection = connection;
                command.CommandText = "SELECT * FROM Books WHERE DeletedDateUtc IS NULL ORDER BY Title LIMIT @PageSize OFFSET @Offset";
                command.Parameters.AddWithValue("@PageSize", pageSize ?? int.MaxValue);
                command.Parameters.AddWithValue("@Offset", (pageNumber.HasValue ? pageNumber.Value - 1 : 0) * (pageSize ?? int.MaxValue));

                using var reader = await command.ExecuteReaderAsync(cancellationToken);

                while (await reader.ReadAsync(cancellationToken))
                {
                    books.Add(new Book
                    {
                        Id = reader.GetGuid(reader.GetOrdinal("Id")),
                        Title = reader.GetString(reader.GetOrdinal("Title")),
                        Author = reader.GetString(reader.GetOrdinal("Author")),
                        ISBN = await reader.IsDBNullAsync(reader.GetOrdinal("ISBN"), cancellationToken) ? null : reader.GetString(reader.GetOrdinal("ISBN")),
                        CreatedBy = reader.GetString(reader.GetOrdinal("CreatedBy")),
                        CreatedDateUtc = reader.GetDateTime(reader.GetOrdinal("CreatedDateUtc")),
                        ModifiedBy = await reader.IsDBNullAsync(reader.GetOrdinal("ModifiedBy"), cancellationToken) ? null : reader.GetString(reader.GetOrdinal("ModifiedBy")),
                        ModifiedDateUtc = await reader.IsDBNullAsync(reader.GetOrdinal("ModifiedDateUtc"), cancellationToken) ? null : reader.GetDateTime(reader.GetOrdinal("ModifiedDateUtc")),
                        DeletedBy = await reader.IsDBNullAsync(reader.GetOrdinal("DeletedBy"), cancellationToken) ? null : reader.GetString(reader.GetOrdinal("DeletedBy")),
                        DeletedDateUtc = await reader.IsDBNullAsync(reader.GetOrdinal("DeletedDateUtc"), cancellationToken) ? null : reader.GetDateTime(reader.GetOrdinal("DeletedDateUtc"))
                    });
                }
            }
            return books;
        }

        public async Task<Book?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            Book? book = null;
            using (var connection = new NpgsqlConnection(_connectionStringAccessor.ConnectionString))
            {
                await connection.OpenAsync(cancellationToken);
                using var command = new NpgsqlCommand();
                command.Connection = connection;
                command.CommandText = "SELECT * FROM Books WHERE Id = @Id AND DeletedDateUtc IS NULL";
                command.Parameters.AddWithValue("@Id", id);

                using var reader = await command.ExecuteReaderAsync(cancellationToken);
                if (await reader.ReadAsync(cancellationToken))
                {
                    book = new Book
                    {
                        Id = reader.GetGuid(reader.GetOrdinal("Id")),
                        Title = reader.GetString(reader.GetOrdinal("Title")),
                        Author = reader.GetString(reader.GetOrdinal("Author")),
                        ISBN = await reader.IsDBNullAsync(reader.GetOrdinal("ISBN"), cancellationToken) ? null : reader.GetString(reader.GetOrdinal("ISBN")),
                        CreatedBy = reader.GetString(reader.GetOrdinal("CreatedBy")),
                        CreatedDateUtc = reader.GetDateTime(reader.GetOrdinal("CreatedDateUtc")),
                        ModifiedBy = await reader.IsDBNullAsync(reader.GetOrdinal("ModifiedBy"), cancellationToken) ? null : reader.GetString(reader.GetOrdinal("ModifiedBy")),
                        ModifiedDateUtc = await reader.IsDBNullAsync(reader.GetOrdinal("ModifiedDateUtc"), cancellationToken) ? null : reader.GetDateTime(reader.GetOrdinal("ModifiedDateUtc")),
                        DeletedBy = await reader.IsDBNullAsync(reader.GetOrdinal("DeletedBy"), cancellationToken) ? null : reader.GetString(reader.GetOrdinal("DeletedBy")),
                        DeletedDateUtc = await reader.IsDBNullAsync(reader.GetOrdinal("DeletedDateUtc"), cancellationToken) ? null : reader.GetDateTime(reader.GetOrdinal("DeletedDateUtc"))
                    };
                }
            }
            return book;
        }

        public async Task<int> AddAsync(Book book, CancellationToken cancellationToken)
        {
            using var connection = new NpgsqlConnection(_connectionStringAccessor.ConnectionString);

            await connection.OpenAsync(cancellationToken);

            using var command = new NpgsqlCommand();

            command.Connection = connection;
            command.CommandText = "INSERT INTO Books (Id, Title, Author, ISBN, CreatedBy, CreatedDateUtc) VALUES (@Id, @Title, @Author, @ISBN, @CreatedBy, @CreatedDateUtc)";
            book.Id = Guid.NewGuid();
            command.Parameters.AddWithValue("@Id", book.Id);
            command.Parameters.AddWithValue("@Title", book.Title);
            command.Parameters.AddWithValue("@Author", book.Author);
            command.Parameters.AddWithValue("@ISBN", (object?)book.ISBN ?? DBNull.Value);
            command.Parameters.AddWithValue("@CreatedBy", book.CreatedBy);
            command.Parameters.AddWithValue("@CreatedDateUtc", book.CreatedDateUtc);

            return await command.ExecuteNonQueryAsync(cancellationToken);
        }

        public async Task<int> UpdateAsync(Book book, CancellationToken cancellationToken)
        {
            using var connection = new NpgsqlConnection(_connectionStringAccessor.ConnectionString);
            await connection.OpenAsync(cancellationToken);
            using var command = new NpgsqlCommand();
            command.Connection = connection;
            command.CommandText = "UPDATE Books SET Title = @Title, Author = @Author, ISBN = @ISBN, ModifiedBy = @ModifiedBy, ModifiedDateUtc = @ModifiedDateUtc WHERE Id = @Id";
            command.Parameters.AddWithValue("@Id", book.Id);
            command.Parameters.AddWithValue("@Title", book.Title);
            command.Parameters.AddWithValue("@Author", book.Author);
            command.Parameters.AddWithValue("@ISBN", (object?)book.ISBN ?? DBNull.Value);
            command.Parameters.AddWithValue("@ModifiedBy", book.ModifiedBy!);
            command.Parameters.AddWithValue("@ModifiedDateUtc", book.ModifiedDateUtc!);

            return await command.ExecuteNonQueryAsync(cancellationToken);
        }

        public async Task<int> DeleteAsync(Guid id, string deletedBy, CancellationToken cancellationToken)
        {
            using var connection = new NpgsqlConnection(_connectionStringAccessor.ConnectionString);
            await connection.OpenAsync(cancellationToken);
            using var command = new NpgsqlCommand();
            command.Connection = connection;
            command.CommandText = "UPDATE Books SET DeletedBy = @DeletedBy, DeletedDateUtc = @DeletedDateUtc WHERE Id = @Id";
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@DeletedBy", deletedBy);
            command.Parameters.AddWithValue("@DeletedDateUtc", DateTimeOffset.UtcNow);

            return await command.ExecuteNonQueryAsync(cancellationToken);
        }
    }
}