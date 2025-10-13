namespace CETExamApp.Models
{
    public class TestQuestion
    {
        public int Id { get; set; }

        public int TestId { get; set; }

        public int QuestionId { get; set; }

        public int QuestionOrder { get; set; }

        public int Marks { get; set; }

        // Navigation properties
        public virtual Test? Test { get; set; }
        public virtual Question? Question { get; set; }
    }
}

