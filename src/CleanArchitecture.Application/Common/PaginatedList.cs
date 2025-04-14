namespace CleanArchitecture.Application.Common
{
  public class PaginatedList<T>
  {

    private PaginatedList(List<T> items, int pageIndex, int pageSize, int totalPages)
    {
      Items = items;
      PageIndex = pageIndex;
      TotalPages = totalPages;
      PageSize = pageSize;
    }

    public List<T> Items { get; }
    public int PageIndex { get; }
    public int TotalPages { get; }
    public int PageSize { get; }
    public bool HasPreviousPage => PageIndex > 1;
    public bool HasNextPage => PageIndex < TotalPages;

    public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> query, int pageIndex, int pageSize)
    {
      if (pageIndex <= 0)
        pageIndex = 1;

      var itemCounts = await query.CountAsync();

      var totalPages = (int)Math.Ceiling(itemCounts / (double)pageSize);
      if (pageIndex > totalPages)
        pageIndex = 1;

      var items = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();

      return new(items, pageIndex, pageSize, totalPages);
    }
  }
}