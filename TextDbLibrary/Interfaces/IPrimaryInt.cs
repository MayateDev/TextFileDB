using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextDbLibrary.Interfaces
{
    public interface IPrimaryInt : IEntity
    {
        int Id { get; set; }
    }
}
