namespace ContactsAPI.Models
{

    public class MessageHelper
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
    }

    public class MessageHelper<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? obj { get; set; }
    }

}
