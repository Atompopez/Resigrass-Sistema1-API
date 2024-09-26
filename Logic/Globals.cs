using Microsoft.Extensions.Configuration;

namespace ResiGrass_API.Logic
{
    public static class Globals
    {
        private static string? _connectionString;
        public static void Initialize(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("PostgresConnection");
        }

        public static string ConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(_connectionString))
                {
                    throw new InvalidOperationException("La cadena de conexión no ha sido inicializada.");
                }
                return _connectionString;
            }
        }
    }
}
