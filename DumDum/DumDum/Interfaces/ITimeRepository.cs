﻿using DumDum.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DumDum.Interfaces
{
    public interface ITimeRepository : IRepository<LastChange>
    {
        void AddTime(LastChange time);
    }
}