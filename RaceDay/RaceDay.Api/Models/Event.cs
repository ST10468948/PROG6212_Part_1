using System;
using System.Collections.Generic;

namespace RaceDay.Api.Models
{
    public class Event
    {
        public int EventID { get; set; }
        public int OrganiserID { get; set; }
        public string EventName { get; set; }
        public string Description { get; set; }
        public DateTime EventDate { get; set; }
        public string Location { get; set; }
        public decimal DistanceKm { get; set; }
        public string EventType { get; set; } 
        public string BannerUrl { get; set; }
        public DateTime CreatedAt { get; set; }

        
        public AppUser Organiser { get; set; }
        public ICollection<Category> Categories { get; set; }
        public ICollection<Enrolment> Enrolments { get; set; }
    }
}