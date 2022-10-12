using AutoMapper;
using Core.Dtos;
using Data.Entities;

namespace Core.Mapping;

public class CustomerProfile : Profile
{
    public CustomerProfile()
    {
        CreateMap<Customer, CustomerDto>()
            .ReverseMap();
    }
}