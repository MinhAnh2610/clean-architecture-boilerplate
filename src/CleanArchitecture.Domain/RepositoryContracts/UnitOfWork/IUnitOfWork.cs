namespace CleanArchitecture.Domain.RepositoryContracts.UnitOfWork;

public interface IUnitOfWork : IDisposable
{
  Task<int> CompleteAsync();
}
