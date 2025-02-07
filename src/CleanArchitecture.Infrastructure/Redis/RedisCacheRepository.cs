using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace CleanArchitecture.Infrastructure.Redis;

public interface IRedisCacheRepository
{
  Task SetAsync<T>(string key, T value, TimeSpan expiration, CancellationToken cancellationToken = default);
  Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);
  Task RemoveAsync(string key, CancellationToken cancellationToken = default);
}

public class RedisCacheRepository : IRedisCacheRepository
{
  private readonly IDistributedCache _cache;

  public RedisCacheRepository(IDistributedCache cache)
  {
    _cache = cache;
  }

  public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
  {
    var data = await _cache.GetStringAsync(key, cancellationToken);
    return (data == null) ? JsonConvert.DeserializeObject<T>(data!) : default;
  }

  public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
  {
    await _cache.RemoveAsync(key, cancellationToken);
  }

  public async Task SetAsync<T>(string key, T value, TimeSpan expiration, CancellationToken cancellationToken = default)
  {
    var data = JsonConvert.SerializeObject(value);
    await _cache.SetStringAsync(key, data, cancellationToken);
  }
}
