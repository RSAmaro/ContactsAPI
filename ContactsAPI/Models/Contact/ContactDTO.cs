namespace ContactsAPI.Dto
{
    public class ContactDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int Phone { get; set; }

        public int TypeId { get; set; }
        public string TypeName { get; set; }

        public ContactDTO(Contact contact)
        {
            Id = contact.Id;
            Name = contact.Name;
            Phone = contact.Phone;
            TypeId = contact.TypeId;
            TypeName = contact.Type.Name;
        }

    }
}
