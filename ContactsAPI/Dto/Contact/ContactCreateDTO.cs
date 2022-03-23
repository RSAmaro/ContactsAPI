namespace ContactsAPI.Dto
{
    public class ContactCreateDTO
    {
        public string Name { get; set; }
        public int Phone { get; set; }
        public int TypeId { get; set; }

        public Contact ToEntity()
        {
            return new Contact { Name = Name, Phone = Phone, TypeId = TypeId };
        }
    }
}
