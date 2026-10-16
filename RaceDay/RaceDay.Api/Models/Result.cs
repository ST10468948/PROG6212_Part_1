using System;
using System.ComponentModel.DataAnnotations;

namespace RaceDay.Api.Models
{
    public class Result
    {
        [Key]
        public int ResultID { get; set; }
        public int EnrolmentID { get; set; }
        public int ParticipantID { get; set; }
        public string FinishTime { get; set; }
        public int Position { get; set; }
        public int TotalFinishers { get; set; }
        public DateTime RecordedDate { get; set; }

       
        public Enrolment Enrolment { get; set; }
        public AppUser Participant { get; set; }
    }
}