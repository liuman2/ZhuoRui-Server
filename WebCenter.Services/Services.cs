 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebCenter.Entities;
using WebCenter.IServices;
 
using WebCenter.IDAL;
using WebCenter.DAL;
namespace WebCenter.Services
{
   
	
	public partial class CustomerService:BaseService<Customer>,ICustomerService
    {   
        public CustomerService()
        {}
        public override void SetCurrentRepository()
        {
            CurrentRepository = _dbSession.CustomerRepository;
        }  
    }
	
}