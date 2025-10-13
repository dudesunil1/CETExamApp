using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CETExamApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        [StringLength(100)]
        public string? FirstName { get; set; }
        
        [StringLength(100)]
        public string? LastName { get; set; }
        
        [StringLength(50)]
        public string? StudentUsername { get; set; }
        
        public int? ClassId { get; set; }
        
        public int? GroupId { get; set; }
        
        [StringLength(500)]
        public string? PhotoPath { get; set; }
        
        [StringLength(15)]
        [Phone]
        public string? MobileNo { get; set; }
        
        [StringLength(15)]
        [Phone]
        public string? ParentsMobileNo { get; set; }
        
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual Class? Class { get; set; }
        public virtual Group? Group { get; set; }
        public virtual ICollection<TestAllocation>? TestAllocations { get; set; }
        public virtual ICollection<TestResult>? TestResults { get; set; }
    }
}

