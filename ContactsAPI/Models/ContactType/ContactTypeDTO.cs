
namespace ContactsAPI.Dto
{
    public class ContactTypeDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ContactTypeDTO(ContactType type)
        {
            Id = type.Id;
            Name = type.Name;
        }
    }
}
