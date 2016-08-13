 

using System;
using System.Collections.Generic;
using System.Web.Mvc;
using WebCenter.Entities;
using Ninject;
using Ninject.Web.Common;
using WebCenter.IServices;
using WebCenter.Services;
namespace WebCenter.Web.Infrastructure
{

public partial class  NinjectDependencyResolver:IDependencyResolver
{
 
private void AutoAddBinds()
{
	
	   kernel.Bind<IareaService>().To<areaService>().InRequestScope();
	
	   kernel.Bind<IauditService>().To<auditService>().InRequestScope();
	
	   kernel.Bind<Ibank_accountService>().To<bank_accountService>().InRequestScope();
	
	   kernel.Bind<IcustomerService>().To<customerService>().InRequestScope();
	
	   kernel.Bind<Icustomer_timelineService>().To<customer_timelineService>().InRequestScope();
	
	   kernel.Bind<IdictionaryService>().To<dictionaryService>().InRequestScope();
	
	   kernel.Bind<Idictionary_groupService>().To<dictionary_groupService>().InRequestScope();
	
	   kernel.Bind<IincomeService>().To<incomeService>().InRequestScope();
	
	   kernel.Bind<ImemberService>().To<memberService>().InRequestScope();
	
	   kernel.Bind<IorganizationService>().To<organizationService>().InRequestScope();
	
	   kernel.Bind<IpositionService>().To<positionService>().InRequestScope();
	
	   kernel.Bind<Ireg_abroadService>().To<reg_abroadService>().InRequestScope();
	
	   kernel.Bind<Ireg_historyService>().To<reg_historyService>().InRequestScope();
	
	   kernel.Bind<Ireg_internalService>().To<reg_internalService>().InRequestScope();
	
	   kernel.Bind<Ireg_internal_historyService>().To<reg_internal_historyService>().InRequestScope();
	
	   kernel.Bind<IroleService>().To<roleService>().InRequestScope();
	
	   kernel.Bind<IsequenceService>().To<sequenceService>().InRequestScope();
	
	   kernel.Bind<ItimelineService>().To<timelineService>().InRequestScope();
  kernel.Bind<IUnitOfWork>().To<UnitOfWork>().InRequestScope();

}
  

}
}