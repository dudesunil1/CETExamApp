using System.ComponentModel.DataAnnotations;

namespace CETExamApp.Models
{
    public class TestSchedule
    {
        public int Id { get; set; }

        [Required]
        public int TestId { get; set; }

        [Required]
        public DateTime ScheduledStartTime { get; set; }

        [Required]
        public DateTime ScheduledEndTime { get; set; }

        public bool IsCompleted { get; set; } = false;

        public DateTime? CompletedDate { get; set; }

        public string? AllocatedBy { get; set; }

        public DateTime CreateDate { get; set; } = DateTime.Now.AddHours(5.5); // IST default

        // Navigation properties
        public virtual Test? Test { get; set; }
        public virtual ICollection<TestAllocation>? TestAllocations { get; set; }
    }
}
