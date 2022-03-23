namespace ContactsAPI.Pagination
{
    public class PaginationParams
    {
        // Default Pagination Values
        private readonly int _maxItemsPerPage = 50;
        private int itemsPerPage;

        // Search Queries
        public string? Qry { get; set; }
        public string[]? QryParam { get; set; }

        // Pagination
        public int Page { get; set; } = 1;
        public string? Sort { get; set; }
        public int ItemsPerPage
        {
            get => itemsPerPage;
            set => itemsPerPage = value > _maxItemsPerPage ? _maxItemsPerPage : value;
        }
    }
}
