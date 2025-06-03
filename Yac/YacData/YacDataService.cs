using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YacData.Models;

namespace YacData
{
    public class YacDataService(YacDataContext context)
    {

        public async Task InsertPassingStat(GamePlayerPassingStats stat)
        {
            await context.GamePlayerPassingStats.AddAsync(stat);

        }

        public async Task SaveChanges()
        {
            await context.SaveChangesAsync();
        }
    }
}
