using FluentValidation;

namespace ContactsAPI.Models.User
{
    public class RegisterDTO
    {
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
    }

    public class RegisterDTOValidator : AbstractValidator<RegisterDTO>
    {
        public RegisterDTOValidator()
        {
            RuleFor(x => x.Username).NotNull().WithMessage("Insert Username")
            .NotEmpty().WithMessage("Insert Username");

            RuleFor(x => x.Email).NotNull().WithMessage("Insert Email")
            .NotEmpty().WithMessage("Insert Email");

            RuleFor(x => x.Password).NotNull().WithMessage("Insert Password")
            .NotEmpty().WithMessage("Insert Password");
        }
    }
}
