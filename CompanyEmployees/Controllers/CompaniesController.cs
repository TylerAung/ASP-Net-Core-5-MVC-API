using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Contracts;
using Entities.DataTransferObjects;
using AutoMapper;
using Entities.Models;
using CompanyEmployees.ModelBinders;
using CompanyEmployees.Extensions.ActionFilters;

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
        [HttpGet]
        //public IActionResult GetCompanies() 
        //{ 
        //    //_repository.Company.GetAllCompanies
        //        var companies = _repository.Company.GetAllCompanies(trackChanges: false);
        public async Task<IActionResult> GetCompanies()
        {
            var companies = await _repository.Company.GetAllCompaniesAsync(trackChanges: false);
            var companiesDto = _mapper.Map<IEnumerable<CompanyDto>>(companies); //Alternative to Manual DTO
           return Ok(companiesDto);    
        }

        [HttpGet("{id}", Name = "CompanyById")]
        public async Task<IActionResult> GetCompany(Guid id)
        {
            var company = await _repository.Company.GetCompanyAsync(id, trackChanges: false);
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

        [HttpGet("collection/({ids})", Name = "CompanyCollection")]
        //public IActionResult GetCompanyCollection(IEnumerable<Guid> ids)
        //public IActionResult GetCompanyCollection([ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> ids)
        //{
        public async Task<IActionResult> GetCompanyCollection([ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> ids)
        { 
            if (ids == null)
            {
                _logger.LogError("Parameter ids is null");
                return BadRequest("Parameter ids is null");
            }
            //var companyEntities = _repository.Company.GetByIds(ids, trackChanges: false);
            var companyEntities = await _repository.Company.GetByIdsAsync(ids, trackChanges: false);
            if (ids.Count() != companyEntities.Count())
            {
                _logger.LogError("Some ids are not valid in a collection");
                return NotFound();
            }
            var companiesToReturn = _mapper.Map<IEnumerable<CompanyDto>>(companyEntities);
            return Ok(companiesToReturn);
        }

        [HttpPost]
        //public IActionResult CreateCompany([FromBody] CompanyForCreationDto company)
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> CreateCompany([FromBody] CompanyForCreationDto company)
        {
            //if (company == null)
            //{
            //    _logger.LogError("CompanyForCreationDto object sent from client is null.");
            //    return BadRequest("CompanyForCreationDto object is null");
            //}
            //if (!ModelState.IsValid)
            //{
            //    _logger.LogError("Invalid model state for the CompanyForCreationDto object");
            //    return UnprocessableEntity(ModelState);
            //}
            var companyEntity = _mapper.Map<Company>(company);
            _repository.Company.CreateCompany(companyEntity);
            //_repository.Save();
            await _repository.SaveAsync();
            var companyToReturn = _mapper.Map<CompanyDto>(companyEntity);
            return CreatedAtRoute("CompanyById", new
            {
                id = companyToReturn.Id
            }, companyToReturn);
            //CreatedAtRoute = Returns a URI to the newly created resource when
            //you invoke a POST method to store some new object
        }

        [HttpPost("collection")]
        //public IActionResult CreateCompanyCollection([FromBody] IEnumerable<CompanyForCreationDto> companyCollection)
        public async Task<IActionResult> CreateCompanyCollection([FromBody] IEnumerable<CompanyForCreationDto> companyCollection)
        {
            if (companyCollection == null)
            {
                _logger.LogError("Company collection sent from client is null.");
                return BadRequest("Company collection is null");
            }
            var companyEntities = _mapper.Map<IEnumerable<Company>>(companyCollection);
            foreach (var company in companyEntities)
            {
                _repository.Company.CreateCompany(company);
            }
            //_repository.Save();
            await _repository.SaveAsync();
            var companyCollectionToReturn = _mapper.Map<IEnumerable<CompanyDto>>(companyEntities);
            var ids = string.Join(",", companyCollectionToReturn.Select(c => c.Id));
            return CreatedAtRoute("CompanyCollection", new
            {
                ids
            }, companyCollectionToReturn);
        }

        [HttpPut("{id}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [ServiceFilter(typeof(ValidateCompanyExistsAttribute))]
        //public IActionResult UpdateCompany(Guid id, [FromBody] CompanyForUpdateDto company)
        public async Task<IActionResult> UpdateCompany(Guid id, [FromBody] CompanyForUpdateDto company)
        {
            //if (company == null)
            //{
            //    _logger.LogError("CompanyForUpdateDto object sent from client is null.");
            //    return BadRequest("CompanyForUpdateDto object is null");
            //}
            //if (!ModelState.IsValid)
            //{
            //    _logger.LogError("Invalid model state for the CompanyForCreationDto object");
            //    return UnprocessableEntity(ModelState);
            //}
            //var companyEntity = _repository.Company.GetCompany(id, trackChanges: true);
            //var companyEntity = await _repository.Company.GetCompanyAsync(id, trackChanges: true); //Version 1
            var companyEntity = HttpContext.Items["company"] as Company; //Version 2
            if (companyEntity == null)
            {
                _logger.LogInfo($"Company with id: {id} doesn't exist in the database.");
                return NotFound();
            } //Version 1, removed in Version 2
            _mapper.Map(company, companyEntity);
            //_repository.Save();
            await _repository.SaveAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [ServiceFilter(typeof(ValidateCompanyExistsAttribute))]
        //public IActionResult DeleteCompany(Guid id)
        public async Task<IActionResult> DeleteCompany(Guid id)
        {
            var company = HttpContext.Items["company"] as Company; //Version 2
            _repository.Company.DeleteCompany(company);
            await _repository.SaveAsync();
            return NoContent();
        }
    }
}
