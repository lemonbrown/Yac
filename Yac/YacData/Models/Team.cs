using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YacData.Models
{
    public class Team {

        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string Name { get; set; } = "";

        public string Abbreviation { get; set; } = "";

        public int Divison { get; set; }

        public DateTime ActiveStartDateTime { get; set; }

        public DateTime ActiveEndDateTime { get; set; }
    }
}
