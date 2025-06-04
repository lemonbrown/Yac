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
        public int Id { get; set; }

        public string Name { get; set; } = "";

        public string Abbreviation { get; set; } = ""; 
        
        public string Location { get; set; } = "";

        public DateTime ActiveStartDateTime { get; set; }

        public DateTime? ActiveEndDateTime { get; set; }
    }
}
