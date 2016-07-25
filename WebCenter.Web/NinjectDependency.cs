﻿ 

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
	
	   kernel.Bind<ImemberService>().To<memberService>().InRequestScope();
	
	   kernel.Bind<IorganizationService>().To<organizationService>().InRequestScope();
	
	   kernel.Bind<IpositionService>().To<positionService>().InRequestScope();
	
	   kernel.Bind<IroleService>().To<roleService>().InRequestScope();
	
	   kernel.Bind<IsequenceService>().To<sequenceService>().InRequestScope();
  kernel.Bind<IUnitOfWork>().To<UnitOfWork>().InRequestScope();

}
  

}
}