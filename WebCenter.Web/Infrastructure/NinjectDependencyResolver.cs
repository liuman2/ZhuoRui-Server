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
    public  partial class NinjectDependencyResolver : IDependencyResolver
    {
        private IKernel kernel;
        public NinjectDependencyResolver(IKernel kernelParam)
        {
            kernel = kernelParam;
            this.kernel.Settings.InjectNonPublic = true;
            AddBindings();
            AutoAddBinds();
            
        }

        public object GetService(Type serviceType)
        {
            return kernel.TryGet(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return kernel.GetAll(serviceType);
        }

        private void AddBindings()
        {
            

        }
    }
}
