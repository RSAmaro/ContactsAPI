namespace ContactsAPI.Pagination
{
    public class PaginationParams
    {
        private int _maxItemsPerPage = 50;
        private int itemsPerPage;

        public string? Qry { get; set; }
        public int Page { get; set; } = 1;
        public int ItemsPerPage
        {
            get => itemsPerPage;
            set => itemsPerPage = value > _maxItemsPerPage ? _maxItemsPerPage : value;
        }
    }
}
