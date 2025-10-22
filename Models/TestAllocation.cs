namespace CETExamApp.Models
{
    public class TestAllocation
    {
        public int Id { get; set; }

        public int TestId { get; set; }

        public int? ScheduleId { get; set; }

        public string StudentId { get; set; } = string.Empty;

        public DateTime AllocatedDate { get; set; } = DateTime.Now.AddHours(5.5); // IST default

        public DateTime? ScheduledStartTime { get; set; }

        public DateTime? ScheduledEndTime { get; set; }

        public bool IsCompleted { get; set; } = false;

        public DateTime? CompletedDate { get; set; }

        public string? AllocatedBy { get; set; }

        // Navigation properties
        public virtual Test? Test { get; set; }
        public virtual ApplicationUser? Student { get; set; }
        public virtual TestSchedule? TestSchedule { get; set; }
    }
}

