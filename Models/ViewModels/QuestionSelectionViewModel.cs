namespace CETExamApp.Models.ViewModels
{
    public class QuestionSelectionViewModel
    {
        public int QuestionId { get; set; }
        public string QuestionTextPreview { get; set; } = string.Empty;
        public string QuestionType { get; set; } = string.Empty;
        public string DifficultyLevel { get; set; } = string.Empty;
        public int Marks { get; set; }
        public int TopicId { get; set; }
        public string TopicName { get; set; } = string.Empty;
        public bool IsSelected { get; set; }
    }
}
