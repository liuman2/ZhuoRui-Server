 

 

namespace WebCenter.IServices
{
 
    public partial interface IUnitOfWork
{  
	
	  Iannual_examService Iannual_examService{get;set;} 
	
	  IareaService IareaService{get;set;} 
	
	  IauditService IauditService{get;set;} 
	
	  Ibank_accountService Ibank_accountService{get;set;} 
	
	  IcustomerService IcustomerService{get;set;} 
	
	  Icustomer_timelineService Icustomer_timelineService{get;set;} 
	
	  IdictionaryService IdictionaryService{get;set;} 
	
	  Idictionary_groupService Idictionary_groupService{get;set;} 
	
	  IincomeService IincomeService{get;set;} 
	
	  ImemberService ImemberService{get;set;} 
	
	  IorganizationService IorganizationService{get;set;} 
	
	  IpatentService IpatentService{get;set;} 
	
	  IpositionService IpositionService{get;set;} 
	
	  Ireg_abroadService Ireg_abroadService{get;set;} 
	
	  Ireg_historyService Ireg_historyService{get;set;} 
	
	  Ireg_internalService Ireg_internalService{get;set;} 
	
	  Ireg_internal_historyService Ireg_internal_historyService{get;set;} 
	
	  IroleService IroleService{get;set;} 
	
	  IsequenceService IsequenceService{get;set;} 
	
	  IsettingService IsettingService{get;set;} 
	
	  ItimelineService ItimelineService{get;set;} 
	
	  ItrademarkService ItrademarkService{get;set;} 
	
}
}