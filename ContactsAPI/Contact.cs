using System.ComponentModel.DataAnnotations;

namespace ContactsAPI
{
    public class Contact
    {
        public int Id { get; set; }

        [StringLength(20)]
        public string Name { get; set; } = String.Empty;

        [StringLength(20)]
        public string Phone { get; set; } = String.Empty;

        public int TypeId { get; set; }

        public ContactType? TypeName { get; set; }
    }
}
