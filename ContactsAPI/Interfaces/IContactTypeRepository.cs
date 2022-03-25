using ContactsAPI.Dto;
using ContactsAPI.Models;
using ContactsAPI.Pagination;

namespace ContactsAPI.Interfaces
{
    public interface IContactTypeRepository 
    {
        Task<PaginationMetadata<ContactTypeDTO>> GetAllPaginated(PaginationParams parameter);
        Task<MessageHelper<ContactTypeDTO>> GetByIdAsync(int id);
        Task<List<ContactTypeDTO>> GetAllAsync();
        Task<MessageHelper> CreateTypeAsync(ContactTypeCreateDTO type);
        Task<MessageHelper<ContactTypeDTO>> Edit(int id, ContactTypeEditDTO c);

    }
}
