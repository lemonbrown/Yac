using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YacData.Models
{
    public record Team(int Id, string Name, string Abbreviation, string Location, DateTime ActiveFromDateTime, DateTime? ActiveEndDateTime);
}
