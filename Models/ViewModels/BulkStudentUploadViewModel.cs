using System.ComponentModel.DataAnnotations;

namespace CETExamApp.Models.ViewModels
{
    public class BulkStudentUploadViewModel
    {
        [Required(ErrorMessage = "Please select a file to upload")]
        [Display(Name = "CSV File")]
        public IFormFile? File { get; set; }

        [Display(Name = "Preview Data")]
        public List<StudentUploadRow>? PreviewData { get; set; }

        [Display(Name = "Upload Results")]
        public BulkUploadResult? UploadResult { get; set; }
    }

    public class StudentUploadRow
    {
        public int RowNumber { get; set; }

        [Required(ErrorMessage = "Student name is required")]
        [Display(Name = "Student Name")]
        public string StudentName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Class ID is required")]
        [Display(Name = "Class ID")]
        public int ClassId { get; set; }

        [Display(Name = "Class Name")]
        public string? ClassName { get; set; }

        [Required(ErrorMessage = "Group ID is required")]
        [Display(Name = "Group ID")]
        public int GroupId { get; set; }

        [Display(Name = "Group Name")]
        public string? GroupName { get; set; }

        [Phone(ErrorMessage = "Please enter a valid mobile number")]
        [Display(Name = "Mobile Number")]
        public string? MobileNo { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [Display(Name = "Username")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long")]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string? Email { get; set; }

        [Display(Name = "Gender")]
        public string Gender { get; set; } = "Male";

        public bool IsValid { get; set; } = true;
        public List<string> Errors { get; set; } = new List<string>();
    }

    public class BulkUploadResult
    {
        public int TotalRows { get; set; }
        public int SuccessfulUploads { get; set; }
        public int FailedUploads { get; set; }
        public List<string> ErrorMessages { get; set; } = new List<string>();
        public List<string> SuccessMessages { get; set; } = new List<string>();
    }
}
