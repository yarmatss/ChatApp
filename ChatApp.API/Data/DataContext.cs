using ChatApp.Shared;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.API.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }
    public DbSet<Message> Messages { get; set; }
}
