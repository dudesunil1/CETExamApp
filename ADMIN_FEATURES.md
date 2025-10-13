# Admin Features Documentation

## Overview

The CET Exam Application now includes comprehensive admin functionality for managing the complete examination system. All admin features are protected by role-based authorization - only users with the "Admin" role can access these features.

## Admin Access

**Default Admin Credentials:**
- Email: `admin@cetexam.com`
- Password: `Admin@123`

## Admin Dashboard

The Admin Dashboard (`/Admin/Dashboard`) serves as the central hub for all administrative functions, providing:

- **Quick Statistics**: Total users, students, admins, and active users count
- **Navigation Cards**: Easy access to all admin sections
- **User Overview**: Table showing all users with their roles and status

## Admin Features

### 1. Student Registration (`/StudentsManagement`)

**Purpose:** Register and manage student accounts

**Capabilities:**
- ✓ Register new students with email, name, password
- ✓ Assign students to groups
- ✓ Edit student details (name, email, group)
- ✓ Activate/deactivate student accounts
- ✓ Delete students
- ✓ View all registered students with group assignments

**Authorization:** `[Authorize(Roles = "Admin")]`

**Key Features:**
- Automatic assignment to "Student" role
- Optional group assignment
- Email confirmed by default for admin-created accounts

---

### 2. Subject Master (`/Subjects`)

**Purpose:** Manage academic subjects

**Capabilities:**
- ✓ Create new subjects with code and description
- ✓ Edit subject details
- ✓ Activate/deactivate subjects
- ✓ Delete subjects (if no dependencies)
- ✓ View all subjects

**Authorization:** `[Authorize(Roles = "Admin")]`

**Data Fields:**
- Name (required)
- Code (optional)
- Description (optional)
- IsActive status

---

### 3. Class Master (`/Classes`)

**Purpose:** Manage class/grade levels

**Capabilities:**
- ✓ Create new classes
- ✓ Edit class details
- ✓ Activate/deactivate classes
- ✓ Delete classes
- ✓ View all classes

**Authorization:** `[Authorize(Roles = "Admin")]`

**Relationships:**
- Classes can have multiple groups
- Classes can be assigned to tests

---

### 4. Group Master (`/Groups`)

**Purpose:** Manage student groups/sections

**Capabilities:**
- ✓ Create new groups
- ✓ Assign groups to classes
- ✓ Edit group details
- ✓ Activate/deactivate groups
- ✓ Delete groups
- ✓ View all groups with class information

**Authorization:** `[Authorize(Roles = "Admin")]`

**Relationships:**
- Groups belong to classes (optional)
- Students are assigned to groups
- Used for organizing students

---

### 5. Topic Master (`/Topics`)

**Purpose:** Manage topics within subjects

**Capabilities:**
- ✓ Create new topics
- ✓ Assign topics to subjects
- ✓ Edit topic details
- ✓ Activate/deactivate topics
- ✓ Delete topics
- ✓ View all topics with subject information

**Authorization:** `[Authorize(Roles = "Admin")]`

**Relationships:**
- Topics belong to subjects (required)
- Questions are categorized by topics

---

### 6. Question Bank (`/Questions`)

**Purpose:** Manage exam questions

**Capabilities:**
- ✓ Create questions with multiple types:
  - Multiple Choice
  - True/False
  - Short Answer
  - Essay
- ✓ Set difficulty levels (Easy, Medium, Hard)
- ✓ Add options for MCQ questions (A, B, C, D)
- ✓ Define correct answers
- ✓ Add explanations
- ✓ Set marks per question
- ✓ Filter questions by topic and difficulty
- ✓ View question details
- ✓ Edit questions
- ✓ Delete questions

**Authorization:** `[Authorize(Roles = "Admin")]`

**Question Types:**
1. **Multiple Choice**: 4 options (A-D), single correct answer
2. **True/False**: Boolean answer
3. **Short Answer**: Text-based answer
4. **Essay**: Long-form answer

**Data Tracked:**
- Question text
- Question type
- Topic association
- Difficulty level
- Options (for MCQ)
- Correct answer
- Explanation
- Marks
- Created by
- Active status

---

### 7. Test Creation (`/Tests`)

**Purpose:** Create and manage tests/exams

**Capabilities:**
- ✓ Create new tests with:
  - Title and description
  - Subject assignment
  - Class assignment (optional)
  - Duration in minutes
  - Total marks
  - Passing marks threshold
  - Scheduled date/time
  - Test settings (shuffle, late submission, show results)
- ✓ Add questions to tests
- ✓ Set individual question marks
- ✓ Remove questions from tests
- ✓ Edit test details
- ✓ View test details with question list
- ✓ Delete tests

**Authorization:** `[Authorize(Roles = "Admin")]`

**Test Status:**
- Draft
- Published
- In Progress
- Completed
- Cancelled

**Test Settings:**
- `AllowLateSubmission`: Allow submissions after end time
- `ShuffleQuestions`: Randomize question order
- `ShowResultsImmediately`: Show results right after submission

**Workflow:**
1. Create test with basic details
2. Add questions from question bank
3. Set marks for each question
4. Publish test
5. Allocate to students

---

### 8. Test Allocation (`/TestAllocations`)

**Purpose:** Allocate tests to students and manage schedules

**Capabilities:**
- ✓ Allocate tests to multiple students at once
- ✓ Set scheduled start and end times
- ✓ View all test allocations
- ✓ Filter allocations by test
- ✓ Reschedule individual allocation
- ✓ Reschedule all allocations for a test
- ✓ Remove test allocations
- ✓ Track completion status

**Authorization:** `[Authorize(Roles = "Admin")]`

**Reschedule Features:**
1. **Individual Reschedule**: Change dates for a single student
2. **Bulk Reschedule**: Change dates for all students taking a test

**Allocation Data:**
- Test reference
- Student reference
- Allocated date
- Scheduled start time
- Scheduled end time
- Completion status
- Allocated by (admin username)

---

### 9. Results & Reports (`/Results`)

**Purpose:** View and analyze test results

**Capabilities:**
- ✓ View all test results
- ✓ Filter by test
- ✓ Filter by student
- ✓ View detailed result breakdown
- ✓ View student answers and correctness
- ✓ Generate test reports with statistics:
  - Total allocated vs completed
  - Average score
  - Pass/fail counts
  - Highest/lowest scores
- ✓ Generate student reports with:
  - Total tests taken
  - Pass/fail statistics
  - Average percentage
  - Performance trends

**Authorization:** `[Authorize(Roles = "Admin")]`

**Result Data Displayed:**
- Test title
- Student name
- Submission date/time
- Obtained marks
- Total marks
- Percentage
- Pass/fail status
- Time taken

**Reports Available:**
1. **Test Report** (`/Results/TestReport/{testId}`):
   - Overall test statistics
   - Student-wise results
   - Performance analysis

2. **Student Report** (`/Results/StudentReport/{studentId}`):
   - Student's test history
   - Performance metrics
   - Subject-wise analysis

---

### 10. Exam Center Configuration (`/ExamCenterConfig`)

**Purpose:** Configure exam center branding and details

**Capabilities:**
- ✓ Set exam center name
- ✓ Upload center logo
- ✓ Set contact information (address, phone, email, website)
- ✓ Configure brand colors:
  - Primary color (for buttons, headers)
  - Secondary color (for accents)
- ✓ Track configuration changes (updated by, last updated)

**Authorization:** `[Authorize(Roles = "Admin")]`

**Configuration Fields:**
- **Center Name**: Displayed in navbar and throughout the app
- **Logo**: Uploaded image displayed in navbar
- **Address**: Physical address of center
- **Phone**: Contact phone number
- **Email**: Contact email
- **Website**: Center website URL
- **Primary Color**: Main brand color (hex code)
- **Secondary Color**: Secondary brand color (hex code)

**Logo Upload:**
- Supports image files (JPEG, PNG, GIF, etc.)
- Stored in `wwwroot/images/`
- Unique filename generated to prevent conflicts
- Previous logo replaced on new upload

---

## Database Schema

### New Tables

1. **Subjects**: Academic subjects
2. **Classes**: Class/grade levels
3. **Groups**: Student groups/sections
4. **Topics**: Topics within subjects
5. **Questions**: Question bank
6. **Tests**: Test/exam definitions
7. **TestQuestions**: Questions included in tests (junction table)
8. **TestAllocations**: Test assignments to students
9. **TestResults**: Completed test results
10. **StudentAnswers**: Individual answers for each question
11. **ExamCenterConfigs**: Exam center configuration

### Updated Tables

- **AspNetUsers (ApplicationUser)**: Added `GroupId` foreign key

### Relationships

```
Subject 1 ─── ∞ Topic
Topic 1 ─── ∞ Question
Subject 1 ─── ∞ Test
Class 1 ─── ∞ Group
Class 1 ─── ∞ Test (optional)
Group 1 ─── ∞ ApplicationUser
Test 1 ─── ∞ TestQuestion ──∞ 1 Question
Test 1 ─── ∞ TestAllocation ──∞ 1 ApplicationUser
Test 1 ─── ∞ TestResult ──∞ 1 ApplicationUser
TestResult 1 ─── ∞ StudentAnswer ──∞ 1 Question
```

---

## Security & Authorization

All admin controllers are protected with the `[Authorize(Roles = "Admin")]` attribute:

```csharp
[Authorize(Roles = "Admin")]
public class SubjectsController : Controller
{
    // Only admins can access
}
```

**Access Control:**
- ✓ All admin routes require authentication
- ✓ All admin routes require "Admin" role
- ✓ Unauthorized access redirects to `/Account/AccessDenied`
- ✓ Anonymous users redirected to `/Account/Login`

---

## Workflows

### Complete Exam Setup Workflow

1. **Configure Exam Center**
   - Go to Exam Center Config
   - Set center name and logo
   - Configure branding colors

2. **Set Up Master Data**
   - Create subjects (e.g., Mathematics, Science)
   - Create classes (e.g., Grade 10, Grade 11)
   - Create groups (e.g., Section A, Section B)
   - Create topics under subjects

3. **Register Students**
   - Go to Student Management
   - Register students with email and password
   - Assign to appropriate groups

4. **Build Question Bank**
   - Go to Questions
   - Create questions for each topic
   - Set difficulty levels
   - Add correct answers and explanations

5. **Create Tests**
   - Go to Tests
   - Create new test with details
   - Add questions from question bank
   - Set marks for each question
   - Configure test settings

6. **Allocate Tests**
   - Go to Test Allocations
   - Select test
   - Select students
   - Set schedule (start/end times)

7. **Monitor & Reschedule**
   - View allocations
   - Reschedule if needed
   - Track completion status

8. **View Results**
   - Go to Results
   - View individual results
   - Generate reports
   - Analyze performance

---

## Best Practices

1. **Master Data First**: Set up subjects, classes, groups, and topics before creating questions
2. **Question Organization**: Organize questions by topic and difficulty for easy test creation
3. **Test Drafts**: Keep tests in draft status while adding questions
4. **Bulk Operations**: Use bulk reschedule for efficiency when changing dates for multiple students
5. **Regular Backups**: Back up database regularly to prevent data loss
6. **Report Generation**: Generate reports after tests are completed for analysis

---

## Tips & Tricks

1. **Filter Questions**: Use topic and difficulty filters in question bank for easier selection
2. **Question Reuse**: Questions can be used in multiple tests
3. **Group Management**: Assign students to groups for easier bulk test allocation
4. **Status Tracking**: Use active/inactive status to hide old data without deleting
5. **Configuration Updates**: Changes to exam center config apply immediately across the app

---

## Future Enhancements

Planned features for future releases:

- [ ] Bulk student import via CSV
- [ ] Question import/export
- [ ] Automated test generation based on topic distribution
- [ ] Time-based test access control
- [ ] Student photo upload
- [ ] Email notifications for test allocations
- [ ] PDF report generation
- [ ] Question difficulty auto-adjustment based on performance
- [ ] Advanced analytics and charts
- [ ] Test templates
- [ ] Question categories/tags
- [ ] Multi-language support

---

## Troubleshooting

### Common Issues

**Q: Can't delete a subject**
A: Make sure there are no topics or tests associated with the subject first

**Q: Student not seeing allocated test**
A: Check if the test is published and scheduled time is correct

**Q: Logo not displaying**
A: Ensure the image is in `wwwroot/images/` directory and path is correct

**Q: Can't access admin features**
A: Verify you're logged in as a user with "Admin" role

---

## Support

For additional help:
1. Check the main [README.md](README.md) for setup instructions
2. Review [QUICKSTART.md](QUICKSTART.md) for quick start guide
3. Check controller code for detailed business logic
4. Review model validation rules in model classes

---

**Last Updated**: October 2025
**Version**: 1.0.0

