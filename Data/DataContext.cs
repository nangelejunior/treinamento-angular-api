using Microsoft.EntityFrameworkCore;
using Treinamento.Angular.Api.Models;

namespace Treinamento.Angular.Api.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
    }
}