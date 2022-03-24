using ContactsAPI.Dto;
using ContactsAPI.Models;
using ContactsAPI.Pagination;
using Microsoft.AspNetCore.Mvc;

namespace ContactsAPI.Interfaces
{
    public interface IContactRepository
    {
        Task<PaginationMetadata<ContactDTO>> GetAllPaginated(PaginationParams parameter);
        Task<MessageHelper<ContactListDTO>> GetByIdAsync(int id);
        Task<MessageHelper> CreateContactAsync(ContactCreateDTO contact);
        Task<MessageHelper<ContactListDTO>> Edit(int id, ContactEditDTO contact);
        Task<MessageHelper> Delete(int id);
    }
}
