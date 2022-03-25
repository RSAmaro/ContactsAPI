using ContactsAPI.Dto;
using ContactsAPI.Interfaces;
using ContactsAPI.Pagination;
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

        // GET: api/ContactType
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContactTypeDTO>>> GetTypes()
        {
            return Ok(await _types.GetAllAsync());
        }

        // GET: api/ContactType/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ContactListDTO>> GetContact(int id)
        {
            return Ok(await _types.GetByIdAsync(id));
        }

        // POST: api/ContactType
        [HttpPost]
        public async Task<ActionResult<ContactTypeCreateDTO>> PostType(ContactTypeCreateDTO type)
        {
            return Ok(await _types.CreateTypeAsync(type));
        }

        // GET: api/ContactType/List
        [HttpPost("List")]
        public async Task<PaginationMetadata<ContactTypeDTO>> Get([FromBody] PaginationParams @params)
        {
            return await _types.GetAllPaginated(@params);
        }

        // PUT: api/ContactType/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<ActionResult<ContactListDTO>> PutContact(int id, ContactTypeEditDTO contact)
        {
            return Ok(await _types.Edit(id, contact));
        }
    }
}
