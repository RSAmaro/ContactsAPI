namespace ContactsAPI.Dto
{
    public class ContactListDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int Phone { get; set; }
        public int TypeId { get; set; }

        public ContactListDTO(Contact contact)
        {
            Id = contact.Id;
            Name = contact.Name;
            Phone = contact.Phone;
            TypeId = contact.TypeId;
        }
    }
}
