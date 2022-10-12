using Api.Models.Request;
using Api.Models.Responses;
using Api.Models.ViewModels;
using AutoMapper;
using Core.Dtos;

namespace Api.Mapping;

public class CustomerProfile : Profile
{
    public CustomerProfile()
    {
        CreateMap<CreateCustomerRequest, CustomerDto>()
            .ForMember(x => x.Id, opt => opt.Ignore());

        CreateMap<CustomerDto, CustomerViewModel>();

        CreateMap<CustomerDto, CreateCustomerResponse>()
            .ForMember(response => response.Customer, opt => opt.MapFrom(customerDto => customerDto));

        CreateMap<CustomerDto, GetCustomerResponse>()
            .ForMember(response => response.Customer, opt => opt.MapFrom(customerDto => customerDto));
    }
}