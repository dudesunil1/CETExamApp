using System.ComponentModel.DataAnnotations;

namespace CETExamApp.Models
{
    public enum QuestionType
    {
        MCQ,                          // Multiple Choice Question
        TrueFalse,                    // True/False Question
        MCQWithAllOfAbove            // MCQ with "All of the above" as last option
    }

    public enum DifficultyLevel
    {
        Easy,
        Medium,
        Hard
    }

    public class Question
    {
        public int Id { get; set; }

        [Required]
        public string QuestionText { get; set; } = string.Empty;

        // Image path for question (optional)
        [StringLength(500)]
        public string? QuestionImagePath { get; set; }

        [Required]
        public QuestionType QuestionType { get; set; }

        public DifficultyLevel DifficultyLevel { get; set; } = DifficultyLevel.Medium;

        [Required]
        public int TopicId { get; set; }

        // For MCQ questions - 3 options + 1 correct answer = 4 total
        public string? OptionA { get; set; }
        public string? OptionAImagePath { get; set; }
        
        public string? OptionB { get; set; }
        public string? OptionBImagePath { get; set; }
        
        public string? OptionC { get; set; }
        public string? OptionCImagePath { get; set; }
        
        public string? OptionD { get; set; }
        public string? OptionDImagePath { get; set; }

        [Required]
        public string CorrectAnswer { get; set; } = string.Empty;

        // Explanation text
        public string? Explanation { get; set; }
        
        // Explanation image (optional)
        [StringLength(500)]
        public string? ExplanationImagePath { get; set; }

        [Range(0, 100)]
        public int Marks { get; set; } = 1;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public string? CreatedBy { get; set; }

        // Navigation properties
        public virtual Topic? Topic { get; set; }
        public virtual ICollection<TestQuestion>? TestQuestions { get; set; }
    }
}

