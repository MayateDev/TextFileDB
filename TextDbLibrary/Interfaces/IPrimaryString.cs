﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextDbLibrary.Interfaces
{
    public interface IPrimaryString : IEntity
    {
        string Id { get; set; }
    }
}
