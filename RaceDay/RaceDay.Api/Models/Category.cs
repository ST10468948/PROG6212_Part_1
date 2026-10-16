using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RaceDay.Api.Models
{
    public class Category
    {
        [Key]
        public int CategoryID { get; set; }
        public int EventID { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }

        
        public Event Event { get; set; }
        public ICollection<Enrolment> Enrolments { get; set; }
    }
}