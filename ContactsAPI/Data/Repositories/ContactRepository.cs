using ContactsAPI.Dto;
using ContactsAPI.Interfaces;
using ContactsAPI.Models;
using ContactsAPI.Pagination;
using FluentValidation;
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

        public async Task<MessageHelper> CreateContactAsync(ContactCreateDTO contact)
        {
            MessageHelper result = new();
            ContactValidator validation = new();

            var responseValidate = await validation.ValidateAsync(contact);
            if (responseValidate is null || responseValidate.IsValid == false)
            {
                result.Success = false;
                result.Message = (responseValidate != null) ? responseValidate!.Errors.FirstOrDefault()!.ErrorMessage : "Erro a validar Campos!";
                return result;
            }

            _ctx.Contacts.Add(contact.ToEntity());
            await _ctx.SaveChangesAsync();

            result.Success = true;
            result.Message = "Criado com sucesso!";

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

            if (parameter.QryParam is not null && parameter.Qry is not null)
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

        public async Task<MessageHelper<ContactListDTO>> GetByIdAsync(int id)
        {
            MessageHelper<ContactListDTO> result = new();
            try
            {
                var contact = await _ctx.Contacts.Include(t => t.Type).FirstOrDefaultAsync(contact => contact.Id == id);
                if (contact == null)
                {
                    result.Message = "Não foi encontrado nenhum Contato com esse ID!";
                    result.Success = false;
                    result.obj = null;
                    return result;
                }

                result.Message = "Encontrado com sucesso!";
                result.Success = true;
                result.obj = new(contact);
                return result;
            }
            catch (Exception)
            {
                result.Message = "Não foi encontrado nenhum Contato com esse ID!";
                result.Success = false;
                result.obj = null;
                return result;
            }
           
        }

        public async Task<MessageHelper<ContactListDTO>> Edit(int id, ContactEditDTO c)
        {
            MessageHelper<ContactListDTO> result = new();
            ContactValidatorEdit validation = new();

            var responseValidate = await validation.ValidateAsync(c);
            if (responseValidate is null || responseValidate.IsValid == false)
            {
                result.Success = false;
                result.Message = (responseValidate != null) ? responseValidate!.Errors.FirstOrDefault()!.ErrorMessage : "Erro a validar Campos!";
                return result;
            }

            Contact contact = c.ToEntity();
            if (id != contact.Id)
            {
                result.Message = "Request Inválido";
                result.Success = false;
                result.obj = null;
                return result;
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

            result.Message = "Editado com Sucesso!";
            result.Success = true;
            result.obj = new(contact);

            return result;
        }

        public async Task<MessageHelper> Delete(int id)
        {
            MessageHelper result = new();
            var contact = await _ctx.Contacts.FindAsync(id);

            if (contact == null)
            {
                result.Message = "Não existe nenhum contato com esse ID!";
                result.Success = false;
                return result;
            }

            if (typeof(Auditable).IsAssignableFrom(typeof(Contact)))
            {
                contact.DateDeleted = DateTimeOffset.UtcNow;
                _ctx.Entry(contact).CurrentValues.SetValues(contact);

                //_ctx.Contacts.Remove(contact);
                await _ctx.SaveChangesAsync();

                result.Message = "Apagado com Sucesso!";
                result.Success = true;
                return result;
            }

            result.Message = "Não foi apagado!";
            result.Success = true;
            return result;
        }

       /* private bool ContactExists(int id)
        {
            return _ctx.Contacts.Any(e => e.Id == id);
        }*/
    }
}
