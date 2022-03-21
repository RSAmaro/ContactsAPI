namespace ContactsAPI.Interfaces
{
    public interface IContactTypeRepository 
    {
        Task<List<ContactType>> GetAllAsync();
    }
}
