using CETExamApp.Models;
using System.ComponentModel.DataAnnotations;

namespace CETExamApp.Models.ViewModels
{
    public class TestCreationWizardViewModel
    {
        public int Id { get; set; }

        // Step 1: Test Details
        [Required(ErrorMessage = "Test Name is required.")]
        [StringLength(200, ErrorMessage = "Test Name cannot exceed 200 characters.")]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Duration is required.")]
        [Range(1, 1440, ErrorMessage = "Duration must be between 1 and 1440 minutes.")]
        public int DurationMinutes { get; set; } = 60;

        [Required(ErrorMessage = "Total Questions is required.")]
        [Range(1, 500, ErrorMessage = "Total Questions must be between 1 and 500.")]
        public int TotalQuestions { get; set; } = 20;

        [Required(ErrorMessage = "Total Marks is required.")]
        [Range(1, 1000, ErrorMessage = "Total Marks must be between 1 and 1000.")]
        public int TotalMarks { get; set; } = 100;

        [Required(ErrorMessage = "Passing Marks is required.")]
        [Range(0, 1000, ErrorMessage = "Passing Marks must be between 0 and 1000.")]
        public int PassingMarks { get; set; } = 50;

        // Step 2: Class & Subject Configuration
        [Required(ErrorMessage = "Class is required.")]
        public int ClassId { get; set; }

        [Required(ErrorMessage = "Test Type is required.")]
        [Display(Name = "Test Type")]
        public TestType TestType { get; set; } = TestType.CET;

        public List<SubjectConfigViewModel> SubjectConfigs { get; set; } = new List<SubjectConfigViewModel>();

        // Step 3 & 4: Question Selection
        public Dictionary<int, List<int>> SelectedQuestionsBySubject { get; set; } = new Dictionary<int, List<int>>();

        // Step 5: Final Settings
        public bool ShuffleQuestions { get; set; } = false;
        public bool ShuffleOptionsPosition { get; set; } = false;

        public TestStatus Status { get; set; } = TestStatus.Draft;
    }

    public class SubjectConfigViewModel
    {
        public int SubjectId { get; set; }
        public string SubjectName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Number of questions is required.")]
        [Range(1, 500, ErrorMessage = "Number of questions must be between 1 and 500.")]
        public int NumberOfQuestions { get; set; }

        public List<TopicSelectionViewModel> Topics { get; set; } = new List<TopicSelectionViewModel>();
    }

    public class TopicSelectionViewModel
    {
        public int TopicId { get; set; }
        public string TopicName { get; set; } = string.Empty;
        public bool IsSelected { get; set; }
    }


    public class QuestionReviewViewModel
    {
        public int SubjectId { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public List<QuestionReviewItemViewModel> Questions { get; set; } = new List<QuestionReviewItemViewModel>();
    }

    public class QuestionReviewItemViewModel
    {
        public int QuestionId { get; set; }
        public string QuestionTextPreview { get; set; } = string.Empty;
        public string QuestionType { get; set; } = string.Empty;
        public string DifficultyLevel { get; set; } = string.Empty;
        public int Marks { get; set; }
        public string TopicName { get; set; } = string.Empty;
    }
}
