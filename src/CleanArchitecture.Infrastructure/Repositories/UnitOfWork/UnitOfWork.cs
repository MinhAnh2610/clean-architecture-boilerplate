using CleanArchitecture.Application.ServiceContracts;
using CleanArchitecture.Domain.RepositoryContracts;
using CleanArchitecture.Domain.RepositoryContracts.UnitOfWork;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Infrastructure.Repositories.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
  private readonly ApplicationDbContext _context;
  private IDbContextTransaction _transaction;
  private bool _commited;
  private readonly ILogger<UnitOfWork> _logger;
  private readonly ITimeZoneService _timeZoneService;

  #region Repositories



  #endregion

  public UnitOfWork(ApplicationDbContext context, ILogger<UnitOfWork> logger, ITimeZoneService timeZoneService)
  {
    _context = context;
    _logger = logger;
    _timeZoneService = timeZoneService;
    _transaction = _context.Database.BeginTransaction();
  }

  public async Task RollBackAsync()
  {
    await _transaction.RollbackAsync();
  }

  public async Task<bool> CompleteAsync()
  {
    try
    {
      await _context.SaveChangesAsync();
      await _transaction.CommitAsync();
      return true;
    }
    catch (Exception ex)
    {
      _transaction.RollbackAsync();
      _logger.LogError($"Database saved failed at {_timeZoneService.ConvertToLocalTime(DateTime.UtcNow)}\n" +
                       $"with error: {ex.Message}");
      return false;
    }
  }
  public async Task<IDbContextTransaction> BeginTransactionAsync()
  {
    return await _context.Database.BeginTransactionAsync();
  }
  public void Dispose()
  {
    _transaction?.Dispose();
    _transaction = null;
  }
}