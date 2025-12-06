/**
 * Interface genérica para resposta paginada da API
 */
export interface PagedResponse<T> {
  items: T[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

/**
 * Interface para parâmetros de requisição paginada
 */
export interface PagedRequest {
  pageNumber: number;
  pageSize: number;
  searchTerm?: string;
}

