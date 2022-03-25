using ContactsAPI.Dto;
using ContactsAPI.Interfaces;
using ContactsAPI.Models;
using ContactsAPI.Pagination;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using static ContactsAPI.Dto.ContactTypeCreateDTO;
using static ContactsAPI.Dto.ContactTypeEditDTO;

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

        public async Task<MessageHelper> CreateTypeAsync(ContactTypeCreateDTO type)
        {
            MessageHelper result = new();
            ContactTypeValidator validation = new();

            var responseValidate = await validation.ValidateAsync(type);
            if (responseValidate is null || responseValidate.IsValid == false)
            {
                result.Success = false;
                result.Message = (responseValidate != null) ? responseValidate!.Errors.FirstOrDefault()!.ErrorMessage : "Erro a validar Campos!";
                return result;
            }

            _ctx.ContactTypes.Add(type.ToEntity());
            await _ctx.SaveChangesAsync();

            result.Success = true;
            result.Message = "Criado com sucesso!";

            return result;
        }

        public async Task<PaginationMetadata<ContactTypeDTO>> GetAllPaginated(PaginationParams parameter)
        {
            IQueryable<ContactType> types = _ctx.ContactTypes.Where(c => c.DateDeleted == null);
            PaginationMetadata<ContactTypeDTO> ListTypes = new(types.Count(), parameter.Page, parameter.ItemsPerPage);

            if ((parameter.Page < 1) || (parameter.ItemsPerPage < 1))
            {
                ListTypes.Success = false;
                ListTypes.Message = "Page ou Pagination tem parametros inválidos";
                return ListTypes;
            }

            if (parameter.Qry != null && (parameter.QryParam == null || parameter.QryParam.Length < 1))
                types = types.Where(c => c.Name.Contains(parameter.Qry.Trim()) ||
                                               Convert.ToString(c.Id).Contains(parameter.Qry));

            if (parameter.QryParam is not null && parameter.Qry is not null)
                foreach (var item in parameter.QryParam)
                {
                    switch (item)
                    {
                        case "id":
                            types = types.Where(c => Convert.ToString(c.Id).Contains(parameter.Qry.Trim()));
                            break;
                        case "name":
                            types = types.Where(c => c.Name.Contains(parameter.Qry.Trim()));
                            break;
                        default:
                            break;
                    }
                }

            types = parameter.Sort switch
            {
                "id" => types.OrderBy(c => c.Id),
                "id_desc" => types.OrderByDescending(c => c.Id),
                "name" => types.OrderBy(c => c.Name),
                "name_desc" => types.OrderByDescending(c => c.Name),
                _ => types.OrderBy(c => c.Id),
            };

            ListTypes.TotalCount = types.Count();

            var items = await types.Skip((parameter.Page - 1) * parameter.ItemsPerPage)
                                      .Take(parameter.ItemsPerPage)
                                      .ToListAsync();

            ListTypes.Results = items.Select(t => new ContactTypeDTO(t)).ToList();

            ListTypes.Success = true;
            ListTypes.Message = "Encontrado com Sucesso!";

            return ListTypes;
        }

        public async Task<MessageHelper<ContactTypeDTO>> Edit(int id, ContactTypeEditDTO c)
        {
            MessageHelper<ContactTypeDTO> result = new();
            ContactTypeValidatorEdit validation = new();

            var responseValidate = await validation.ValidateAsync(c);
            if (responseValidate is null || responseValidate.IsValid == false)
            {
                result.Success = false;
                result.Message = (responseValidate != null) ? responseValidate!.Errors.FirstOrDefault()!.ErrorMessage : "Erro a validar Campos!";
                return result;
            }

            ContactType type = c.ToEntity();
            if (id != type.Id)
            {
                result.Message = "Request Inválido";
                result.Success = false;
                result.obj = null;
                return result;
            }

            _ctx.Entry(type).State = EntityState.Modified;

            try
            {
                await _ctx.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                foreach (var entry in ex.Entries)
                {
                    if (entry.Entity is ContactTypeDTO)
                    {
                        var proposedValues = entry.CurrentValues;
                        var databaseValues = entry.GetDatabaseValues();

                        foreach (var property in proposedValues.Properties)
                        {
                            var proposedValue = proposedValues[property];
                            var databaseValue = databaseValues[property];
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
            }

            result.Message = "Editado com Sucesso!";
            result.Success = true;
            result.obj = new(type);

            return result;
        }

        public async Task<MessageHelper<ContactTypeDTO>> GetByIdAsync(int id)
        {
            MessageHelper<ContactTypeDTO> result = new();
            try
            {   
                var contactType = await _ctx.ContactTypes.FindAsync(id);

                if (contactType == null)
                {
                    result.Message = "Não foi encontrado nenhum Tipo de Contato!";
                    result.Success = false;
                    result.obj = null;
                    return result;
                }

                result.Message = "Encontrado com sucesso!";
                result.Success = true;
                result.obj = new(contactType);
                return result;
            }
            catch (Exception)
            {
                result.Message = "Erro nos parametros ou não foi encontrado!";
                result.Success = false;
                result.obj = null;
                return result;
            }
            
        }
    }
}
