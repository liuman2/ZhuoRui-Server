 

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
	
	   kernel.Bind<ICustomerService>().To<CustomerService>().InRequestScope();
  kernel.Bind<IUnitOfWork>().To<UnitOfWork>().InRequestScope();

}
  

}
}