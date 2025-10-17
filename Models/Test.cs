using System.ComponentModel.DataAnnotations;

namespace CETExamApp.Models
{
    public enum TestStatus
    {
        Draft,
        Published,
        InProgress,
        Completed,
        Cancelled
    }

    public class Test
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Test Name")]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [Display(Name = "Class")]
        public int? ClassId { get; set; }

        [Required]
        [Range(1, 1440)]
        [Display(Name = "Duration (Minutes)")]
        public int DurationMinutes { get; set; }

        [Required]
        [Display(Name = "Total Marks")]
        public int TotalMarks { get; set; }

        [Range(0, 100)]
        [Display(Name = "Passing Marks")]
        public int PassingMarks { get; set; }

        public TestStatus Status { get; set; } = TestStatus.Draft;

        [Display(Name = "Allow Late Submission")]
        public bool AllowLateSubmission { get; set; } = false;

        [Display(Name = "Shuffle Questions Per Student")]
        public bool ShuffleQuestions { get; set; } = false;

        [Display(Name = "Show Results Immediately")]
        public bool ShowResultsImmediately { get; set; } = false;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public string? CreatedBy { get; set; }

        // Navigation properties
        public virtual Class? Class { get; set; }
        public virtual ICollection<TestQuestion>? TestQuestions { get; set; }
        public virtual ICollection<TestAllocation>? TestAllocations { get; set; }
        public virtual ICollection<TestResult>? TestResults { get; set; }
    }
}

