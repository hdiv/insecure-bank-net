using System.Data.Common;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace insecure_bank_net
{
    public class Program
    {
        private static IWebHost Host { get; set; }
        private static DbConnection Connection { get; set;}

        public static void Main(string[] args)
        {
            Host = CreateWebHost(args);
            Connection = PersistentConnection();
            Host.Run();
        }

        private static IWebHost CreateWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
        
        // Connection that will be kept persistent holding the in memory database
        // https://docs.microsoft.com/es-es/dotnet/standard/data/sqlite/in-memory-databases
        private static DbConnection PersistentConnection() {
            var configuration = Host.Services.GetService(typeof(IConfiguration)) as IConfiguration;
            var conn = new SqliteConnection(configuration.GetConnectionString("DefaultConnection"));
            conn.Open();
            return conn;
        }
    }
}
