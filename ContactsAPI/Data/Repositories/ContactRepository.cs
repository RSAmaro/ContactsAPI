using ContactsAPI.Dto;
using ContactsAPI.Interfaces;
using ContactsAPI.Models;
using ContactsAPI.Pagination;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ContactsAPI.Data.Repositories
{
    public class ContactRepository : IContactRepository
    {
        private readonly DataContext _ctx;

        public ContactRepository(DataContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<Contact> CreateContactAsync(Contact contact)
        {
            _ctx.Contacts.Add(contact);
            await _ctx.SaveChangesAsync();
            return contact;
        }

        public async Task<List<Contact>> GetAllAsync()
        {
            return await _ctx.Contacts.Where(c => c.DateDeleted == null).ToListAsync();
        }

        public async Task<PaginationMetadata<ContactDTO>> GetAllPaginated(PaginationParams parameter)
        {
            IQueryable<Contact> contacts = _ctx.Contacts.Where(c => c.DateDeleted == null);
            PaginationMetadata<ContactDTO> ListContacts = new(contacts.Count(), parameter.Page, parameter.ItemsPerPage);

            if ((parameter.Page < 1) || (parameter.ItemsPerPage < 1))
            {
                ListContacts.Success = false;
                ListContacts.Message = "Page ou Pagination tem parametros inválidos";
                return ListContacts;
            }

            if (parameter.Qry != null)
                contacts = contacts.Where(c => c.Name.Contains(parameter.Qry.Trim()) ||
                                               Convert.ToString(c.Id).Contains(parameter.Qry) ||
                                               Convert.ToString(c.Phone).Contains(parameter.Qry));

            if (parameter.QryId != null)
                contacts = contacts.Where(c => Convert.ToString(c.Id).Contains(parameter.QryId.Trim()));

            if (parameter.QryName != null)
                contacts = contacts.Where(c => c.Name.Contains(parameter.QryName.Trim()));

            if (parameter.QryPhone != null)
                contacts = contacts.Where(c => Convert.ToString(c.Phone).Contains(parameter.QryPhone.Trim()));

            contacts = parameter.Sort switch
            {
                "id" => contacts.OrderBy(c => c.Id),
                "id_desc" => contacts.OrderByDescending(c => c.Id),
                "name" => contacts.OrderBy(c => c.Name),
                "name_desc" => contacts.OrderByDescending(c => c.Name),
                "phone" => contacts.OrderBy(p => p.Phone),
                "phone_desc" => contacts.OrderByDescending(c => c.Phone),
                _ => contacts.OrderBy(c => c.Id),
            };


            var items = await contacts.Skip((parameter.Page - 1) * parameter.ItemsPerPage)
                                      .Take(parameter.ItemsPerPage)
                                      .ToListAsync();

            ListContacts.Results = items.Select(c => new ContactDTO
            {
                Id = c.Id,
                Name = c.Name,
                Phone = c.Phone
            }).ToList();

            ListContacts.Success = true;
            ListContacts.Message = "Encontrado com Sucesso!";

            return ListContacts;
        }

        public async Task<Contact> GetByIdAsync(int id)
        {
            //var contact = await _context.Contacts.FindAsync(id);

            //if (contact == null)
            //{
            //    return NotFound();
            //}

            return await _ctx.Contacts.FirstOrDefaultAsync(contact => contact.Id == id);
        }

        public async Task<MessageHelper<Contact>> Edit(int id, Contact contact)
        {
            if (id != contact.Id)
            {
                return new MessageHelper<Contact>(false, "Request Inválido!", contact);
            }

            _ctx.Entry(contact).State = EntityState.Modified;

            try
            {
                await _ctx.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                foreach (var entry in ex.Entries)
                {
                    if (entry.Entity is Contact)
                    {
                        var proposedValues = entry.CurrentValues;
                        var databaseValues = entry.GetDatabaseValues();

                        foreach (var property in proposedValues.Properties)
                        {
                            var proposedValue = proposedValues[property];
                            var databaseValue = databaseValues[property];

                            // TODO: decide which value should be written to database
                            // proposedValues[property] = <value to be saved>;
                            // databaseValues[property] = proposedValue;

                            //proposedValues = contact;
                        }

                        // Refresh original values to bypass next concurrency check
                        entry.OriginalValues.SetValues(databaseValues);
                    }
                    else
                    {
                        throw new NotSupportedException(
                            "Don't know how to handle concurrency conflicts for "
                            + entry.Metadata.Name);
                    }
                }


                //if (!ContactExists(id))
                //{
                //    return new MessageHelper<Contact>(false, "Não foi encontrado nenhum contato!", contact);
                //}
                //else
                //{
                //    throw;
                //}
            }

            return new MessageHelper<Contact>(true, "Editado com Sucesso!", contact);
        }

        public async Task<MessageHelper> Delete(int id)
        {
            var contact = await _ctx.Contacts.FindAsync(id);

            if (contact == null)
                return new MessageHelper(false, "Não existe nenhum contato com esse ID");

            if (typeof(Auditable).IsAssignableFrom(typeof(Contact))) {
                contact.DateDeleted = DateTimeOffset.UtcNow;
                _ctx.Entry(contact).CurrentValues.SetValues(contact);

                //_ctx.Contacts.Remove(contact);
                await _ctx.SaveChangesAsync();

                return new MessageHelper(true, "Apagado com Sucesso");
            }

            return new MessageHelper(false, "Não foi apagado");
        }

        private bool ContactExists(int id)
        {
            return _ctx.Contacts.Any(e => e.Id == id);
        }
    }
}
