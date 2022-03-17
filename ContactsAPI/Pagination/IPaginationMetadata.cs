
namespace ContactsAPI.Pagination
{
    public interface IPaginationMetadata<T>
    {
        int CurrentPage { get; set; }
        List<T> Results { get; set; }
        int TotalCount { get; set; }
        int TotalPages { get; set; }
    }
}