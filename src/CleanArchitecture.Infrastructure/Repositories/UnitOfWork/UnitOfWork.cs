using CleanArchitecture.Domain.RepositoryContracts.UnitOfWork;

namespace CleanArchitecture.Infrastructure.Repositories.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
  private readonly ApplicationDbContext _context;

  public UnitOfWork(ApplicationDbContext context)
  {
    _context = context;
  }

  public async Task<int> CompleteAsync()
  {
    return await _context.SaveChangesAsync();
  }

  public void Dispose()
  {
    _context.Dispose();
  }
}
