 

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace WebCenter.IDAL
{
    public partial interface IDbSession
    {
   
	  

		IDAL.ICustomerRepository CustomerRepository { get; }
	}
}