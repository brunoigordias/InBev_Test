namespace InBev.Application.DTOs;

/// <summary>
/// DTO genérico para respostas paginadas
/// </summary>
/// <typeparam name="T">Tipo dos itens da página</typeparam>
public class PagedResponse<T>
{
    /// <summary>
    /// Lista de itens da página atual
    /// </summary>
    public IReadOnlyList<T> Items { get; set; } = new List<T>();

    /// <summary>
    /// Número da página atual
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// Quantidade de itens por página
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total de itens (em todas as páginas)
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Total de páginas
    /// </summary>
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

    /// <summary>
    /// Indica se existe página anterior
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>
    /// Indica se existe próxima página
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;

    public PagedResponse()
    {
    }

    public PagedResponse(IReadOnlyList<T> items, int count, int pageNumber, int pageSize)
    {
        Items = items;
        TotalCount = count;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }
}

