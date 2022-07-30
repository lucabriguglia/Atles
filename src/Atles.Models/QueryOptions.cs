namespace Atles.Models
{
    public class QueryOptions
    {
        private const int DefaultPageSize = 10;

        public string Search { get; set; }

        public string OrderByField { get; set; }
        public OrderByDirectionType? OrderByDirection { get; set; }

        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = DefaultPageSize;

        public int Skip => (CurrentPage - 1) * PageSize;

        public QueryOptions()
        {
        }

        public QueryOptions(int? currentPage = 1, string search = null, string orderByField = null, string orderByDirection = null, int? pageSize = null)
        {
            CurrentPage = SetCurrentPage(currentPage);
            Search = search;
            OrderByField = orderByField;
            PageSize = pageSize ?? DefaultPageSize;

            if (!string.IsNullOrWhiteSpace(orderByDirection) && Enum.TryParse(orderByDirection, out OrderByDirectionType orderByDirectionType))
            {
                OrderByDirection = orderByDirectionType;
            }
        }

        private static int SetCurrentPage(int? currentPage) => currentPage == null || currentPage.Value < 1 ? 1 : currentPage.Value;

        public bool SearchIsDefined() => !string.IsNullOrWhiteSpace(Search);
        public bool OrderByIsDefined() => !string.IsNullOrWhiteSpace(OrderByField) && OrderByDirection != null;
    }
}
