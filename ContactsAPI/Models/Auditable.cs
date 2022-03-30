using System.ComponentModel.DataAnnotations;

namespace ContactsAPI
{
    public abstract class Auditable
    {
        public DateTimeOffset DateCreated { get; set; }

        //[ConcurrencyCheck]
        public DateTimeOffset? DateUpdated { get; set; }
        public DateTimeOffset? DateDeleted { get; set; }

    }
}
