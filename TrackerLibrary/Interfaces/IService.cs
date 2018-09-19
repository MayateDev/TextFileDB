using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary.Interfaces
{
    public interface IService<TModel>
    {
        TModel Add(TModel model);
        TModel Read(int id);
        TModel Update(TModel model);
        void Delete(TModel model);
        IEnumerable<TModel> List();
    }
}
