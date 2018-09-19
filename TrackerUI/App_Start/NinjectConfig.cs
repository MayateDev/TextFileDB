using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TrackerLibrary.Interfaces;
using TrackerLibrary.Services;

namespace TrackerUI
{
    public class NinjectConfig : IDependencyResolver
    {
        private readonly IKernel _kernel;

        public NinjectConfig()
        {
            _kernel = new StandardKernel();
            AddBindings();
        }

        public object GetService(Type serviceType)
        {
            return _kernel.TryGet(serviceType);
        }

        public IKernel Kernel { get { return _kernel; } }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return _kernel.GetAll(serviceType);
        }

        private void AddBindings()
        {
            _kernel.Bind<IPersonService>().To<PersonService>();
            _kernel.Bind<ITeamService>().To<TeamService>();
            _kernel.Bind<IPrizeService>().To<PrizeService>();
            _kernel.Bind<ISqlParserService>().To<SqlParserService>();
            TrackerLibrary.Config.NinjectConfig.Config(_kernel);
        }
    }
}