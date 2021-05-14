using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObjects
{
    public class CompanyDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string FullAddress { get; set; }

        //Employees property is removed, and FullAddress property to concatenate the Addres & Country properties from
        //company class. As this class is only to return response to client, validation attribute are not required

        //Company.cs
        //public Guid Id { get; set; }
        //public string Name { get; set; }
        //public string Address { get; set; }
        //public string Country { get; set; }
        //public ICollection<Employee> Employees { get; set; }


    }
}
