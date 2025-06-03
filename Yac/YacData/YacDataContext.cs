using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YacData.Models;

namespace YacData
{
    public class YacDataContext : DbContext
    {
        public DbSet<GamePlayerPassingStats> GamePlayerPassingStats { get; set; }

        public YacDataContext(DbContextOptions options) : base(options)
        {
        }


    }
}
