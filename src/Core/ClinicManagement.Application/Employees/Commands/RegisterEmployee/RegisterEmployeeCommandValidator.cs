using FluentValidation;

namespace ClinicManagement.Application.Employees.Commands.RegisterEmployee;

public class RegisterEmployeeCommandValidator : AbstractValidator<RegisterEmployeeCommand>
{
    public RegisterEmployeeCommandValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("الاسم الكامل مطلوب.")
            .MaximumLength(100).WithMessage("الاسم الكامل يجب ألا يتجاوز 100 حرف.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("البريد الإلكتروني مطلوب.")
            .EmailAddress().WithMessage("صيغة البريد الإلكتروني غير صحيحة.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("كلمة المرور مطلوبة.")
            .MinimumLength(6).WithMessage("يجب أن تتكون كلمة المرور من 6 أحرف على الأقل.");
    }
}
