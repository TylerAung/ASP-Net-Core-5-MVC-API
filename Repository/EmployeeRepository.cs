using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Contracts;
using Entities;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class EmployeeRepository : RepositoryBase<Employee>, IEmployeeRepository
    {
        //Enforcement here comes from Contracts/IEmployeeRepository
        public EmployeeRepository(RepositoryContext repositoryContext) : base(repositoryContext) { }

        //Get All Employees
        public async Task<IEnumerable<Employee>> GetEmployeesAsync(Guid companyId, bool trackChanges) => 
            await FindByCondition(e => e.CompanyId.Equals(companyId),
                trackChanges)
            .OrderBy(e => e.Name)
            .ToListAsync();
        //Get Single Employee under Company Id
        public async Task<Employee> GetEmployeeAsync(Guid companyId, Guid id, bool trackChanges) => 
            await FindByCondition(e => e.CompanyId.Equals(companyId) && e.Id.Equals(id),
            trackChanges).SingleOrDefaultAsync();
        //Create Employee Under Company
        public void CreateEmployeeForCompany(Guid companyId, Employee employee)
        {
            employee.CompanyId = companyId;
            Create(employee);
        }
        //For Deleteing Employee
        public void DeleteEmployee(Employee employee)
        {
            Delete(employee);
        }

        //public void DeleteEmployee(Task<Employee> employeeForCompany)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
