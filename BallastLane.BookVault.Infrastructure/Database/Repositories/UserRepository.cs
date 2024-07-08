using BallastLane.BookVault.Domain.Entities;
using BallastLane.BookVault.Domain.Repositories;
using Npgsql;
namespace BallastLane.BookVault.Infrastructure.Database.Repositories
{
    public class UserRepository(ConnectionStringAccessor connectionStringAccessor) : IUserRepository
    {
        private readonly ConnectionStringAccessor _connectionStringAccessor = connectionStringAccessor;

        public async Task<User?> GetUserByUsername(string username, CancellationToken cancellationToken)
        {
            User? user = null;
            using (var connection = new NpgsqlConnection(_connectionStringAccessor.ConnectionString))
            {
                await connection.OpenAsync(cancellationToken);
                using var command = new NpgsqlCommand();
                command.Connection = connection;
                command.CommandText = "SELECT * FROM Users WHERE Username = @Username";
                command.Parameters.AddWithValue("@Username", username);

                using var reader = await command.ExecuteReaderAsync(cancellationToken);
                if (await reader.ReadAsync(cancellationToken))
                {
                    user = new User
                    {
                        Id = reader.GetGuid(reader.GetOrdinal("Id")),
                        Username = reader.GetString(reader.GetOrdinal("Username")),
                        PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),
                        Salt = reader.GetString(reader.GetOrdinal("Salt"))
                    };
                }
            }
            return user;
        }

        public async Task<bool> ExistsByUsername(string username, CancellationToken cancellationToken)
        {
            using var connection = new NpgsqlConnection(_connectionStringAccessor.ConnectionString);
            await connection.OpenAsync(cancellationToken);
            using var command = new NpgsqlCommand();
            command.Connection = connection;
            command.CommandText = "SELECT COUNT(1) FROM Users WHERE Username = @Username";
            command.Parameters.AddWithValue("@Username", username);

            return (long)(await command.ExecuteScalarAsync(cancellationToken) ?? 0) > 0;
        }

        public async Task<int> AddUser(User user, CancellationToken cancellationToken)
        {
            using var connection = new NpgsqlConnection(_connectionStringAccessor.ConnectionString);
            await connection.OpenAsync(cancellationToken);
            using var command = new NpgsqlCommand();
            command.Connection = connection;
            command.CommandText = "INSERT INTO Users (Id, Username, PasswordHash, Salt) VALUES (@Id, @Username, @PasswordHash, @Salt)";
            user.Id = Guid.NewGuid();
            command.Parameters.AddWithValue("@Id", user.Id);
            command.Parameters.AddWithValue("@Username", user.Username);
            command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
            command.Parameters.AddWithValue("@Salt", user.Salt);

            return await command.ExecuteNonQueryAsync(cancellationToken);
        }
    }
}