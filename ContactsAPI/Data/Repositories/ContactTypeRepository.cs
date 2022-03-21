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

        public async Task<List<ContactType>> GetAllAsync()
        {
            return await _ctx.ContactTypes.ToListAsync();
        }
    }
}
