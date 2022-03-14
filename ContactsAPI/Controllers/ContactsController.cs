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

namespace ContactsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        private readonly DataContext _context;

        public ContactsController(DataContext context)
        {
            _context = context;
        }

        // GET: api/Contacts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Contact>>> GetContacts()
        {
            return await _context.Contacts.ToListAsync();
        }

        // GET: api/Contacts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Contact>> GetContact(int id)
        {
            var contact = await _context.Contacts.FindAsync(id);

            if (contact == null)
            {
                return NotFound();
            }

            return contact;
        }

        // GET: api/Contacts/List
        [HttpGet("List")]
        public async Task<IActionResult> Get([FromQuery] PaginationParams @params)
        {
            try
            {
            IOrderedQueryable<Contact> contacts;

            if ( (@params.Page < 1) || (@params.ItemsPerPage < 1) )
                return ValidationProblem("Verifique os campos Page e ItemsPerPage!");

            if (@params.Qry != null)         
                contacts = _context.Contacts.Where(p => p.Name.Contains(@params.Qry)).OrderBy(p => p.Id);
            else 
                contacts = _context.Contacts.OrderBy(p => p.Id);

            if (contacts == null)
                return NotFound();
            

            PaginationMetadata<ContactDTO> ListContacts = new PaginationMetadata<ContactDTO>(contacts.Count(), @params.Page, @params.ItemsPerPage);

            //var paginationMetadata = new PaginationMetadata(contacts.Count(), @params.Page, @params.ItemsPerPage);
            //Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            var items = await contacts.Skip((@params.Page - 1) * @params.ItemsPerPage)
                                      .Take(@params.ItemsPerPage)
                                      .ToListAsync();

            ListContacts.Results = items.Select(e => new ContactDTO {
                Id = e.Id,
                Name = e.Name,
                Phone = e.Phone
            }).ToList();

            return Ok(ListContacts);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        // PUT: api/Contacts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutContact(int id, Contact contact)
        {
            if (id != contact.Id)
            {
                return BadRequest();
            }

            _context.Entry(contact).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContactExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Contacts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Contact>> PostContact(Contact contact)
        {
            _context.Contacts.Add(contact);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetContact", new { id = contact.Id }, contact);
        }

        // DELETE: api/Contacts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContact(int id)
        {
            var contact = await _context.Contacts.FindAsync(id);
            if (contact == null)
            {
                return NotFound();
            }

            _context.Contacts.Remove(contact);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ContactExists(int id)
        {
            return _context.Contacts.Any(e => e.Id == id);
        }
    }
}
