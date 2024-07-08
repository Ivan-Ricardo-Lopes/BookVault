namespace BallastLane.BookVault.Infrastructure.Database
{
    public class ConnectionStringAccessor(string connectionString)
    {
        public string ConnectionString { get; set; } = connectionString;
    }
}
