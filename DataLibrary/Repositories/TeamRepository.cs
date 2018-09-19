﻿using DataLibrary.Entities;
using DataLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLibrary.Repositories
{
    public class TeamRepository : Repository<DbContext, Team>, ITeamRepository
    {
    }
}