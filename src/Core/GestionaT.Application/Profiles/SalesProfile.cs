using AutoMapper;
using GestionaT.Application.Features.Sales.Queries;
using GestionaT.Domain.Entities;

namespace GestionaT.Application.Profiles
{
    public class SalesProfile : Profile
    {
        public SalesProfile()
        {
            CreateMap<Sale, SalesResponse>()
                .ForMember(dest => dest.NameCustomer, opt => opt.MapFrom(src => src.Customer.Name));
        }
    }
}
