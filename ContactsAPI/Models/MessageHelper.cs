namespace ContactsAPI.Models
{

    public class MessageHelper
    {
        public MessageHelper(bool success, string message)
        {
            Success = success;
            Message = message;
        }

        public bool Success { get; set; }
        public string? Message { get; set; }
    }

    public class MessageHelper<T>
    {
        public MessageHelper(bool success, string? message, T obj)
        {
            Success = success;
            Message = message;
            this.obj = obj;
        }

        public bool Success { get; set; }
        public string? Message { get; set; }
        public T obj { get; set; }
    }

}
