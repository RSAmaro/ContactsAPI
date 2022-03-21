using System.ComponentModel.DataAnnotations;

namespace ContactsAPI
{
    public class ContactType
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; }
    }
}