using AutoMapper;
using SalesService.Domain.Entities;
using SalesService.Application.DTOs;

namespace SalesService.Application.Profiles;

public class SaleProfile : Profile
{
    public SaleProfile()
    {
        CreateMap<Sale, SaleDto>();
        CreateMap<SaleItem, SaleItemDto>();
        // Add more mappings as needed
    }
} 