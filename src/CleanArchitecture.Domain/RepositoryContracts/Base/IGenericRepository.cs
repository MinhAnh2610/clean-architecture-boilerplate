namespace CleanArchitecture.Domain.RepositoryContracts.Base
{
  public interface IGenericRepository<T> where T : class
  {
    void Create(T entity);
    Task CreateAsync(T entity);
    List<T> GetAll();
    Task<List<T>> GetAllAsync();
    T GetById(Guid code);
    T GetById(int id);
    T GetById(string code);
    Task<T?> GetByIdAsync(Guid code);
    Task<T?> GetByIdAsync(int id);
    Task<T?> GetByIdAsync(string code);
    void Remove(T entity);
    void Update(T entity);
    void Attach(T entity);
    IQueryable<T> GetQueryable();
  }
}