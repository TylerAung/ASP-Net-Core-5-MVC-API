using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Contracts;
using Entities;
using Entities.Models;

namespace Repository
{
    public class EmployeeRepository : RepositoryBase<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(RepositoryContext repositoryContext) : base(repositoryContext) { }

        //Get All Employees
        public IEnumerable<Employee> GetEmployees(Guid companyId, bool trackChanges)
            => FindByCondition(e => e.CompanyId.Equals(companyId),
                trackChanges).OrderBy(e => e.Name);
        //Get Single Employee under Company Id
        public Employee GetEmployee(Guid companyId, Guid id, bool trackChanges)
        => FindByCondition(e => e.CompanyId.Equals(companyId) && e.Id.Equals(id),
            trackChanges).SingleOrDefault();
    }
}
