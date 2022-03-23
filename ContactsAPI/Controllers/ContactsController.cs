#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContactsAPI;
using ContactsAPI.Data;
using ContactsAPI.Pagination;
using ContactsAPI.Dto;
using System.Text.Json;
using ContactsAPI.Interfaces;
using ContactsAPI.Models;

namespace ContactsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        private readonly IContactRepository _contacts;

        public ContactsController(IContactRepository contacts)
        {
            _contacts = contacts;
        }

        // GET: api/Contacts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContactDTO>>> GetContacts()
        {
            return Ok(await _contacts.GetAllAsync());
        }

        // GET: api/Contacts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ContactListDTO>> GetContact(int id)
        {
            return Ok(await _contacts.GetByIdAsync(id));
        }

        // GET: api/Contacts/List
        [HttpPost("List")]
        public async Task<PaginationMetadata<ContactDTO>> Get([FromBody] PaginationParams @params)
        {
            return await _contacts.GetAllPaginated(@params);
        }

        // PUT: api/Contacts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<ActionResult<ContactListDTO>> PutContact(int id, ContactEditDTO contact)
        {
            return Ok(await _contacts.Edit(id, contact));
        }

        // POST: api/Contacts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ContactCreateDTO>> PostContact(ContactCreateDTO contact)
        {
            return Ok(await _contacts.CreateContactAsync(contact));
        }

        //// DELETE: api/Contacts/5
        [HttpDelete("{id}")]
        public async Task<MessageHelper> DeleteContact(int id)
        {
            return await _contacts.Delete(id);
        }

    }
}
