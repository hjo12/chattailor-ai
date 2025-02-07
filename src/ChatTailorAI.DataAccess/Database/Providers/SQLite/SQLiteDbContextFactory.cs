using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace ChatTailorAI.DataAccess.Database.Providers.SQLite
{
    public class SQLiteDbContextFactory : IDesignTimeDbContextFactory<SQLiteDb>
    {
        public SQLiteDb CreateDbContext(string[] args)
        {
            // Adjust the path to reach appsettings.json in the WinUI project
            // This is only ran locally so just hardcoding to the path
            var basePath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "../ChatTailorAI.WinUI/"));

            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("Configuration/appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<SQLiteDb>();

            // Use DbContextConfiguration to configure options
            DbContextConfiguration.ConfigureDbContextOptions(optionsBuilder, configuration);

            return new SQLiteDb(optionsBuilder.Options);
        }
    }
}
