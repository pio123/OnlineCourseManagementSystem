using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using OnlineCourseManagementSystem.Data;
using System.IO;

public class SchoolContextFactory : IDesignTimeDbContextFactory<SchoolContext>
{
    public SchoolContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        var optionsBuilder = new DbContextOptionsBuilder<SchoolContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new SchoolContext(optionsBuilder.Options);
    }
}
