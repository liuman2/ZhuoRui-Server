 

 using WebCenter.IServices;

namespace WebCenter.Services
{
 
    public partial class UnitOfWork:IUnitOfWork
{  
	
  [Ninject.Inject]
	 public  ICustomerService ICustomerService
    {
        get;
        set;
    } 
	
}
}