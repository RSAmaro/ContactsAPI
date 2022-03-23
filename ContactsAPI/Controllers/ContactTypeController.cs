using ContactsAPI.Dto;
using ContactsAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ContactsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactTypeController : ControllerBase
    {
        private readonly IContactTypeRepository _types;

        public ContactTypeController(IContactTypeRepository types)
        {
            _types = types;
        }

        // GET: api/Contacts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContactTypeDTO>>> GetTypes()
        {
            return Ok(await _types.GetAllAsync());
        }

        // POST: api/Contacts
        [HttpPost]
        public async Task<ActionResult<ContactTypeCreateDTO>> PostType(ContactTypeCreateDTO type)
        {
            return Ok(await _types.CreateAsync(type));
        }

    }
}
