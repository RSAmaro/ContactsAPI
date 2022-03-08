using System.ComponentModel.DataAnnotations;

namespace ContactsAPI
{
    public class ContactType
    {
        public int Id { get; set; }

        [StringLength(20)]
        public string TypeName { get; set; } = String.Empty;
    }
}
