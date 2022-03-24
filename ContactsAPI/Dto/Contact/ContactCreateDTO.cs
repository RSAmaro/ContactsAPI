using FluentValidation;

namespace ContactsAPI.Dto
{
    public class ContactCreateDTO
    {
        public string Name { get; set; }
        public int Phone { get; set; }
        public int TypeId { get; set; }

        public Contact ToEntity()
        {
            return new Contact { Name = Name, Phone = Phone, TypeId = TypeId };
        }
    }

    public class ContactValidator : AbstractValidator<ContactCreateDTO>
    {
        public ContactValidator()
        {
            RuleFor(c => c.Name).NotEmpty().WithMessage("Nome inválido ou vazio!");
            RuleFor(c => c.Phone.ToString())
                .NotNull().WithMessage("Número de Telemóvel Inválido ou Vazio!")
                .MinimumLength(9).WithMessage("Número de Telemóvel deve conter 9 digitos!")
                .MaximumLength(9).WithMessage("Número de Telemóvel deve conter 9 digitos!");
            RuleFor(c => c.TypeId).NotNull().WithMessage("Deve ter um tipo de contato!");
        }
    }
}
