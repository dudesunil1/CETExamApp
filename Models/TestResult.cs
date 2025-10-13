namespace CETExamApp.Models
{
    public class TestResult
    {
        public int Id { get; set; }

        public int TestId { get; set; }

        public string StudentId { get; set; } = string.Empty;

        public DateTime StartedAt { get; set; }

        public DateTime? SubmittedAt { get; set; }

        public int ObtainedMarks { get; set; }

        public int TotalMarks { get; set; }

        public decimal Percentage { get; set; }

        public bool IsPassed { get; set; }

        public int TimeTakenMinutes { get; set; }

        public string? Remarks { get; set; }

        // Navigation properties
        public virtual Test? Test { get; set; }
        public virtual ApplicationUser? Student { get; set; }
        public virtual ICollection<StudentAnswer>? StudentAnswers { get; set; }
    }
}

