using amorphie.shield.Certificates;
using FluentValidation;

namespace amorphie.shield.Validator;
public sealed class CertificateValidator : AbstractValidator<Certificate>
{
    public CertificateValidator()
    {
        RuleFor(x => x.PublicCert).NotNull();
    }
}


