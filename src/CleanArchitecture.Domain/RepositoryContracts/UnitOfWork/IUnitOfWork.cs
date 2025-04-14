using Microsoft.EntityFrameworkCore.Storage;

namespace CleanArchitecture.Domain.RepositoryContracts.UnitOfWork;

public interface IUnitOfWork : IDisposable
{
  Task<IDbContextTransaction> BeginTransactionAsync();
  Task RollBackAsync();
  Task<bool> CompleteAsync();
}
