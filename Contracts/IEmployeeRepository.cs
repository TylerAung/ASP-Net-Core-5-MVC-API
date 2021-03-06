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
        //IEnumerable<Employee> GetEmployees(Guid companyId, bool trackChanges); // Version 1
        Task<IEnumerable<Employee>> GetEmployeesAsync(Guid companyId, bool trackChanges);
        //Get Single Employee
        //Employee GetEmployee(Guid companyId, Guid id, bool trackChanges); // Version 1
        Task<Employee> GetEmployeeAsync(Guid companyId, Guid id, bool trackChanges);
        //Creating Employee
        void CreateEmployeeForCompany(Guid companyId, Employee employee);
        //For Deleteing Employee
        void DeleteEmployee(Employee employee);
        //void DeleteEmployee(Task<Employee> employeeForCompany);
    }
}
