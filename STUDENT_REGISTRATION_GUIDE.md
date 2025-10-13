# Student Registration Module - Complete Guide

## Overview

The enhanced Student Registration module provides comprehensive student management with photo upload, contact information, and automatic role assignment.

## Features

### ✅ Complete Student Profile Management

1. **Personal Information**
   - First Name (Required)
   - Last Name (Required)
   - Photo Upload (Optional - JPG, PNG)

2. **Academic Information**
   - Class Assignment (Required - 10th/11th/12th)
   - Group Assignment (Required - PCMB/PCB/PCM)

3. **Contact Information**
   - Mobile Number (Required)
   - Parent's Mobile Number (Required)

4. **Login Credentials**
   - Email/Username (Required)
   - Password (Required, min 6 characters)
   - Auto-assign Student Role

5. **Status Management**
   - Active/Inactive status toggle

---

## Student Registration Form

### Access
- **Route**: `/StudentsManagement/Create`
- **From**: Admin Dashboard → Student Registration → Register New Student
- **Authorization**: Admin only

### Form Fields

#### Personal Information Section
```
First Name*             [Text Input]
Last Name*              [Text Input]
Class*                  [Dropdown: 10th/11th/12th]
Group*                  [Dropdown: PCMB/PCB/PCM]
Photo                   [File Upload: JPG/PNG]
```

#### Contact Information Section
```
Mobile Number*          [Phone Input]
Parent's Mobile Number* [Phone Input]
```

#### Login Credentials Section
```
Email (Username)*       [Email Input]
Password*               [Password Input - min 6 chars]
Confirm Password*       [Password Input]
```

### Field Validations

| Field | Validation | Max Length |
|-------|------------|------------|
| First Name | Required | 100 chars |
| Last Name | Required | 100 chars |
| Class | Required | - |
| Group | Required | - |
| Photo | Optional | Image file |
| Mobile No | Required, Phone format | 15 chars |
| Parent's Mobile | Required, Phone format | 15 chars |
| Email | Required, Valid email | - |
| Password | Required, Min 6 chars | 100 chars |

---

## Photo Upload Feature

### Specifications
- **File Types**: JPG, JPEG, PNG, GIF
- **Storage Location**: `wwwroot/uploads/students/`
- **File Naming**: `GUID_originalfilename.ext`
- **Display**: Circular thumbnail (40x40px in list, 150x150px in forms)

### How Photo Upload Works

1. **On Create**:
   - User selects photo file
   - File is validated
   - Unique filename generated
   - File saved to `/uploads/students/`
   - Path stored in database

2. **On Edit**:
   - Current photo displayed
   - User can upload new photo (optional)
   - Old photo deleted when new one uploaded
   - Path updated in database

3. **On Delete**:
   - Student record deleted
   - Associated photo file deleted
   - Cleanup automatic

### Default Photo Display
If no photo uploaded:
- Circular gray placeholder with person icon
- Consistent with Bootstrap design

---

## Database Schema

### ApplicationUser Model (Enhanced)

```csharp
public class ApplicationUser : IdentityUser
{
    [StringLength(100)]
    public string? FirstName { get; set; }
    
    [StringLength(100)]
    public string? LastName { get; set; }
    
    public int? ClassId { get; set; }          // NEW
    
    public int? GroupId { get; set; }
    
    [StringLength(500)]
    public string? PhotoPath { get; set; }      // NEW
    
    [StringLength(15)]
    [Phone]
    public string? MobileNo { get; set; }       // NEW
    
    [StringLength(15)]
    [Phone]
    public string? ParentsMobileNo { get; set; } // NEW
    
    public DateTime CreatedDate { get; set; }
    
    public bool IsActive { get; set; }

    // Navigation properties
    public virtual Class? Class { get; set; }   // NEW
    public virtual Group? Group { get; set; }
}
```

### New Fields Added

| Field | Type | Purpose |
|-------|------|---------|
| ClassId | int? | Reference to Class (10th/11th/12th) |
| PhotoPath | string? | Path to uploaded photo |
| MobileNo | string? | Student's mobile number |
| ParentsMobileNo | string? | Parent/guardian mobile number |

---

## Student Management Views

### 1. Index View (List)

**Displays:**
- Photo (circular thumbnail or placeholder)
- Full Name
- Class (badge)
- Group (badge)
- Mobile Number
- Email
- Active/Inactive status
- Action buttons (Edit/Delete)

**Features:**
- Sortable columns
- Status indicators
- Photo thumbnails
- Responsive design

### 2. Create View (Register New Student)

**Layout:**
Three organized sections:
1. Personal Information
   - Name, Class, Group, Photo
2. Contact Information
   - Student mobile, Parent mobile
3. Login Credentials
   - Email, Password, Confirm Password

**Features:**
- File upload for photo
- Dropdown selects for Class/Group
- Phone number validation
- Password strength requirements
- Info alert about auto-role assignment

### 3. Edit View (Update Student)

**Layout:**
Similar to Create, but:
- Shows current photo
- Photo upload is optional
- No password fields
- Active/Inactive toggle

**Features:**
- Pre-filled with current data
- Option to change photo
- Update all information except password

### 4. Delete View (Confirmation)

**Displays:**
- Student photo (if exists)
- All student details
- Warning message

**Features:**
- Confirmation required
- Shows all data before deletion
- Deletes photo file automatically

---

## Automatic Role Assignment

### How It Works

When a student is registered:
1. Admin fills registration form
2. Student record created in database
3. **Automatic**: "Student" role assigned
4. Student can immediately login
5. Student sees Student Dashboard

### No Manual Role Assignment Needed
- ✅ Automatic on registration
- ✅ Cannot be unassigned
- ✅ Consistent for all students

---

## Usage Workflow

### Register a New Student

1. **Navigate**: Admin Dashboard → Student Registration
2. **Click**: "Register New Student" button
3. **Fill Personal Info**:
   - Enter first and last name
   - Select class (10th/11th/12th)
   - Select group (PCMB/PCB/PCM)
   - Upload photo (optional)
4. **Fill Contact Info**:
   - Enter student's mobile
   - Enter parent's mobile
5. **Set Credentials**:
   - Enter email (this becomes username)
   - Create password (min 6 characters)
   - Confirm password
6. **Submit**: Click "Register Student"
7. **Success**: Student can now login

### Update Student Information

1. **Navigate**: Student Management → Student List
2. **Click**: "Edit" button for student
3. **Update** any field(s):
   - Change name, class, or group
   - Upload new photo
   - Update mobile numbers
   - Change email
   - Toggle active status
4. **Submit**: Click "Update Student"

### Delete a Student

1. **Navigate**: Student Management → Student List
2. **Click**: "Delete" button for student
3. **Review** student details
4. **Confirm**: Click "Delete Student"
5. **Result**: Student and photo removed

---

## Sample Data

### Example Registration

```
Personal Information:
- First Name: Rajesh
- Last Name: Kumar
- Class: 11th Standard
- Group: PCM (Engineering Stream)
- Photo: [Upload rajesh.jpg]

Contact Information:
- Mobile Number: +91 98765 43210
- Parent's Mobile: +91 98765 43211

Login Credentials:
- Email: rajesh.kumar@school.com
- Password: Student@123
- Confirm Password: Student@123
```

### Result
- Student registered successfully
- Auto-assigned "Student" role
- Can login with: rajesh.kumar@school.com / Student@123
- Photo stored at: `/uploads/students/abc123_rajesh.jpg`
- Appears in Student Management list

---

## Student Login

After registration, students can:
1. Navigate to login page
2. Enter email (username)
3. Enter password
4. Click "Login"
5. Redirected to Student Dashboard

### Student Dashboard Access
- View personal profile
- See assigned tests
- Take tests (when allocated)
- View results

---

## Admin Functions

### View All Students
- **Route**: `/StudentsManagement`
- **Shows**: Complete list with photos
- **Filter**: By class, group, status
- **Actions**: Edit, Delete

### Bulk Operations (Future Enhancement)
- Import students from CSV
- Export student list
- Bulk class/group assignment
- Bulk email notifications

---

## Photo Management

### Upload Directory Structure
```
wwwroot/
  └── uploads/
      └── students/
          ├── .gitkeep
          ├── abc123_student1.jpg
          ├── def456_student2.png
          └── ghi789_student3.jpg
```

### Photo File Naming
Format: `{GUID}_{OriginalFileName}.{Extension}`

Example:
- Original: `profile.jpg`
- Saved as: `550e8400-e29b-41d4-a716-446655440000_profile.jpg`

### Storage Considerations
- Photos stored in file system (not database)
- Unique filenames prevent conflicts
- Automatic cleanup on delete
- .gitkeep ensures directory exists

---

## Security Features

### Password Requirements
- Minimum 6 characters
- Must contain uppercase letter
- Must contain lowercase letter  
- Must contain digit
- No special characters required

### Data Protection
- Passwords hashed (ASP.NET Identity)
- Phone numbers validated
- Email uniqueness enforced
- Photo uploads validated

### Access Control
- Only Admin can register students
- Students cannot self-register via this module
- Students can only view their own data
- Admin can manage all students

---

## Validation Rules

### Client-Side Validation
- Required field checking
- Email format validation
- Phone number format
- Password match confirmation
- File type validation (images only)

### Server-Side Validation
- Model state validation
- Duplicate email check
- Password strength validation
- File size limits
- Image format validation

---

## Error Handling

### Common Errors

**Email Already Exists**
- Error: "Email 'xyz@example.com' is already taken"
- Solution: Use different email address

**Invalid Phone Number**
- Error: "The field Mobile Number must be a valid phone number"
- Solution: Enter valid phone format

**Password Too Short**
- Error: "Password must be at least 6 characters long"
- Solution: Create longer password

**Invalid Image File**
- Error: "Only image files are allowed"
- Solution: Upload JPG, PNG, or GIF file

---

## Integration with Other Modules

### Class Master
- Students assigned to classes
- Dropdown populated from active classes
- Validation ensures class exists

### Group Master
- Students assigned to groups
- Dropdown populated from active groups
- Groups determine subject combination

### Test Allocation
- Tests allocated to students
- Based on class and group
- Only active students can be assigned

### Results & Reports
- Student results tracked
- Reports generated per student
- Performance analytics

---

## Best Practices

### Registration
1. ✅ Always assign both class and group
2. ✅ Upload student photo for easy identification
3. ✅ Verify mobile numbers are correct
4. ✅ Use school email format for consistency
5. ✅ Set strong initial passwords

### Photo Upload
1. ✅ Use clear, recent photos
2. ✅ Prefer passport-style photos
3. ✅ Keep file sizes reasonable (<1MB)
4. ✅ Use standard formats (JPG/PNG)

### Data Entry
1. ✅ Double-check all information
2. ✅ Verify class/group assignments
3. ✅ Test login after registration
4. ✅ Keep parent contact updated

---

## Troubleshooting

### Photo Not Displaying
**Issue**: Uploaded photo doesn't show
**Solutions**:
- Check file was uploaded successfully
- Verify `/uploads/students/` directory exists
- Check file permissions
- Confirm path in database is correct

### Cannot Login
**Issue**: Student cannot login after registration
**Solutions**:
- Verify email address is correct
- Check password was set correctly
- Confirm "Student" role was assigned
- Check IsActive is set to true

### Validation Errors
**Issue**: Form won't submit
**Solutions**:
- Fill all required fields (marked with *)
- Ensure phone numbers are valid
- Verify password meets requirements
- Check email format is valid

---

## Migration Instructions

### Creating Migration for New Fields

```bash
# Create migration
dotnet ef migrations add EnhanceStudentRegistration

# Update database
dotnet ef database update
```

### What Gets Created
- ClassId column added to AspNetUsers
- PhotoPath column added to AspNetUsers
- MobileNo column added to AspNetUsers
- ParentsMobileNo column added to AspNetUsers
- Foreign key to Classes table
- Index on ClassId

---

## Summary

### What's New in Enhanced Registration

| Feature | Before | After |
|---------|--------|-------|
| Class Assignment | ❌ No | ✅ Yes (10th/11th/12th) |
| Group Assignment | ⚠️ Optional | ✅ Required (PCMB/PCB/PCM) |
| Photo Upload | ❌ No | ✅ Yes (with preview) |
| Mobile Number | ❌ No | ✅ Yes (required) |
| Parent Mobile | ❌ No | ✅ Yes (required) |
| Auto Role Assignment | ✅ Yes | ✅ Yes (maintained) |

### Key Benefits
- ✅ Complete student profiles
- ✅ Visual identification with photos
- ✅ Emergency contact information
- ✅ Proper academic organization
- ✅ Streamlined registration process

---

**Version**: 2.2.0  
**Last Updated**: October 2025  
**Module**: Student Registration (Enhanced)

