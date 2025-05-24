using AutoMapper;
using GestionaT.Application.Features.Customers;
using GestionaT.Application.Features.Customers.Commands.CreateCustomer;
using GestionaT.Application.Features.Customers.Commands.UpdateCustomer;
using GestionaT.Domain.Entities;

namespace GestionaT.Application.Mappings
{
    public class CustomerProfile : Profile
    {
        public CustomerProfile()
        {
            CreateMap<CreateCustomerCommand, Customer>();

            CreateMap<UpdateCustomerCommand, Customer>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Business, opt => opt.Ignore());

            CreateMap<Customer, CustomerResponse>()
                .ForMember(dest => dest.BusinessName, opt => opt.MapFrom(src => src.Business.Name));
        }
    }
}
