namespace ContactsAPI.Dto
{
    public class ContactEditDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Phone { get; set; }
        public int TypeId { get; set; }

        public Contact ToEntity()
        {
            return new Contact { Id = Id, Name = Name, Phone = Phone, TypeId = TypeId };
        }
    }
}
