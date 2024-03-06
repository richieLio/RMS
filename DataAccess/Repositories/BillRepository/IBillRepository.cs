﻿using DataAccess.Entities;
using DataAccess.Repositories.GenericRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.BillRepository
{
    public interface IBillRepository : IRepository<Bill>
    {
    }
}