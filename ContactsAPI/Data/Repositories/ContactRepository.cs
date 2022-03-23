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

        public async Task<ContactCreateDTO> CreateContactAsync(ContactCreateDTO contact)
        {
            _ctx.Contacts.Add(contact.ToEntity());
            await _ctx.SaveChangesAsync();
            return contact;
        }


        public async Task<List<ContactDTO>> GetAllAsync()
        {
            var items = await _ctx.Contacts.Include(t => t.Type).Where(c => c.DateDeleted == null).ToListAsync();
            List<ContactDTO> result = items.Select(c => new ContactDTO(c)).ToList();
            return result;
        }

        public async Task<PaginationMetadata<ContactDTO>> GetAllPaginated(PaginationParams parameter)
        {
            IQueryable<Contact> contacts = _ctx.Contacts.Include(t => t.Type).Where(c => c.DateDeleted == null);
            PaginationMetadata<ContactDTO> ListContacts = new(contacts.Count(), parameter.Page, parameter.ItemsPerPage);

            if ((parameter.Page < 1) || (parameter.ItemsPerPage < 1))
            {
                ListContacts.Success = false;
                ListContacts.Message = "Page ou Pagination tem parametros inválidos";
                return ListContacts;
            }

            if (parameter.Qry != null && (parameter.QryParam == null || parameter.QryParam.Length < 1))
                contacts = contacts.Where(c => c.Name.Contains(parameter.Qry.Trim()) ||
                                               Convert.ToString(c.Id).Contains(parameter.Qry) ||
                                               Convert.ToString(c.Phone).Contains(parameter.Qry) ||
                                               c.Type.Name.Contains(parameter.Qry.Trim()));

            if(parameter.QryParam is not null && parameter.Qry is not null)
                foreach (var item in parameter.QryParam)
                {
                    switch (item)
                    {
                        case "id":
                            contacts = contacts.Where(c => Convert.ToString(c.Id).Contains(parameter.Qry.Trim()));
                            break;
                        case "name":
                            contacts = contacts.Where(c => c.Name.Contains(parameter.Qry.Trim()));
                            break;
                        case "phone":
                            contacts = contacts.Where(c => Convert.ToString(c.Phone).Contains(parameter.Qry.Trim()));
                            break;
                        case "typeName":
                            contacts = contacts.Where(c => c.Type.Name.Contains(parameter.Qry.Trim()));
                            break;
                        default:
                            break;
                    }
                }

            contacts = parameter.Sort switch
            {
                "id" => contacts.OrderBy(c => c.Id),
                "id_desc" => contacts.OrderByDescending(c => c.Id),
                "name" => contacts.OrderBy(c => c.Name),
                "name_desc" => contacts.OrderByDescending(c => c.Name),
                "phone" => contacts.OrderBy(p => p.Phone),
                "phone_desc" => contacts.OrderByDescending(c => c.Phone),
                "typeName" => contacts.OrderBy(p => p.Type.Name),
                "typeName_desc" => contacts.OrderByDescending(c => c.Type.Name),
                _ => contacts.OrderBy(c => c.Id),
            };

            ListContacts.TotalCount = contacts.Count();

            var items = await contacts.Skip((parameter.Page - 1) * parameter.ItemsPerPage)
                                      .Take(parameter.ItemsPerPage)
                                      .ToListAsync();

            ListContacts.Results = items.Select(c => new ContactDTO(c)).ToList();

            ListContacts.Success = true;
            ListContacts.Message = "Encontrado com Sucesso!";

            return ListContacts;
        }

        public async Task<ContactListDTO> GetByIdAsync(int id)
        {
            //var contact = await _context.Contacts.FindAsync(id);

            //if (contact == null)
            //{
            //    return NotFound();
            //}
            var items = await _ctx.Contacts.Include(t => t.Type).FirstOrDefaultAsync(contact => contact.Id == id);
            
            ContactListDTO result = new(items);
            return result;
        }

        public async Task<MessageHelper<ContactListDTO>> Edit(int id, ContactEditDTO c)
        {
            Contact contact = c.ToEntity();
            if (id != contact.Id)
            {
                return new MessageHelper<ContactListDTO>(false, "Request Inválido!", new ContactListDTO(contact));
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
                    if (entry.Entity is ContactListDTO)
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

            return new MessageHelper<ContactListDTO>(true, "Editado com Sucesso!", new ContactListDTO(contact));
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
