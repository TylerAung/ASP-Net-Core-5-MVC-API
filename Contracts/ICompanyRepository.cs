using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Entities.Models;

namespace Contracts
{
    public interface ICompanyRepository
    {
        //Get All Companies
        IEnumerable<Company> GetAllCompanies(bool trackChanges);
        //Get One Company
        Company GetCompany(Guid companyId, bool trackChanges);
        void CreateCompany(Company company);
        IEnumerable<Company> GetByIds(IEnumerable<Guid> ids, bool trackChanges);
        //Cascade delete parent & child method
        void DeleteCompany(Company company);
    }
}
