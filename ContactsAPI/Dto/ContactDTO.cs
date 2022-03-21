namespace ContactsAPI.Dto
{
    public class ContactDTO : Auditable
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int Phone { get; set; }
        public ContactType Type { get; set; }
    }
}
