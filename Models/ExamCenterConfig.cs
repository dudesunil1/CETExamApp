using System.ComponentModel.DataAnnotations;

namespace CETExamApp.Models
{
    public class ExamCenterConfig
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string CenterName { get; set; } = string.Empty;

        [StringLength(500)]
        public string? LogoPath { get; set; }

        [StringLength(500)]
        public string? Address { get; set; }

        [StringLength(20)]
        public string? Phone { get; set; }

        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(500)]
        public string? Website { get; set; }

        [StringLength(7)]
        public string PrimaryColor { get; set; } = "#007bff";

        [StringLength(7)]
        public string SecondaryColor { get; set; } = "#6c757d";

        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        public string? UpdatedBy { get; set; }
    }
}

