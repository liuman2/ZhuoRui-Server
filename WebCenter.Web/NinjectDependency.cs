 

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
	
	   kernel.Bind<Iannual_examService>().To<annual_examService>().InRequestScope();
	
	   kernel.Bind<IareaService>().To<areaService>().InRequestScope();
	
	   kernel.Bind<IattachmentService>().To<attachmentService>().InRequestScope();
	
	   kernel.Bind<IauditService>().To<auditService>().InRequestScope();
	
	   kernel.Bind<Iaudit_bankService>().To<audit_bankService>().InRequestScope();
	
	   kernel.Bind<IbankService>().To<bankService>().InRequestScope();
	
	   kernel.Bind<Ibank_accountService>().To<bank_accountService>().InRequestScope();
	
	   kernel.Bind<IcustomerService>().To<customerService>().InRequestScope();
	
	   kernel.Bind<Icustomer_timelineService>().To<customer_timelineService>().InRequestScope();
	
	   kernel.Bind<IdictionaryService>().To<dictionaryService>().InRequestScope();
	
	   kernel.Bind<Idictionary_groupService>().To<dictionary_groupService>().InRequestScope();
	
	   kernel.Bind<IhistoryService>().To<historyService>().InRequestScope();
	
	   kernel.Bind<IincomeService>().To<incomeService>().InRequestScope();
	
	   kernel.Bind<IleaveService>().To<leaveService>().InRequestScope();
	
	   kernel.Bind<IlectureService>().To<lectureService>().InRequestScope();
	
	   kernel.Bind<Ilecture_customerService>().To<lecture_customerService>().InRequestScope();
	
	   kernel.Bind<ImailService>().To<mailService>().InRequestScope();
	
	   kernel.Bind<ImemberService>().To<memberService>().InRequestScope();
	
	   kernel.Bind<ImenuService>().To<menuService>().InRequestScope();
	
	   kernel.Bind<InoticeService>().To<noticeService>().InRequestScope();
	
	   kernel.Bind<IoperationService>().To<operationService>().InRequestScope();
	
	   kernel.Bind<IorganizationService>().To<organizationService>().InRequestScope();
	
	   kernel.Bind<IpatentService>().To<patentService>().InRequestScope();
	
	   kernel.Bind<IpositionService>().To<positionService>().InRequestScope();
	
	   kernel.Bind<Ireg_abroadService>().To<reg_abroadService>().InRequestScope();
	
	   kernel.Bind<Ireg_historyService>().To<reg_historyService>().InRequestScope();
	
	   kernel.Bind<Ireg_internalService>().To<reg_internalService>().InRequestScope();
	
	   kernel.Bind<Ireg_internal_historyService>().To<reg_internal_historyService>().InRequestScope();
	
	   kernel.Bind<IroleService>().To<roleService>().InRequestScope();
	
	   kernel.Bind<Irole_memberService>().To<role_memberService>().InRequestScope();
	
	   kernel.Bind<Irole_memuService>().To<role_memuService>().InRequestScope();
	
	   kernel.Bind<Irole_operationService>().To<role_operationService>().InRequestScope();
	
	   kernel.Bind<IsequenceService>().To<sequenceService>().InRequestScope();
	
	   kernel.Bind<IsettingService>().To<settingService>().InRequestScope();
	
	   kernel.Bind<ItimelineService>().To<timelineService>().InRequestScope();
	
	   kernel.Bind<ItrademarkService>().To<trademarkService>().InRequestScope();
	
	   kernel.Bind<IwaitdealService>().To<waitdealService>().InRequestScope();
  kernel.Bind<IUnitOfWork>().To<UnitOfWork>().InRequestScope();

}
  

}
}