using System.Reflection;

namespace CleanArchitecture.Infrastructure.Data;

public interface IApplicationDbContext
{
  Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}


public class ApplicationDbContext : DbContext, IApplicationDbContext
{
  public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
  {

  }

  protected override void OnModelCreating(ModelBuilder builder)
  {
    builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    base.OnModelCreating(builder);
  }
}
