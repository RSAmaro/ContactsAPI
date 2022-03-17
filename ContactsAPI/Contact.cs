using System.ComponentModel.DataAnnotations;

namespace ContactsAPI
{
    public class Contact
    {
        public int Id { get; set; }

        [StringLength(20, MinimumLength = 2)]
        [ConcurrencyCheck]
        [Required]
        public string Name { get; set; } = String.Empty;

        [RegularExpression(@"^\d{9}$")]
        [ConcurrencyCheck]
        [Required]
        public int Phone { get; set; }

    }
}
