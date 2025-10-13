using System.ComponentModel.DataAnnotations;

namespace CETExamApp.Models
{
    public class Topic
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Code { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        public int SubjectId { get; set; }

        [Required]
        public int ClassId { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual Subject? Subject { get; set; }
        public virtual Class? Class { get; set; }
        public virtual ICollection<Question>? Questions { get; set; }
    }
}

