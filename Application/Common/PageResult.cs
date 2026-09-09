namespace Training.Angular.Api.Application.Common;

public sealed class PageResult<T>
{
    public const int DefaultPage = 1;
    public const int DefaultPageSize = 20;
    public const int MaxPageSize = 100;

    public int Page { get; }

    public int PageSize { get; }

    public IReadOnlyList<T> Items { get; }

    public PageResult(int page, int pageSize, IReadOnlyList<T> items)
    {
        Page = page;
        PageSize = pageSize;
        Items = items;
    }

    public static (int Page, int PageSize) Clamp(int page, int pageSize)
    {
        if (page < DefaultPage)
            page = DefaultPage;

        if (pageSize < DefaultPageSize)
            pageSize = DefaultPageSize;

        if (pageSize > MaxPageSize)
            pageSize = MaxPageSize;

        return (page, pageSize);
    }
}
