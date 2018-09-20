using System.Collections.Generic;

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
