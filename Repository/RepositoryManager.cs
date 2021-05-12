using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Contracts;
using Entities;

namespace Repository
{
    public class RepositoryManager : IRepositoryManager
    {
        private RepositoryContext _repositoryContext;
        //Properties that will expose the concrete repositories
        private ICompanyRepository _companyRepository; 
        private IEmployeeRepository _employeeRepository;

        public RepositoryManager(RepositoryContext repositoryContext) 
        {
            _repositoryContext = repositoryContext; 
        }

        //Exposes
        public ICompanyRepository Company 
        { 
            get 
            { 
                if (_companyRepository == null) 
                    _companyRepository = new CompanyRepository(_repositoryContext); 
                return _companyRepository; 
            } 
        }

        public IEmployeeRepository Employee 
        { 
            get 
            { 
                if (_employeeRepository == null) 
                    _employeeRepository = new EmployeeRepository(_repositoryContext); 
                return _employeeRepository; 
            }
        }

        //Method to be used after all the modifications are finished on a certain object
        public void Save() => _repositoryContext.SaveChanges();
    }
}
