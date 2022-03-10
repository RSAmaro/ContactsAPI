using System.ComponentModel.DataAnnotations;

namespace ContactsAPI
{
    public class Contact
    {
        public int Id { get; set; }

        [StringLength(20, MinimumLength = 2)]
        [Required]
        public string Name { get; set; } = String.Empty;

        [RegularExpression(@"^\d{9}$")]
        [Required]
        public int Phone { get; set; }

    }
}
