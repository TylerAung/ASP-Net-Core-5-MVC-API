using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Contracts;
using Entities.DataTransferObjects;
using AutoMapper;

namespace CompanyEmployees.Controllers
{
    [Route("api/companies")] //Represent routing
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly IRepositoryManager _repository; 
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        public CompaniesController(IRepositoryManager repository, ILoggerManager logger, IMapper mapper) 
        { 
            _repository = repository; 
            _logger = logger;
            _mapper = mapper;
        }
        [HttpGet] public IActionResult GetCompanies() 
        { 
            //_repository.Company.GetAllCompanies
                var companies = _repository.Company.GetAllCompanies(trackChanges: false);
                var companiesDto = _mapper.Map<IEnumerable<CompanyDto>>(companies); //Alternative to Manual DTO
           return Ok(companiesDto);    
        }

        [HttpGet("{id}")]
        public IActionResult GetCompany(Guid id)
        {
            var company = _repository.Company.GetCompany(id, trackChanges: false);
            if (company == null)
            {
                _logger.LogInfo($"Company with id: {id} doesn't exist in the database.");
                return NotFound();//Built in method to return 404
            }
            else
            {
                var companyDto = _mapper.Map<CompanyDto>(company);
                return Ok(companyDto);
            }
        }
    }
}
