using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;
using Entities.DataTransferObjects;
using Entities.Models;

namespace CompanyEmployees.Extensions
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Company, CompanyDto>()
                .ForMember(c => c.FullAddress,
                    opt => opt.MapFrom(x => string.Join(' ', x.Address, x.Country)));

            CreateMap<Employee, EmployeeDto>();//For Employee DTO

            CreateMap<CompanyForCreationDto, Company>(); //Post Company Creation with DTO

            CreateMap<EmployeeForCreationDto, Employee>(); //Post Employee under Company with DTO

            CreateMap<EmployeeForUpdateDto, Employee>().ReverseMap(); //PUT Handler, Employee for Update

            CreateMap<CompanyForUpdateDto, Company>(); //PUT Handler, Company update
        }


    }
}
