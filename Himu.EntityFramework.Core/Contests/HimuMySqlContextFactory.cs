using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Himu.EntityFramework.Core.Contests
{
    public class HimuMySqlContextFactory<T> : IDesignTimeDbContextFactory<T> 
        where T : DbContext
    {
        virtual public T CreateDbContext(string[] args)
        {
            DbContextOptionsBuilder<T> builder = new();
            const string connectionString = "server=localhost;uid=root;pwd=liuhuan123;database=himuoj;Character Set=utf8;persist security info=True";

            builder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
                   .EnableDetailedErrors()
                   .EnableSensitiveDataLogging();

            return (T)Activator.CreateInstance(typeof(T), builder.Options)!;
        }
    }

}
