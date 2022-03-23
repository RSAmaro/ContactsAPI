using ContactsAPI.Dto;
using ContactsAPI.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ContactsAPI.Data.Repositories
{
    public class ContactTypeRepository : IContactTypeRepository
    {
        private readonly DataContext _ctx;

        public ContactTypeRepository(DataContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<List<ContactTypeDTO>> GetAllAsync()
        {
            var items = await _ctx.ContactTypes.Where(c => c.DateDeleted == null).ToListAsync();
            List<ContactTypeDTO> result = items.Select(c => new ContactTypeDTO(c)).ToList();
            return result;
        }

        public async Task<ContactTypeCreateDTO> CreateAsync(ContactTypeCreateDTO type)
        {
            _ctx.ContactTypes.Add(type.ToEntity());
            await _ctx.SaveChangesAsync();

            return type;
        }

    }
}
