
using System;

namespace Organizer.Models
{
    public class TimeInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? StopTime { get; set; }
    }
}
