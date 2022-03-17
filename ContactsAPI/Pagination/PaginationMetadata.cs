namespace ContactsAPI.Pagination
{
    public class PaginationMetadata<T> : IPaginationMetadata<T>
    {
        public List<T> Results { get; set; }

        public PaginationMetadata(int totalCount, int currentPage, int itemPerPage)
        {
            this.Results = new List<T>();
            TotalCount = totalCount;
            CurrentPage = currentPage;
            TotalPages = (int)Math.Ceiling(TotalCount / (double)itemPerPage);
        }

        public int CurrentPage { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }

        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
