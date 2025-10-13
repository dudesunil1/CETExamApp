namespace CETExamApp.Models.ViewModels
{
    public class StudentResultViewModel
    {
        public ApplicationUser? Student { get; set; }
        public List<TestResult> TestResults { get; set; } = new();
        public int TotalTestsTaken { get; set; }
        public int TestsPassed { get; set; }
        public int TestsFailed { get; set; }
        public decimal AveragePercentage { get; set; }
        public int TotalMarksObtained { get; set; }
        public int TotalMaxMarks { get; set; }
        public Dictionary<string, TopicPerformance> TopicWisePerformance { get; set; } = new();
        public Dictionary<string, SubjectPerformance> SubjectWisePerformance { get; set; } = new();
    }

    public class TopicPerformance
    {
        public string TopicName { get; set; } = string.Empty;
        public int QuestionsAttempted { get; set; }
        public int CorrectAnswers { get; set; }
        public int WrongAnswers { get; set; }
        public decimal Percentage { get; set; }
        public int MarksObtained { get; set; }
        public int TotalMarks { get; set; }
    }

    public class SubjectPerformance
    {
        public string SubjectName { get; set; } = string.Empty;
        public int TestsCount { get; set; }
        public decimal AveragePercentage { get; set; }
        public int TotalMarksObtained { get; set; }
        public int TotalMaxMarks { get; set; }
    }

    public class TestWiseSummaryViewModel
    {
        public Test? Test { get; set; }
        public int TotalAllocated { get; set; }
        public int TotalCompleted { get; set; }
        public int TotalPending { get; set; }
        public int PassCount { get; set; }
        public int FailCount { get; set; }
        public decimal AverageScore { get; set; }
        public decimal AveragePercentage { get; set; }
        public int HighestScore { get; set; }
        public int LowestScore { get; set; }
        public List<TestResult> Results { get; set; } = new();
        public Dictionary<string, TopicPerformance> TopicWiseAnalysis { get; set; } = new();
    }

    public class DetailedAnswerKeyViewModel
    {
        public TestResult? TestResult { get; set; }
        public Test? Test { get; set; }
        public ApplicationUser? Student { get; set; }
        public List<StudentAnswerDetail> Answers { get; set; } = new();
    }

    public class StudentAnswerDetail
    {
        public int QuestionNumber { get; set; }
        public Question? Question { get; set; }
        public StudentAnswer? Answer { get; set; }
        public bool IsCorrect { get; set; }
        public int MarksObtained { get; set; }
        public int MaxMarks { get; set; }
    }
}

