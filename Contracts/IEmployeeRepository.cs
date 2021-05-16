using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Entities.Models;

namespace Contracts
{
    public interface IEmployeeRepository
    {
        //Get All Employees
        IEnumerable<Employee> GetEmployees(Guid companyId, bool trackChanges);
        //Get Single Employee
        Employee GetEmployee(Guid companyId, Guid id, bool trackChanges);
    }
}
