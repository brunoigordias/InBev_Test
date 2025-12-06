namespace InBev.Application.DTOs;

/// <summary>
/// DTO para requisições paginadas
/// </summary>
public class PagedRequest
{
    private const int MaxPageSize = 100;
    private int _pageSize = 10;

    /// <summary>
    /// Número da página (começa em 1)
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Quantidade de itens por página (máximo 100)
    /// </summary>
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }

    /// <summary>
    /// Termo de busca (opcional)
    /// </summary>
    public string? SearchTerm { get; set; }
}

