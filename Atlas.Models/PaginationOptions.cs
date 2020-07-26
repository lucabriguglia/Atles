namespace Atlas.Models
{
    public class PaginationOptions
    {
        private const int DefaultPageSize = 2;

        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = DefaultPageSize;

        public int Skip => (CurrentPage - 1) * PageSize;

        public PaginationOptions()
        {
            
        }

        public PaginationOptions(int? currentPage, int? pageSize = DefaultPageSize)
        {
            CurrentPage = currentPage ?? 1;
            PageSize = pageSize ?? DefaultPageSize;
        }
    }
}
