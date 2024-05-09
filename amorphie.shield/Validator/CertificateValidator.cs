using FluentValidation;
using amorphie.shield.core.Model;

namespace amorphie.shield.Validator;
public sealed class CertificateValidator : AbstractValidator<Certificate>
{
    public CertificateValidator()
    {
        RuleFor(x => x.CertificateData).NotNull();
        RuleFor(x => x.Status).MinimumLength(10);
    }
}

