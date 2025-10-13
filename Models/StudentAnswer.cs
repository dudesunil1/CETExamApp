namespace CETExamApp.Models
{
    public enum QuestionStatus
    {
        Unvisited,           // Red - Not visited yet
        Visited,             // Blue - Visited but not answered
        Answered,            // Green - Answered
        MarkedForReview      // Yellow - Answered and marked for review
    }

    public class StudentAnswer
    {
        public int Id { get; set; }

        public int TestResultId { get; set; }

        public int QuestionId { get; set; }

        public string? AnswerText { get; set; }

        public bool IsCorrect { get; set; }

        public int MarksObtained { get; set; }

        public QuestionStatus Status { get; set; } = QuestionStatus.Unvisited;

        public bool IsMarkedForReview { get; set; } = false;

        public DateTime? AnsweredAt { get; set; }

        // Navigation properties
        public virtual TestResult? TestResult { get; set; }
        public virtual Question? Question { get; set; }
    }
}

