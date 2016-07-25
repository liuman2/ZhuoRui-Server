 

 

namespace WebCenter.IServices
{
 
    public partial interface IUnitOfWork
{  
	
	  ImemberService ImemberService{get;set;} 
	
	  IorganizationService IorganizationService{get;set;} 
	
	  IpositionService IpositionService{get;set;} 
	
	  IroleService IroleService{get;set;} 
	
	  IsequenceService IsequenceService{get;set;} 
	
}
}