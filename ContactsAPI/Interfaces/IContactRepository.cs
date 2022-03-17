using ContactsAPI.Dto;
using ContactsAPI.Models;
using ContactsAPI.Pagination;
using Microsoft.AspNetCore.Mvc;

namespace ContactsAPI.Interfaces
{
    public interface IContactRepository
    {
        Task<List<Contact>> GetAllAsync();
        Task<PaginationMetadata<ContactDTO>> GetAllPaginated(PaginationParams parameter);
        Task<Contact> GetByIdAsync(int id);
        Task<Contact> CreateContactAsync(Contact contact);
        Task<MessageHelper<Contact>> Edit(int id, Contact contact);
        Task<MessageHelper> Delete(int id);
    }
}
