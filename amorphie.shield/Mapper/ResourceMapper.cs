using amorphie.shield.Certificates;
using AutoMapper;

namespace amorphie.shield.Mapper;
public sealed class ResourceMapper : Profile
{
    public ResourceMapper()
    {
        CreateMap<Certificate, CertificateDto>().ReverseMap();
    }
}
