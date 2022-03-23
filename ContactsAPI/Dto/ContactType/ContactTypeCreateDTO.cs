namespace ContactsAPI.Dto
{
    public class ContactTypeCreateDTO
    {
        public string Name { get; set; }

        public ContactType ToEntity()
        {
            return new ContactType { Name = Name };
        }
    }
}
