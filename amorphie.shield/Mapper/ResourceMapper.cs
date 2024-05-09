using amorphie.shield.core.Model;
using AutoMapper;

namespace amorphie.shield.Mapper;
public sealed class ResourceMapper : Profile
{
    public ResourceMapper()
    {
        CreateMap<Certificate, CertificateDTO>().ReverseMap();
    }
}
