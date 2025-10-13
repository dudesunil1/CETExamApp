using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using CETExamApp.Models.Attributes;

namespace CETExamApp.Models.ViewModels
{
    public class StudentRegistrationViewModel
    {
        [Required]
        [Display(Name = "First Name")]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Last Name")]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Class")]
        public int ClassId { get; set; }

        [Required]
        [Display(Name = "Group")]
        public int GroupId { get; set; }

        [Display(Name = "Photo")]
        public IFormFile? Photo { get; set; }

        [Required]
        [Display(Name = "Mobile Number")]
        [Phone]
        [StringLength(15)]
        public string MobileNo { get; set; } = string.Empty;

        [Display(Name = "Parent's Mobile Number")]
        [OptionalPhone]
        [StringLength(15)]
        public string? ParentsMobileNo { get; set; }

        [Required]
        [Display(Name = "Username")]
        [StringLength(50, MinimumLength = 3)]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Username can only contain letters, numbers, and underscores")]
        public string Username { get; set; } = string.Empty;

        [OptionalEmail]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class StudentEditViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Required]
        [Display(Name = "First Name")]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Last Name")]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Class")]
        public int ClassId { get; set; }

        [Required]
        [Display(Name = "Group")]
        public int GroupId { get; set; }

        [Display(Name = "Photo")]
        public IFormFile? Photo { get; set; }

        public string? CurrentPhotoPath { get; set; }

        [Required]
        [Display(Name = "Mobile Number")]
        [Phone]
        [StringLength(15)]
        public string MobileNo { get; set; } = string.Empty;

        [Display(Name = "Parent's Mobile Number")]
        [OptionalPhone]
        [StringLength(15)]
        public string? ParentsMobileNo { get; set; }

        [Required]
        [Display(Name = "Username")]
        [StringLength(50, MinimumLength = 3)]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Username can only contain letters, numbers, and underscores")]
        public string Username { get; set; } = string.Empty;

        [OptionalEmail]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;
    }
}

