using FluentValidation;

namespace ContactsAPI.Dto
{
    public class ContactTypeEditDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ContactType ToEntity()
        {
            return new ContactType { Id = Id, Name = Name };
        }

        public class ContactTypeValidatorEdit : AbstractValidator<ContactTypeEditDTO>
        {
            public ContactTypeValidatorEdit()
            {
                RuleFor(t => t.Id).NotEmpty().WithMessage("Id inválido ou vazio!");
                RuleFor(t => t.Name)
                    .NotEmpty().WithMessage("Nome inválido ou vazio!")
                    .MaximumLength(20).WithMessage("Nome deve conter menos que 20 carateres!");
            }
        }
    }
}
