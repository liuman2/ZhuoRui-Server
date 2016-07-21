 
 
using WebCenter.IDAL;

namespace WebCenter.DAL
{
    public partial class DbSession:IDbSession  
    {   
	
	public IDAL.ICustomerRepository CustomerRepository { get { return new CustomerRepository(); } }
	}
}