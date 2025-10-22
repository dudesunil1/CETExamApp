using System.ComponentModel.DataAnnotations;

namespace CETExamApp.Models
{
    public enum TestAttemptStatus
    {
        NotStarted,
        InProgress,
        Submitted,
        TimeExpired
    }

    public class TestAttempt
    {
        public int Id { get; set; }

        [Required]
        public int TestAllocationId { get; set; }

        [Required]
        public string StudentId { get; set; } = string.Empty;

        [Required]
        public int TestId { get; set; }

        public DateTime StartedAt { get; set; } = DateTime.Now.AddHours(5.5); // IST default

        public DateTime? SubmittedAt { get; set; }

        public TestAttemptStatus Status { get; set; } = TestAttemptStatus.InProgress;

        public int CurrentQuestionIndex { get; set; } = 0;

        public DateTime? LastActivityAt { get; set; } = DateTime.Now.AddHours(5.5); // IST default

        // Shuffled question order for this student (comma-separated IDs)
        public string? ShuffledQuestionOrder { get; set; }

        // Navigation properties
        public virtual TestAllocation? TestAllocation { get; set; }
        public virtual ApplicationUser? Student { get; set; }
        public virtual Test? Test { get; set; }
    }
}

