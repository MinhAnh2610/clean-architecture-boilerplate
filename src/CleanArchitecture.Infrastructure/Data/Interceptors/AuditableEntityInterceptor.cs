using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace CleanArchitecture.Infrastructure.Data.Interceptors;

public class AuditableEntityInterceptor : SaveChangesInterceptor
{
  private readonly IHttpContextAccessor _httpContextAccessor;

  public AuditableEntityInterceptor(IHttpContextAccessor httpContextAccessor)
  {
    _httpContextAccessor = httpContextAccessor;
  }

  public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
  {
    UpdateEntities(eventData.Context);
    return base.SavingChanges(eventData, result);
  }

  public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
  {
    UpdateEntities(eventData.Context);
    return base.SavingChangesAsync(eventData, result, cancellationToken);
  }

  public void UpdateEntities(DbContext? context)
  {
    if (context == null) return;

    var user = _httpContextAccessor.HttpContext?.User.Identity?.Name ?? "Unknown";

    foreach (var entry in context.ChangeTracker.Entries<IEntity>())
    {
      if (entry.State == EntityState.Added)
      {
        entry.Entity.CreatedBy = user;
        entry.Entity.CreateAt = DateTime.UtcNow;
      }

      if (entry.State == EntityState.Added || entry.State == EntityState.Modified || entry.HasChangedOwnedEntities())
      {
        entry.Entity.LastModifiedBy = user;
        entry.Entity.LastModified = DateTime.UtcNow;
      }
    }
  }
}

public static class Extensions
{
  public static bool HasChangedOwnedEntities(this EntityEntry entry) =>
    entry.References.Any(r =>
      r.TargetEntry != null &&
      r.TargetEntry.Metadata.IsOwned() &&
      (r.TargetEntry.State == EntityState.Added || r.TargetEntry.State == EntityState.Modified));
}
