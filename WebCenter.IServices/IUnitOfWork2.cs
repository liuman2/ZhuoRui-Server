 

 

namespace WebCenter.IServices
{
 
    public partial interface IUnitOfWork
{  
	
	  IareaService IareaService{get;set;} 
	
	  Ibank_accountService Ibank_accountService{get;set;} 
	
	  IcustomerService IcustomerService{get;set;} 
	
	  Icustomer_timelineService Icustomer_timelineService{get;set;} 
	
	  IdictionaryService IdictionaryService{get;set;} 
	
	  Idictionary_groupService Idictionary_groupService{get;set;} 
	
	  IincomeService IincomeService{get;set;} 
	
	  ImemberService ImemberService{get;set;} 
	
	  IorganizationService IorganizationService{get;set;} 
	
	  IpositionService IpositionService{get;set;} 
	
	  Ireg_abroadService Ireg_abroadService{get;set;} 
	
	  IroleService IroleService{get;set;} 
	
	  IsequenceService IsequenceService{get;set;} 
	
}
}