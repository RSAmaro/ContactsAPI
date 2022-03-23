using ContactsAPI.Dto;
using ContactsAPI.Models;
using ContactsAPI.Pagination;
using Microsoft.AspNetCore.Mvc;

namespace ContactsAPI.Interfaces
{
    public interface IContactRepository
    {
        Task<List<ContactDTO>> GetAllAsync();
        Task<PaginationMetadata<ContactDTO>> GetAllPaginated(PaginationParams parameter);
        Task<ContactListDTO> GetByIdAsync(int id);
        Task<ContactCreateDTO> CreateContactAsync(ContactCreateDTO contact);
        Task<MessageHelper<ContactListDTO>> Edit(int id, ContactEditDTO contact);
        Task<MessageHelper> Delete(int id);
    }
}
