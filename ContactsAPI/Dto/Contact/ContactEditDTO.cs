using FluentValidation;

namespace ContactsAPI.Dto
{
    public class ContactEditDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Phone { get; set; }
        public int TypeId { get; set; }

        public Contact ToEntity()
        {
            return new Contact { Id = Id, Name = Name, Phone = Phone, TypeId = TypeId };
        }
    }
    public class ContactValidatorEdit : AbstractValidator<ContactEditDTO>
    {
        public ContactValidatorEdit()
        {
            RuleFor(c => c.Id).NotEmpty().WithMessage("Id inválido ou vazio!");
            RuleFor(c => c.Name)
                .NotEmpty().WithMessage("Nome inválido ou vazio!")
                .MaximumLength(20).WithMessage("Nome deve conter menos que 20 carateres!");
            RuleFor(c => c.Phone.ToString())
                .NotNull().WithMessage("Número de Telemóvel Inválido ou Vazio!")
                .MinimumLength(9).WithMessage("Número de Telemóvel deve conter 9 digitos!")
                .MaximumLength(9).WithMessage("Número de Telemóvel deve conter 9 digitos!");
            RuleFor(c => c.TypeId).NotNull().WithMessage("Deve ter um tipo de contato!");
        }
    }
}
