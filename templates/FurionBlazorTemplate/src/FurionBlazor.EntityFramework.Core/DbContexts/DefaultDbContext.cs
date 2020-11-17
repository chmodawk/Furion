﻿using Furion.DatabaseAccessor;
using Microsoft.EntityFrameworkCore;

namespace FurionBlazor.EntityFramework.Core
{
    [AppDbContext("FurionBlazor")]
    public class DefaultDbContext : AppDbContext<DefaultDbContext>
    {
        public DefaultDbContext(DbContextOptions<DefaultDbContext> options) : base(options)
        {
        }
    }
}