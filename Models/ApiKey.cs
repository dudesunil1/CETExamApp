using System.ComponentModel.DataAnnotations;

namespace CETExamApp.Models
{
    public class ApiKey
    {
        public int Id { get; set; }

        [Required]
        [StringLength(500)]
        [Display(Name = "API Key")]
        public string ApiKeyValue { get; set; } = string.Empty;

        [Display(Name = "Inactive")]
        public bool InActive { get; set; } = false;

        [StringLength(200)]
        [EmailAddress]
        public string? Email { get; set; }

        [StringLength(200)]
        public string? Password { get; set; }

        [Display(Name = "Description")]
        [StringLength(500)]
        public string? Description { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Display(Name = "Last Modified")]
        public DateTime? LastModified { get; set; }
    }
}

