using FluentValidation;

namespace ContactsAPI.Dto
{
    public class ContactTypeCreateDTO
    {
        public string Name { get; set; }
        
        public ContactType ToEntity()
        {
            return new ContactType { Name = Name };
        }

        public class ContactTypeValidator : AbstractValidator<ContactTypeCreateDTO>
        {
            public ContactTypeValidator()
            {
                RuleFor(t => t.Name)
                    .NotEmpty().WithMessage("Nome inválido ou vazio!")
                    .MaximumLength(20).WithMessage("Nome deve conter menos que 20 carateres!");
            }
        }
    }
}
