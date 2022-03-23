using ContactsAPI.Dto;

namespace ContactsAPI.Interfaces
{
    public interface IContactTypeRepository 
    {
        Task<ContactTypeCreateDTO> CreateAsync(ContactTypeCreateDTO type);
        Task<List<ContactTypeDTO>> GetAllAsync();
    }
}
