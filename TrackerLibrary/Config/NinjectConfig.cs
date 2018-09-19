using DataLibrary.Interfaces;
using DataLibrary.Repositories;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary.Config
{
    public class NinjectConfig
    {
        public static void Config(IKernel kernel)
        {
            kernel.Bind<IPersonRepository>().To<PersonRepository>();
            kernel.Bind<ITeamRepository>().To<TeamRepository>();
            kernel.Bind<IPrizeRepository>().To<PrizeRepository>();
            kernel.Bind<ISqlParserRepository>().To<SqlParserRepository>();
        }
    }
}
